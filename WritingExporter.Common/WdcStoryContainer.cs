using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.Common.Models;
using WritingExporter.Common.Storage;

namespace WritingExporter.Common
{
    /// <summary>
    /// Thread-safe collection for holding interactive stories.
    /// </summary>
    public class WdcStoryContainer : IWdcStoryContainer, IDisposable
    {
        private const int SAVE_CHECK_INTERVAL_MS = 2000; // How often to check if a story needs saving.
        private const string SAVE_DIR = "Stories"; // Relative directory to save stories to.

        private static readonly ILogger _log = LogManager.GetLogger(typeof(WdcStoryContainer));

        public event EventHandler<WdcStoryContainerEventArgs> OnUpdate;

        // Services
        IStoryFileStore _fileStore;

        //Private members
        ICollection<WdcStoryContainerWrapper> _storyCollection;
        object _lock = new object();
        Task _saveCheckWorker;
        CancellationTokenSource _ctSource;
        bool _started = false;

        public WdcStoryContainer(IStoryFileStore fileStore)
        {
            _fileStore = fileStore;
            _storyCollection = new Collection<WdcStoryContainerWrapper>();
        }

        public void Start()
        {
            if (_started) throw new InvalidOperationException("The story container has already been started, and cannot be started again.");

            _log.Debug("Starting");
            LoadAllStories();
            StartSaveWorker();
            _started = true;
        }

        public void Dispose()
        {
            StopSaveWorker();
        }

        private void StartSaveWorker()
        {
            if (_saveCheckWorker != null)
                throw new Exception("Cannot start the save check worker after it has already been started");

            _ctSource = new CancellationTokenSource();
            var newWorkerTask = new Task(() => SaveCheckWorkerBrain());
            newWorkerTask.Start();
            _saveCheckWorker = newWorkerTask;
        }

        private void StopSaveWorker()
        {
            if (_saveCheckWorker == null)
            {
                return; // Do nothing
            }

            _ctSource.Cancel();
        }

        // Brain function that the save check worker will use
        private void SaveCheckWorkerBrain()
        {
            _log.Debug("Starting save check worker");
            var ct = _ctSource.Token;
            while (true)
            {
                try
                {
                    _log.Debug("Checking if any stories need saving");

                    ct.ThrowIfCancellationRequested();

                    foreach (var sw in _storyCollection)
                    {
                        ct.ThrowIfCancellationRequested();
                        if (sw.NeedsSave)
                        {
                            SaveStory(sw.Story);
                            sw.NeedsSave = false;
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    _log.Debug("Stopping save check worker");
                    return;
                }
                catch (Exception ex)
                {
                    _log.Warn("Exception in save check worker!", ex);
                    throw ex;
                }
                finally
                {
                    // Pausing, always
                    ct.ThrowIfCancellationRequested();
                    Thread.Sleep(SAVE_CHECK_INTERVAL_MS);
                }
            }
        }

        private void SaveStory(WdcInteractiveStory story)
        {
            _fileStore.SaveStory(story);
        }

        private void LoadStory(string filename)
        {
            _log.Debug($"Loading story: {filename}");
            this.LoadStory(_fileStore.LoadStory(filename));
        }

        private void LoadAllStories()
        {
            _log.Debug("Loading all stories");
            foreach (var story in _fileStore.LoadAllStories())
            {
                AddStory(story, false);
            }
        }

        private void DeleteStory(WdcInteractiveStory story)
        {
            _log.Debug($"Deleting story: {story.ID}");
            _fileStore.DeleteStory(story);
        }

        private WdcStoryContainerWrapper GetNewWrapper(WdcInteractiveStory story)
        {
            return new WdcStoryContainerWrapper()
            {
                Story = story,
                NeedsSave = false
            };
        }

        private void DoEvent(string storyID, WdcStoryContainerEventType eventType)
        {
            OnUpdate?.Invoke(this, new WdcStoryContainerEventArgs() { StoryID = storyID, EventType = eventType });
        }

        #region StoryAccess
        public ICollection<WdcInteractiveStory> GetAllStories()
        {
            var newList = new Collection<WdcInteractiveStory>();

            lock (_lock)
            {
                foreach (var sw in _storyCollection)
                {
                    newList.Add(sw.Story.DeepClone());
                }
            }

            return newList;
        }

        public bool HasStory(string storyID)
        {
            lock (_lock)
            {
                return _HasStory(storyID);
            }
        }

        private bool _HasStory(string storyID)
        {
            return _GetStory(storyID) != null;
        }

        public WdcInteractiveStory GetStory(string storyID)
        {
            lock (_lock)
            {
                return _GetStory(storyID);
            }
        }

        private WdcInteractiveStory _GetStory(string storyID)
        {
            var storyEntry = _storyCollection.Where(s => s.Story.ID == storyID).SingleOrDefault();
            return storyEntry?.Story;
        }

        /// <summary>
        /// Add a story to the container.
        /// </summary>
        /// <param name="newStory"></param>
        public void AddStory(WdcInteractiveStory newStory, bool needsSave)
        {
            
            lock (_lock)
            {
                _AddStory(newStory, needsSave);
            }

            DoEvent(newStory.ID, WdcStoryContainerEventType.Add);
        }

        /// <summary>
        /// Add a story to the container.
        /// </summary>
        /// <param name="newStory"></param>
        public void AddStory(WdcInteractiveStory newStory)
        {
            AddStory(newStory, true);
        }

        /// <summary>
        /// Add a story to the container, that doesn't need saving right away, and don't trigger any events.
        /// </summary>
        /// <param name="story"></param>
        private void LoadStory(WdcInteractiveStory newStory)
        {
            lock (_lock)
            {
                _AddStory(newStory, false);
            }
        }

        private void _AddStory(WdcInteractiveStory newStory, bool needsSave)
        {
            if (_HasStory(newStory.ID))
                throw new ArgumentException($"A story with the ID of '{newStory.ID}' already exists.");

            var sw = GetNewWrapper(newStory);
            sw.NeedsSave = needsSave;
            _storyCollection.Add(sw);
        }

        public void RemoveStory(string storyID)
        {
            lock (_lock)
            {
                _RemoveStory(storyID, true);
            }

            DoEvent(storyID, WdcStoryContainerEventType.Remove);
        }

        private void _RemoveStory(string storyID, bool deleteFile = true)
        {
            var existingStory = _storyCollection.Where(s => s.Story.ID == storyID).SingleOrDefault();
            if (existingStory == null)
                throw new ArgumentOutOfRangeException($"A story with the ID of '{storyID}' does not exist.");

            _storyCollection.Remove(existingStory);
        }

        public void UpdateStory(WdcInteractiveStory newStory)
        {
            lock (_lock)
            {
                var existingStory = _GetStory(newStory.ID);
                if (existingStory == null)
                    throw new ArgumentOutOfRangeException($"A story with the ID of '{newStory.ID}' does not exist.");

                // Copy it over, including all the chapters
                // Remove the old story, add in the replacement
                _RemoveStory(newStory.ID, false);
                _AddStory(newStory, true);
            }

            DoEvent(newStory.ID, WdcStoryContainerEventType.Update);
        }

        #endregion
    }

    public class WdcStoryContainerEventArgs
    {
        public string StoryID { get; set; }
        public WdcStoryContainerEventType EventType { get; set; }
    }

    public enum WdcStoryContainerEventType
    {
        Add,
        Remove,
        Update
    }

    public class WdcStoryContainerWrapper
    {
        public WdcInteractiveStory Story { get; set; }
        public bool NeedsSave { get; set; } = false;
    }

}
