using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.StorySync
{
    public class StorySyncWorkerStoryStatusEventArgs : EventArgs
    {
        public StorySyncWorkerStoryStatus OldStatus { get; set; }
        public StorySyncWorkerStoryStatus NewStatus { get; set; }
    }
}
