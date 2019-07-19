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
using WritingExporter.Common.Wdc;
using WritingExporter.Common.Configuration;
using WritingExporter.Common.Models;
using WritingExporter.Common.StorySync;
using WritingExporter.Common.Storage;

namespace WritingExporter.WinForms.Forms
{
    public partial class MainForm : Form
    {
        private const string COL_NAME_STATUS = "Status";
        private const string COL_NAME_PROGRESS = "Progess";
        private const string DEFAULT_PROGRESS_VALUE = "-";

        private static ILogger _log = LogManager.GetLogger(typeof(AddStoryWdcForm));

        IWdcStoryContainer _storyContainer;
        IStorySyncWorker _syncWorker;
        IConfigProvider _configProvider;
        SimpleInjector.Container _diContainer; // TODO remove dependancy on SimpleInjector to get other forms
        IStoryFileStore _fileStore;

        public MainForm(IConfigProvider configProvider,
            IWdcStoryContainer storyContainer,
            IStorySyncWorker syncWorker,
            SimpleInjector.Container diContainer,
            IStoryFileStore fileStore
            )
        {
            _configProvider = configProvider;
            _storyContainer = storyContainer;
            _diContainer = diContainer;
            _syncWorker = syncWorker;
            _fileStore = fileStore;

            InitializeComponent();

            // Set a few things up
            InitStoryList();
            RefreshStoryList();
            UpdateStatusMessage(_syncWorker.GetCurrentStatus().Message);

            // Subscribe to some events
            LogManager.OnLogEvent += new EventHandler<LogEventArgs>(OnLogEvent);
            _storyContainer.OnUpdate += new EventHandler<WdcStoryContainerEventArgs>(OnStoryContainerUpdate);
            _syncWorker.OnWorkerStatusChange += new EventHandler<StorySyncWorkerStatusEventArgs>(OnSyncWorkerStatusEvent);
            _syncWorker.OnStoryStatusChange += new EventHandler<StorySyncWorkerStoryStatusEventArgs>(OnSyncWorkerStoryStatusEvent);

            CheckInitialSetupRequired();
        }

        #region Event handing stuff

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

        private void OnSyncWorkerStatusEvent(object sender, StorySyncWorkerStatusEventArgs args)
        {
            UpdateStatusMessage(args.NewStatus.Message); // Update the bottom status message with the sync worker's last message
        }

        private void OnSyncWorkerStoryStatusEvent(object sender, StorySyncWorkerStoryStatusEventArgs args)
        {
            UpdateStoryStatus(args.NewStatus.StoryID, args.NewStatus);

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

            if (txtConsoleOutput.IsDisposed) return;
            
            txtConsoleOutput.AppendText(msg + '\n');

            // TODO: cannot disable autoscroll. Need to find a way to prevent auto scroll
            if (cbConsoleAutoScroll.Checked) txtConsoleOutput.ScrollToCaret(); // Scroll to the bottom every time we update it
            
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

            // Progress column
            var progressColumn = new DataGridViewTextBoxColumn();
            progressColumn.Name = COL_NAME_PROGRESS;
            progressColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            dgvStories.Columns.Add(progressColumn);

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
                _AddStoryRow(story);
            }
        }

        

        private delegate void UpdateStoryStatusDelegate(string storyID, StorySyncWorkerStoryStatus status);
        private void UpdateStoryStatus(string storyID, StorySyncWorkerStoryStatus status)
        {
            if (dgvStories.InvokeRequired)
            {
                this.Invoke(new UpdateStoryStatusDelegate(UpdateStoryStatus), storyID, status);
            }

            _log.Debug($"Updating story status '{storyID}'");

            foreach (DataGridViewRow row in dgvStories.Rows)
            {
                // Find the story row
                if (row.Tag.ToString() == storyID)
                {
                    UpdateStoryListRowStatus(row, status);
                    UpdateStoryListRowProgress(row, status);

                    return; // Go no further
                }
            }

            // Made it out here, didn't find the story
            _log.Debug($"Couldn't find a row for that story {storyID}. Something is wrong.");
        }

