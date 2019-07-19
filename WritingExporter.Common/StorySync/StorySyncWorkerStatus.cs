using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.StorySync
{
    
    [Serializable]
    public class StorySyncWorkerStatus : ICloneable
    {
        public StorySyncWorkerState State { get; set; } = StorySyncWorkerState.Idle;
        public string Message { get; set; }
        public string CurrentStoryID { get; set; }

        public object Clone()
        {
            return this.DeepClone();
        }
    }
}
