using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WritingExporter.SimpleExporter
{
    public partial class TextboxDialogForm : Form
    {
        public TextboxDialogForm()
        {
            InitializeComponent();
        }

        public TextboxDialogForm(string content, string title)
        {
            InitializeComponent();

            tbContents.Text = content;
            this.Text = title;
        }

        [Obsolete("STAThread nonsense, complains wherever I put it")]
        private void CopyContentsToClipboard()
        {
            Clipboard.SetText(tbContents.Text); // set contents to clipboard
            SystemSounds.Beep.Play(); // Ding
        }
    }
}
