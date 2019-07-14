using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.StorySyncWorker
{
    public class DummyStorySyncWorker : IWdcStorySyncWorker
    {
        StorySyncWorkerStatus _status;

        public DummyStorySyncWorker()
        {
            _status = new StorySyncWorkerStatus()
            {
                Message = "Dummy sync worker, will do nothing."
            };
        }

        public event EventHandler<StorySyncWorkerStatusEventArgs> OnWorkerStatusChange;

        public StorySyncWorkerStatus GetCurrentStatus()
        {
            return _status;
        }

        public void StartWorker()
        {
            // Do nothing
        }
    }
}
