using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using WritingExporter.SimpleExporter.Models;
using System.Text.RegularExpressions;

namespace WritingExporter.SimpleExporter
{
    /// <summary>
    /// Utility class to save a story to a HTML file.
    /// </summary>
    public class WStoryToHtml
    {
        // Allowed self-closing tags
        // http://xahlee.info/js/html5_non-closing_tag.html
        private static string[] ALLOWED_SELF_CLOSING_ELEMENTS =
        {
            "area",
            "base",
            "br",
            "col",
            "embed",
            "hr",
            "img",
            "input",
            "link",
            "meta",
            "param",
            "source",
            "track",
            "wbr",
        };

        private static string[] TEMPLATE_MANIFEST = {
            "Chapter",
            "ChapterChoiceList",
            "ChapterChoiceListEnd",
            "ChapterChoiceInvalid",
            "ChapterChoiceValid",
            "Page",
            "StoryOutline",
            "StoryOutlineItem",
            "StorySummary",
            };

        private static ILogger log = LogManager.GetLogger(typeof(WStoryToHtml));

        private Dictionary<string, string> Templates = new Dictionary<string, string>();

        public WStoryToHtml()
        {
            LoadTemplates();
        }

        public void LoadTemplates()
        {
            log.Debug("Loading HTML templates");

            var newTemplates = new Dictionary<string, string>();
            Assembly assembly = Assembly.GetExecutingAssembly();

            // Load those templates!
            foreach (string item in TEMPLATE_MANIFEST)
            {
                string resourceName = $"WritingExporter.SimpleExporter.HtmlTemplates.{item}.html";
                log.DebugFormat("Loading template resource: {0}", resourceName);
                using (Stream stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null)
                        throw new Exception($"Tried to load a HTML template, but couldn't find it. Is it embedded and in the manifest? ${resourceName}");

                    using (StreamReader reader = new StreamReader(stream))
                    {


                        newTemplates[item] = reader.ReadToEnd();
                    }
                }
                
            }

            // Done
            Templates = newTemplates;
        }

        public string ConvertStory(WInteractiveStory story)
        {
            var output = string.Empty;

            // Make a copy of the chapter list, so we can do some work on it
            //var chapterList = new List<WInteractiveChapter>(story.Chapters);
            var chapterList = story.Chapters.OrderBy(c => c.Path).ToList();

            // Do the story summary content block
            log.Debug("Building the story summary block");
            var storySummaryContent = Templates["StorySummary"]
                .Replace("{StoryTitle}", story.Name)
                //.Replace("{StoryAuthor}", story.Author.Name) // Author isn't currently supported
                .Replace("{StoryShortDescription}", story.ShortDescription) // The tagline / short description isn't currentl supported
                .Replace("{StoryDescription}", story.Description);

            // Do the story chapter outline content block
            log.Debug("Building the story outline content block");
            var chapterOutlineLines = new List<string>();
            foreach (var chapter in chapterList)
            {
                chapterOutlineLines.Add(
                    Templates["StoryOutlineItem"]
                    .Replace("{ChapterPathDisplay}", StoryOutlineChapterPath(chapter.Path))
                    .Replace("{ChapterPath}", chapter.Path)
                    .Replace("{ChapterName}", chapter.Title)
                    );
            }

            var chapterOutlineContent = Templates["StoryOutline"]
                .Replace("{Chapters}", string.Join("\n", chapterOutlineLines));

            // Build the chapter content block
            var sbChapters = new StringBuilder();

            foreach (var chapter in chapterList)
            {
                log.DebugFormat("Adding chapter {0}", chapter.Path);

                //DEBUG
                if (chapter.Path == "1131222")
                {
                    var lolwut = "test";
                }

                string choiceListBlock = "";

                if (chapter.IsEnd)
                {
                    choiceListBlock = Templates["ChapterChoiceListEnd"];
                }
                else
                {
                    var sbChapterChoices = new StringBuilder();
                    foreach (var choice in chapter.Choices)
                    {
                        bool isChoiceValid = story.Chapters.SingleOrDefault(c => c.Path == choice.MapLink) != null;
                        // Choice links to a chapter that exists, choice is valid
                        var choiceTemplate = isChoiceValid ? "ChapterChoiceValid" : "ChapterChoiceInvalid";
                        sbChapterChoices.AppendLine(Templates[choiceTemplate])
                            .Replace("{ChoicePath}", choice.MapLink)
                            .Replace("{ChoiceName}", choice.Name);
                    }

                    choiceListBlock = Templates["ChapterChoiceList"]
                        .Replace("{Choices}", sbChapterChoices.ToString() );
                }

                string sourceChoiceLink = String.Empty;
                if (chapter.Path != "1")
                    sourceChoiceLink = string.Format("<a href='#{0}'>Go back</a>", chapter.Path.Substring(0, chapter.Path.Length-1));

                string chapterContentElementsClosed;
                string chapterContent = CloseUnclosedElements(chapter.Content, out chapterContentElementsClosed);
                if (!string.IsNullOrEmpty(chapterContentElementsClosed))
                    log.Debug($"Chapter {chapter.Path} had unclosed HTML elements: {chapterContentElementsClosed}");

                sbChapters.AppendLine(Templates["Chapter"]
                    .Replace("{Path}", chapter.Path)
                    .Replace("{PathDisplay}", StoryOutlineChapterPath(chapter.Path))
                    .Replace("{Title}", chapter.Title)
                    .Replace("{SourceChoice}", chapter.Path == "1" ? "(empty)" : chapter.SourceChoiceTitle)
                    .Replace("{SourceChapterLink}", sourceChoiceLink)
                    .Replace("{AuthorName}", chapter.Author.Name)
                    .Replace("{Content}", chapterContent)
                    .Replace("{Choices}", choiceListBlock)
                    );
            }


            // Final put together
            output = Templates["Page"]
                .Replace("{StorySummary}", storySummaryContent)
                .Replace("{StoryTitle}", story.Name)
                .Replace("{StoryOutline}", chapterOutlineContent)
                .Replace("{Chapters}", sbChapters.ToString());


            return output;
        }

