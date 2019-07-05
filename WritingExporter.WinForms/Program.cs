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
            new AppContext().Setup().Start();
        }
    }
}
