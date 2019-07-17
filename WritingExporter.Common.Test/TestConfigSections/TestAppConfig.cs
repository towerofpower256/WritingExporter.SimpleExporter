using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritingExporter.Common.Configuration;

namespace WritingExporter.Common.Test.TestConfigSections
{
    [Serializable]
    public class TestAppConfig : BaseConfigSection
    {
        public bool AutoStart { get; set; } = true;
        public int SyncInterval { get; set; } = 5;
        public DateTime LastStart { get; set; } = DateTime.Now;
        public Collection<string> RecentFiles = new Collection<string>();
    }
}
