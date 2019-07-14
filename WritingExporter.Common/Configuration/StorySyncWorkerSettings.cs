using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Configuration
{
    public class StorySyncWorkerSettings : BaseConfigSection
    {
        // Refresh story details after this many seconds.
        // Default: 10 days
        public int SyncStoryIntervalSeconds { get; set; } = 864000;

        // Refresh chapter details after this many seconds.
        // Default: 10 days
        public int SyncChapterIntervalSeconds { get; set; } = 864000;

        // Check the chapter map for new chapters after this many seconds.
        // Default: 6 hours
        public int SyncChapterOutlineIntervalSeconds { get; set; } = 21600;

        // Should chapters that have already been scraped be updated again? Default: no
        public bool UpdateKnownChapters { get; set; } = false;

        // A loop of the worker should only occur no faster than this.
        // This is to prevent the loop going full-speed and consuming all CPU
        // Default: 1 second
        public int WorkerLoopPauseMs { get; set; } = 1000;

        // Is sync enabled?
        // Default: yes
        public bool SyncEnabled { get; set; } = true;

        // How many seconds to not sync anything for that story after an Interactives Temporarily Unavailable message is encountered.
        // Default: 15
        public int ItuPauseDuration { get; set; }
    }
}
