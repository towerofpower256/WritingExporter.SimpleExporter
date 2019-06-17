using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using WritingExporter.Common.Models;
using WritingExporter.Common.Exceptions;

namespace WritingExporter.Common.Test
{
    [TestClass]
    public class WdcReaderTests
    {
        IWdcClient _client;
        IWdcReader _reader;

        public WdcReaderTests()
        {
            TestUtil.SetupLogging();
            _client = new DummyWdcClient("Data");
            _reader = new WdcReader(_client);
        }

        [TestMethod]
        public async Task WdcReaderInteractiveStoryAuthor()
        {
            WdcResponse payload = new WdcResponse();
            payload.WebResponse = TestUtil.GetDataFile("sample_set_13_06_2019.Looking for adventure - homepage - logged in.html");

            WdcAuthor result = _reader.GetInteractiveStoryAuthor(payload);

            Assert.AreEqual("The Nameless hermit", result.Name, "The author's name is not what was expected");
            Assert.AreEqual("blackdragon", result.Username, "The author's username is not what was expected");
        }

        [TestMethod]
        public async Task WdcReaderInteractiveStoryShortDescription()
        {
            WdcResponse payload = new WdcResponse();
            payload.WebResponse = TestUtil.GetDataFile("sample_set_13_06_2019.Looking for adventure - homepage - logged in.html");

            var result = _reader.GetInteractiveStoryShortDescription(payload);

            Assert.AreEqual("Four people looking for adventure, and dealing with past demons", result);
        }

        [TestMethod]
        public async Task WdcReaderInteractiveStoryDescription()
        {
            WdcResponse payload = new WdcResponse();
            payload.WebResponse = TestUtil.GetDataFile("sample_set_13_06_2019.Looking for adventure - homepage - logged in.html");

            var result = _reader.GetInteractiveStoryDescription(payload);
            var expected = TestUtil.GetDataFile("expected_set_13_06_2019.WdcReaderInteractiveStoryDescription-ExpectedResult_13_06_2019.txt");

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public async Task WdcReaderInteractiveStoryTitle()
        {
            WdcResponse payload = new WdcResponse();
            payload.WebResponse = TestUtil.GetDataFile("sample_set_13_06_2019.Looking for adventure - homepage - logged in.html");

            var result = _reader.GetInteractiveStoryTitle(payload);

            Assert.AreEqual("Looking for adventure", result);
        }

        [TestMethod]
        public async Task WdcReaderInteractiveChapterFirstPageLoggedIn()
        {
            // Expected results
            var expectedChapter = new WdcInteractiveChapter();
            expectedChapter.Path = "1";
            expectedChapter.Title = "The Great War";
            expectedChapter.SourceChoiceTitle = string.Empty;
            expectedChapter.Content = TestUtil.GetDataFile("expected_set_13_06_2019.WdcReaderInteractiveChapter_Content.txt");
            expectedChapter.IsEnd = false;
            expectedChapter.Author = new WdcAuthor()
            {
                Name = "The Nameless Hermit",
                Username = "blackdragon",
            };
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "11", Name = "Be Jace" });
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "12", Name = "Be Rhea" });
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "13", Name = "Be Marek" });
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "14", Name = "Be Tara" });

            WdcResponse payload = new WdcResponse();
            payload.WebResponse = TestUtil.GetDataFile("sample_set_13_06_2019.Looking for adventure - chapter 1 - logged in.html");
            payload.Address = "https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/map/1";

            WdcInteractiveChapter testChapter = await _reader.GetInteractiveChaper("TEST", expectedChapter.Path, payload);

            

            // Tests
            //Assert.AreEqual(expectedChapterTitle, chapterResult.Title);
            //Assert.AreEqual(expectedChapterSourceTitle, chapterResult.SourceChoiceTitle);
            //Assert.AreEqual(expectedChapterContent, chapterResult.Content);
            //Assert.AreEqual(expectedIsEnd, chapterResult.IsEnd);
            //Assert.AreEqual(expectedAuthorName, chapterResult.Author.Name);
            //Assert.AreEqual(expectedAuthorUsername, chapterResult.Author.Username);
            //for (var i=0; i < chapterResult.Choices.Count; i++)
            //{
            //    Assert.AreEqual(expectedChoices[i].Name, chapterResult.Choices[i].Name, "Chapter choice name doesn't match");
            //    Assert.AreEqual(expectedChoices[i].PathLink, chapterResult.Choices[i].PathLink, "Chapter choice path doesn't match");
            //}
            CompareInteractiveChapters(expectedChapter, testChapter);
        }

        [TestMethod]
        public async Task WdcReaderInteractiveChapterFirstPageLoggedOut()
        {
            // Expected results
            var expectedChapter = new WdcInteractiveChapter();
            expectedChapter.Path = "1";
            expectedChapter.Title = "The Great War";
            expectedChapter.SourceChoiceTitle = string.Empty;
            expectedChapter.Content = TestUtil.GetDataFile("expected_set_13_06_2019.WdcReaderInteractiveChapter1_Content.txt");
            expectedChapter.IsEnd = false;
            expectedChapter.Author = new WdcAuthor()
            {
                Name = "The Nameless Hermit",
                Username = "blackdragon",
            };
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "11", Name = "Be Jace" });
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "12", Name = "Be Rhea" });
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "13", Name = "Be Marek" });
            expectedChapter.Choices.Add(new WdcInteractiveChapterChoice() { PathLink = "14", Name = "Be Tara" });

            // Set things up
            WdcResponse payload = new WdcResponse();
            payload.WebResponse = TestUtil.GetDataFile("sample_set_13_06_2019.Looking for adventure - chapter 1 - logged out.html");
            payload.Address = "https://www.writing.com/main/interact/item_id/209084-Looking-for-adventure/map/1";

            WdcInteractiveChapter testChapter = await _reader.GetInteractiveChaper("TEST", expectedChapter.Path, payload);

            // Compare
            CompareInteractiveChapters(expectedChapter, testChapter);
        }

        private void CompareInteractiveChapters(WdcInteractiveChapter expectedChapter, WdcInteractiveChapter testChapter)
        {
            Assert.AreEqual(expectedChapter.Path, testChapter.Path);
            Assert.AreEqual(expectedChapter.Content, testChapter.Content);
            Assert.AreEqual(expectedChapter.IsEnd, testChapter.IsEnd);
            Assert.AreEqual(expectedChapter.SourceChoiceTitle, testChapter.SourceChoiceTitle);
            Assert.AreEqual(expectedChapter.Title, testChapter.Title);
            Assert.AreEqual(expectedChapter.Author.Name, testChapter.Author.Name);
            Assert.AreEqual(expectedChapter.Author.Username, testChapter.Author.Username);

            Assert.AreEqual(expectedChapter.Choices.Count, testChapter.Choices.Count, "Chapters do not have the same number of choices.");

            for (var i = 0; i < expectedChapter.Choices.Count; i++)
            {
                Assert.AreEqual(expectedChapter.Choices[i].Name, testChapter.Choices[i].Name, "Chapter choice name doesn't match.");
                Assert.AreEqual(expectedChapter.Choices[i].PathLink, testChapter.Choices[i].PathLink, "Chapter choice path doesn't match.");
            }
        }
    }
}
