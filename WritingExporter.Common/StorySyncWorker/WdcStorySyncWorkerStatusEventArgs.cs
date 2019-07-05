using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.StorySyncWorker
{
    public class WdcStorySyncWorkerStatusEventArgs
    {
        public bool StateChanged { get; set; }
        public bool MessageChanged { get; set; }
        public bool ProgressChanged { get; set; }
        public bool CurrentStoryChanged { get; set; }
        public bool CurrentChapterChanged { get; set; }
    }
}
