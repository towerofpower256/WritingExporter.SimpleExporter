using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritingExporter.Common.Models;

namespace WritingExporter.Common
{
    /// <summary>
    /// Thread-safe collection for holding interactive stories.
    /// </summary>
    public class WdcStoryContainer
    {
        public event EventHandler<WdcStoryContainerEventArgs> OnUpdate;

        ICollection<WdcInteractiveStory> _storyCollection;
        object _lock;

        public WdcStoryContainer()
        {
            _storyCollection = new Collection<WdcInteractiveStory>();
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
                //foreach (var story in _storyCollection)
                //{
                //    newList.Add(story.DeepClone());
                //}

                return _storyCollection.DeepClone();
            }

            //return newList;
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
            return _storyCollection.Where(s => s.ID == storyID).SingleOrDefault();
        }

        public void AddStory(WdcInteractiveStory newStory)
        {
            lock (_lock)
            {
                if (_HasStory(newStory.ID))
                    throw new Exception($"A story with the ID of '{newStory.ID}' already exists.");

                _storyCollection.Add(newStory);
            }

            DoEvent(newStory.ID, WdcStoryContainerEventType.Add);
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
            var existingStory = _GetStory(storyID);
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
                _storyCollection.Add(newStory);
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

}
