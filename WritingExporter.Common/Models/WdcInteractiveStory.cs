using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Models
{
    [Serializable]
    public class WdcInteractiveStory
    {
        public string Name { get; set; } // E.g. Short stories by the people
        public string ID { get; set; } // E.g. 1824771-short-stories-by-the-people
        public string Url { get; set; } // E.g. https://www.writing.com/main/interact/item_id/1824771-short-stories-by-the-people
        public string ShortDescription { get; set; }
        public string Description { get; set; }
        public WdcAuthor Author { get; set; }
        public DateTime LastUpdated { get; set; } // When the last scrape update was run against Writing.com

        public List<WdcInteractiveChapter> Chapters = new List<WdcInteractiveChapter>();
    }
}
