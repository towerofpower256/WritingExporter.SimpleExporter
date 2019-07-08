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
    public class ConfigSectionChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Name of the section that was updated.
        /// </summary>
        public string SectionName { get; set; }

        /// <summary>
        /// Quick and type-safe method of checking if the updated section is of a type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public bool IsSectionType(Type type)
        {
            return string.Equals(type.Name, SectionName);
        }
    }

    public class ConfigProvider : IConfigProvider
    {
        const string SETTINGS_FILENAME = "config.xml";
        const string SECTION_ROOT_ELEMENT_NAME = "ConfigSections";

        public event EventHandler<ConfigSectionChangedEventArgs> OnSectionChanged;

        private static ILogger _log = LogManager.GetLogger(typeof(ConfigProvider));

        Dictionary<string, XElement> _configSections;
        object _lock;

        public ConfigProvider()
        {
            _configSections = new Dictionary<string, XElement>();
            _lock = new object();
        }

        public TSection GetSection<TSection>() where TSection : BaseConfigSection
        {
            return this.GetSection<TSection>(typeof(TSection).Name);
        }

        public TSection GetSection<TSection>(string sectionName) where TSection : BaseConfigSection
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            _log.Debug($"Getting section: {sectionName}");

            lock (_lock)
            {
                if (_configSections.ContainsKey(sectionName))
                {
                    return FromXElement<TSection>(_configSections[sectionName]);
                }
            }

            // Made it out here, it wasn't a section seen before
            // Just create a new one
            _log.Debug($"Tried to get unset section, returning default instance of type {typeof(TSection).Name}");
            return (TSection)Activator.CreateInstance(typeof(TSection));
        }

        public void SetSection<TSection>(TSection updatedSection) where TSection : BaseConfigSection
        {
            this._SetSection<TSection>(typeof(TSection).Name, updatedSection);
        }

        public void SetSection<TSection>(string sectionName, TSection updatedSection) where TSection : BaseConfigSection
        {
            this._SetSection(sectionName, updatedSection);
        }

        private void _SetSection<TSection>(string sectionName, TSection updatedSection, bool silent = false)
        {
            if (string.IsNullOrEmpty(sectionName))
                throw new ArgumentNullException("sectionName");

            if (updatedSection == null)
                throw new ArgumentNullException("updatedSection");

            _log.Debug($"Updating section: {sectionName}");

            lock (_lock)
            {
                _configSections[sectionName] = ToXElement<TSection>(updatedSection);
                if (!silent) DoSectionChangedEvent(sectionName);
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

            foreach (var key in _configSections.Keys)
            {
                DoSectionChangedEvent(key);
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

        private void DoSectionChangedEvent(string sectionName)
        {
            _log.Debug($"Triggering SectionChanged event for: {sectionName}");

            OnSectionChanged?.Invoke(this, new ConfigSectionChangedEventArgs()
            {
                SectionName = sectionName
            });
        }

        private XElement ToXElement<T>(T obj)
        {
            using (var stream = new MemoryStream())
            {
                using (var streamWriter = new StreamWriter(stream))
                {
                    // Use a custom namespace, to prevent the namespace declarations
                    var ns = new XmlSerializerNamespaces();
                    ns.Add(string.Empty, string.Empty);

                    var xmlSerializer = new XmlSerializer(typeof(T));
                    xmlSerializer.Serialize(streamWriter, obj, ns);
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
