using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritingExporter.Common.Models;
using System.IO;

namespace WritingExporter.Common.Export
{
    /// <summary>
    /// <para>Use this to export stories to a collection of HTML files, 1 file per page.</para>
    /// <para>Note that this will cause issues if the chapter path is over ~200 levels deep. It will breach the 255 character filename limit.</para>
    /// </summary>
    // NOTE: The map depth limit won't be circumvented by sticking them in an archive file, they have about the same file size restriction.
    // Possible solution: The map depth limit may be circumvented by naming the files with a GUID. It will
    //     make the file names less human readable, but it would work. I'll leave it for the future.
    //     I'm hoping that writing.com has a similar limitation, and this issue won't ever be encountered.
    public class WdcStoryExporterHtmlCollection
    {
        private const string HTML_TEMPLATE_ROOT = "WritingExporter.Common.Export.HtmlCollectionTemplates."; // Update this if the templates ever move.
        private const string STORY_HOMEPAGE_FILENAME = "index.html";
        private const string STORY_OUTLINE_FILENAME = "outline.html";
        private const string STORY_RECENT_CHAPTERS_FILENAME = "recent_chapters.html";
        private const string STORY_CHAPTER_FILENAME = "chapter-{0}.html";
        private const string CHAPTER_NOT_EXPORTED_PLACEHOLDER = "(chapter hasn't been exported)";

        public static ILogger _log = LogManager.GetLogger(typeof(WdcStoryExporterHtmlCollection));

        public event EventHandler<WdcStoryExporterProgressUpdateArgs> OnProgressUpdate;

        private string _outputDir;
        private Dictionary<string, string> _chapterPathFilename = new Dictionary<string, string>();
        private int _progressValue;
        private int _progressMax;
        private string  _lastMessage;

        public WdcStoryExporterHtmlCollection(string outputDir)
        {
            _outputDir = outputDir;
        }

        private void DoProgressUpdate(string msg, int progressValue, int progressMax)
        {
            msg = string.IsNullOrEmpty(msg) ? _lastMessage : msg;
            _progressValue = progressValue;
            _progressMax = progressMax;

            OnProgressUpdate?.Invoke(this, new WdcStoryExporterProgressUpdateArgs()
            {
                ProgressMax = progressMax,
                ProgressValue = progressValue,
                Message = msg
            });
        }

        public void ExportStory(WdcInteractiveStory story)
        {
            _log.InfoFormat("Exporting story '{0}' to path: {1}", story.ID, _outputDir);

            DoProgressUpdate("Getting ready to export", 0, 0);

            // Create the missing directories, if need be
            Directory.CreateDirectory(_outputDir);

            // Create the chapter path / chapter filename map.
            // Not sure, but one day these might not be so simple, like if we hit the filename length limit.
            _chapterPathFilename = new Dictionary<string, string>();
            foreach (var chapter in story.Chapters)
            {
                _chapterPathFilename.Add(chapter.Path, GetChapterFileName(chapter));
            }

            DoProgressUpdate("Exporting story homepage", 0, _chapterPathFilename.Count);
            ExportStoryHomepage(story);

            DoProgressUpdate("Exporting story outline", 0, _progressMax);
            ExportStoryOutline(story);

            DoProgressUpdate("Exporting recent chapters", 0, _progressMax);
            ExportStoryRecentChapters(story);

            //foreach (var chapter in story.Chapters)
            for (var i=0; i < story.Chapters.Count; i++)
            {
                var chapter = story.Chapters[i];
                var chapterIsReady = IsChapterReady(chapter);
                DoProgressUpdate($"Exporting chapter {chapter.Path}", i, _progressMax);
                if (chapterIsReady) ExportStoryChapter(story, chapter, _chapterPathFilename[chapter.Path]);
            }
        }

        public void ExportStoryHomepage(WdcInteractiveStory story)
        {
            _log.Debug("Exporting story homepage");

            string htmlContent = GetTemplate("StoryHomepage.html")
                .Replace("{StoryTitle}", story.Name)
                //.Replace("{StoryAuthor}", story.Author.Name) // Author isn't currently supported
                .Replace("{StoryShortDescription}", story.ShortDescription)
                .Replace("{StoryDescription}", story.Description)
                .Replace("{FirstPageLink}", _chapterPathFilename["1"])
                .Replace("{StoryOutlineLink}", GetOutlineFileName())
                .Replace("{RecentChaptersLink}", GetRecentChaptersFileName());

            var html = GetPageTemplate(story.Name, story.Name, htmlContent);

            var fname = GetHomepageFileName();
            File.WriteAllText(Path.Combine(_outputDir, fname), html);
        }

