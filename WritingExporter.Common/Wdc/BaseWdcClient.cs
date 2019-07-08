using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WritingExporter.Common.Wdc
{
    public abstract class BaseWdcClient
    {
        internal const string LOGIN_FIELD_NAME_USERNAME = "login_username";
        internal const string LOGIN_FIELD_NAME_PASSWORD = "login_password";

        public bool IsLoginPage(string html)
        {
            // Regex to detect the login fields, which should only be visible if we've tried to access something that requires logging in
            Regex UsernameFieldRegex = new Regex(
                string.Format(@"<input.*{0}.*>", LOGIN_FIELD_NAME_USERNAME),
                RegexOptions.IgnoreCase
                );

            //Method: look for a username and password field
            return UsernameFieldRegex.IsMatch(html);
        }

        public bool IsLoginFailedPage(string html)
        {
            // Regex to detect the "Failed Login" page. Look for it in the page's title
            Regex FailedLoginRegex = new Regex(
                @"<title>.*(Login Error|Login failed).*<\/title>",
                RegexOptions.IgnoreCase
            );

            return FailedLoginRegex.IsMatch(html);
        }

        public bool IsInteractivesUnavailablePage(string content)
        {
            // Regex to detect the "Interactives temporarily unavailable due to resource limitations" message
            Regex InteractivesUnavailableRegex1 = new Regex(
                @"<title>.*(Interactives Temporarily Unavailable|Interactive Stories Are Temporarily Unavailable).*<\/title>",
                RegexOptions.IgnoreCase
                );

            // Regex to detect the "Interactive Stories are temporarily unavailable" message
            // Can sometimes appear different to the first message
            Regex InteractivesUnavailableRegex2 = new Regex(
                @"<b>Interactive Stories</b> are <br><i>temporarily</i> unavailable",
                RegexOptions.IgnoreCase
                );

            return InteractivesUnavailableRegex1.IsMatch(content) || InteractivesUnavailableRegex2.IsMatch(content);
        }
    }
}
