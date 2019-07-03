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

            var sectionA = new TestAppConfig();
            var sectionB = new TestUiConfigSection();

            configSave.SetSection(sectionA);
            configSave.SetSection(sectionB);

            configSave.SaveSettings();

            var configLoad = new ConfigProvider();
            configLoad.LoadSettings();

            // TODO check if the sections are the same
            Assert.IsTrue(TestUtil.SerializeAndCompare(
                configSave.GetSection<TestAppConfig>(),
                configLoad.GetSection<TestAppConfig>()
                ));

            Assert.IsTrue(TestUtil.SerializeAndCompare(
                configSave.GetSection<TestUiConfigSection>(),
                configLoad.GetSection<TestUiConfigSection>()
                ));
        }

        // Get an unset section. It should safely return the a new instance of that section with defaults set, and not throw an exception.
        [TestMethod]
        public void GetDefault()
        {
            var config = new ConfigProvider();
            config.GetSection<TestUiConfigSection>();
        }

        // Test the SectionChanged event
        [TestMethod]
        public void OnSectionChangedEvent()
        {
            ConfigSectionChangedEventArgs testEventArgs = null;

            var config = new ConfigProvider();
            config.OnSectionChanged += new EventHandler<ConfigSectionChangedEventArgs>((sender, args) => testEventArgs = args);

            var newSection = new TestAppConfig();
            config.SetSection(newSection);

            Assert.IsNotNull(testEventArgs);
        }
    }
}
