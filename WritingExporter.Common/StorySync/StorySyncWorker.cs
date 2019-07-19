using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Exceptions;
using WritingExporter.Common.Gui;
using WritingExporter.Common.Models;
using WritingExporter.Common.Storage;
using WritingExporter.Common.Wdc;

namespace WritingExporter.Common.StorySync
{
    public class StorySyncWorker : IStorySyncWorker
    {
        private static ILogger _log = LogManager.GetLogger(typeof(StorySyncWorker));

        // Services
        IConfigProvider _configProvider;
        IWdcStoryContainer _storyContainer;
        IWdcReader _wdcReader;
        IWdcClient _wdcClient;
        IFileDumper _fileDumper;
        IGuiContext _gui;

        // Working variables
        StorySyncWorkerSettings _settings;
        Task _workerThread;
        CancellationTokenSource _ctSource;
        StorySyncWorkerStatus _status;
        Dictionary<string, StorySyncWorkerStoryStatus> _storyStatusList;
        int _lastStoryIndex = 0;
        object _storyStatusLock = new object();
        object _statusLock = new object();
        object _settingsLock = new object();

        public event EventHandler<StorySyncWorkerStatusEventArgs> OnWorkerStatusChange;
        public event EventHandler<StorySyncWorkerStoryStatusEventArgs> OnStoryStatusChange;

        public StorySyncWorker(IWdcStoryContainer storyContainer,
            IWdcReader wdcReader,
            IWdcClient wdcClient,
            IConfigProvider configProvider,
            IFileDumper fileDumper,
            IGuiContext guiContext
            )
        {
            _storyContainer = storyContainer;
            _wdcReader = wdcReader;
            _wdcClient = wdcClient;
            _configProvider = configProvider;
            _fileDumper = fileDumper;
            _gui = guiContext;
            SetSettings(configProvider.GetSection<StorySyncWorkerSettings>());

            // Init some stuff
            _storyStatusList = new Dictionary<string, StorySyncWorkerStoryStatus>();
            _ctSource = new CancellationTokenSource();
            _status = new StorySyncWorkerStatus();

            // TODO add functionality to react to configuration changes. E.g. Cancel process, update settings, start again.
            // Subscribe to some events
            _storyContainer.OnUpdate += new EventHandler<WdcStoryContainerEventArgs>(OnStoryContainerChanged);
            _configProvider.OnSectionChanged += new EventHandler<ConfigSectionChangedEventArgs>(OnSettingsChanged);
        }

        #region Event handling stuff

        private void OnSettingsChanged(object sender, ConfigSectionChangedEventArgs args)
        {
            if (args.SectionName == typeof(StorySyncWorkerSettings).Name)
            {
                // The sync worker settings have changed.
                // Update the settings
                // TODO: should I cancel the worker, and have it start again?
                _log.Debug("Reacting to a change in settings");
                SetSettings(_configProvider.GetSection<StorySyncWorkerSettings>());
            }
        }

        private void OnStoryContainerChanged(object sender, WdcStoryContainerEventArgs args)
        {
            //_log.Debug("Reacting to a change in stories");

            // TODO: instead of cancelling the sync,
            //    try to make the sync loop smart enough to react to changes in either syncing or about to sync stories

            // Was the current story removed?
            if (args.EventType == WdcStoryContainerEventType.Remove && args.StoryID == GetCurrentStatus().CurrentStoryID)
            {
                _log.Debug("The story being synced was removed, reacting");
                Cancel();
            }
            
        }

        #endregion

        #region Story status stuff

        private void SetStoryStatusError(string storyID, string errorMsg)
        {
            var currentStatus = GetStoryStatus(storyID);
            currentStatus.State = StorySyncWorkerStoryState.Error;
            currentStatus.ErrorMessage = errorMsg;
            SetStoryStatus(currentStatus);
        }

        private void SetStoryStatusState(string storyID, StorySyncWorkerStoryState newState)
        {
            var currentStatus = GetStoryStatus(storyID);

            // Don't care if we try to change the state to what it already is
            if (currentStatus.State != newState)
            {
                currentStatus.State = newState;
                SetStoryStatus(currentStatus);
            }
        }

