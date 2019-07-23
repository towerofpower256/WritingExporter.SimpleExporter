using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WritingExporter.Common;
using WritingExporter.Common.Models;
using WritingExporter.Common.StorySync;
using System.IO;
using WritingExporter.Common.Storage;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Wdc;
using WritingExporter.Common.Gui;

namespace WritingExporter.Common.Test
{
    [TestClass]
    public class StorySyncWorkerTest
    {
        public StorySyncWorkerTest()
        {
            TestUtil.SetupLogging();
        }

        [TestMethod]
        public void TestSync()
        {
            var config = new ConfigProvider();
            var fileStore = new XmlStoryFileStore();
            var storyContainer = new WdcStoryContainer(fileStore);
            var wdcClient = new WdcClient(config);
            var wdcReader = new WdcReader();
            var fileDumper = new FileDumper();
            var guiContext = new DummyGuiContext();
            var syncWorker = new StorySyncWorker(storyContainer, wdcReader, wdcClient, config, fileDumper, guiContext);

            // Add a story
            storyContainer.AddStory(new WdcInteractiveStory()
            {
                ID = "https://www.writing.com/main/interact/item_id/1824771-short-stories-by-the-people"
            }, false);

            // Start the worker
            syncWorker.StartWorker();

            // Pause forever
            Console.ReadLine();
        }
    }
}
