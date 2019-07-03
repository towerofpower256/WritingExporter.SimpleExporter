using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Exceptions;
using WritingExporter.Common.Models;

namespace WritingExporter.Common
{
    // Class to get HTML from Writing.com
    public class WdcClient : BaseWdcClient, IWdcClient
    {
        private static ILogger _log = LogManager.GetLogger(typeof(WdcClient));

        private const string URL_ROOT = "https://www.writing.com/";
        private const string HTTP_SET_COOKIE_HEADER = "Set-Cookie";
        private const string LOGIN_URL_SEGMENT = "main/login.php";

        private HttpClientHandler httpClientHandler;
        private HttpClient httpClient;
        private CookieContainer httpCookies;
        private Dictionary<string, string> cookieDict = new Dictionary<string, string>();
        private WdcClientConfiguration settings;

        private IConfigProvider _configProvider;
        

        public WdcClient(IConfigProvider configProvider)
        {
            _log.Debug("Starting");

            _configProvider = configProvider;
            _configProvider.OnSectionChanged += new EventHandler<ConfigSectionChangedEventArgs>(OnSettingsUpdate);

            UpdateSettings();
            httpCookies = new CookieContainer();
            httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = httpCookies;
            httpClient = new HttpClient(httpClientHandler, true);
        }

        public void Reset()
        {
            _log.Debug("Resetting");
            ClearCookies();
        }

        private void OnSettingsUpdate(object sender, ConfigSectionChangedEventArgs args)
        {
            if (args.IsSectionType(typeof(WdcClientConfiguration)))
            {
                UpdateSettings();
            }
        }

        private void UpdateSettings()
        {
            _log.Debug("Updating settings");
            // Grab a new settings object from the config provider
            settings = _configProvider.GetSection<WdcClientConfiguration>();
        }

        private void ClearCookies()
        {
            _log.Debug("Clearing cookies");
            var newCookieContainer = new CookieContainer();

            httpCookies = newCookieContainer;
            httpClientHandler.CookieContainer = newCookieContainer;
        }

        public async Task LoginAsync(CancellationToken ct)
        {
            var username = settings.WritingUsername;
            var password = settings.WritingPassword;

            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username", "The Writing.com username cannot be empty");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("username", "The Writing.com password cannot be empty");

            await LoginAsync(username, password, ct);
        }

        public async Task LoginAsync(string username, string password, CancellationToken ct)
        {
            _log.Debug("Logging into writing.com");

            // Endcode login info, ready to be included in the POST data.
            Dictionary<string, string> contentDict = new Dictionary<string, string>();
            contentDict.Add(LOGIN_FIELD_NAME_USERNAME, username);
            contentDict.Add(LOGIN_FIELD_NAME_PASSWORD, password);
            // Not sure if this is needed in the POST data, but it iss by some browsers.
            contentDict.Add("submit", "submit");
            contentDict.Add("send_to", "/");
            FormUrlEncodedContent formEncContent = new FormUrlEncodedContent(contentDict);

            // Send the post request
            Uri loginUrl = GetPathToLogin();
            HttpResponseMessage response = await httpClient.PostAsync(loginUrl, formEncContent, ct);
            response.EnsureSuccessStatusCode();
            // Don't need to handle cookies here, the httpClient has been given its own cookie store.

            // Check for a failed login
            string contentString = await response.Content.ReadAsStringAsync();
            if (IsLoginFailedPage(contentString))
                throw new WritingLoginFailed("Writing.com login failed");

        }

        public async Task<WdcResponse> GetInteractiveHomepage(string interactiveID, CancellationToken ct)
        {
            Uri interactiveUri = GetPathToInteractive(interactiveID);
            _log.DebugFormat("Getting interactive story: {0}", interactiveUri);
            var r = new WdcResponse();
            r.Address = interactiveUri.ToString();
            r.WebResponse = await GetWdcPage(interactiveUri, ct);
            return r;
        }

        public async Task<WdcResponse> GetInteractiveChapter(string interactiveID, string chapterID, CancellationToken ct)
        {
            Uri chapterUri = GetPathToInteractiveChapter(interactiveID, chapterID);
            _log.DebugFormat("Getting interactive story chapter: {0}", chapterUri);
            var r = new WdcResponse();
            r.Address = chapterUri.ToString();
            r.WebResponse = await GetWdcPage(chapterUri, ct);
            return r;
        }

        public async Task<WdcResponse> GetInteractiveOutline(string interactiveID, CancellationToken ct)
        {
            Uri outlineUri = GetPathToInteractiveOutline(interactiveID);
            _log.DebugFormat("Getting interactive story outline: {0}", outlineUri);
            var r = new WdcResponse();
            r.Address = outlineUri.ToString();
            r.WebResponse = await GetWdcPage(outlineUri, ct);
            return r;
        }

        public async Task<WdcResponse> GetInteractiveRecentAdditions(string interactiveID, CancellationToken ct)
        {
            Uri recentAdditionsUri = GetPathToInteractiveRecentAdditions(interactiveID);
            _log.DebugFormat("Getting interactive story recent additions: {0}", recentAdditionsUri);
            var r = new WdcResponse();
            r.Address = recentAdditionsUri.ToString();
            r.WebResponse = await GetWdcPage(recentAdditionsUri, ct);
            return r;
        }

        public async Task<string> GetWdcPage(Uri uri, CancellationToken ct)
        {
            string html = await HttpGetAsyncAsString(uri, ct);

            if (IsInteractivesUnavailablePage(html))
                throw new InteractivesTemporarilyUnavailableException();

            // Detect "Login required" or "access restricted"
            if (IsLoginPage(html))
            {
                // We need to log in, and get the chapter again
                _log.Debug("Login required while trying to get page");
                await LoginAsync(ct);

                // Get it again
                html = await HttpGetAsyncAsString(uri, ct);

                // Check if it's a login again. If it is, login failed
                if (IsLoginPage(html))
                    throw new WritingLoginFailed($"Failed to login to get page: {uri.ToString()}");
            }

            return html;
        }

        public Uri GetPathToRoot()
        {
            return new Uri(URL_ROOT);
        }

        public Uri GetPathToLogin()
        {
            return new Uri(GetPathToRoot(), LOGIN_URL_SEGMENT);
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure
        public Uri GetPathToInteractive(string storyId)
        {
            return new Uri(GetPathToRoot(), $"/main/interact/item_id/{storyId}");
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/action/outline
        public Uri GetPathToInteractiveOutline(string storyId)
        {
            return new Uri(GetPathToInteractive(storyId), "/action/outline");
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/action/recent_chapters
        public Uri GetPathToInteractiveRecentAdditions(string storyId)
        {
            return new Uri(GetPathToInteractive(storyId), "/action/recent_chapters");
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/map/1
        public Uri GetPathToInteractiveChapter(string storyId, string chapterId)
        {
            return new Uri(GetPathToInteractive(storyId), $"/map/{chapterId}");
        }

        private async Task<HttpResponseMessage> HttpGetAsync(Uri urlToGet, CancellationToken ct)
        {
            HttpResponseMessage response = await httpClient.GetAsync(urlToGet, ct);
            response.EnsureSuccessStatusCode(); // Fail if result is not 200 OK

            return response;
        }

        private async Task<string> HttpGetAsyncAsString(Uri urlToGet, CancellationToken ct)
        {
            HttpResponseMessage response = await HttpGetAsync(urlToGet, ct);
            return await response.Content.ReadAsStringAsync();
        }

        private CookieCollection GetCookies(HttpClientHandler handler, Uri url)
        {
            return handler.CookieContainer.GetCookies(url);
        }
    }
}
