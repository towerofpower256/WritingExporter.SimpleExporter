using System;
using System.Threading.Tasks;
using WritingExporter.Common.Models;

namespace WritingExporter.Common
{
    public interface IWdcClient
    {
        Task<WdcResponse> GetInteractiveChapter(string interactiveID, string chapterID);
        Task<WdcResponse> GetInteractiveHomepage(string interactiveID);
        Task<WdcResponse> GetInteractiveOutline(string interactiveID);
        Task<WdcResponse> GetInteractiveRecentAdditions(string interactiveID);
        bool IsInteractivesUnavailablePage(string content);
        bool IsLoginFailedPage(string html);
        bool IsLoginPage(string html);
    }
}