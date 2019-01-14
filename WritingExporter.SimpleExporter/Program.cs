using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WritingExporter.SimpleExporter.Models;

namespace WritingExporter.SimpleExporter
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Setup logging
            LogManager.Setup();

            // Debug
            //DoStory2HtmlTest();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void DoStory2HtmlTest()
        {
            // Get the story
            WInteractiveStory story;

            var currentDir = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var inputFilename = currentDir + "\\test-story.bin";
            var outputFilename = currentDir + "\\test-story.html";

            using (System.IO.FileStream fs = new System.IO.FileStream(inputFilename, System.IO.FileMode.Open))
            {
                var bf = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                story = (WInteractiveStory)bf.Deserialize(fs);
            }

            // Try to convert it to HTML
            var converter = new WStoryToHtml();
            var htmlPayload = converter.ConvertStory(story);

            // Save it, because why not
            System.IO.File.WriteAllText(outputFilename, htmlPayload);
        }
    }
}
