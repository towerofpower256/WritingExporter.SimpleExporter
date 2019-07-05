using System;

namespace WritingExporter.Common.StorySyncWorker
{
    public interface IWdcStorySyncWorker
    {
        event EventHandler<WdcStorySyncWorkerStatusEventArgs> OnWorkerStatusChange;

        WdcStorySyncWorkerStatus GetCurrentStatus();
        void StartWorker();
    }
}