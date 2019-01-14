using System;
using System.Collections.Generic;
using System.Text;

namespace WritingExporter.SimpleExporter.Models
{
    [Serializable]
    public class WInteractiveStory
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public string UrlID { get; set; } // e.g. "1824771-short-stories-by-the-people" from https://www.writing.com/main/interact/item_id/1824771-short-stories-by-the-people
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public WAuthor Author { get; set; }
        public List<WInteractiveChapter> Chapters { get; set; } = new List<WInteractiveChapter>();
        public DateTime LastUpdated { get; set; }
        public DateTime LastSynced { get; set; }
    }
}
