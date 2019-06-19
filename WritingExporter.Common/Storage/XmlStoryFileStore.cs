using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using WritingExporter.Common.Models;
using System.IO;

namespace WritingExporter.Common.Storage
{
    public class XmlStoryFileStore : IStoryFileStore
    {
        private ILogger _log = LogManager.GetLogger(typeof(XmlStoryFileStore));
        XmlSerializer _serializer;

        public XmlStoryFileStore()
        {
            // Init the serializer
            _serializer = new XmlSerializer(typeof(WdcInteractiveStory));
        }

        public void SaveStory(WdcInteractiveStory story, string filePath)
        {
            _log.Debug($"Saving story '{story.ID}' to: {filePath}");
            
            using (var f = File.Open(filePath, FileMode.Create))
            {
                SerializeStory(story, f);
            }
        }

        public WdcInteractiveStory LoadStory(string filePath)
        {
            _log.Debug($"Saving story from: {filePath}");

            using (var f = File.Open(filePath, FileMode.Open))
            {
                return DeserializeStory(f);
            }
        }

        public WdcInteractiveStory DeserializeStory(Stream stream)
        {
            return _serializer.Deserialize(stream) as WdcInteractiveStory;
        }

        public WdcInteractiveStory DeserializeStory(string payload)
        {
            using (StringReader tr = new StringReader(payload))
            {
                return _serializer.Deserialize(tr) as WdcInteractiveStory;
            }
        }

        public string SerializeStoryToString(WdcInteractiveStory story)
        {
            using (StringWriter sw = new StringWriter())
            {
                _serializer.Serialize(sw, story);
                return sw.ToString();
            }
        }

        public void SerializeStory(WdcInteractiveStory story, Stream stream)
        {
            _serializer.Serialize(stream, story);
        }
    }
}