        public void ExportStoryOutline(WdcInteractiveStory story)
        {
            _log.Debug("Exporting story outline");

            StringBuilder htmlStoryOutline = new StringBuilder();
            foreach (var chapter in story.Chapters)
            {
                var chapterIsReady = IsChapterReady(chapter);
                htmlStoryOutline.AppendLine(
                    GetTemplate("StoryOutlineItem.html")
                    .Replace("{ChapterPathDisplay}", GetPrettyChapterPath(chapter.Path))
                    .Replace("{ChapterPath}", chapter.Path)
                    .Replace("{ChapterName}", chapterIsReady ? chapter.Title : CHAPTER_NOT_EXPORTED_PLACEHOLDER)
                    .Replace("{ChapterLink}", chapterIsReady ? _chapterPathFilename[chapter.Path] : "#")
                    );
            }

            string htmlContent = GetTemplate("StoryOutline.html")
                .Replace("{StoryTitle}", story.Name)
                .Replace("{OutlineContent}", htmlStoryOutline.ToString())
                .Replace("{HomeLink}", GetHomepageFileName());

            var html = GetPageTemplate($"{story.Name} - Chapter outline", "Chapter outline", htmlContent);

            var fname = GetOutlineFileName();
            File.WriteAllText(Path.Combine(_outputDir, fname), html);
        }

        public void ExportStoryRecentChapters(WdcInteractiveStory story)
        {
            _log.Debug("Exporting story recent chapters");

            var chaptersStorted = story.Chapters.OrderByDescending(c => c.FirstSeen).ToArray();

            var htmlRecentChaptersItems = new StringBuilder();
            foreach (var chapter in chaptersStorted)
            {
                var chapterIsReady = IsChapterReady(chapter);

                htmlRecentChaptersItems.AppendLine(
                    GetTemplate("StoryRecentChaptersItem.html")
                    .Replace("{ChapterName}", chapterIsReady ? chapter.Title : CHAPTER_NOT_EXPORTED_PLACEHOLDER)
                    .Replace("{ChapterSeen}", chapter.FirstSeen.ToString())
                    .Replace("{ChapterPath}", GetPrettyChapterPath(chapter.Path))
                    .Replace("{ChapterLink}", chapterIsReady ? _chapterPathFilename[chapter.Path] : "#")
                    );
            }

            string htmlContent = GetTemplate("StoryRecentChapters.html")
                .Replace("{StoryTitle}", story.Name)
                .Replace("{RecentChapterItems}", htmlRecentChaptersItems.ToString())
                .Replace("{HomeLink}", GetHomepageFileName());

            var html = GetPageTemplate($"{story.Name} - Recent chapters", "Recent chapters", htmlContent);

            var fname = GetRecentChaptersFileName();
            File.WriteAllText(Path.Combine(_outputDir, fname), html);
        }

        public void ExportStoryChapter(WdcInteractiveStory story, WdcInteractiveChapter chapter, string filename)
        {
            _log.DebugFormat("Exporting chapter: {0}", chapter.Path);

            var chapterIsReady = IsChapterReady(chapter);
            var htmlContent = GetTemplate("Chapter.html")
                .Replace("{AuthorName}", chapter.Author.Name)
                .Replace("{ChapterPath}", chapter.Path)
                .Replace("{ChapterPathDisplay}", GetPrettyChapterPath(chapter.Path))
                .Replace("{SourceChapterChunk}", GetPreviousChapterLink(story, chapter))
                .Replace("{ChapterChoices}", GetChapterChoices(story, chapter))
                .Replace("{ChapterContent}", chapter.Content)
                .Replace("{ChapterTitle", chapter.Title)
                .Replace("{HomeLink}", GetHomepageFileName());

            var html = GetPageTemplate($"{story.Name} - {chapter.Title}", chapter.Title, htmlContent);

            // TODO: Fix the "Go back" link


            File.WriteAllText(Path.Combine(_outputDir, filename), html);
        }