        private void SetStoryStatusProgress(string storyID, int progressValue, int progressMax)
        {
            var currentStatus = GetStoryStatus(storyID);

            // Don't care if we try to change the progress to what it already is
            if (currentStatus.ProgressMax != progressMax || currentStatus.ProgressValue != progressValue)
            {
                currentStatus.ProgressValue = progressValue;
                currentStatus.ProgressMax = progressMax;
                SetStoryStatus(currentStatus);
            }
        }

        public void SetStoryStatus(StorySyncWorkerStoryStatus newStatus)
        {
            lock (_storyStatusLock)
            {
                _SetStoryStatus(newStatus);

                DoStoryStatusChange(newStatus);
            }
        }

        private void _SetStoryStatus(StorySyncWorkerStoryStatus newStatus)
        {
            if (newStatus == null) throw new ArgumentNullException("newStatus");

            var storyID = newStatus.StoryID;
            if (!_storyStatusList.ContainsKey(storyID))
            {
                _storyStatusList.Add(storyID, newStatus);
            }
            else
            {
                var existingStatus = _storyStatusList[storyID];

                // Was the state changed from Error? If so, clear the error message.
                if (existingStatus.State == StorySyncWorkerStoryState.Error && newStatus.State != StorySyncWorkerStoryState.Error)
                {
                    newStatus.ErrorMessage = String.Empty;
                }

                if (existingStatus.State != newStatus.State && newStatus.State == StorySyncWorkerStoryState.WaitingItu)
                {
                    newStatus.LastItu = DateTime.Now;
                }

                // Did the state change? if so, update the LastStateChange timestamp
                if (existingStatus.State != newStatus.State)
                {
                    newStatus.StateLastSet = DateTime.Now;
                }

                _storyStatusList[storyID] = newStatus;
            }
        }

        public IEnumerable<StorySyncWorkerStoryStatus> GetAllStoryStatus()
        {
            lock (_storyStatusLock)
            {
                return _storyStatusList.Values.DeepClone();
            }
        }

        public StorySyncWorkerStoryStatus GetStoryStatus(string storyID)
        {
            if (string.IsNullOrEmpty(storyID)) throw new ArgumentNullException("storyID");

            lock (_storyStatusLock)
            {
                return _GetStoryStatus(storyID);
            }
        }

        private StorySyncWorkerStoryStatus _GetStoryStatus(string storyID)
        {
            return _storyStatusList.ContainsKey(storyID)
                ? _storyStatusList[storyID].DeepClone()
                : _GetNewStoryStatus(storyID) ; // Either return the status, or return a new status object
        }

        private StorySyncWorkerStoryStatus _GetNewStoryStatus(string storyID)
        {
            return new StorySyncWorkerStoryStatus() { StoryID = storyID };

        }

        private void DoStoryStatusChange(StorySyncWorkerStoryStatus newStatus)
        {
            var oldStatus = GetStoryStatus(newStatus.StoryID);
            DoStoryStatusChange(oldStatus, newStatus);
        }

        private void DoStoryStatusChange(StorySyncWorkerStoryStatus oldStatus, StorySyncWorkerStoryStatus newStatus)
        {
            OnStoryStatusChange?.Invoke(this, new StorySyncWorkerStoryStatusEventArgs() { OldStatus = oldStatus, NewStatus = newStatus });
        }

        #endregion

        #region Worker status stuff


        private void DoWorkerStatusChange(StorySyncWorkerStatusEventArgs args)
        {
            OnWorkerStatusChange?.Invoke(this, args);
        }

        public StorySyncWorkerStatus GetCurrentStatus()
        {
            lock (_statusLock)
                return _status.DeepClone();
        }

        private void SetCurrentStatus(StorySyncWorkerState state, string msg)
        {
            var status = GetCurrentStatus();
            status.State = state;
            status.Message = msg;

            _SetCurrentStatus(status);
        }

        private void SetCurrentStatus(StorySyncWorkerState state, string msg, string storyID)
        {
            var status = GetCurrentStatus();
            status.State = state;
            status.Message = msg;
            status.CurrentStoryID = storyID;

            _SetCurrentStatus(status);
        }

        private void _SetCurrentStatus(StorySyncWorkerStatus newStatus)
        {
            var currentStatus = this.GetCurrentStatus();

            lock (_statusLock)
                _status = newStatus;


            var statusChangeArgs = new StorySyncWorkerStatusEventArgs() { NewStatus = newStatus };

            // Something has changed, send event
            DoWorkerStatusChange(statusChangeArgs);
        }

        #endregion

