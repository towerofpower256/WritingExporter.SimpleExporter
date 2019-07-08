using System.Collections.Generic;
using System.IO;
using WritingExporter.Common.Models;

namespace WritingExporter.Common.Storage
{
    public interface IStoryFileStore
    {
        void DeleteStory(string fileName);
        void DeleteStory(WdcInteractiveStory story);
        WdcInteractiveStory DeserializeStory(Stream stream);
        WdcInteractiveStory DeserializeStory(string payload);
        WdcInteractiveStory LoadStory(string filePath);
        IEnumerable<WdcInteractiveStory> LoadAllStories();
        void SaveStory(WdcInteractiveStory story);
        void SaveStory(WdcInteractiveStory story, string filePath);
        void SerializeStory(WdcInteractiveStory story, Stream stream);
        string SerializeStoryToString(WdcInteractiveStory story);
        string GenerateFilename(WdcInteractiveStory story);
    }
}