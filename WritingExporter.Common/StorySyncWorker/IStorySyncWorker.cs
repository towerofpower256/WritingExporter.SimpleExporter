using System;
using System.Collections.Generic;

namespace WritingExporter.Common.StorySyncWorker
{
    public interface IStorySyncWorker
    {
        event EventHandler<StorySyncWorkerStatusEventArgs> OnWorkerStatusChange;
        event EventHandler<StorySyncWorkerStoryStatusEventArgs> OnStoryStatusChange;

        StorySyncWorkerStatus GetCurrentStatus();
        IEnumerable<StorySyncWorkerStoryStatus> GetAllStoryStatus();
        StorySyncWorkerStoryStatus GetStoryStatus(string storyID);

        void StartWorker();
    }
}