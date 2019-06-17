using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WritingExporter.Common
{
    /// <summary>
    /// Utilities for use with Writing.com.
    /// </summary>
    public static class WdcUtil
    {
        /// <summary>
        /// Certain text symbols have an HTML escape encoding (there's probably a proper name, idk). This replaces those with the correct characters.
        /// </summary>
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

        /// <summary>
        /// Check if a chapter path is valid. Should be a string of all digits, nothing else.
        /// </summary>
        /// <param name="chapterPath"></param>
        /// <returns></returns>
        public static bool IsValidChapterPath(string chapterPath)
        {
            Regex numbersOnlyRegex = new Regex(@"\d+");
            return numbersOnlyRegex.IsMatch(chapterPath);
        }

        /// <summary>
        /// Things like forward slashes and dots cause shenanigans with regex. This escapes those.
        /// </summary>
        public static string RegexSafeUrl(object url)
        {
            return url.ToString().Replace(".", "\\.").Replace("/", "\\/");
        }

        /// <summary>
        /// Just return whatever is after the last '/' slash
        /// </summary>
        public static string GetFinalParmFromUrl(object url)
        {
            string urlString = url.ToString();
            int indexOfSlash = urlString.LastIndexOf('/');
            return urlString.Substring(indexOfSlash + 1, urlString.Length - indexOfSlash - 1); // Just get the numbers at the end
        }
    }
}
