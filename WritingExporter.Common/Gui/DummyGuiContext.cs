using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Gui
{
    public class DummyGuiContext : IGuiContext
    {
        private ILogger _log = LogManager.GetLogger(typeof(DummyGuiContext));

        public void ShellExecute(string command)
        {
            _log.Info("ShellExecute");
        }

        public void ShowMessageBox(string title, string message, GuiMessageBoxIcon icon)
        {
            _log.Info("ShowMessageBox");
        }

        public void ShowMessageBoxDialog(object parent, string title, string message, GuiMessageBoxIcon icon)
        {
            _log.Info("ShowMessageBoxDialog");
        }

        public void Start()
        {
            _log.Info("Start");
        }
    }
}