        private static string StoryOutlineChapterPath(string chapterPath)
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

        // Add closing tags for all unclosed elements
        // Was running into issues where there were opening HTML elements without a closing element
        // E.g. <i> without a matching </i> to go with it, and it makes the rest of the story italic
        public static string CloseUnclosedElements(string html, out string elementsClosed)
        {
            var closingElements = new List<string>();

            var foundTagEnds = new Dictionary<string, int>();

            // Regexes for detecting the starts and ends of HTML elements
            Regex ElementStartRegex = new Regex(@"<[^\/>]+[^\/]?>", RegexOptions.Singleline);
            Regex ElementEndRegex = new Regex(@"<\/.+?>", RegexOptions.Singleline);

            var elementEndMatches = ElementEndRegex.Matches(html);
            foreach (var match in elementEndMatches)
            {
                string matchString = match.ToString();
                int matchLength = matchString.Length;
                string elementName = matchString.Trim('<', '/', '>').Trim(); // Try to just get the element name, '</ div>' to 'div'

                if (foundTagEnds.ContainsKey(elementName))
                {
                    foundTagEnds[elementName]++;
                }
                else
                {
                    foundTagEnds.Add(elementName, 1);
                }
            }

            // Look for <something> with extra bits, but not with a forward slash at either the start or the end of the element
            var elementStartMatches = ElementStartRegex.Matches(html);

            foreach (var match in elementStartMatches)
            {
                // For each tag opener, take 1 from the end tag pool.
                // If there isn't or was never any end tags of this type in the pool,
                //  a closing tag is needed

                string elementName = match.ToString().TrimStart('<').TrimEnd('>').Split(' ')[0];

                if (foundTagEnds.ContainsKey(elementName))
                {
                    if (foundTagEnds[elementName] > 0)
                    {
                        // Closing tags of this type have been found, and there are some in the pool.
                        // Take 1 from the pool and move on
                        foundTagEnds[elementName]--;
                    }
                    else
                    {
                        // Closing tags of this type have been found, but there are none left in the pool.
                        // This means that there were more start tags than closing tags
                        // Add an end tag
                        closingElements.Add(string.Format("</{0}>", elementName));
                    }
                }
                else
                {
                    // No end tags of this type were ever found.
                    // Add an end tag
                    closingElements.Add(string.Format("</{0}>", elementName));
                }
            }

            closingElements.Reverse();
            var sb = new StringBuilder();
            foreach (var ct in closingElements) sb.Append(ct);

            elementsClosed = sb.ToString(); // Pass the closing elements upwards

            return html + sb.ToString();
        }
    }
}
