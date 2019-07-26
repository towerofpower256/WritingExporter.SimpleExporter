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
using WritingExporter.Common.Export;
using System.Threading;

namespace WritingExporter.WinForms.Forms
{
    public partial class ProgressDialogForm : Form
    {
        private static ILogger _log = LogManager.GetLogger(typeof(ProgressDialogForm));

        public event EventHandler CancelButtonClicked;

        Task _refreshTask; // Is this really nessessary? 
        CancellationTokenSource _ctSource;

        public ProgressDialogForm()
        {
            InitializeComponent();


            // Is this overkill?
            _ctSource = new CancellationTokenSource();
            _refreshTask = new Task(() =>
            {
                var ct = _ctSource.Token;
                while (true)
                {
                    if (ct.IsCancellationRequested) return;
                    Thread.Sleep(500);
                    if (!this.IsDisposed) this.Refresh();
                }
            });
            _refreshTask.Start();
        }

        private void DoCancel()
        {
            _log.Debug("Doing cancel");
            CancelButtonClicked?.Invoke(this, new EventArgs());
            _ctSource.Cancel();
        }

        #region Form update functions

        private delegate void UpdateProgressDelegate(int progressValue, int progressMax);
        public void UpdateProgress(int progressValue, int progressMax)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke(new UpdateProgressDelegate(UpdateProgress), progressValue, progressMax);
                return;
            }

            progressBar.Style = progressMax == 0 ? ProgressBarStyle.Marquee : ProgressBarStyle.Continuous;
            if (progressMax <= progressBar.Value) progressBar.Value = progressMax; // Safety, so that a decrease in max value is never greater than the current progress value
            progressBar.Maximum = progressMax;
            progressBar.Value = progressValue;
        }

        private delegate void UpdateMessageDelegate(string msg);
        public void UpdateMessage(string msg)
        {
            if (lblMessage.InvokeRequired)
            {
                lblMessage.Invoke(new UpdateMessageDelegate(UpdateMessage), msg);
                return;
            }

            lblMessage.Text = msg;
        }

        private delegate void CloseFormDelegate();
        public void CloseForm()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new CloseFormDelegate(CloseForm));
                return;
            }

            this.Close();
        }

        #endregion

        private void btnCancel_Click(object sender, EventArgs e)
        {
            DoCancel();
        }

        private void ProgressDialogForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            _ctSource.Cancel();
        }
    }
}
