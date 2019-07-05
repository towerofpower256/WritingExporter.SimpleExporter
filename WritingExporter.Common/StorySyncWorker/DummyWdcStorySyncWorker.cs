using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.StorySyncWorker
{
    public class DummyWdcStorySyncWorker : IWdcStorySyncWorker
    {
        WdcStorySyncWorkerStatus _status;

        public DummyWdcStorySyncWorker()
        {
            _status = new WdcStorySyncWorkerStatus()
            {
                Message = "Dummy sync worker, will do nothing."
            };
        }

        public event EventHandler<WdcStorySyncWorkerStatusEventArgs> OnWorkerStatusChange;

        public WdcStorySyncWorkerStatus GetCurrentStatus()
        {
            return _status;
        }

        public void StartWorker()
        {
            // Do nothing
        }
    }
}