        private delegate void UpdateStatusMessageDelegate(string msg);
        private void UpdateStatusMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateStatusMessageDelegate(UpdateStatusMessage), msg);
                return;
            }

            tsStatus.Text = msg;
        }

        #endregion

        #region General internal functions

        /// <summary>
        /// Show the "Add story" form
        /// </summary>
        private void ShowAddStoryDialog()
        {
            var newStoryForm = _diContainer.GetInstance<AddStoryWdcForm>();
            newStoryForm.ShowDialog(this);
        }

        /// <summary>
        /// Show the "Edit story" form
        /// </summary>
        private void ShowAddManualStoryDialog()
        {
            var newStoryForm = _diContainer.GetInstance<EditStoryForm>();
            newStoryForm.ShowDialog(this);
        }

        /// <summary>
        /// Show the Edit story form.
        /// </summary>
        /// <param name="story"></param>
        private void ShowEditStoryForm(WdcInteractiveStory story)
        {
            var editStoryForm = _diContainer.GetInstance<EditStoryForm>();
            editStoryForm.SetStory(story);
            editStoryForm.ShowDialog(this);
        }

        /// <summary>
        /// Show the settings form.
        /// </summary>
        private void ShowSettingsForm()
        {
            var newForm = _diContainer.GetInstance<SettingsForm>();
            newForm.ShowDialog(this);
        }

        /// <summary>
        /// Show dialog to export a story to a user-defined location.
        /// </summary>
        /// <param name="story"></param>
        private void ShowExportDialog(WdcInteractiveStory story)
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = $"Story file|{_fileStore.GetDefaultFileSuffix()}";
            sfd.FileName = _fileStore.GenerateFilename(story);
            sfd.DefaultExt = _fileStore.GetDefaultFileSuffix();
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog(this) == DialogResult.OK)
            {
                _fileStore.SaveStory(story, sfd.FileName);
            }
        }

        private void CheckInitialSetupRequired()
        {
            if (!_diContainer.IsVerifying) // Only do this if the container is not doing its verification process
            {
                var wdcClientSettings = _configProvider.GetSection<WdcClientConfiguration>();

                if (string.IsNullOrEmpty(wdcClientSettings.WritingUsername) || string.IsNullOrEmpty(wdcClientSettings.WritingPassword))
                {
                    // Empty username or password, should show alert.
                    MessageBox.Show(
                        "It looks like WritingExporter has not been set up yet.\nPlease provide a Writing.com username and password.",
                        "Setup required",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information
                        );
                    ShowSettingsForm();
                }
            }
        }

        private void RemoveSelectedStories()
        {
            // Get the currently selected stories
            foreach (DataGridViewRow row in dgvStories.SelectedRows)
            {
                _storyContainer.RemoveStory(row.Tag.ToString());
            }
        }

        private void RemoveStory(string storyID)
        {
            _storyContainer.RemoveStory(storyID);
        }

        // Update a single list row with the sync status from the sync worker
        private void UpdateStoryListRow(DataGridViewRow row, StorySyncWorkerStoryStatus status)
        {
            // Update the status indicator
            UpdateStoryListRowStatus(row, status);
            UpdateStoryListRowProgress(row, status);
        }

        // WARNING: Is not thread safe.
        private void UpdateStoryListRowProgress(DataGridViewRow row, StorySyncWorkerStoryStatus status)
        {
            var newProgValue = DEFAULT_PROGRESS_VALUE;
            if (status.ProgressMax > 0)
            {
                newProgValue = $"{status.ProgressValue} / {status.ProgressMax}";
            }

            var progCell = row.Cells[COL_NAME_PROGRESS];
            progCell.Value = newProgValue;
        }

        // WARNING: is not thread safe.
        private void UpdateStoryListRowStatus(DataGridViewRow row, StorySyncWorkerStoryStatus status)
        {
            // Update the status
            var statusCell = row.Cells[COL_NAME_STATUS];

            var newValue = "";
            var newTooltip = "";

            // Update the status
            switch (status.State)
            {
                case StorySyncWorkerStoryState.Error:
                    newValue = "Error";
                    newTooltip = status.ErrorMessage;
                    break;
                case StorySyncWorkerStoryState.WaitingItu:
                    newValue = "Waiting";
                    newTooltip = $"Interactive temporarily unavailable. Last seen at {status.StateLastSet}";
                    break;
                case StorySyncWorkerStoryState.Working:
                    newValue = "Syncing";
                    break;
                case StorySyncWorkerStoryState.Paused:
                    newValue = "Paused";
                    break;
            }

            statusCell.Value = newValue;
            statusCell.ToolTipText = newTooltip;
        }

        // WARNING: is not thread safe
        private void _AddStoryRow(WdcInteractiveStory story)
        {
            var newRow = new DataGridViewRow();
            newRow.Tag = story.ID;
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = null });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = null });
            newRow.Cells.Add(new DataGridViewTextBoxCell() { Value = story.Name, ToolTipText = story.Name });

            dgvStories.Rows.Add(newRow);

            // Update row from sync status
            UpdateStoryListRow(newRow, _syncWorker.GetStoryStatus(story.ID));
        }

        #endregion

        #region Form events

        private void miAddStory_Click(object sender, EventArgs e)
        {
            ShowAddStoryDialog();
        }

        private void miAddStoryAdvanced_Click(object sender, EventArgs e)
        {
            ShowAddManualStoryDialog();
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ShowSettingsForm();
        }

        private void btnRemoveStory_Click(object sender, EventArgs e)
        {
            RemoveSelectedStories();
        }

        #endregion

        private void miExit_Click(object sender, EventArgs e)
        {
            // TODO: Does this need to be sent through the AppCOntext? Should it have an Exit function?
            this.Close();
        }
    }
}