        private string GetPreviousChapterLink(WdcInteractiveStory story, WdcInteractiveChapter chapter)
        {
            if (chapter.Path.Length > 1)
            {
                // E.g. 15524
                var previousChapterPath = _chapterPathFilename[chapter.Path.Substring(0, chapter.Path.Length - 1)];
                return $"This choice: <b>{chapter.SourceChoiceTitle}</b> <a href='{previousChapterPath}'>Go back</a>";
            }
            else
            {
                // E.g. 1
                return $"<a href='{STORY_HOMEPAGE_FILENAME}'>Go back</a>";
            }
        }

        private string GetChapterChoices(WdcInteractiveStory story, WdcInteractiveChapter chapter)
        {
            if (chapter.IsEnd || chapter.Choices.Count < 1)
            {
                // Is end
                return GetTemplate("ChapterChoiceEnd.html");
            }
            else
            {
                // There are choices
                var sbChapterChoices = new StringBuilder();
                foreach (var choice in chapter.Choices)
                {
                    // Does this choice lead to a valid chapter?
                    bool isChoiceValid = story.Chapters.SingleOrDefault(c => c.Path == choice.PathLink) != null;

                    var choiceHtml = String.Empty;

                    // Choice links to a chapter that exists, choice is valid

                    if (isChoiceValid && _chapterPathFilename != null)
                    {
                        choiceHtml = GetTemplate("ChapterChoiceItemValid.html")
                            .Replace("{ChoicePath}", choice.PathLink)
                            .Replace("{ChoiceLink}", _chapterPathFilename[choice.PathLink])
                            .Replace("{ChoiceName}", choice.Name);
                    } else
                    {
                        // Is either an invalid choice, or the path / filename lookup is missing (could be for a once-off chapter export)
                        choiceHtml = GetTemplate("ChapterChoiceItemInvalid.html")
                            .Replace("{ChoiceName}", choice.Name);
                    }

                    sbChapterChoices.AppendLine(choiceHtml);    
                }

                return GetTemplate("ChapterChoiceList.html")
                    .Replace("{ChoiceList}", sbChapterChoices.ToString());
            }
        }

        private string GetPrettyChapterPath(string chapterPath)
        {
            if (string.IsNullOrWhiteSpace(chapterPath))
                return chapterPath;

            // Convert this: 12345
            // to this: 1-2-3-4-<b>#5</b>
            var sb = new StringBuilder();
            for (var i = 0; i < chapterPath.Length; i++)
            {
                if (i != (chapterPath.Length - 1))
                {
                    // if this is not the last character
                    sb.Append($"{chapterPath[i]}-");
                }
                else
                {
                    // If this is the last character
                    sb.Append($"<b>#{chapterPath[i]}</b>");
                }
            }

            return sb.ToString();
        }

        private string GetTemplate(string name)
        {
            return DataUtil.GetEmbeddedResource(HTML_TEMPLATE_ROOT + name);
        }

        private string GetPageTemplate(string title, string header, string content)
        {
            var html = GetTemplate("PageTemplate.html")
                .Replace("{PageHeader}", header)
                .Replace("{StyleData}", GetTemplate("Style.css"))
                .Replace("{DateTime}", DateTime.Now.ToString())
                .Replace("{PageTitle}", title)
                .Replace("{PageContent}", content);
            return html;
        }

        #region Filename generation functions

        public string GetHomepageFileName()
        {
            return STORY_HOMEPAGE_FILENAME;
        }

        public string GetOutlineFileName()
        {
            return STORY_OUTLINE_FILENAME;
        }

        public string GetRecentChaptersFileName()
        {
            return STORY_RECENT_CHAPTERS_FILENAME;
        }

        [Obsolete]
        public string GetChapterFileName()
        {
            return string.Format(STORY_CHAPTER_FILENAME, Guid.NewGuid().ToString("N"));
        }

        public string GetChapterFileName(WdcInteractiveChapter chapter)
        {
            return string.Format(STORY_CHAPTER_FILENAME, chapter.Path);
        }

        private bool IsChapterReady(WdcInteractiveChapter chapter)
        {
            return chapter.LastSynced != DateTime.MinValue;
        }

        #endregion
    }
}
