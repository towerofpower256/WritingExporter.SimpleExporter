using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WritingExporter.Common.Storage
{
    public class FileDumper : IFileDumper
    {
        private const string OUTPUT_DIR = "Logs";
        private const string FILE_EXT = "txt";
        private const int MAX_DUPLICATE_FILE_ATTEMPTS = 100; // If a duplicate file exists, this is how many times it will try to create a unique filename
        private const char ILLEGAL_CHAR_REPLACEMENT = '_'; // Replace any illegal filename characters with this character

        public string DumpFile(string name, string content)
        {
            Directory.CreateDirectory(OUTPUT_DIR); // Create the output dir if it doesn't already exist

            var filename = GenerateFilename(name);

            string filepath = string.Empty;
            for (var i=0; i < MAX_DUPLICATE_FILE_ATTEMPTS; i++)
            {
                string fileNumber = i == 0 ? string.Empty : $"_{i}";
                filepath = Path.Combine(OUTPUT_DIR, $"{filename}{fileNumber}.{FILE_EXT}");
                if (File.Exists(filepath))
                {
                    continue; // duplicate file exists, try again
                }
                else
                {
                    File.WriteAllText(filepath, content);
                    break; // Don't loop again
                }
            }

            return filepath;
        }

        private string GenerateFilename(string name)
        {
            var now = DateTime.Now;
            var timestamp = now.ToString("yyyy-MM-dd_HHmmss");
            var filename = $"{timestamp}_{name}";

            // Escape any illegal characters
            foreach (var c in Path.GetInvalidFileNameChars())
            {
                filename = filename.Replace(c, ILLEGAL_CHAR_REPLACEMENT);
            }

            return filename;
        }
    }
}
