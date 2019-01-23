using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.SimpleExporter.Exceptions;
using WritingExporter.SimpleExporter.Models;

namespace WritingExporter.SimpleExporter
{
    public class WStoryExporterUpdateEventArgs : EventArgs
    {
        public bool MessageUpdated { get; set; } = false;
        public string Message { get; set; }
        public bool ProgressUpdated { get; set; } = false;
        public int ProgressValue { get; set; }
        public int ProgressMax { get; set; }
    }

    public class WStoryExporterExportSettings
    {
        public bool UpdateExisting { get; set; } = false;
    }

    /// <summary>
    /// Class used to export stories.
    /// This one is pretty slim, it just grabs the story.
    /// </summary>
    public class WStoryExporter
    {
        /// <summary>
        /// When you get the "interactives temporarily unavailable" message from Writing.com, how long to pause in milliseconds.
        /// </summary>
        public const int ITU_PAUSE_MS = 15000; // 30s

        private static ILogger _log = LogManager.GetLogger(typeof(WStoryExporter));

        private WritingClient _wc;
        private WInteractiveStory _story;
        private CancellationTokenSource _cancelTokenSource;
        private WStoryExporterProgress _progress;

        public bool IsExporting { get; private set; }

        public event EventHandler<WStoryExporterUpdateEventArgs> OnStatusUpdate;

        public WStoryExporter(WritingClient writingClient)
        {
            _wc = writingClient;
            Init();
        }

        private string GetStateUpdateMsg(string msg)
        {
            if (_progress != null)
            {
                return string.Format($"({_progress.ProgressIndex + 1}/{_progress.ProgressMax}) {msg}");
            }
            else
            {
                return msg;
            }
        }

        private void DoStateUpdate(string msg)
        {
            OnStatusUpdate?.Invoke(this, new WStoryExporterUpdateEventArgs()
            {
                MessageUpdated = true,
                Message = GetStateUpdateMsg(msg)
            });
        }

        private void DoStateUpdate(string msg, int progressValue, int progressMax)
        {
            OnStatusUpdate?.Invoke(this, new WStoryExporterUpdateEventArgs()
            {
                MessageUpdated = true,
                Message = GetStateUpdateMsg(msg),
                ProgressUpdated = true,
                ProgressValue = progressValue,
                ProgressMax = progressMax
            });
        }

        public async Task<WInteractiveStory> ExportStory(WInteractiveStory story, WStoryExporterExportSettings exportSettings)
        {
            var cancelToken = _cancelTokenSource.Token;
            _story = story;

            if (IsExporting)
                throw new Exception("Cannot export a story while already exporting a story");

            IsExporting = true;

            try
            {
                DoStateUpdate("Exporting story", 0, 1);

                // Get the story map
                await UpdateStory();

                // Cancellation support
                if (cancelToken.IsCancellationRequested)
                    return null;

                // Update all the stories
                //foreach (var chapter in _story.Chapters)
                _progress = new WStoryExporterProgress();
                _progress.ProgressMax = _story.Chapters.Count;
                for (var i=0; i < _story.Chapters.Count; i++)
                {
                    // Cancellation support
                    if (cancelToken.IsCancellationRequested)
                        return null;

                    _progress.ProgressIndex = i;
                    var chapter = _story.Chapters[i];

                    if (string.IsNullOrEmpty(chapter.Content) || exportSettings.UpdateExisting == true)
                    {
                        DoStateUpdate($"Exporting chapter: {chapter.Path}", i, _story.Chapters.Count);

                        await UpdateChapter(chapter);
                    }
                }

                // Done
                _log.Info("Story export complete");
                _progress = null;
                DoStateUpdate("Finished exporting story, ready to save", 1, 1);
                return _story;
            }
            finally
            {
                IsExporting = false;
                _story = null;
            }
        }

        public void Cancel()
        {
            _log.Info("Cancelling story export");
            _cancelTokenSource?.Cancel();
            DoStateUpdate("Story export cancelled", 0, 1);
        }

        private void Init()
        {
            _cancelTokenSource = new CancellationTokenSource();
            _story = null;
            IsExporting = false;
        }

        private void ITUPause(string msg)
        {
            var cancelToken = _cancelTokenSource.Token;
            DateTime countDownTo = DateTime.Now + new TimeSpan(0, 0, 0, 0, ITU_PAUSE_MS);

            while (DateTime.Now < countDownTo && !cancelToken.IsCancellationRequested)
            {

                int secondsLeft = (int)Math.Floor((countDownTo - DateTime.Now).TotalSeconds);
                DoStateUpdate(string.Format(msg, secondsLeft));

                Thread.Sleep(200);
            }
        }

