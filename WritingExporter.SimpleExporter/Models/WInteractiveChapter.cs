using System;
using System.Collections.Generic;
using System.Text;

namespace WritingExporter.SimpleExporter.Models
{
    [Serializable]
    public class WInteractiveChapter
    {
        public string Path { get; set; }
        //public List<WInteractiveChapterVersion> Versions { get; set; } = new List<WInteractiveChapterVersion>();
        public DateTime VersionFoundAt { get; set; } = DateTime.Now; // The datetime that this verison was found by the exporter
        public string Title { get; set; }
        public string SourceChoiceTitle { get; set; } // Yes, we will to save this, because sometimes it's different from the title
        public WAuthor Author { get; set; } // The author, not sure if it changes with edits
        public string Content { get; set; } // The content / writing of the chapter, the flesh of it
        public List<WInteractiveChapterChoice> Choices { get; set; } = new List<WInteractiveChapterChoice>(); // The choices at the end of this chapter.
        public bool IsEnd { get; set; } // Is this chapter a dead end in the story tree

        public DateTime LastUpdated { get; set; }
        public DateTime LastSynced { get; set; }
        public bool HasBeenSeen { get; set; }
    }
}
