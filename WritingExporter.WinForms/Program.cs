using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using WritingExporter.WinForms.Forms;
using WritingExporter.Common;

namespace WritingExporter.WinForms
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
            var lf = new Log4NetLogFactory();
            lf.AddConsoleAppender().AddFileAppender().EndConfig();
            LogManager.SetLogFactory(lf);

            // Setup GUI
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Compose application
            var container = SimpleInjector.Container();
            container.Register

            // Run
            Application.Run(new MainForm());
        }
    }
}
