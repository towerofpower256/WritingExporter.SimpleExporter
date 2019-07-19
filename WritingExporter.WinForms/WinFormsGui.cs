using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WritingExporter.Common.Gui;

namespace WritingExporter.WinForms
{
    public class WinFormsGui : IGuiContext, IDisposable
    {

        public void Start()
        {
            // TODO
        }

        public void Dispose()
        {
            // Do nothing for now
        }

        // TODO: implement desktop alerts
        
        public void ShowMessageBoxDialog(object parent, string title, string message, GuiMessageBoxIcon icon)
        {
            _ShowMessageBox((IWin32Window) parent, title, message, icon);
        }

        public void ShowMessageBox(string title, string message, GuiMessageBoxIcon icon)
        {
            _ShowMessageBox(null, title, message, icon);
        }

        private void _ShowMessageBox(IWin32Window parent, string title, string message, GuiMessageBoxIcon icon)
        {
            var msgBoxIcon = MessageBoxIcon.None;

            switch (icon)
            {
                case GuiMessageBoxIcon.Info:
                    msgBoxIcon = MessageBoxIcon.Information;
                    break;
                case GuiMessageBoxIcon.Alert:
                    msgBoxIcon = MessageBoxIcon.Exclamation;
                    break;
                case GuiMessageBoxIcon.Warn:
                    msgBoxIcon = MessageBoxIcon.Warning;
                    break;
                case GuiMessageBoxIcon.Error:
                    msgBoxIcon = MessageBoxIcon.Error;
                    break;
                case GuiMessageBoxIcon.Question:
                    msgBoxIcon = MessageBoxIcon.Question;
                    break;
            }

            MessageBox.Show(null, message, title, MessageBoxButtons.OK, msgBoxIcon);
        }
    }

    
}
