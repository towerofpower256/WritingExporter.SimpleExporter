using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using WritingExporter.SimpleExporter.Exceptions;
using WritingExporter.SimpleExporter.Models;

namespace WritingExporter.SimpleExporter
{
    // NOTE: These are not yet thread safe! Do not smash using multiple threads!
    // This is attempt number 2. I want to make this one more static.
    public class WritingClientSettings
    {
        private const string DEFAULT_WRITING_ROOT = "https://www.writing.com";

        public NetworkCredential WritingCredentials { get; set; }
        public Uri WritingUrlRoot { get; set; } = new Uri(DEFAULT_WRITING_ROOT);
         // TODO: proxy support
    }
    
    public class WritingClient
    {
        
        private const string LOGIN_FIELD_NAME_USERNAME = "login_username";
        private const string LOGIN_FIELD_NAME_PASSWORD = "login_password";
        private const string HTTP_SET_COOKIE_HEADER = "Set-Cookie";
        private const string LOGIN_URL_SEGMENT = "main/login.php";
        private const string INTERACTIVE_URL_SEGMENT = "main/interact/item_id/{0}";
        private const string INTERACTIVE_CHAPTER_URL_SEGMENT = "main/interact/item_id/{0}/map/{1}";
        private const string INTERACTIVE_OUTLINE_URL_SEGMENT = "main/interact/item_id/{0}/action/outline";

        private static ILogger log = LogManager.GetLogger(typeof(WritingClient));

        private HttpClientHandler httpClientHandler;
        private HttpClient httpClient;
        private CookieContainer httpCookies;
        private Dictionary<string, string> cookieDict = new Dictionary<string, string>();

        public WritingClientSettings Settings { get; private set; }

        // Regex to detect the login fields, which should only be visible if we've tried to access something that requires logging in
        private static Regex UsernameFieldRegex = new Regex(
            string.Format(@"<input.*{0}.*>", LOGIN_FIELD_NAME_USERNAME),
            RegexOptions.IgnoreCase
            );

        // Regex to detect the "Failed Login" page. Look for it in the page's title
        private static Regex FailedLoginRegex = new Regex(
            @"<title>.*(Login Error|Login failed).*<\/title>",
            RegexOptions.IgnoreCase
            );

        // Regex to detect the "Interactives temporarily unavailable due to resource limitations" message
        private static Regex InteractivesUnavailableRegex1 = new Regex(
            @"<title>.*(Interactives Temporarily Unavailable|Interactive Stories Are Temporarily Unavailable).*<\/title>",
            RegexOptions.IgnoreCase
            );

        // Regex to detect the "Interactive Stories are temporarily unavailable" message
        // Can sometimes appear different to the first message
        private static Regex InteractivesUnavailableRegex2 = new Regex(
            @"<b>Interactive Stories</b> are <br><i>temporarily</i> unavailable",
            RegexOptions.IgnoreCase
            );


        public WritingClient(WritingClientSettings settings)
        {
            Initialise(settings);
        }

        private void Initialise(WritingClientSettings settings)
        {
            Settings = settings;
            httpCookies = new CookieContainer();
            httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = httpCookies;
            httpClient = new HttpClient(httpClientHandler, true);
        }

        public void UpdateSettings(WritingClientSettings settings)
        {
            log.Debug("Updating writing client settings");

            Settings = settings;
        }

        public async Task LoginAsync()
        {
            var username = Settings.WritingCredentials.UserName;
            var password = Settings.WritingCredentials.Password;

            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username", "The Writing.com username cannot be empty");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("username", "The Writing.com password cannot be empty");

            await LoginAsync(Settings.WritingCredentials.UserName, Settings.WritingCredentials.Password);
        }

