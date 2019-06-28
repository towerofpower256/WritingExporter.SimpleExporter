using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using WritingExporter.Common.Models;
using System.IO;
using System.Collections.ObjectModel;

namespace WritingExporter.Common.Storage
{
    public class XmlStoryFileStore : IStoryFileStore
    {
        private const string DEFAULT_FILE_SUFFIX = ".xml";
        private ILogger _log = LogManager.GetLogger(typeof(XmlStoryFileStore));

        XmlSerializer _serializer;
        string _saveDir;

        public XmlStoryFileStore()
        {
            // Init the serializer
            _serializer = new XmlSerializer(typeof(WdcInteractiveStory));

            //Default
            _saveDir = "Stories";
        }

        public void SaveStory(WdcInteractiveStory story)
        {
            SaveStory(story, Path.Combine(_saveDir, GenerateFilename(story)));
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
            _log.Debug($"Loading story from: {filePath}");

            using (var f = File.Open(filePath, FileMode.Open))
            {
                return DeserializeStory(f);
            }
        }

        public IEnumerable<WdcInteractiveStory> LoadAllStories()
        {
            return LoadAllStories(_saveDir);
        }

        public IEnumerable<WdcInteractiveStory> LoadAllStories(string filePath)
        {
            _log.DebugFormat("Loading all stories from: {0}", filePath);

            var newList = new Collection<WdcInteractiveStory>();

            foreach (var filename in Directory.GetFiles(Path.Combine(_saveDir, $"*{GetDefaultFileSuffix()}")))
            {
                try
                {
                    newList.Add(LoadStory(filename));
                }
                catch (Exception ex) {
                    _log.Warn($"Error loading story while trying to load all stories: {filename}", ex);
                }
            }

            return newList;
        }

        public string GenerateFilename(WdcInteractiveStory story)
        {
            return $"{story.ID}{GetDefaultFileSuffix()}";
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

        public string GetDefaultFileSuffix()
        {
            return DEFAULT_FILE_SUFFIX;
        }
    }
}
