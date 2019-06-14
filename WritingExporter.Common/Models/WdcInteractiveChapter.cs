using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Models
{
    public class WdcInteractiveChapter
    {
        public string Path { get; set; }
        public string Title { get; set; }
        public string SourceChoiceTitle { get; set; } // Yes, we will to save this, because sometimes it's different from the title
        public WdcAuthor Author { get; set; } // The author, not sure if it changes with edits
        public string Content { get; set; } // The content / writing of the chapter, the flesh of it
        public List<WdcInteractiveChapterChoice> Choices { get; set; } = new List<WdcInteractiveChapterChoice>(); // The choices at the end of this chapter.
        public bool IsEnd { get; set; } // Is this chapter a dead end in the story tree

        public DateTime LastUpdated { get; set; }
        public DateTime LastSynced { get; set; }
    }
}
