using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritingExporter.Common.Configuration;

namespace WritingExporter.Common.StorySync
{
    public class DummyStorySyncWorker : IStorySyncWorker
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
        public event EventHandler<StorySyncWorkerStoryStatusEventArgs> OnStoryStatusChange;

        public IEnumerable<StorySyncWorkerStoryStatus> GetAllStoryStatus()
        {
            return new StorySyncWorkerStoryStatus[0];
        }

        public StorySyncWorkerStatus GetCurrentStatus()
        {
            return _status;
        }

        public StorySyncWorkerSettings GetSettings()
        {
            return new StorySyncWorkerSettings();
        }

        public StorySyncWorkerStoryStatus GetStoryStatus(string storyID)
        {
            return new StorySyncWorkerStoryStatus() { StoryID = storyID };
        }

        public void StartWorker()
        {
            // Do nothing
        }
    }
}
