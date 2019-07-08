﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Configuration
{
    public class WdcStorySyncWorkerSettings : BaseConfigSection
    {
        // Refresh story details after this many seconds.
        // Default: 10 days
        public int SyncStoryIntervalSeconds { get; set; } = 864000;

        // Refresh chapter details after this many seconds.
        // Default: 10 days
        public int SyncChapterIntervalSeconds { get; set; } = 864000;

        // Should chapters that have already been scraped be updated again? Default: no
        public bool UpdateKnownChapters { get; set; } = false;

        // A loop of the worker should only occur no faster than this.
        // This is to prevent the loop going full-speed and consuming all CPU
        // Default: 1 second
        public int WorkerLoopPauseMs { get; set; } = 1000;

        // Is sync enabled?
        // Default: yes
        public bool SyncEnabled { get; set; } = true;
    }
}
