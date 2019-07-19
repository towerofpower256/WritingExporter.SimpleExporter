using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.StorySync
{
    public enum StorySyncWorkerState
    {
        Idle,
        WorkingStory,
        WorkingOutline,
        WorkingChapter,
        StoppedInvalidConfig, // Stopped, in case of invalid configuration (e.g. no WDC username / password)
    }
}
