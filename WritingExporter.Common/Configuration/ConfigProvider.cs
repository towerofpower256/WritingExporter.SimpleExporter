using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace WritingExporter.Common.Configuration
{
    public class ConfigProvider : IConfigProvider
    {
        const string SETTINGS_FILENAME = "config.xml";
        const string SECTION_ROOT_ELEMENT_NAME = "ConfigSections";

        private static ILogger _log = LogManager.GetLogger(typeof(ConfigProvider));

        Dictionary<string, XElement> _configSections;
        object _lock;

        public ConfigProvider()
        {
            _configSections = new Dictionary<string, XElement>();
            _lock = new object();
        }

        public T GetSection<T>()
        {
            return this.GetSection<T>(typeof(T).Name);
        }

        public T GetSection<T>(string sectionName)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            _log.Debug($"Getting section: {sectionName}");

            lock (_lock)
            {
                if (_configSections.ContainsKey(sectionName))
                {
                    return FromXElement<T>(_configSections[sectionName]);
                }
            }

            // Made it out here, it wasn't a section seen before
            // Just create a new one
            _log.Debug($"Tried to get unset section, returning default instance of type {typeof(T).Name}");
            return (T)Activator.CreateInstance(typeof(T));
        }

        public void SetSection<T>(T updatedSection)
        {
            this.SetSection<T>(typeof(T).Name, updatedSection);
        }

        public void SetSection<T>(string sectionName, T updatedSection)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            if (updatedSection == null)
                throw new ArgumentNullException("updatedSection");

            _log.Debug($"Updating section: {sectionName}");

            lock (_lock)
            {
                _configSections[sectionName] = ToXElement<T>(updatedSection);
            }
        }

        public void LoadSettings()
        {
            _log.Debug("Loading settings");

            if (!File.Exists(SETTINGS_FILENAME))
            {
                _log.Info("Tried to load settings from file, but settings file doesn't exist.");
                return;
            }

            var newPropDict = new Dictionary<string, XElement>();

            // Load file
            var xmlDoc = XDocument.Load(SETTINGS_FILENAME);
            

            // Read through the elements
            foreach (var element in xmlDoc.Root.Elements())
            {
                var key = (string)element.Attribute("key");
                newPropDict[key] = new XElement(element.Descendants().First());
            }

            lock (_lock)
            {
                // Update sections dictionary
                _configSections = newPropDict;
            }
        }

        public void SaveSettings()
        {
            _log.Debug("Saving settings");

            // Assemble all of the settings
            List<XElement> propsToSave = new List<XElement>();

            lock (_lock)
            {
                foreach (var prop in _configSections)
                {
                    XAttribute keyAttribute = new XAttribute("key", prop.Key);
                    propsToSave.Add(new XElement("Section", keyAttribute, prop.Value));
                }
            }

            // Save
            new XDocument(new XElement(SECTION_ROOT_ELEMENT_NAME, propsToSave)).Save(SETTINGS_FILENAME);
        }

        private XElement ToXElement<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj);
                    return XElement.Parse(Encoding.ASCII.GetString(stream.ToArray()));
                }
            }
        }

        private T FromXElement<T>(XElement xElement)
        {
            var xmlSerializer = new XmlSerializer(typeof(T));
            return (T)xmlSerializer.Deserialize(xElement.CreateReader());
        }
    }
}
