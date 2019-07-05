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
using WritingExporter.Common.StorySyncWorker;

namespace WritingExporter.WinForms.Forms
{
    public partial class MainForm : Form
    {
        private const string COL_NAME_STATUS = "Status";

        private static ILogger _log = LogManager.GetLogger(typeof(AddStoryWdcForm));

        IWdcStoryContainer _storyContainer;
        IWdcStorySyncWorker _syncWorker;
        SimpleInjector.Container _diContainer; // TODO remove dependancy on SimpleInjector to get other forms

        object _consoleOutputLock = new object();

        public MainForm(IWdcStoryContainer storyContainer, IWdcStorySyncWorker syncWorker, SimpleInjector.Container diContainer)
        {
            _storyContainer = storyContainer;
            _diContainer = diContainer;
            _syncWorker = syncWorker;

            InitializeComponent();

            // Set a few things up
            InitStoryList();
            RefreshStoryList();

            // Subscribe to some events
            _storyContainer.OnUpdate += new EventHandler<WdcStoryContainerEventArgs>(OnStoryContainerUpdate);
            LogManager.OnLogEvent += new EventHandler<LogEventArgs>(OnLogEvent);
            _syncWorker.OnWorkerStatusChange += new EventHandler<WdcStorySyncWorkerStatusEventArgs>(OnSyncWorkerEvent);
        }

        #region Event stuff

        private void OnStoryContainerUpdate(object sender, WdcStoryContainerEventArgs args)
        {
            if (args.EventType == WdcStoryContainerEventType.Add || args.EventType == WdcStoryContainerEventType.Remove)
                RefreshStoryList();
        }

        private void OnLogEvent(object sender, LogEventArgs args)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{args.Timestamp.ToShortTimeString()}-{args.Level}-{args.Source}:");
            sb.AppendLine($"{args.Message}");

            if (args.Exception != null)
            {
                sb.AppendLine($"{args.Exception.GetType().ToString()}");
                sb.AppendLine($"{args.Exception.Message}");
            }

            sb.AppendLine();

            AppendToConsoleOutput(sb.ToString());
        }

        private void OnSyncWorkerEvent(object sender, WdcStorySyncWorkerStatusEventArgs args)
        {
            
        }

        #endregion

        #region Form update functions
        private delegate void AppendToConsoleOutputDelegate(string msg);
        private void AppendToConsoleOutput(string msg)
        {
            if (txtConsoleOutput.InvokeRequired)
            {
                txtConsoleOutput.Invoke(new AppendToConsoleOutputDelegate(AppendToConsoleOutput), new object[] { msg });
                return;
            }
            
            txtConsoleOutput.AppendText(msg + '\n');
            txtConsoleOutput.ScrollToCaret(); // Scroll to the bottom every time we update it
            
        }

        private delegate void InitStoryListDelegate();
        private void InitStoryList()
        {
            if (dgvStories.InvokeRequired)
            {
                this.Invoke(new InitStoryListDelegate(InitStoryList));
                return;
            }

            _log.Debug("Story list init");

            // Clear
            dgvStories.Columns.Clear();
            dgvStories.Rows.Clear();

            // Status column
            var statusIconColumn = new DataGridViewTextBoxColumn();
            statusIconColumn.Name = COL_NAME_STATUS;
            statusIconColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.ColumnHeader;
            statusIconColumn.DefaultCellStyle.NullValue = null;
            dgvStories.Columns.Add(statusIconColumn);

            // Name column
            var nameColumn = new DataGridViewTextBoxColumn();
            nameColumn.Name = "Name";
            nameColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgvStories.Columns.Add(nameColumn);
        }

