using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WritingExporter.Common.Models;
using WritingExporter.Common.Exceptions;
using System.Web;

namespace WritingExporter.Common
{
    // A connection context to Writing.com. Use this to scrape content from Writing.com.
    // Not thread safe.
    public class WdcReader
    {
        private static ILogger log = LogManager.GetLogger(typeof(WdcReader));

        private IWdcClient wdcClient;

        public async Task<WdcInteractiveStory> GetInteractiveStory(string interactiveID)
        {
            log.DebugFormat("Getting interactive story: {0}", interactiveID);

            var story = new WdcInteractiveStory();

            var wdcPayload = await wdcClient.GetInteractiveHomepage(interactiveID);

            // Get interactive story title
            story.ID = interactiveID;
            story.Url = wdcPayload.Address;
            story.Name = GetInteractiveStoryTitle(wdcPayload);
            story.ShortDescription = GetInteractiveStoryShortDescription(wdcPayload);
            story.Description = GetInteractiveStoryDescription(wdcPayload);
            story.LastUpdated = DateTime.Now;

            return story;
        }


        // Get the interactive story's title
        // This method grabs it from within the <title> element, not sure if it gets truncated or not.
        public static string GetInteractiveStoryTitle(WdcResponse wdcPayload)
        {
            Regex interactiveTitleRegex = new Regex("(?<=<title>).+?(?= - Writing\\.Com<\\/title>)", RegexOptions.IgnoreCase);
            Match interactiveTitleMatch = interactiveTitleRegex.Match(wdcPayload.WebResponse);
            if (!interactiveTitleMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the title for interactive story '{wdcPayload.Address}'", wdcPayload.WebResponse);
            return HttpUtility.HtmlDecode(WdcUtil.CleanHtmlSymbols(interactiveTitleMatch.Value));
        }

        // Get the interactive's tagline or short description
        // Previously this has been difficult to pin-point
        // However I found this on 11/01/2019, they've got it in a META tag at the top of the HTML
        // E.g. <META NAME="description" content="How will young James fare alone with his mature, womanly neighbors? ">
        public static string GetInteractiveStoryShortDescription(WdcResponse wdcPayload)
        {
            Regex interactiveShortDescRegex = new Regex("(?<=<META NAME=\"description\" content=\").+?(?=\">)", RegexOptions.IgnoreCase);
            Match interactiveShortDescMatch = interactiveShortDescRegex.Match(wdcPayload.WebResponse);
            if (!interactiveShortDescMatch.Success)
                log.Warn($"Couldn't find the short description for interactive story '{wdcPayload.Address}'"); // Just a warning, don't throw an exception over it
            return HttpUtility.HtmlDecode(WdcUtil.CleanHtmlSymbols(interactiveShortDescMatch.Value));
        }

        // Get the interactive story's description
        public static string GetInteractiveStoryDescription(WdcResponse wdcPayload)
        {
            Regex interactiveDescRegex = new Regex("(?<=<td align=left class=\"norm\">).+?(?=<\\/td>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match interactiveDescMatch = interactiveDescRegex.Match(wdcPayload.WebResponse);
            if (!interactiveDescMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the description for interactive story '{wdcPayload.Address}'", wdcPayload.WebResponse);
            return HttpUtility.HtmlDecode(WdcUtil.CleanHtmlSymbols(interactiveDescMatch.Value));
        }

        // TODO get author, looks like it'll be a pain in the ass telling the chapter author apart from the story author
        public static string GetInteractiveStoryAuthor(WdcResponse wdcPayload)
        {
            throw new NotImplementedException();
        }

        public async Task<WdcInteractiveChapter> GetInteractiveChaper(string interactiveID, string chapterPath)
        {
            if (!WdcUtil.IsValidChapterPath(chapterPath))
                throw new ArgumentException($"Chapter '{chapterPath}' is not a valid chapter path", nameof(chapterPath));

            var payload = await wdcClient.GetInteractiveChapter(interactiveID, chapterPath);

            var chapter = new WdcInteractiveChapter();
            chapter.Path = chapterPath;
            chapter.Title = GetInteractiveChapterTitle(payload);
            chapter.Content = GetInteractiveChapterContent(payload);
            if (chapterPath != "1") chapter.SourceChoiceTitle = GetInteractiveChapterSourceChoice(payload); // Only get the source choice if it's not the first chapter
            chapter.LastUpdated = DateTime.Now;
            // TODO chapter author
            // TODO chapter choices
            // TODO chapter IsEnd

            return chapter;
        }

        // Get the chapter's title
        public static string GetInteractiveChapterTitle(WdcResponse payload) => GetInteractiveChapterTitleM2(payload);

        // Get chapter title
        // Method 1. Get it from the "Your path to this chapter"
        // CAUTION: can sometimes get truncated, but this appears to be the the legit title from the database, it was truncated when the chapter was made
        // NOTE: Fails on the first chapter, because there's no choices made yet
        [Obsolete]
        private static string GetInteractiveChapterTitleM1(WdcResponse payload)
        {
            string chapterTitleRegexPattern = string.Format("(?<=\\/map\\/{0}\">).*?(?=<\\/a>)", payload.Address);

            Regex chapterTitleRegex = new Regex(chapterTitleRegexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterTitleMatch = chapterTitleRegex.Match(payload.WebResponse);
            if (!chapterTitleMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the chapter title for chapter '{payload.Address}'", payload.WebResponse);
            return HttpUtility.HtmlDecode(chapterTitleMatch.Value);
        }

        // Get chapter title
        // Method 2. Get it from between <big><big><b>...</b></big></big>
        // There are other isntances of the <big><b> tags in use, but only the chapter title gets wrapped in 2x of them
        // Isn't perfect, but until the website layout changes, it'll work
        private static string GetInteractiveChapterTitleM2(WdcResponse payload)
        {
            string chapterTitleRegexPattern = @"(?<=<big><big><b>).*?(?=<\/b><\/big><\/big>)";

            Regex chapterTitleRegex = new Regex(chapterTitleRegexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterTitleMatch = chapterTitleRegex.Match(payload.WebResponse);
            if (!chapterTitleMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the chapter title for chapter '{payload.Address}'", payload.WebResponse);
            return HttpUtility.HtmlDecode(chapterTitleMatch.Value);
        }

        // Search for the choice that lead to this chapter
        // This usually has the more fleshed out title, as the legit title can sometimes be truncated
        public static string GetInteractiveChapterSourceChoice(WdcResponse payload)
        {
            Regex chapterSourceChoiceRegex = new Regex(@"(?<=This choice: <b>).*?(?=<\/b>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterSourceChoiceMatch = chapterSourceChoiceRegex.Match(payload.WebResponse);
            if (!chapterSourceChoiceMatch.Success) // If we can't find it, and it's not the first chapter
                throw new WritingClientHtmlParseException($"Couldn't find the interactive chapter's source choice and this isn't the first chapter, for chapter '{payload.Address}'", payload.WebResponse);
            return HttpUtility.HtmlDecode(chapterSourceChoiceMatch.Value);
        }

        // Get the chapter's content, it's body
        public static string GetInteractiveChapterContent(WdcResponse payload) => GetInteractiveChapterContentM1(payload);

        // Search for the chapter content, the actual writing
        // <div class="KonaBody">stuff goes here</div>
        // TODO: WDC has changed the layout, and doesn't have "KonaBody" in it anymore
        public static string GetInteractiveChapterContentM1(WdcResponse payload)
        {
            
            Regex chapterContentRegex = new Regex("(?<=<div class=\"KonaBody\">).+?(?=<\\/div>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterContentMatch = chapterContentRegex.Match(payload.WebResponse);
            if (!chapterContentMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the content for the interactive chapter '{payload.Address}'", payload.WebResponse);
            return HttpUtility.HtmlDecode(chapterContentMatch.Value);
        }

        // TODO Get chapter list from story outline
    }
}
