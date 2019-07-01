using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Test.TestConfigSections;

namespace WritingExporter.Common.Test
{
    [TestClass]
    public class ConfigurationTests
    {
        public ConfigurationTests()
        {
            TestUtil.SetupLogging();
        }

        [TestMethod]
        public void SaveAndLoad()
        {
            var configSave = new ConfigProvider();

            configSave.SetSection(new TestAppConfig());
            configSave.SetSection(new TestUiConfigSection());

            configSave.SaveSettings();

            var configLoad = new ConfigProvider();
            configLoad.LoadSettings();
        }

        // Get an unset section. It should safely return the a new instance of that section with defaults set, and not throw an exception.
        [TestMethod]
        public void GetDefault()
        {
            var config = new ConfigProvider();
            config.GetSection<TestUiConfigSection>();
        }
    }
}