        private void SetSettings(StorySyncWorkerSettings settings)
        {
            lock (_settingsLock)
            {
                _settings = settings;
                Cancel(); // Restart the loop;
            }
        }

        private StorySyncWorkerSettings GetSettings()
        {
            lock (_settingsLock)
            {
                return _settings;
            }
        }

        private void Cancel()
        {
            _log.Info("Cancelling sync");
            _ctSource?.Cancel();
            RefreshCTSource();
        }

        private void RefreshCTSource()
        {
            _ctSource?.Dispose();
            _ctSource = new CancellationTokenSource();
        }

        // TODO: Sync now functionality, to be able to specify which story to sync right now

        public void StartWorker()
        {
            if (_workerThread != null)
            {
                _log.Warn("Something tried to start the sync worker after is has already been started");
                return;
            }
            
            Task newTask = new Task(() => SyncWorkerLoop(), _ctSource.Token);
            newTask.Start();

            _workerThread = newTask;
        }

        private async void SyncWorkerLoop()
        {
            _log.Info("Sync worker starting");
            var lastLoop = DateTime.Now - new TimeSpan(0, 0, 0, 0, GetSettings().WorkerLoopPauseMs);
            while (true)
            {
                // Should we pause?
                int pauseIntervalMs = GetSettings().WorkerLoopPauseMs;
                var msSinceLastLoop = (DateTime.Now - lastLoop).TotalMilliseconds;
                if (msSinceLastLoop < pauseIntervalMs)
                {
                    _log.Debug("Pausing until next loop");
                    int pauseDuration = pauseIntervalMs - Convert.ToInt32(msSinceLastLoop);
                    Thread.Sleep(pauseDuration);
                }
                lastLoop = DateTime.Now;

                // Are we stopped?
                if (GetCurrentStatus().State == StorySyncWorkerState.StoppedInvalidConfig) continue; // Go no further if we've been stopped

                // TODO add functionality to re-start the worker if it gets stopped.

                try
                {
                    await SyncWorkerMain(); // Do the thing
                }
                catch (OperationCanceledException)
                {
                    // Do nothing, would've been deliberately cancelled
                }
                catch (Exception ex)
                {
                    _log.Warn("Unhandled exception thrown by sync worker.", ex);
                }

                // Refresh the cancellation token source if it needs it
                if (_ctSource.IsCancellationRequested) RefreshCTSource();
            }
        }

        private async Task SyncWorkerMain()
        {
            var ct = _ctSource.Token;

            ct.ThrowIfCancellationRequested();

            // Get a fresh copy of the story collection
            var storyCollection = _storyContainer.GetAllStories();

            // Loop through stories in the story container
            //foreach (var story in storyCollection)
            while (_lastStoryIndex < storyCollection.Count)
            {
                ct.ThrowIfCancellationRequested();
                var story = storyCollection.ElementAt(_lastStoryIndex);

                var storyStatus = GetStoryStatus(story.ID);

                try
                {
                    // Skip this story if we're still waiting for the ITU pause to be over
                    if (storyStatus.State == StorySyncWorkerStoryState.WaitingItu)
                    {
                        var ituPauseDuration = GetSettings().ItuPauseDuration;
                        if ((storyStatus.LastItu + new TimeSpan(0, 0, ituPauseDuration)) > DateTime.Now) // if the last ITU + the pause duration is in the future
                        {
                            int secondsLeft = Convert.ToInt32((storyStatus.LastItu + new TimeSpan(0, 0, ituPauseDuration) - DateTime.Now).TotalSeconds);
                            _log.Debug($"Still waiting for ITU cooldown for '{story.ID}', {secondsLeft}s remaining.");
                            continue;
                        }
                    }

                    // Skip this story if it has an error
                    if (storyStatus.State == StorySyncWorkerStoryState.Error)
                    {
                        _log.Debug($"Skipping story which has experienced an error: {story.ID}");
                        continue;
                    }

                    // Lets do it
                    await SyncStory(story);
                }
                finally
                {
                    // Always up the index 
                    _lastStoryIndex++; // Up the index by 1
                }
            } // End of story loop block

            _lastStoryIndex = 0; // Reset the index if we're at the end of the story collection

            SetCurrentStatus(StorySyncWorkerState.Idle, "Idle", string.Empty);
        }

