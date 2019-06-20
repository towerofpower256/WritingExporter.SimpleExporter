using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WritingExporter.Common.Models;
using WritingExporter.Common.Exceptions;
using System.Web;
using System.Threading;

namespace WritingExporter.Common
{
    // A connection context to Writing.com. Use this to scrape content from Writing.com.
    // Not thread safe.
    public class WdcReader : IWdcReader
    {
        private static ILogger log = LogManager.GetLogger(typeof(WdcReader));

        public WdcReader()
        {
            
        }

        public async Task<WdcInteractiveStory> GetInteractiveStory(string interactiveID, IWdcClient wdcClient, CancellationToken ct)
        {
            var wdcPayload = await wdcClient.GetInteractiveHomepage(interactiveID, ct);

            return GetInteractiveStory(interactiveID, wdcPayload);
        }

        public WdcInteractiveStory GetInteractiveStory(string interactiveID, WdcResponse wdcPayload)
        {
            log.DebugFormat("Getting interactive story: {0}", interactiveID);

            var story = new WdcInteractiveStory();

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
        public string GetInteractiveStoryTitle(WdcResponse wdcPayload)
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
        public string GetInteractiveStoryShortDescription(WdcResponse wdcPayload)
        {
            Regex interactiveShortDescRegex = new Regex("(?<=<META NAME=\"description\" content=\").+?(?=\">)", RegexOptions.IgnoreCase);
            Match interactiveShortDescMatch = interactiveShortDescRegex.Match(wdcPayload.WebResponse);
            if (!interactiveShortDescMatch.Success)
                log.Warn($"Couldn't find the short description for interactive story '{wdcPayload.Address}'"); // Just a warning, don't throw an exception over it
            return HttpUtility.HtmlDecode(WdcUtil.CleanHtmlSymbols(interactiveShortDescMatch.Value));
        }

        // Get the interactive story's description
        public string GetInteractiveStoryDescription(WdcResponse wdcPayload)
        {
            Regex interactiveDescRegex = new Regex("(?<=<td align=left class=\"norm\">).+?(?=<\\/td>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match interactiveDescMatch = interactiveDescRegex.Match(wdcPayload.WebResponse);
            if (!interactiveDescMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the description for interactive story '{wdcPayload.Address}'", wdcPayload.WebResponse);
            return HttpUtility.HtmlDecode(WdcUtil.CleanHtmlSymbols(interactiveDescMatch.Value));
        }

        // TODO get author, looks like it'll be a pain in the ass telling the chapter author apart from the story author
        public WdcAuthor GetInteractiveStoryAuthor(WdcResponse wdcPayload)
        {
            throw new NotImplementedException();
        }

        public async Task<WdcInteractiveChapter> GetInteractiveChaper(string interactiveID, string chapterPath, IWdcClient wdcClient, CancellationToken ct)
        {
            WdcResponse payload = await wdcClient.GetInteractiveChapter(interactiveID, chapterPath, ct);

            return GetInteractiveChaper(interactiveID, chapterPath, payload);
        }

        public WdcInteractiveChapter GetInteractiveChaper(string interactiveID, string chapterPath, WdcResponse payload)
        {

            if (!WdcUtil.IsValidChapterPath(chapterPath))
                throw new ArgumentException($"Chapter '{chapterPath}' is not a valid chapter path", nameof(chapterPath));

            var chapter = new WdcInteractiveChapter();
            chapter.Path = chapterPath;
            chapter.Title = GetInteractiveChapterTitle(payload);
            chapter.Content = GetInteractiveChapterContent(payload);
            if (chapterPath != "1") chapter.SourceChoiceTitle = GetInteractiveChapterSourceChoice(payload); // Only get the source choice if it's not the first chapter
            else chapter.SourceChoiceTitle = "";
            chapter.LastUpdated = DateTime.Now;
            // TODO chapter author
            chapter.Author = GetInteractiveChapterAuthor(payload);

            var choices = GetInteractiveChapterChoices(payload);
            if (choices == null)
                chapter.IsEnd = true;
            else
            {
                chapter.Choices.AddRange(choices);
            }

            return chapter;
        }

        // Get the chapter's title
        public string GetInteractiveChapterTitle(WdcResponse payload) => GetInteractiveChapterTitleM2(payload);

        // Get chapter title
        // Method 1. Get it from the "Your path to this chapter"
        // CAUTION: can sometimes get truncated, but this appears to be the the legit title from the database, it was truncated when the chapter was made
        // NOTE: Fails on the first chapter, because there's no choices made yet
        [Obsolete]
        private string GetInteractiveChapterTitleM1(WdcResponse payload)
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
        private string GetInteractiveChapterTitleM2(WdcResponse payload)
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
        public string GetInteractiveChapterSourceChoice(WdcResponse payload)
        {
            Regex chapterSourceChoiceRegex = new Regex(@"(?<=This choice: <b>).*?(?=<\/b>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterSourceChoiceMatch = chapterSourceChoiceRegex.Match(payload.WebResponse);
            if (!chapterSourceChoiceMatch.Success) // If we can't find it, and it's not the first chapter
                throw new WritingClientHtmlParseException($"Couldn't find the interactive chapter's source choice and this isn't the first chapter, for chapter '{payload.Address}'", payload.WebResponse);
            return HttpUtility.HtmlDecode(chapterSourceChoiceMatch.Value);
        }

        // Get the chapter's content, it's body
        public string GetInteractiveChapterContent(WdcResponse payload) => GetInteractiveChapterContentM2(payload);

        // Search for the chapter content, the actual writing
        // <div class="KonaBody">stuff goes here</div>
        private string GetInteractiveChapterContentM1(WdcResponse payload)
        {
            
            Regex chapterContentRegex = new Regex("(?<=<div class=\"KonaBody\">).+?(?=<\\/div>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterContentMatch = chapterContentRegex.Match(payload.WebResponse);
            if (!chapterContentMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the content for the interactive chapter '{payload.Address}'", payload.WebResponse);
            return HttpUtility.HtmlDecode(chapterContentMatch.Value);
        }

        // Get the chapter content
        // WDC has changed the layout, and doesn't have "KonaBody" in it anymore
        // It looks like they've just set it to <div class=""> in the HTML, and that's the only instance of an empty class
        private string GetInteractiveChapterContentM2(WdcResponse payload)
        {
            Regex chapterContentRegex = new Regex("(?<=<div class=\"\">).+?(?=<\\/div>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterContentMatch = chapterContentRegex.Match(payload.WebResponse);
            if (!chapterContentMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the content for the interactive chapter '{payload.Address}'", payload.WebResponse);
            return HttpUtility.HtmlDecode(chapterContentMatch.Value);
        }

        // Get the author
        // <a title="Username: rpcity Member Since: July 4th, 2002 Click for links!" style="font - size:1em; font - weight:bold; cursor: pointer; ">SmittySmith</a>
        public WdcAuthor GetInteractiveChapterAuthor(WdcResponse payload)
        {
            Regex chapterAuthorChunkRegex = new Regex("<a title=\"Username: .*?<\\/a>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterAuthorChunkMatch = chapterAuthorChunkRegex.Match(payload.WebResponse);
            if (!chapterAuthorChunkMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the HTML chunk containing the author for the interactive chapter '{payload.Address}'", payload.WebResponse);
            string chapterAuthorChunk = chapterAuthorChunkMatch.Value;

            // Get the author username
            Regex chapterAuthorUsernameRegex = new Regex("(?<=Username: )[a-zA-Z]+");
            Match chapterAuthorUsernameMatch = chapterAuthorUsernameRegex.Match(chapterAuthorChunk);
            string chapterAuthorUsername = chapterAuthorUsernameMatch.Value;

            // Get the author display name
            Regex chapterAuthorNameRegex = new Regex("(?<=>).+?(?=<)");
            Match chapterAuthorNameMatch = chapterAuthorNameRegex.Match(chapterAuthorChunk);
            string chapterAuthorName = chapterAuthorNameMatch.Value;

            return new WdcAuthor()
            {
                Name = chapterAuthorName,
                Username = chapterAuthorUsername
            };
        }

        // Get the available choices
        // This one is going to be complicated, because none of the divs or whatnot have ID's
        // First, get a chunk of the HTML that contains the choices, we'll break them down later
        public IEnumerable<WdcInteractiveChapterChoice> GetInteractiveChapterChoices(WdcResponse payload)
        {
            if (IsInteractiveChapterEnd(payload)) return null;

            var choices = new List<WdcInteractiveChapterChoice>();

            Regex chapterChoicesChunkRegex = new Regex("(?<=<b>You have the following choice(s)?:<\\/b>).*?(?=<\\/div><div id=\"end_of_choices\")",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
            Match chapterChoicesChunkMatch = chapterChoicesChunkRegex.Match(payload.WebResponse);
            if (!chapterChoicesChunkMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the HTML chunk containing choices for interactive chapter '{payload.Address}'", payload.WebResponse);
            string chapterChoicesChunkHtml = chapterChoicesChunkMatch.Value;

            // Then try to get the individual choices
            Regex chapterChoicesRegex = new Regex("<a .*?href=\".+?\">.+?<\\/a>", RegexOptions.IgnoreCase);
            MatchCollection chapterChoicesMatches = chapterChoicesRegex.Matches(chapterChoicesChunkHtml);
            foreach (Match match in chapterChoicesMatches)
            {
                var newChoice = new WdcInteractiveChapterChoice();
                string choiceUrl;

                // Get the URL
                Regex choiceUrlRegex = new Regex("(?<=href=\").+?(?=\")");
                Match choiceUrlMatch = choiceUrlRegex.Match(match.Value);
                if (!choiceUrlMatch.Success)
                    throw new WritingClientHtmlParseException($"Could not find the URL of choice '{match.Value}' on interactive chapter '{payload.Address}'", payload.WebResponse);
                choiceUrl = choiceUrlMatch.Value;

                // Get just the numbers from the URL
                newChoice.PathLink = WdcUtil.GetFinalParmFromUrl(choiceUrl);

                // Get the choice name / description
                // Get what's in between the > and the <
                int indexOfGt = match.Value.IndexOf('>');
                int indexofLt = match.Value.LastIndexOf('<') - 1;
                newChoice.Name = HttpUtility.HtmlDecode(match.Value.Substring(indexOfGt + 1, indexofLt - indexOfGt));

                choices.Add(newChoice);
            }

            return choices.ToArray();
        }

        // TODO Get chapter list from story outline
        public async Task<IEnumerable<Uri>> GetInteractiveChapterList(string interactiveID, IWdcClient wdcClient, CancellationToken ct)
        {
            var wdcPayload = await wdcClient.GetInteractiveOutline(interactiveID, ct);
            ct.ThrowIfCancellationRequested();
            return GetInteractiveChapterList(interactiveID, wdcClient.GetPathToRoot(), wdcPayload);
        }

        public IEnumerable<Uri> GetInteractiveChapterList(string interactiveID, Uri pathToRoot, WdcResponse wdcPayload)
        {
            var chapters = new List<Uri>();

            // Find the links to the interactive's pages
            // Create the regex that will find chapter links
            // E.g. https:\/\/www\.writing\.com\/main\/interact\/item_id\/1824771-short-stories-by-the-people\/map\/(\d)+
            string chapterLinkRegexPattern = pathToRoot.ToString() + string.Format("main/interact/item_id/{0}/map/{1}", interactiveID, @"(\d)+");
            chapterLinkRegexPattern = WdcUtil.RegexSafeUrl(chapterLinkRegexPattern);
            Regex chapterLinkRegex = new Regex(chapterLinkRegexPattern, RegexOptions.IgnoreCase);
            MatchCollection matches = chapterLinkRegex.Matches(wdcPayload.WebResponse);

            foreach (Match match in matches)
            {
                chapters.Add(new Uri(match.Value));
            }

            return chapters.ToArray();
        }

        public bool IsInteractiveChapterEnd(WdcResponse payload)
        {
            //Regex chapterEndRegex = new Regex("<big>THE END.<\\/big>");// Turns out this doesn't work, because they HTML tagging is sloppy and overlaps. <i><b>THE END.</i></b>
            Regex chapterEndRegex = new Regex(">You have come to the end of the story. You can:<\\/");
            return chapterEndRegex.IsMatch(payload.WebResponse);
        }
    }
}
