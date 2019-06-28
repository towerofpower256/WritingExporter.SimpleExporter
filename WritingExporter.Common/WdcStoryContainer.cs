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
    public class WdcStoryContainer
    {
        private const int SAVE_CHECK_INTERVAL_MS = 2000; // How often to check if a story needs saving.
        private const string SAVE_DIR = "Stories"; // Relative directory to save stories to.

        private static readonly ILogger _log = LogManager.GetLogger(typeof(WdcStoryContainer));

        public event EventHandler<WdcStoryContainerEventArgs> OnUpdate;

        // Services
        IStoryFileStore _fileStore;

        //Private members
        ICollection<WdcStoryContainerWrapper> _storyCollection;
        object _lock;
        Task _saveCheckWorker;

        public WdcStoryContainer(IStoryFileStore fileStore)
        {
            _fileStore = fileStore;
            _storyCollection = new Collection<WdcStoryContainerWrapper>();

            StartSaveWorker();
        }

        private void StartSaveWorker()
        {
            if (_saveCheckWorker != null)
                throw new Exception("Cannot start the save check worker after it has already been started");
        }

        // Brain function that the save check worker will use
        private void SaveCheckWorkerBrain()
        {
            _log.Debug("Starting save check worker");
            while (true)
            {
                _log.Debug("Checking if any stories need saving");

                foreach (var sw in _storyCollection)
                {
                    if (sw.NeedsSave)
                    {
                        SaveStory(sw.Story);
                    }
                }

                // Pausing
                Thread.Sleep(SAVE_CHECK_INTERVAL_MS);
            }
        }

        private void SaveStory(WdcInteractiveStory story)
        {
            _fileStore.SaveStory(story);
        }

        private void LoadStory(string filename)
        {
            _log.Debug($"Loading story: {filename}");
            this.AddStory(_fileStore.LoadStory(filename));
        }

        private void LoadAllStories()
        {
            _log.Debug("Loading all stories");
            foreach (var story in _fileStore.LoadAllStories())
            {
                AddStory(story);
            }
        }

        private WdcStoryContainerWrapper GetNewWrapper(WdcInteractiveStory story)
        {
            return new WdcStoryContainerWrapper()
            {
                Story = story,
                NeedsSave = true
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
            return _storyCollection.Where(s => s.Story.ID == storyID).SingleOrDefault().Story;
        }

        public void AddStory(WdcInteractiveStory newStory)
        {
            lock (_lock)
            {
                _AddStory(newStory);
            }

            DoEvent(newStory.ID, WdcStoryContainerEventType.Add);
        }

        private void _AddStory(WdcInteractiveStory newStory)
        {
            if (_HasStory(newStory.ID))
                throw new Exception($"A story with the ID of '{newStory.ID}' already exists.");

            _storyCollection.Add(GetNewWrapper(newStory));
        }

        public void RemoveStory(string storyID)
        {
            lock (_lock)
            {
                _RemoveStory(storyID);
            }

            DoEvent(storyID, WdcStoryContainerEventType.Remove);
        }

        private void _RemoveStory(string storyID)
        {
            var existingStory = _storyCollection.Where(s => s.Story.ID == storyID).SingleOrDefault();
            if (existingStory == null)
                throw new Exception($"A story with the ID of '{storyID}' does not exist.");

            _storyCollection.Remove(existingStory);
        }

        public void UpdateStory(WdcInteractiveStory newStory)
        {
            lock (_lock)
            {
                var existingStory = _GetStory(newStory.ID);
                if (existingStory == null)
                    throw new Exception($"A story with the ID of '{newStory.ID}' does not exist.");

                // Copy it over, including all the chapters
                // Remove the old story, add in the replacement
                _RemoveStory(newStory.ID);
                _AddStory(newStory);
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