        private async Task UpdateStory()
        {
            _log.DebugFormat("Updating story: {0}", _story.UrlID);
            var cancelToken = _cancelTokenSource.Token;

            WInteractiveStory retrievedStory;
            while (true)
            {
                try
                {
                    retrievedStory = await _wc.GetInteractive(_story.UrlID);

                    break; // Got the chapter successfully, excape this crude retry loop
                }
                catch (InteractivesTemporarilyUnavailableException)
                {
                    _log.InfoFormat("Encountered 'interactives temporarily unavailable' page, pausing for {0}ms", ITU_PAUSE_MS);
                    ITUPause("Interactives temporarily unavailable, waiting {0}s");

                    // Cancellation support
                    if (cancelToken.IsCancellationRequested)
                        return;

                    // This method of pausing won't work that well when it comes to cancelling.
                    // It'll do for now, but we'll need something better
                    // E.g. it sents a "Pause until" DateTime, and keeps checking every second, or something.
                    /*
                    _log.DebugFormat("Encountered 'interactives temporarily unavailable' page, pausing for {0}ms", ITUPauseMS);
                    DoStateUpdate("Interactives temporarily unavailable, waiting before trying again");
                    Thread.Sleep(ITUPauseMS);
                    */
                }
            }

            // Cancellation support
            if (cancelToken.IsCancellationRequested)
                return;

            // Update the story
            _story.LastSynced = DateTime.Now;

            if (_story.Name != retrievedStory.Name
                || _story.Description != retrievedStory.Description
                || _story.ShortDescription != retrievedStory.ShortDescription
                )
            {
                _log.DebugFormat("Updating story details");

                _story.Name = retrievedStory.Name;
                _story.ShortDescription = retrievedStory.ShortDescription;
                _story.Description = retrievedStory.Description;
                _story.LastUpdated = DateTime.Now;
                _story.HasChanged = true;
            }

            // Cancellation support
            if (cancelToken.IsCancellationRequested)
                return;

            // Check if there's any new chapters that we haven't seen before
            //DoStatusUpdate("Checking for new chapters");
            Uri[] chapters = new Uri[0];

            while (true)
            {
                try
                {
                    DoStateUpdate("Retrieving chapter list");
                    chapters = await _wc.GetAllInteractiveChapterUrls(_story.UrlID);
                    break; // Got what we need, lets escape this loop
                }
                catch (InteractivesTemporarilyUnavailableException)
                {
                    _log.InfoFormat("Encountered 'interactives temporarily unavailable' page, pausing for {0}ms", ITU_PAUSE_MS);
                    ITUPause("Interactives temporarily unavailable, waiting {0}s");

                    // Cancellation support
                    if (cancelToken.IsCancellationRequested)
                        return;
                }
            }
            

            foreach (Uri chapterUrl in chapters)
            {
                // Cancellation support
                if (cancelToken.IsCancellationRequested)
                    return;

                // Do we already have this chapter?
                string chapterMapId = _wc.GetMapUrlParameter(chapterUrl.ToString());
                WInteractiveChapter localChapter = _story.Chapters.Find(c => c.Path == chapterMapId);

                if (localChapter == null)
                {
                    // Add a placeholder chapter to this story's list of chapters
                    // The sync worker should see this and get the rest of the details
                    _story.Chapters.Add(new WInteractiveChapter()
                    {
                        Path = _wc.GetMapUrlParameter(chapterUrl)
                    });
                }
            }
        }

        private async Task UpdateChapter(WInteractiveChapter localChapter)
        {
            _log.DebugFormat("Updating chapter {0}", localChapter.Path);

            var cancelToken = _cancelTokenSource.Token;

            Uri chapterUrl = _wc.GetInteractiveChapterUrl(_story.UrlID, localChapter.Path);

            WInteractiveChapter retrievedChapter;
            while (true)
            {
                // Cancellation support
                if (cancelToken.IsCancellationRequested)
                    return;

                try
                {
                    retrievedChapter = await _wc.GetInteractiveChapter(chapterUrl);
                    break; // Got the chapter successfully, excape this crude retry loop
                }
                catch (InteractivesTemporarilyUnavailableException)
                {
                    _log.InfoFormat("Encountered 'interactives temporarily unavailable' page, pausing for {0}ms", ITU_PAUSE_MS);
                    ITUPause("Interactives temporarily unavailable, waiting {0}s");

                    // Cancellation support
                    if (cancelToken.IsCancellationRequested)
                        return;
                }
            }

            // Cancellation support
            if (cancelToken.IsCancellationRequested)
                return;

            // Got the chapter, lets sync and merge the changes into the local copy
            //log.Debug("Updating chapter");
            localChapter.LastSynced = DateTime.Now;

            _story.HasChanged = true;
            localChapter.Author = retrievedChapter.Author;
            localChapter.Choices = retrievedChapter.Choices;
            localChapter.Content = retrievedChapter.Content;
            localChapter.IsEnd = retrievedChapter.IsEnd;
            if (localChapter.Path == String.Empty) localChapter.Path = retrievedChapter.Path; // Should I risk changing this?
            localChapter.SourceChoiceTitle = retrievedChapter.SourceChoiceTitle;
            localChapter.Title = retrievedChapter.Title;
            localChapter.VersionFoundAt = retrievedChapter.VersionFoundAt;
        }

        private class WStoryExporterProgress
        {
            public int ProgressMax { get; set; }
            public int ProgressIndex { get; set; }
        }
    }
}
