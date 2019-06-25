using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WritingExporter.Common.Models;
using WritingExporter.Common;

namespace WritingExporter.WinForms.Forms
{
    public partial class AddStoryForm : Form
    {
        private static ILogger _log = LogManager.GetLogger(typeof(AddStoryForm));

        public AddStoryForm()
        {
            InitializeComponent();
        }
    }
}