        public async Task LoginAsync(string username, string password)
        {
            log.Debug("Logging into writing.com");

            // Endcode login info, ready to be included in the POST data.
            Dictionary<string, string> contentDict = new Dictionary<string, string>();
            contentDict.Add(LOGIN_FIELD_NAME_USERNAME, username);
            contentDict.Add(LOGIN_FIELD_NAME_PASSWORD, password);
            // Not sure if this is needed in the POST data, but it's by some browsers.
            contentDict.Add("submit", "submit");
            contentDict.Add("send_to", "/");
            FormUrlEncodedContent formEncContent = new FormUrlEncodedContent(contentDict);

            // Send the post request
            //HttpClientHandler httpHandler = new HttpClientHandler();
            //httpHandler.CookieContainer = new CookieContainer();
            //HttpClient httpClient = new HttpClient();
            Uri loginUrl = GetUrlFromRelative(LOGIN_URL_SEGMENT);
            HttpResponseMessage response = await httpClient.PostAsync(loginUrl, formEncContent);
            response.EnsureSuccessStatusCode();

            // Interpret the response
            // Update the cookies
            //UpdateCookies(response);

            // Check for a failed login
            string contentString = await response.Content.ReadAsStringAsync();
            if (FailedLoginRegex.IsMatch(contentString))
                throw new Exception("Writing.com login failed");
            

        }

        public Uri GetUrlFromRelative(string relativePath)
        {
            return new Uri(Settings.WritingUrlRoot, relativePath);
        }

        public Uri GetInteractiveUrl(string interactiveUrlParm)
        {
            // E.g. https://www.writing.com/main/interact/item_id/1824771-short-stories-by-the-people/
            return GetUrlFromRelative(string.Format(INTERACTIVE_URL_SEGMENT, interactiveUrlParm));
        }

        public Uri GetInteractiveOutlineMapUrl(string interactiveUrlParm)
        {
            // E.g. https://www.writing.com/main/interact/item_id/1824771-short-stories-by-the-people/action/outline
            return GetUrlFromRelative(string.Format(INTERACTIVE_OUTLINE_URL_SEGMENT, interactiveUrlParm));
        }

        public Uri GetInteractiveChapterUrl(string interactiveUrlParm, string chapterMap)
        {
            // E.g. https://www.writing.com/main/interact/item_id/1824771-short-stories-by-the-people/map/11212
            return GetUrlFromRelative(string.Format(INTERACTIVE_CHAPTER_URL_SEGMENT, interactiveUrlParm, chapterMap));
        }

        // TODO: for some reason it gets the first chapter twice. I don't know why, but it's not a huge deal, so I'm in no hurry.
        public async Task<Uri[]> GetAllInteractiveChapterUrls(string interactiveUrlParm)
        {
            log.DebugFormat("Getting interactive story chapter map for {0}", interactiveUrlParm);

            var newMap = new List<Uri>();

            Uri interactiveOutlineUrl = GetInteractiveOutlineMapUrl(interactiveUrlParm);
            string interactiveMapHtml = await HttpGetAsyncAsString(interactiveOutlineUrl);

            // Do we need to login?
            if (IsLoginPage(interactiveMapHtml))
            {
                // We need to log in, and get the map again
                log.Debug("Login required while trying to get an interactive chapter map");
                await LoginAsync();

                // Try and get the map again after logging in
                interactiveMapHtml = await HttpGetAsyncAsString(interactiveOutlineUrl);

                //If it's a login page again, shit self
                if (IsLoginPage(interactiveMapHtml))
                    throw new Exception("Failed to login to retrieve the interactive story outline");
            }

            // Detect "Interactives temporarily unavailable"
            if (IsInteractivesUnavailablePage(interactiveMapHtml))
            {
                throw new InteractivesTemporarilyUnavailableException();
            }


            // Find the links to the interactive's pages
            // Create the regex that will find chapter links
            // E.g. https:\/\/www\.writing\.com\/main\/interact\/item_id\/1824771-short-stories-by-the-people\/map\/(\d)+
            string mapLinkRegexPattern = Settings.WritingUrlRoot.ToString() + string.Format(INTERACTIVE_CHAPTER_URL_SEGMENT, interactiveUrlParm, @"(\d)+");
            mapLinkRegexPattern = RegexSafeUrl(mapLinkRegexPattern);
            Regex mapLinkRegex = new Regex(mapLinkRegexPattern, RegexOptions.IgnoreCase);
            MatchCollection matches = mapLinkRegex.Matches(interactiveMapHtml);

            foreach (Match match in matches)
            {
                newMap.Add(new Uri(match.Value));
            }

            return newMap.ToArray();
        }

