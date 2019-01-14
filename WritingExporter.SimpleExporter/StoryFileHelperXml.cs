using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using WritingExporter.SimpleExporter.Models;

namespace WritingExporter.SimpleExporter
{
    public static class StoryFileHelperXml
    {
        private static ILogger log = LogManager.GetLogger(typeof(StoryFileHelperXml));
        private static XmlSerializer storySerializer;

        static StoryFileHelperXml()
        {
            storySerializer = new XmlSerializer(typeof(WInteractiveStory));
            
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

        public static void SerializeInteractiveStory(Stream stream, WInteractiveStory storyToSerialize)
        {
            storySerializer.Serialize(stream, storyToSerialize);

            //return JsonConvert.SerializeObject(storyToSerialize);
        }

        public static string SerializeInteractiveStoryToString(WInteractiveStory storyToSerialize)
        {
            using (StringWriter sw = new StringWriter())
            {
                storySerializer.Serialize(sw, storyToSerialize);
                return sw.ToString();
            }
        }

        public static WInteractiveStory DeserializeInteractiveStory(string storyToDeserialize)
        {
            using (StringReader tr = new StringReader(storyToDeserialize))
            {
                return storySerializer.Deserialize(tr) as WInteractiveStory;
            }

            //return JsonConvert.DeserializeObject<WInteractiveStory>(storyToDeserialize);
        }

        public static WInteractiveStory DeserializeInteractiveStory(Stream storyToDeserialize)
        {
            return storySerializer.Deserialize(storyToDeserialize) as WInteractiveStory;
        }
    }
}
