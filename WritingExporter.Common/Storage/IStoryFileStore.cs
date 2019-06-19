using System.IO;
using WritingExporter.Common.Models;

namespace WritingExporter.Common.Storage
{
    public interface IStoryFileStore
    {
        WdcInteractiveStory DeserializeStory(Stream stream);
        WdcInteractiveStory DeserializeStory(string payload);
        WdcInteractiveStory LoadStory(string filePath);
        void SaveStory(WdcInteractiveStory story, string filePath);
        void SerializeStory(WdcInteractiveStory story, Stream stream);
        string SerializeStoryToString(WdcInteractiveStory story);
    }
}