using System;
using System.Collections.Generic;
using WritingExporter.Common.Configuration;

namespace WritingExporter.Common.StorySync
{
    public interface IStorySyncWorker
    {
        event EventHandler<StorySyncWorkerStatusEventArgs> OnWorkerStatusChange;
        event EventHandler<StorySyncWorkerStoryStatusEventArgs> OnStoryStatusChange;

        StorySyncWorkerStatus GetCurrentStatus();
        StorySyncWorkerSettings GetSettings();
        IEnumerable<StorySyncWorkerStoryStatus> GetAllStoryStatus();
        StorySyncWorkerStoryStatus GetStoryStatus(string storyID);

        void StartWorker();
    }
}