using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Exceptions;
using WritingExporter.Common.Models;
using WritingExporter.Common.Wdc;

namespace WritingExporter.Common.StorySyncWorker
{
    public class StorySyncWorker : IStorySyncWorker
    {
        private static ILogger _log = LogManager.GetLogger(typeof(StorySyncWorker));

        IConfigProvider _configProvider;
        IWdcStoryContainer _storyContainer;
        IWdcReader _wdcReader;
        IWdcClient _wdcClient;
        StorySyncWorkerSettings _settings;

        Task _workerThread;
        CancellationTokenSource _ctSource;
        StorySyncWorkerStatus _status;
        Dictionary<string, StorySyncWorkerStoryStatus> _storyStatusList;
        object _storyStatusLock = new object();
        object _statusLock = new object();
        object _settingsLock = new object();

        public event EventHandler<StorySyncWorkerStatusEventArgs> OnWorkerStatusChange;
        public event EventHandler<StorySyncWorkerStoryStatusEventArgs> OnStoryStatusChange;

        public StorySyncWorker(IWdcStoryContainer storyContainer, IWdcReader wdcReader, IWdcClient wdcClient, IConfigProvider configProvider)
        {
            _storyContainer = storyContainer;
            _wdcReader = wdcReader;
            _wdcClient = wdcClient;
            _configProvider = configProvider;
            _settings = configProvider.GetSection<StorySyncWorkerSettings>();

            // Init some stuff
            _storyStatusList = new Dictionary<string, StorySyncWorkerStoryStatus>();
            _ctSource = new CancellationTokenSource();
            _status = new StorySyncWorkerStatus();

            // TODO add functionality to react to configuration changes. E.g. Cancel process, update settings, start again.
        }

        #region Story status stuff

        private void SetStoryStatusState(string storyID, StorySyncWorkerStoryState newState)
        {
            var status = GetStoryStatus(storyID);
            status.State = newState;
            SetStoryStatus(status);
        }

        private void SetStoryStatusProgress(string storyID, int progressValue, int progressMax)
        {
            var status = GetStoryStatus(storyID);
            status.ProgressValue = progressValue;
            status.ProgressMax = progressMax;
            SetStoryStatus(status);
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
            if (_storyStatusList.ContainsKey(storyID))
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
            _ctSource.Cancel();
            RefreshCTSource();
        }

        private void RefreshCTSource()
        {
            _ctSource.Dispose();
            _ctSource = new CancellationTokenSource();
        }


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
            _log.Info("Sync worker loop starting");
            var lastLoop = DateTime.Now - new TimeSpan(0, 0, 0, GetSettings().WorkerLoopPauseMs);
            while (true)
            {
                // Should we pause?
                var msSinceLastLoop = (DateTime.Now - lastLoop).TotalMilliseconds;
                if (msSinceLastLoop < GetSettings().WorkerLoopPauseMs)
                {
                    _log.Debug("Pausing until next loop");
                    Thread.Sleep(GetSettings().WorkerLoopPauseMs - (int)msSinceLastLoop);
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
            foreach (var story in storyCollection)
            {
                ct.ThrowIfCancellationRequested();

                var storyStatus = GetStoryStatus(story.ID);

                // Skip this story if we're still waiting for the ITU pause to be over
                if (storyStatus.State == StorySyncWorkerStoryState.WaitingItu)
                {
                    if ((DateTime.Now - storyStatus.LastItu) > new TimeSpan(0, 0, GetSettings().ItuPauseDuration))
                    {
                        _log.Debug($"Still waiting for ITU cooldown: {story.ID}");
                        continue;
                    }
                }

                // Update story's status, mark as in progress
                SetStoryStatusState(story.ID, StorySyncWorkerStoryState.Working);
                this.SetStoryStatusProgress(story.ID, 0, 0);

                // Catch specific exceptions
                try
                {
                    if (CheckIfStoryInfoNeedsSync(story))
                    {
                        this.SetCurrentStatus(StorySyncWorkerState.WorkingStory, $"Updating story info: {story.ID}");
                        await SyncStoryInfo(story);
                    }

                    if (CheckIfChapterOutlineNeedsSync(story))
                    {
                        this.SetCurrentStatus(StorySyncWorkerState.WorkingOutline, $"Updating chapter outline: {story.ID}");
                        await SyncStoryChapterList(story);
                    }

                    // Build a list of chapters that need updating
                    this.SetCurrentStatus(StorySyncWorkerState.WorkingStory, $"Checking for any chapters that need syncing: {story.ID}");
                    List<WdcInteractiveChapter> chaptersToSync = new List<WdcInteractiveChapter>();
                    foreach (var chapter in story.Chapters)
                    {
                        if (CheckIfChapterNeedsSync(chapter)) chaptersToSync.Add(chapter);
                    }

                    ct.ThrowIfCancellationRequested();

                    // Start syncing the chapters
                    for (var i = 0; i < chaptersToSync.Count; i++)
                    {
                        var chapter = chaptersToSync[i];

                        // Update status

                        this.SetCurrentStatus(StorySyncWorkerState.WorkingChapter, $"Updating story chapter: {story.ID}, {chapter.Path}");
                        SetStoryStatusProgress(story.ID, story.Chapters.Count - i, story.Chapters.Count);

                        // Sync the chapter
                        await SyncChapter(story, chapter);
                    }

                    // Done sync for this story. Mark story status as idle
                    SetStoryStatusState(story.ID, StorySyncWorkerStoryState.Idle);
                } // End of try block
                catch (InteractivesTemporarilyUnavailableException)
                {
                    _log.Info($"Encountered ITU message for story: ${story.ID}");
                    SetStoryStatusState(story.ID, StorySyncWorkerStoryState.WaitingItu);
                }
            } // End of story loop block


        }

        private bool CheckIfStoryInfoNeedsSync(WdcInteractiveStory story)
        {
            return (story.LastUpdatedInfo <= DateTime.Now + new TimeSpan(0, 0, GetSettings().SyncStoryIntervalSeconds, 0));
        }

        private bool CheckIfChapterNeedsSync(WdcInteractiveChapter chapter)
        {
            return chapter.LastSynced == DateTime.MinValue
                || (GetSettings().UpdateKnownChapters && chapter.LastSynced <= DateTime.Now + new TimeSpan(0, 0, GetSettings().SyncChapterIntervalSeconds, 0));
        }

        private bool CheckIfChapterOutlineNeedsSync(WdcInteractiveStory story)
        {
            return story.LastUpdatedChapterOutline <= DateTime.Now + new TimeSpan(0, 0, GetSettings().SyncChapterOutlineIntervalSeconds, 0);
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

                }
            }

            ct.ThrowIfCancellationRequested();

            // Save changes
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
