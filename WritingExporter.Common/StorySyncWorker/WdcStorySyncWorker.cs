using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Models;
using WritingExporter.Common.Wdc;

namespace WritingExporter.Common.StorySyncWorker
{
    public class WdcStorySyncWorker : IWdcStorySyncWorker
    {
        private static ILogger _log = LogManager.GetLogger(typeof(WdcStorySyncWorker));

        IConfigProvider _configProvider;
        IWdcStoryContainer _storyContainer;
        IWdcReader _wdcReader;
        IWdcClient _wdcClient;
        WdcStorySyncWorkerSettings _settings;

        Task _workerThread;
        CancellationTokenSource _ctSource;
        WdcStorySyncWorkerStatus _status;
        object _statusLock = new object();
        object _settingsLock = new object();

        public event EventHandler<WdcStorySyncWorkerStatusEventArgs> OnWorkerStatusChange;

        public WdcStorySyncWorker(IWdcStoryContainer storyContainer, IWdcReader wdcReader, IWdcClient wdcClient, IConfigProvider configProvider)
        {
            _storyContainer = storyContainer;
            _wdcReader = wdcReader;
            _wdcClient = wdcClient;
            _configProvider = configProvider;
            _settings = configProvider.GetSection<WdcStorySyncWorkerSettings>();

            // Init some stuff
            _ctSource = new CancellationTokenSource();
            _status = new WdcStorySyncWorkerStatus();

            // TODO add functionality to react to configuration changes. E.g. Cancel process, update settings, start again.
        }

        public void SetSettings(WdcStorySyncWorkerSettings settings)
        {
            lock (_settingsLock)
            {
                _settings = settings;
                Cancel(); // Restart the loop;
            }
        }

        public WdcStorySyncWorkerSettings GetSettings()
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

        private void DoWorkerStatusChange(WdcStorySyncWorkerStatusEventArgs args)
        {
            OnWorkerStatusChange?.Invoke(this, args);
        }

        public WdcStorySyncWorkerStatus GetCurrentStatus()
        {
            lock (_statusLock)
                return _status.DeepClone();
        }

        private void SetCurrentStatus(WdcStorySyncWorkerState state, string msg)
        {
            var status = GetCurrentStatus();
            status.State = state;
            status.Message = msg;
            status.CurrentChapterId = String.Empty;
            status.CurrentStoryId = String.Empty;
            status.ProgressValue = 0;
            status.ProgressMax = 0;

            _SetCurrentStatus(status);
        }

        

        private void SetCurrentStatusProgress(int progressValue, int progressMax)
        {
            var status = GetCurrentStatus();
            status.ProgressValue = progressValue;
            status.ProgressMax = progressMax;
            _SetCurrentStatus(status);
        }

        private void _SetCurrentStatus(WdcStorySyncWorkerStatus newStatus)
        {
            var currentStatus = this.GetCurrentStatus();

            lock (_statusLock)
                _status = newStatus;

            //Compare, and trigger event if nessessary
            var statusChangeArgs = new WdcStorySyncWorkerStatusEventArgs();

            if (newStatus.State != currentStatus.State) statusChangeArgs.StateChanged = true;
            if (newStatus.ProgressMax != currentStatus.ProgressMax || newStatus.ProgressValue != currentStatus.ProgressValue) statusChangeArgs.ProgressChanged = true;
            if (newStatus.Message != currentStatus.Message) statusChangeArgs.MessageChanged = true;
            if (newStatus.CurrentStoryId != currentStatus.CurrentStoryId) statusChangeArgs.CurrentStoryChanged = true;
            if (newStatus.CurrentChapterId != currentStatus.CurrentChapterId) statusChangeArgs.CurrentChapterChanged = true;

            if (statusChangeArgs.StateChanged 
                || statusChangeArgs.ProgressChanged 
                || statusChangeArgs.MessageChanged 
                || statusChangeArgs.CurrentStoryChanged 
                || statusChangeArgs.CurrentChapterChanged)
            {
                // Something has changed, send event
                DoWorkerStatusChange(statusChangeArgs);
            }

            
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
                if (GetCurrentStatus().State == WdcStorySyncWorkerState.StoppedInvalidConfig) continue; // Go no further if we've been stopped

                // TODO add functionality to re-start the worker if it gets stopped.

                try
                {
                    await SyncWorkerMain(); // Do the thing
                }
                catch (OperationCanceledException)
                {
                    // Do nothing
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

            // Get a fresh copy of the story collection
            var storyCollection = _storyContainer.GetAllStories();

            // Loop through stories in the story container
            foreach (var story in storyCollection)
            {
                await SyncStory(story);

                await SyncStoryChapterList(story);

                foreach (var chapter in story.Chapters)
                {
                    await SyncChapter(story, chapter);
                }

            }


        }

        // Update the story details
        private async Task SyncStory(WdcInteractiveStory story)
        {
            var ct = _ctSource.Token;
            // If the story hasn't been updated in a while, or hasn't been updated ever, update the story details

            if (story.LastUpdated <= DateTime.Now + new TimeSpan(0, 0, GetSettings().SyncStoryIntervalSeconds, 0))
            {
                _log.InfoFormat("Story {0} needs updating", story.ID);

                var remoteStory = await _wdcReader.GetInteractiveStory(story.ID, _wdcClient, _ctSource.Token);
                ct.ThrowIfCancellationRequested();

                // Bring over changes
                story.Author = remoteStory.Author;
                story.Name = remoteStory.Name;
                story.ShortDescription = remoteStory.ShortDescription;
                story.Description = remoteStory.Description;
                story.LastUpdated = DateTime.Now;

                // Save changes
                ct.ThrowIfCancellationRequested();
                _storyContainer.UpdateStory(story);
            }
        }

        // Check the list of chapters in WDC. If there are some that we haven't seen before, add placeholder chapters to the story.
        private async Task SyncStoryChapterList(WdcInteractiveStory story)
        {
            var ct = _ctSource.Token;

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