        public bool CheckIfStoryInfoNeedsSync(WdcInteractiveStory story)
        {
            return (story.LastUpdatedInfo + new TimeSpan(0, 0, GetSettings().SyncStoryIntervalSeconds, 0) < DateTime.Now);
        }

        public bool CheckIfChapterNeedsSync(WdcInteractiveChapter chapter)
        {
            return chapter.LastSynced == DateTime.MinValue
                || (GetSettings().UpdateKnownChapters && DateTime.Now >= chapter.LastSynced + new TimeSpan(0, 0, GetSettings().SyncChapterIntervalSeconds, 0));
        }

        public bool CheckIfChapterOutlineNeedsSync(WdcInteractiveStory story)
        {
            return (story.LastUpdatedChapterOutline + new TimeSpan(0, 0, GetSettings().SyncChapterOutlineIntervalSeconds, 0) < DateTime.Now);
        }

        private async Task SyncStory(WdcInteractiveStory story)
        {
            var ct = _ctSource.Token;

            // Update story's status, mark as in progress
            //SetStoryStatusState(story.ID, StorySyncWorkerStoryState.Working);
            SetCurrentStatus(StorySyncWorkerState.WorkingStory, $"Working on story: {story.ID}", story.ID);
            //SetStoryStatusProgress(story.ID, 0, 0);

            // Catch specific exceptions
            try
            {
                if (CheckIfStoryInfoNeedsSync(story))
                {
                    SetCurrentStatus(StorySyncWorkerState.WorkingStory, $"Updating story info: {story.ID}", story.ID);
                    await SyncStoryInfo(story);
                }
                else
                {
                    _log.Debug("Story info does not need updating");
                }

                if (CheckIfChapterOutlineNeedsSync(story))
                {
                    SetCurrentStatus(StorySyncWorkerState.WorkingOutline, $"Updating chapter outline: {story.ID}", story.ID);
                    await SyncStoryChapterList(story);
                }
                else
                {
                    _log.Debug("Story chapter outline does not need updating");
                }

                // Build a list of chapters that need updating
                SetCurrentStatus(StorySyncWorkerState.WorkingStory, $"Checking for any chapters that need syncing: {story.ID}", story.ID);
                List<WdcInteractiveChapter> chaptersToSync = new List<WdcInteractiveChapter>();
                foreach (var chapter in story.Chapters)
                {
                    if (CheckIfChapterNeedsSync(chapter)) chaptersToSync.Add(chapter);
                }

                ct.ThrowIfCancellationRequested();

                // If there are no chapters to update, set progress to maximum
                if (chaptersToSync.Count < 1)
                    SetStoryStatusProgress(story.ID, story.Chapters.Count, story.Chapters.Count);
                else
                    SetStoryStatusState(story.ID, StorySyncWorkerStoryState.Working); // About to start working


                // Start syncing the chapters
                for (var i = 0; i < chaptersToSync.Count; i++)
                {
                    var chapter = chaptersToSync[i];

                    // Update status

                    SetCurrentStatus(StorySyncWorkerState.WorkingChapter, $"Updating story chapter: {story.ID}, {chapter.Path}", story.ID);
                    SetStoryStatusProgress(story.ID, story.Chapters.Count - (chaptersToSync.Count - i), story.Chapters.Count);

                    // Sync the chapter
                    await SyncChapter(story, chapter);

                    // Save changes after updating the chapter
                    // TODO: is this too much? updating the entire sory when just a single chapter changes? Will it cause issues with stuff listening for container change events?
                    _storyContainer.UpdateStory(story); 
                }

                // Done sync for this story. Mark story status as idle
                SetStoryStatusState(story.ID, StorySyncWorkerStoryState.Idle);
                SetCurrentStatus(StorySyncWorkerState.Idle, $"Story update complete: {story.ID}", string.Empty);

            } // End of try block
            catch (InteractivesTemporarilyUnavailableException)
            {
                _log.Info($"Encountered ITU message for story: {story.ID}");
                SetStoryStatusState(story.ID, StorySyncWorkerStoryState.WaitingItu);
            }
            catch (WritingClientHtmlParseException ex)
            {
                _log.Warn("", ex);

                // Dump the HTML
                var dumpFilePath = _fileDumper.DumpFile(ex.Address, ex.HtmlResult);

                // Show an error message
                var sbExMsg = new StringBuilder();
                sbExMsg.AppendLine("A HTML parse exception was encountered with the below details:");
                sbExMsg.AppendLine("======================");
                sbExMsg.AppendLine(ex.Message);
                sbExMsg.AppendLine("======================");
                sbExMsg.AppendLine("The HTML response has been dumped to this location:");
                sbExMsg.AppendLine(dumpFilePath);

                _gui.ShowMessageBox("HTML parse exception", sbExMsg.ToString(), GuiMessageBoxIcon.Error);

                // Set the story's status to Error, so it doesn't keep trying to sync and cause more errors
                SetStoryStatusError(story.ID, ex.Message);
            }
            catch (Exception ex)
            {
                _log.Warn($"Encountered unhandled exception while syncing story '{story.ID}'", ex);

                var sb = new StringBuilder();
                sb.AppendLine(ex.GetType().Name);
                sb.AppendLine(ex.Message);

                SetStoryStatusError(story.ID, sb.ToString());
            }

            SetCurrentStatus(StorySyncWorkerState.Idle, $"Story update complete: {story.ID}", string.Empty);
        }

