using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Test.TestConfigSections
{
    [Serializable]
    public class TestUiConfigSection
    {
        public bool ButtonOneEnabled { get; set; }
        public bool ButtonTwoEnabled { get; set; }
        public int TimesButtonOnePressed { get; set; }
    }
}
