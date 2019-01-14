using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WritingExporter.SimpleExporter.Models;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

namespace WritingExporter.SimpleExporter
{
    public static class StoryFileHelperJson
    {
        private static ILogger log = LogManager.GetLogger(typeof(StoryFileHelperJson));

        static StoryFileHelperJson()
        {
            
            
        }

        public static void SaveInteractiveStory(string filePath, WInteractiveStory storyToSave)
        {
            log.DebugFormat("Saving interactive story {0}", filePath);
            string serializedStory = SerializeInteractiveStoryToString(storyToSave);
            File.WriteAllText(filePath, serializedStory);
        }

        public static WInteractiveStory LoadInteractiveStory(string filePath)
        {
            string storyFileContents = File.ReadAllText(filePath);
            return DeserializeInteractiveStory(storyFileContents);
        }

        public static void SerializeInteractiveStory(TextWriter stream, WInteractiveStory storyToSerialize)
        {
            JsonSerializer serializer = new JsonSerializer();
            serializer.Serialize(stream, storyToSerialize);
        }

        public static string SerializeInteractiveStoryToString(WInteractiveStory storyToSerialize)
        {
            return JsonConvert.SerializeObject(storyToSerialize);
        }

        public static WInteractiveStory DeserializeInteractiveStory(string storyToDeserialize)
        {
            return JsonConvert.DeserializeObject<WInteractiveStory>(storyToDeserialize);
        }

        /*
        public static WInteractiveStory DeserializeInteractiveStory(Stream storyToDeserialize)
        {
            return storySerializer.Deserialize(storyToDeserialize) as WInteractiveStory;
        }
        */
    }
}