        private delegate void RefreshStoryListDelegate();
        private void RefreshStoryList()
        {
            if (dgvStories.InvokeRequired)
            {
                this.Invoke(new RefreshStoryListDelegate(RefreshStoryList));
                return;
            }

            _log.Debug("Story list refresh");

            // Clear
            dgvStories.Rows.Clear();

            // Fill
            foreach (var story in _storyContainer.GetAllStories())
            {
                var newRow = new DataGridViewRow();
                newRow.Tag = story.ID;
                newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = null });
                newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = story.Name, ToolTipText = story.Name });

                dgvStories.Rows.Add(newRow);
            }
        }

        private void UpdateStoryListFromSyncWorker()
        {
            UpdateStoryListFromSyncWorker(_syncWorker.GetCurrentStatus());
        }

        private void UpdateStoryListFromSyncWorker(WdcStorySyncWorkerStatus syncStatus)
        {
            foreach (DataGridViewRow row in dgvStories.Rows)
            {
                var storyID = row.Tag.ToString();

                // Try to find the story in the sync worker
                if (syncStatus.StoryStatus.ContainsKey(storyID))
                {
                    var storyStatus = syncStatus.StoryStatus[storyID];
                    var cell = row.Cells[COL_NAME_STATUS];

                    var newValue = "";
                    var newTooltip = "";

                    // Update the status
                    switch (storyStatus.State)
                    {
                        case WdcStorySyncWorkerStatus.StoryStatusEntryState.Error:
                            newValue = "Error";
                            newTooltip = storyStatus.ErrorMessage;
                            break;
                        case WdcStorySyncWorkerStatus.StoryStatusEntryState.WaitingItu:
                            newValue = "Waiting";
                            newTooltip = $"Interactive temporarily unavailable. Last seen at {storyStatus.LastItu}";
                            break;
                        case WdcStorySyncWorkerStatus.StoryStatusEntryState.Working:
                            newValue = "Working";
                            break;
                    }

                    cell.Value = newValue;
                    cell.ToolTipText = newTooltip;
                }
            }
        }

        private delegate void UpdateStoryStatusDelegate(string storyID, StoryStatus status, string statusMessage);
        private void UpdateStoryStatus(string storyID, StoryStatus status, string statusMessage)
        {
            if (dgvStories.InvokeRequired)
            {
                this.Invoke(new UpdateStoryStatusDelegate(UpdateStoryStatus), storyID, status, statusMessage);
            }

            _log.Debug($"Updating story '{storyID}' with status '{status}'");

            if (string.IsNullOrEmpty(statusMessage)) statusMessage = status.ToString(); // Default the message to the name of the status enum

            foreach (DataGridViewRow row in dgvStories.Rows)
            {
                // Find the story row
                if (row.Tag.ToString() == storyID)
                {
                    var cell = (DataGridViewTextBoxCell)row.Cells[COL_NAME_STATUS];
                    cell.Value = status.ToString();
                    cell.ToolTipText = statusMessage;

                    return; // Go no further
                }
            }

            // Made it out here, didn't find the story
            _log.Debug($"Couldn't find a row for that story {storyID}. Something is wrong.");
        }

        #endregion

        private void ShowAddStoryDialog()
        {
            var newStoryForm = _diContainer.GetInstance<AddStoryWdcForm>();
            newStoryForm.ShowDialog(this);
        }

        private void ShowAddManualStoryDialog()
        {
            var newStoryForm = _diContainer.GetInstance<EditStoryForm>();
            newStoryForm.ShowDialog(this);
        }

        private void ShowEditStoryForm(WdcInteractiveStory story)
        {
            var editStoryForm = _diContainer.GetInstance<EditStoryForm>();
            editStoryForm.SetStory(story);
            editStoryForm.ShowDialog(this);
        }

        private void miAddStory_Click(object sender, EventArgs e)
        {
            ShowAddStoryDialog();
        }

        private void miAddStoryAdvanced_Click(object sender, EventArgs e)
        {
            ShowAddManualStoryDialog();
        }

        private enum StoryStatus
        {
            None,
            Working,
            Waiting,
            Error
        }

        /*
        private static void SetDvgRowStatusIcon(DataGridViewRow row, StateIcon icon)
        {
            Icon value = null;
            switch (icon) {
                case StateIcon.Error:
                    value = SystemIcons.Error;
                    break;
                case StateIcon.Working:
                    value = SystemIcons.
                    break;
            }

            SystemIcons.

            
        }
        */
    }


    
}
