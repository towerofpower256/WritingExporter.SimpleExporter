using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WritingExporter.Common.Models;
using WritingExporter.Common.Storage;

namespace WritingExporter.Common.Test
{
    [TestClass]
    public class StoryFileStoreTests
    {
        WdcInteractiveStory _testStory;
        List<string> _postTestFileCleanup = new List<string>();

        public StoryFileStoreTests()
        {
            TestUtil.SetupLogging();
            InitTestStory();
        }

        [TestMethod]
        public void XmlSimpleSaveAndLoad()
        {
            var fname = $"{_testStory.ID}-SimpleSaveAndLoad.xml";

            IStoryFileStore fs = new XmlStoryFileStore();
            fs.SaveStory(_testStory, fname);

            WdcInteractiveStory loadedStory = fs.LoadStory(fname);

            AddFileToClean(fname);
        }

        [TestCleanup]
        public void Cleanup()
        {
            foreach (var fname in _postTestFileCleanup)
            {
                File.Delete(fname);
            }
        }

        private void AddFileToClean(string fname)
        {
            _postTestFileCleanup.Add(fname);
        }

        private void InitTestStory()
        {
            var testAuthor = new WdcAuthor() { Name = "Test name", Username = "testuser" };

            // Story
            _testStory = new WdcInteractiveStory();
            _testStory.Author = testAuthor;
            _testStory.ID = "123456-test-story-my-dude";
            _testStory.Name = "Test Story 2: Electric Bogaloo";
            _testStory.LastUpdated = DateTime.Now;
            _testStory.Url = "http://test.local/test-story-my-dude";
            _testStory.ShortDescription = "Some story happened";
            _testStory.Description = "<p>I am a test description</p>" +
                "absolutely <br /> for real.";

            // Chapter 1
            _testStory.Chapters.Add(new WdcInteractiveChapter()
            {
                Author = testAuthor,
                Title = "The test story begins",
                Content = "This is a test... <br />CHAPTERRRRR!",
                Path = "1",
                Choices = new List<WdcInteractiveChapterChoice>()
                {
                    new WdcInteractiveChapterChoice() { Name = "1st choice", PathLink = "11"},
                    new WdcInteractiveChapterChoice() { Name = "2st choice", PathLink = "12"},
                    new WdcInteractiveChapterChoice() { Name = "3st choice", PathLink = "13"},
                }
            });

            // Chapter 2
            _testStory.Chapters.Add(new WdcInteractiveChapter()
            {
                Author = testAuthor,
                Title = "The test story continues",
                Content = "Then the people<br />\r\ndid<br />\r\nthings",
                Path = "11",
                Choices = new List<WdcInteractiveChapterChoice>()
                {
                    new WdcInteractiveChapterChoice() { Name = "1st choice", PathLink = "11"},
                    new WdcInteractiveChapterChoice() { Name = "2st choice", PathLink = "12"},
                    new WdcInteractiveChapterChoice() { Name = "3st choice", PathLink = "13"},
                }
            });
        }
    }
}
