using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WritingExporter.Common;
using WritingExporter.Common.Models;
using WritingExporter.Common.Configuration;

namespace WritingExporter.WinForms.Forms
{
    public partial class SettingsForm : Form
    {
        private static ILogger _log = LogManager.GetLogger(typeof(SettingsForm));

        IConfigProvider _configProvider;

        public SettingsForm(IConfigProvider configProvider)
        {
            // Save services
            _configProvider = configProvider;

            InitializeComponent();

            LoadSettingsToForm();
        }

        private void LoadSettingsToForm()
        {
            // Nice and easy, just get the settings and put them into the form.

            // Wdc Client Settings
            var wdcClientConfig = _configProvider.GetSection<WdcClientConfiguration>();
            txtWdcUsername.Text = wdcClientConfig.WritingUsername;
            txtWdcPassword.Text = wdcClientConfig.WritingPassword;
        }

        private void SaveSettingsFromForm()
        {
            // Nice and easy, just get the settings from the form and save them.

            // Wdc client settings
            var wdcClientConfig = _configProvider.GetSection<WdcClientConfiguration>();
            wdcClientConfig.WritingUsername = txtWdcUsername.Text;
            wdcClientConfig.WritingPassword = txtWdcPassword.Text;
            _configProvider.SetSection(wdcClientConfig);

            // Save settings to file
            _configProvider.SaveSettings();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SaveSettingsFromForm();
            this.Close();
        }
    }
}
