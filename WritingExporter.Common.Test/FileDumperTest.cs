using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using WritingExporter.Common.Storage;
using System.Collections.Generic;

namespace WritingExporter.Common.Test
{
    [TestClass]
    public class FileDumperTest
    {
        [TestMethod]
        public void SimpleDumpTest()
        {
            var testdata = GenerateRandomString();
            IFileDumper dumper = new FileDumper();
            var filename = dumper.DumpFile("testfile", testdata);

            // Validate
            Assert.AreEqual(testdata, File.ReadAllText(filename));

            // Cleanup
            File.Delete(filename);
        }

        [TestMethod]
        public void DumpWithInvalidName()
        {
            var testdata = GenerateRandomString();
            IFileDumper dumper = new FileDumper();
            string invalidCharString = new string(Path.GetInvalidFileNameChars());
            var filename = dumper.DumpFile($"testfile {invalidCharString}", testdata);

            // Validate
            Assert.AreEqual(testdata, File.ReadAllText(filename));

            // Cleanup
            File.Delete(filename);
        }

        [TestMethod]
        public void MultiDump()
        {
            var testdata = GenerateRandomString();
            IFileDumper dumper = new FileDumper();
            var filenames = new List<string>();

            for (var i=0; i < 20; i++)
                filenames.Add(dumper.DumpFile($"testfile", testdata));

            // Validate
            foreach (var fn in filenames)
                Assert.AreEqual(testdata, File.ReadAllText(fn));

            // Cleanup
            foreach (var fn in filenames)
                File.Delete(fn);
        }

        private string GenerateRandomString()
        {
            const string CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            const int LENGTH = 1000;
            Random random = new Random();

            return new string(Enumerable.Repeat(CHARS, LENGTH)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
