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
    public partial class AddStoryWdcForm : Form
    {
        private static ILogger _log = LogManager.GetLogger(typeof(AddStoryWdcForm));

        public AddStoryWdcForm()
        {
            InitializeComponent();
        }
    }
}
