using System;

namespace WritingExporter.Common.StorySyncWorker
{
    public interface IWdcStorySyncWorker
    {
        event EventHandler<StorySyncWorkerStatusEventArgs> OnWorkerStatusChange;

        StorySyncWorkerStatus GetCurrentStatus();
        void StartWorker();
    }
}