        public async Task<string[]> GetAllInteractiveChapterMapIds(string interactiveUrlParm)
        {
            Uri[] chapterUrls = await GetAllInteractiveChapterUrls(interactiveUrlParm);

            string[] chapterMapIds = new string[chapterUrls.Length];
            for (var i=0; i < chapterUrls.Length; i++)
            {
                string chapterUrl = chapterUrls[i].ToString();
                chapterMapIds[i] = GetMapUrlParameter(chapterUrl);
            }

            return chapterMapIds;
        }

        public async Task<WInteractiveChapter> GetInteractiveChapter(Uri chapterUrl)
        {
            log.DebugFormat("Getting interactive chapter {0}", chapterUrl.AbsolutePath);

            var newChapter = new WInteractiveChapter();
            string chapterUrlParm = GetMapUrlParameter(chapterUrl.ToString());

            string chapterHtml = await HttpGetAsyncAsString(chapterUrl);
            // DEBUG
            //string chapterHtml = System.IO.File.ReadAllText("test-interactive-page.html");

            // TODO: detect either requesting a login, or that "interactives temporarily unavailable" mesage

            // Detect "Login required"
            if (IsLoginPage(chapterHtml))
            {
                // We need to log in, and get the chapter again
                log.Debug("Login required while trying to get an interactive chapter");
                await LoginAsync();

                // Get it again
                chapterHtml = await HttpGetAsyncAsString(chapterUrl);

                // Check if it's a login again. If it is, login failed
                if (IsLoginPage(chapterHtml))
                    throw new Exception("Failed to login to retrieve interactive chapter");
            }

            // Detect "Interactives temporarily unavailable"
            // TODO: Should this be in here? If we want to better handle cancelling, I'm wondering if this should throw a specialised exception
            //      and let the thread above handle retrying
            if (IsInteractivesUnavailablePage(chapterHtml))
            {
                throw new InteractivesTemporarilyUnavailableException();
            }

            // Get chapter title

            // Method 1. Get it from the "Your path to this chapter"
            // CAUTION: can sometimes get truncated, but this appears to be the the legit title from the database, it was truncated when the chapter was made
            // NOTE: Fails on the first chapter, because there's no choices made yet
            //string chapterTitleRegexPattern = string.Format("(?<=\\/map\\/{0}\">).*?(?=<\\/a>)", chapterUrlParm);

            // Method 2. Get it from between <big><big><b>...</b></big></big>
            // There are other isntances of the <big><b> tags in use, but only the chapter title gets wrapped in 2x of them
            // Isn't perfect, but until the website layout changes, it'll work
            string chapterTitleRegexPattern = @"(?<=<big><big><b>).*?(?=<\/b><\/big><\/big>)";

            Regex chapterTitleRegex = new Regex(chapterTitleRegexPattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterTitleMatch = chapterTitleRegex.Match(chapterHtml);
            if (!chapterTitleMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the chapter title for chapter '{chapterUrl.ToString()}'", chapterHtml);
            string chapterTitle = HttpUtility.HtmlDecode(chapterTitleMatch.Value);

            // Search for the choice that lead to this chapter
            // This usually has the more fleshed out title, as the legit title can sometimes be truncated
            Regex chapterSourceChoiceRegex = new Regex(@"(?<=This choice: <b>).*?(?=<\/b>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterSourceChoiceMatch = chapterSourceChoiceRegex.Match(chapterHtml);

            if (!chapterSourceChoiceMatch.Success && chapterUrlParm != "1") // If we can't find it, and it's not the first chapter
                throw new WritingClientHtmlParseException($"Couldn't find the interactive chapter's source choice and this isn't the first chapter, for chapter '{chapterUrl.ToString()}'", chapterHtml);
            string chapterSourceChoice = HttpUtility.HtmlDecode(chapterSourceChoiceMatch.Value);

            // Search for the chapter content, the actual writing
            // <div class="KonaBody">stuff goes here</div>
            //Regex chapterContentRegex = new Regex("(?<=<div class=\"KonaBody\">).+?(?=<\\/div>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Regex chapterContentRegex = new Regex("(?<=<div class=\"\">).+?(?=<\\/div>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterContentMatch = chapterContentRegex.Match(chapterHtml);
            if (!chapterContentMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the content for the interactive chapter '{chapterUrl.ToString()}'", chapterHtml);
            string chapterContent = HttpUtility.HtmlDecode(chapterContentMatch.Value);

            // Get the author
            // <a title="Username: rpcity Member Since: July 4th, 2002 Click for links!" style="font - size:1em; font - weight:bold; cursor: pointer; ">SmittySmith</a>
            Regex chapterAuthorChunkRegex = new Regex("<a title=\"Username: .*?<\\/a>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match chapterAuthorChunkMatch = chapterAuthorChunkRegex.Match(chapterHtml);
            if (!chapterAuthorChunkMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the HTML chunk containing the author for the interactive chapter '{chapterUrl.ToString()}'", chapterHtml);
            string chapterAuthorChunk = chapterAuthorChunkMatch.Value;

            // Get the author username
            Regex chapterAuthorUsernameRegex = new Regex("(?<=Username: )[a-zA-Z]+");
            Match chapterAuthorUsernameMatch = chapterAuthorUsernameRegex.Match(chapterAuthorChunk);
            string chapterAuthorUsername = chapterAuthorUsernameMatch.Value;

            // Get the author display name
            Regex chapterAuthorNameRegex = new Regex("(?<=>).+?(?=<)");
            Match chapterAuthorNameMatch = chapterAuthorNameRegex.Match(chapterAuthorChunk);
            string chapterAuthorName = chapterAuthorNameMatch.Value;

            // End end chapters
            if (IsInteractiveChapterEnd(chapterHtml))
            {
                newChapter.IsEnd = true;
            }
            else
            {
                // Search for the available choices
                // This one is going to be complicated, because none of the divs or whatnot have ID's
                // First, get a chunk of the HTML that contains the choices, we'll break them down later
                Regex chapterChoicesChunkRegex = new Regex("(?<=<b>You have the following choice(s)?:<\\/b>).*?(?=<\\/div><div id=\"end_of_choices\")",
                    RegexOptions.Singleline | RegexOptions.IgnoreCase);
                Match chapterChoicesChunkMatch = chapterChoicesChunkRegex.Match(chapterHtml);
                if (!chapterChoicesChunkMatch.Success)
                    throw new WritingClientHtmlParseException($"Couldn't find the HTML chunk containing choices for interactive chapter '{chapterUrl.ToString()}'", chapterHtml);
                string chapterChoicesChunkHtml = chapterChoicesChunkMatch.Value;

                // Then try to get the individual choices
                Regex chapterChoicesRegex = new Regex("<a .*?href=\".+?\">.+?<\\/a>", RegexOptions.IgnoreCase);
                MatchCollection chapterChoicesMatches = chapterChoicesRegex.Matches(chapterChoicesChunkHtml);
                foreach (Match match in chapterChoicesMatches)
                {
                    string choiceUrl;
                    string choiceMapUrlParm;
                    string choiceName;

                    // Get the URL
                    Regex choiceUrlRegex = new Regex("(?<=href=\").+?(?=\")");
                    Match choiceUrlMatch = choiceUrlRegex.Match(match.Value);
                    if (!choiceUrlMatch.Success)
                        throw new WritingClientHtmlParseException($"Could not find the URL of choice '{match.Value}' on interactive chapter '{chapterUrl.ToString()}'", chapterHtml);
                    choiceUrl = choiceUrlMatch.Value;

                    // Get just the numbers from the URL
                    choiceMapUrlParm = GetMapUrlParameter(choiceUrl);

                    // Get the choice name / description
                    int indexOfGt = match.Value.IndexOf('>');
                    int indexofLt = match.Value.LastIndexOf('<') - 1;
                    choiceName = HttpUtility.HtmlDecode(match.Value.Substring(indexOfGt + 1, indexofLt - indexOfGt));

                    newChapter.Choices.Add(new WInteractiveChapterChoice()
                    {
                        MapLink = choiceMapUrlParm,
                        Name = choiceName
                    });
                }
            }



            // Put the rest together
            newChapter.Author = new WAuthor() { Name = chapterAuthorName, Username = chapterAuthorUsername };
            newChapter.Path = chapterUrlParm;
            newChapter.Content = chapterContent;
            newChapter.SourceChoiceTitle = chapterSourceChoice;
            newChapter.Title = chapterTitle;

            return newChapter;
        }

        public async Task<WInteractiveChapter> GetInteractiveChapter(string interactiveUrlParm, string chapterUrlParm)
        {
            Uri chapterUrl = GetInteractiveChapterUrl(interactiveUrlParm, chapterUrlParm);
            return await GetInteractiveChapter(chapterUrl);
        }

        public async Task<WInteractiveStory> GetInteractive(string interactiveUrlParm)
        {
            Uri interactiveUrl = GetInteractiveUrl(interactiveUrlParm);
            return await GetInteractive(interactiveUrl);
        }

        public async Task<WInteractiveStory> GetInteractive(Uri interactiveUrl)
        {
            
            WInteractiveStory newInteractive = new WInteractiveStory();
            log.DebugFormat("Getting interactive story: {0}", interactiveUrl);

            string interactiveHtml = await HttpGetAsyncAsString(interactiveUrl);
            // DEBUG
            //string interactiveHtml = System.IO.File.ReadAllText("test-interactive-cover.html");
            if (IsInteractivesUnavailablePage(interactiveHtml))
                throw new InteractivesTemporarilyUnavailableException();

            // Detect "Login required" or "access restricted"
            if (IsLoginPage(interactiveHtml))
            {
                // We need to log in, and get the chapter again
                log.Debug("Login required while trying to get an interactive's details");
                await LoginAsync();

                // Get it again
                interactiveHtml = await HttpGetAsyncAsString(interactiveUrl);

                // Check if it's a login again. If it is, login failed
                if (IsLoginPage(interactiveHtml))
                    throw new Exception("Failed to login to retrieve interactive details");
            }


            // Get the interactive story's title
            // This method grabs it from within the <title> element, not sure if it gets truncated or not.
            Regex interactiveTitleRegex = new Regex("(?<=<title>).+?(?= - Writing\\.Com<\\/title>)", RegexOptions.IgnoreCase);
            Match interactiveTitleMatch = interactiveTitleRegex.Match(interactiveHtml);
            if (!interactiveTitleMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the title for interactive story '{interactiveUrl.ToString()}'", interactiveHtml);
            string interactiveTitle = HttpUtility.HtmlDecode(interactiveTitleMatch.Value);

            // Get the interactive's tagline or short description
            // Previously this has been difficult o pin-point
            // However I found this on 11/01/2019, they've got it in a META tag at the top of the HTML
            // E.g. <META NAME="description" content="How will young James fare alone with his mature, womanly neighbors? ">
            Regex interactiveShortDescRegex = new Regex("(?<=<META NAME=\"description\" content=\").+?(?=\">)", RegexOptions.IgnoreCase);
            Match interactiveShortDescMatch = interactiveShortDescRegex.Match(interactiveHtml);
            if (!interactiveShortDescMatch.Success)
                log.Warn($"Couldn't find the short description for interactive story '{interactiveUrl.ToString()}'"); // Just a warning, don't throw an exception over it
            string interactiveShortDesc = HttpUtility.HtmlDecode(interactiveShortDescMatch.Value);


            // Get the interactive story's description
            Regex interactiveDescRegex = new Regex("(?<=<td align=left class=\"norm\">).+?(?=<\\/td>)", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            Match interactiveDescMatch = interactiveDescRegex.Match(interactiveHtml);
            if (!interactiveDescMatch.Success)
                throw new WritingClientHtmlParseException($"Couldn't find the description for interactive story '{interactiveUrl.ToString()}'", interactiveHtml);
            string interactiveDesc = HttpUtility.HtmlDecode(interactiveDescMatch.Value);

            // TODO get author, looks like it'll be a pain in the ass telling the chapter author apart from the story author

            newInteractive.Url = interactiveUrl.ToString();
            newInteractive.UrlID = interactiveUrl.Segments[interactiveUrl.Segments.Length-1].TrimEnd('/'); // Did I just break this?
            newInteractive.Name = interactiveTitle;
            newInteractive.Description = interactiveDesc;
            newInteractive.ShortDescription = interactiveShortDesc;

            return newInteractive;
        }

        private string RegexSafeUrl(object url)
        {
            // Things like forward slashes and dots cause shenanigans with regex
            return url.ToString().Replace(".", "\\.").Replace("/", "\\/");
        }

        public string GetMapUrlParameter(object url)
        {
            string urlString = url.ToString();
            int indexOfSlash = urlString.LastIndexOf('/');
            return urlString.Substring(indexOfSlash + 1, urlString.Length - indexOfSlash - 1); // Just get the numbers at the end
        }

        private bool IsLoginPage(string content)
        {
            //Method: look for a username and password field
            return UsernameFieldRegex.IsMatch(content);
        }

        private bool IsInteractiveChapterEnd(string chapterHtml)
        {
            //Regex chapterEndRegex = new Regex("<big>THE END.<\\/big>");// Turns out this doesn't work, because they HTML tagging is sloppy and overlaps. <i><b>THE END.</i></b>
            Regex chapterEndRegex = new Regex(">You have come to the end of the story. You can:<\\/");
            return chapterEndRegex.IsMatch(chapterHtml);
        }

        private bool IsInteractivesUnavailablePage(string content)
        {
            return InteractivesUnavailableRegex1.IsMatch(content) || InteractivesUnavailableRegex2.IsMatch(content);
        }

        private async Task<HttpResponseMessage> HttpGetAsync(Uri urlToGet)
        {
            HttpResponseMessage response = await httpClient.GetAsync(urlToGet);
            response.EnsureSuccessStatusCode(); // Fail if result is not 200 OK

            //UpdateCookies(response);

            return response;
        }

        private async Task<string> HttpGetAsyncAsString(Uri urlToGet)
        {
            HttpResponseMessage response = await HttpGetAsync(urlToGet);
            return await response.Content.ReadAsStringAsync();
        }

        private CookieCollection GetCookies(HttpClientHandler handler, Uri url)
        {
            return handler.CookieContainer.GetCookies(url);
        }

        // Certain text symbols have an HTML escape encoding (there's probably a proper name, idk).
        // This replaces those with the correct characters.
        public static string CleanHtmlSymbols(string rawText)
        {
            int htmlSymbolIndex = rawText.IndexOf("&#");
            while (htmlSymbolIndex >= 0)
            {
                string htmlNumStr = rawText.Substring(htmlSymbolIndex + 2, 2);
                string symbol = "" + (char)int.Parse(htmlNumStr);
                rawText = rawText.Remove(htmlSymbolIndex, 5);
                rawText = rawText.Insert(htmlSymbolIndex, symbol);
                htmlSymbolIndex = rawText.IndexOf("&#");
            }
            return rawText;
        }
    }
}
