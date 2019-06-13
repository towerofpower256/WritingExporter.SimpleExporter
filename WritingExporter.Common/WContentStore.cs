using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common
{
    // Class to get HTML from Writing.com
    public class WContentStore
    {
        private const string URL_ROOT = "https://www.writing.com/";
        private const string LOGIN_FIELD_NAME_USERNAME = "login_username";
        private const string LOGIN_FIELD_NAME_PASSWORD = "login_password";
        private const string HTTP_SET_COOKIE_HEADER = "Set-Cookie";
        private const string LOGIN_URL_SEGMENT = "main/login.php";

        private HttpClientHandler httpClientHandler;
        private HttpClient httpClient;
        private CookieContainer httpCookies;
        private Dictionary<string, string> cookieDict = new Dictionary<string, string>();

        public Uri GetPathToRoot()
        {
            return new Uri(URL_ROOT);
        }

        private void Initialise()
        {
            
            httpCookies = new CookieContainer();
            httpClientHandler = new HttpClientHandler();
            httpClientHandler.CookieContainer = httpCookies;
            httpClient = new HttpClient(httpClientHandler, true);
        }

        public async Task LoginAsync(string username, string password)
        {
            //log.Debug("Logging into writing.com");

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
            Uri loginUrl = GetPathToLogin();
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

        public Uri GetPathToLogin()
        {
            return new Uri(GetPathToRoot(), LOGIN_URL_SEGMENT);
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure
        public Uri GetPathToStory(string storyId)
        {
            return new Uri(GetPathToRoot(), $"/main/interact/item_id/{storyId}");
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/action/outline
        public Uri GetPathToStoryOutline(string storyId)
        {
            return new Uri(GetPathToStory(storyId), "/action/outline");
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/action/recent_chapters
        public Uri GetPathToStoryRecentAdditions(string storyId)
        {
            return new Uri(GetPathToStory(storyId), "/action/recent_chapters");
        }

        // E.g. https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/map/1
        public Uri GetPathToStoryChapter(string storyId, string chapterId)
        {
            return new Uri(GetPathToStory(storyId), $"/map/{chapterId}");
        }
    }
}
