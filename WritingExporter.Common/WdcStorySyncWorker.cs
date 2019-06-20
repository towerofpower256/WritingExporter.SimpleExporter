using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.Common.Models;

namespace WritingExporter.Common
{
    public class WdcStorySyncWorker
    {
        private static ILogger _log = LogManager.GetLogger(typeof(WdcStorySyncWorker));

        WdcStoryContainer _storyContainer;
        IWdcReader _wdcReader;
        IWdcClient _wdcClient;
        WdcStorySyncWorkerSettings _settings;

        Task _workerThread;
        CancellationTokenSource _ctSource;
        WdcStorySyncWorkerStatus _status;
        object _statusLock;
        object _settingsLock;
        bool _syncEnabled;

        public event EventHandler OnWorkerStatusChange;

        public WdcStorySyncWorker(WdcStoryContainer storyContainer, IWdcReader wdcReader, IWdcClient wdcClient, WdcStorySyncWorkerSettings settings)
        {
            _storyContainer = storyContainer;
            _wdcReader = wdcReader;
            _wdcClient = wdcClient;
            _settings = settings;

            // Init some stuff
            _ctSource = new CancellationTokenSource();
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

            SetCurrentStatus(status);
        }

        private void SetCurrentStatus(WdcStorySyncWorkerStatus status)
        {
            lock (_statusLock)
                _status = status;
        }

        private void SetCurrentStatusProgress(int progressValue, int progressMax)
        {
            var status = GetCurrentStatus();
            status.ProgressValue = progressValue;
            status.ProgressMax = progressMax;
            SetCurrentStatus(status);
        }

        public void StartWorker()
        {
            if (_workerThread != null)
            {
                _log.Warn("Something tried to start the sync worker after is has already been started");
                return;
            }
            
            Task newTask = new Task(() => SyncWorkerLoop(), _ctSource.Token);
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

    public class WdcStorySyncWorkerSettings
    {
        // Refresh story details after this many seconds.
        // Default: 10 days
        public int SyncStoryIntervalSeconds { get; set; } = 864000;

        // Refresh chapter details after this many seconds.
        // Default: 10 days
        public int SyncChapterIntervalSeconds { get; set; } = 864000;

        // Should chapters that have already been scraped be updated again? Default: no
        public bool UpdateKnownChapters { get; set; } = false;

        // A loop of the worker should only occur no faster than this.
        // This is to prevent the loop going full-speed and consuming all CPU
        // Default: 1 second
        public int WorkerLoopPauseMs { get; set; } = 1000;

        // Is sync enabled?
        // Default: yes
        public bool SyncEnabled { get; set; } = true;
    }

    [Serializable]
    public class WdcStorySyncWorkerStatus : ICloneable
    {
        public WdcStorySyncWorkerState State { get; set; } = WdcStorySyncWorkerState.Idle;
        public string Message { get; set; }
        public int ProgressValue { get; set; } = 0;
        public int ProgressMax { get; set; } = 0;
        public string CurrentStoryId { get; set; }
        public string CurrentChapterId { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }

    public enum WdcStorySyncWorkerState
    {
        Idle,
        IdleItu, //Interactives Temporarily Unavailable
        WorkingStory,
        WorkingOutline,
        WorkingChapter,
    }

}
