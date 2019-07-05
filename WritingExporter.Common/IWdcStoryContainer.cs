using System;
using System.Collections.Generic;
using WritingExporter.Common.Models;

namespace WritingExporter.Common
{
    public interface IWdcStoryContainer
    {
        event EventHandler<WdcStoryContainerEventArgs> OnUpdate;

        void AddStory(WdcInteractiveStory newStory);
        ICollection<WdcInteractiveStory> GetAllStories();
        WdcInteractiveStory GetStory(string storyID);
        bool HasStory(string storyID);
        void RemoveStory(string storyID);
        void UpdateStory(WdcInteractiveStory newStory);
        void Start();
    }
}