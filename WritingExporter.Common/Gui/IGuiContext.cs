using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WritingExporter.Common.Gui
{
    public interface IGuiContext
    {
        void ShowMessageBoxDialog(object parent, string title, string message, GuiMessageBoxIcon icon);
        void ShowMessageBox(string title, string message, GuiMessageBoxIcon icon);
        void ShellExecute(string command);
        void Start();
    }
}
