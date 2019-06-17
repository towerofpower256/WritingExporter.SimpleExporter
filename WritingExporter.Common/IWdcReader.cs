using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WritingExporter.Common.Models;

namespace WritingExporter.Common
{
    public interface IWdcReader
    {
        Task<WdcInteractiveChapter> GetInteractiveChaper(string interactiveID, string chapterPath);
        Task<WdcInteractiveChapter> GetInteractiveChaper(string interactiveID, string chapterPath, WdcResponse payload);
        string GetInteractiveChapterContent(WdcResponse payload);
        Task<IEnumerable<Uri>> GetInteractiveChapterList(string interactiveID);
        string GetInteractiveChapterSourceChoice(WdcResponse payload);
        string GetInteractiveChapterTitle(WdcResponse payload);
        Task<WdcInteractiveStory> GetInteractiveStory(string interactiveID);
        Task<WdcInteractiveStory> GetInteractiveStory(string interactiveID, WdcResponse payload);
        WdcAuthor GetInteractiveStoryAuthor(WdcResponse wdcPayload);
        string GetInteractiveStoryDescription(WdcResponse wdcPayload);
        string GetInteractiveStoryShortDescription(WdcResponse wdcPayload);
        string GetInteractiveStoryTitle(WdcResponse wdcPayload);
    }
}