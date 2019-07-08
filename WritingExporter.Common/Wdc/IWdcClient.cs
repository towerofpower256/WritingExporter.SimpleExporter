using System;
using System.Threading;
using System.Threading.Tasks;
using WritingExporter.Common.Models;

namespace WritingExporter.Common.Wdc
{
    public interface IWdcClient
    {
        Task<WdcResponse> GetInteractiveChapter(string interactiveID, string chapterID, CancellationToken ct);
        Task<WdcResponse> GetInteractiveHomepage(string interactiveID, CancellationToken ct);
        Task<WdcResponse> GetInteractiveOutline(string interactiveID, CancellationToken ct);
        Task<WdcResponse> GetInteractiveRecentAdditions(string interactiveID, CancellationToken ct);
        bool IsInteractivesUnavailablePage(string content);
        bool IsLoginFailedPage(string html);
        bool IsLoginPage(string html);
        Uri GetPathToRoot();
        Uri GetPathToLogin();
        Uri GetPathToInteractive(string storyId);
        Uri GetPathToInteractiveOutline(string storyId);
        Uri GetPathToInteractiveRecentAdditions(string storyId);
        Uri GetPathToInteractiveChapter(string storyId, string chapterId);
        void Reset();
    }
}