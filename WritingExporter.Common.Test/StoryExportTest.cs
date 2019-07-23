using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WritingExporter.Common;
using WritingExporter.Common.Models;
using WritingExporter.Common.Export;
using WritingExporter.Common.Storage;

namespace WritingExporter.Common.Test
{
    [TestClass]
    public class StoryExportTest
    {
        public StoryExportTest()
        {
            TestUtil.SetupLogging();
        }

        [TestMethod]
        public void ExportWholeStory()
        {
            var storyLoader = new XmlStoryFileStore();
            var story = storyLoader.DeserializeStory(TestUtil.GetDataFile(@"SampleStories.1824771-short-stories-by-the-people.xml"));

            var exporter = new WdcStoryExporterHtmlCollection("StoryOutput");
            exporter.ExportStory(story);
        }
    }
}
