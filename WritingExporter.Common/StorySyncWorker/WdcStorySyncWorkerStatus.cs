using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.StorySyncWorker
{
    [Serializable]
    public class WdcStorySyncWorkerStatus : ICloneable
    {
        public WdcStorySyncWorkerState State { get; set; } = WdcStorySyncWorkerState.Idle;
        public string Message { get; set; }
        public int ProgressValue { get; set; } = 0;
        public int ProgressMax { get; set; } = 0;
        public string CurrentStoryId { get; set; }
        public string CurrentChapterId { get; set; }
        public Dictionary<string, StoryStatusEntry> StoryStatus = new Dictionary<string, StoryStatusEntry>();

        public object Clone()
        {
            return this.DeepClone();
        }

        [Serializable]
        public class StoryStatusEntry
        {
            public string StoryID { get; set; }
            public StoryStatusEntryState State { get; set; }
            public DateTime LastItu { get; set; } // That last time we saw "Interactives Temporarily Unavailable"
            public string ErrorMessage { get; set; }
        }

        public enum StoryStatusEntryState
        {
            Idle,
            WaitingItu,
            Working,
            Error
        }
    }
}
