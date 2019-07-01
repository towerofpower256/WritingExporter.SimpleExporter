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
        private static SimpleInjector.Container _diContainer;

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // Run
            Application.Run(new MainForm());
        }
    }
}