        // Update the story details
        private async Task SyncStoryInfo(WdcInteractiveStory story)
        {
            var ct = _ctSource.Token;
            

            _log.InfoFormat("Syncing story: {0}", story.ID);

            var remoteStory = await _wdcReader.GetInteractiveStory(story.ID, _wdcClient, _ctSource.Token);
            ct.ThrowIfCancellationRequested();

            // Bring over changes
            story.Author = remoteStory.Author;
            story.Name = remoteStory.Name;
            story.ShortDescription = remoteStory.ShortDescription;
            story.Description = remoteStory.Description;
            story.LastUpdatedInfo = DateTime.Now;

            // Save changes
            ct.ThrowIfCancellationRequested();
            _storyContainer.UpdateStory(story);
        }

        // Check the list of chapters in WDC. If there are some that we haven't seen before, add placeholder chapters to the story.
        private async Task SyncStoryChapterList(WdcInteractiveStory story)
        {
            var ct = _ctSource.Token;
            // TODO: should this be here? Isn't this done in the worker main loop?
            this.SetCurrentStatus(StorySyncWorkerState.WorkingOutline, $"Updating story chapter list: {story.ID}");

            var remoteChapterList = await _wdcReader.GetInteractiveChapterList(story.ID, _wdcClient, ct);
            ct.ThrowIfCancellationRequested();

            foreach (var uri in remoteChapterList)
            {
                var chapterPath = WdcUtil.GetFinalParmFromUrl(uri.ToString()); // Just want the final digits

                // Do we already have that chapter?
                if (!story.Chapters.Exists(c => c.Path == chapterPath))
                {
                    ct.ThrowIfCancellationRequested();

                    _log.InfoFormat("Creating placeholder for new chapter: {0}", chapterPath);

                    var newChapter = new WdcInteractiveChapter()
                    {
                        Path = chapterPath,
                        LastSynced = DateTime.MinValue
                    };

                    story.Chapters.Add(newChapter);
                }
            }

            ct.ThrowIfCancellationRequested();

            // Save changes
            story.LastUpdatedChapterOutline = DateTime.Now;
            _storyContainer.UpdateStory(story);
        }

        private async Task SyncChapter(WdcInteractiveStory story, WdcInteractiveChapter chapter)
        {
            var ct = _ctSource.Token;
            ct.ThrowIfCancellationRequested();

            // Update the story status
            // TODO

            // If the last synced value is minimum (always update) or chapter refreshing is enabled and this chapter needs a refresh
            if (
                chapter.LastSynced == DateTime.MinValue 
                || (GetSettings().UpdateKnownChapters && chapter.LastSynced <= DateTime.Now + new TimeSpan(0, 0, GetSettings().SyncChapterIntervalSeconds, 0))
                )
            {
                var remoteChapter = await _wdcReader.GetInteractiveChaper(story.ID, chapter.Path, _wdcClient, ct);
                ct.ThrowIfCancellationRequested();

                // Bring over changes
                chapter.Author = remoteChapter.Author;
                chapter.Choices = remoteChapter.Choices;
                chapter.Content = remoteChapter.Content;
                chapter.IsEnd = remoteChapter.IsEnd;
                chapter.SourceChoiceTitle = chapter.SourceChoiceTitle;
                chapter.Title = chapter.Title;

                chapter.LastSynced = DateTime.Now;
            }
        }
    }
}
