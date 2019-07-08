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
using WritingExporter.Common.Wdc;
using System.Threading;

namespace WritingExporter.WinForms.Forms
{
    public partial class AddStoryWdcForm : Form
    {
        private static ILogger _log = LogManager.GetLogger(typeof(AddStoryWdcForm));

        AddStoryFormState _formState;
        WdcInteractiveStory _story;

        IWdcStoryContainer _wdcStoryContainer;
        IWdcClient _wdcClient;
        IWdcReader _wdcReader;
        CancellationTokenSource _ctSourceWdcClient; 

        public AddStoryWdcForm(IWdcClient client, IWdcReader reader, IWdcStoryContainer storyContainer)
        {
            _wdcClient = client;
            _wdcReader = reader;
            _wdcStoryContainer = storyContainer;
            _ctSourceWdcClient = new CancellationTokenSource();

            InitializeComponent();

            // Set the form
            SetFormState(AddStoryFormState.ReadToGetStory);
        }

        private void Cancel()
        {
            _ctSourceWdcClient.Cancel();
            _ctSourceWdcClient = new CancellationTokenSource();
        }

        private void SetFormState(AddStoryFormState newState)
        {
            _log.Debug($"Setting form state: {newState}.");

            switch (newState)
            {
                case AddStoryFormState.ReadToGetStory:
                    EnableSaveButton(false);
                    EnableGetStoryButton(true);
                    EnableStoryParmInput(true);
                    break;
                case AddStoryFormState.GettingStory:
                    _story = null; // Forget the current story
                    SetStoryInfo(string.Empty); // Clear the story info
                    EnableStoryParmInput(false);
                    EnableGetStoryButton(false);
                    EnableSaveButton(false);
                    break;
                case AddStoryFormState.ReadyToSave:
                    EnableStoryParmInput(true);
                    EnableGetStoryButton(true);
                    EnableSaveButton(true);
                    break;
            }

            _formState = newState;
        }

        private delegate void EnableGetStoryButtonDelegate(bool enabled);
        private void EnableGetStoryButton(bool enabled)
        {
            if (btnGetStory.InvokeRequired)
            {
                Invoke(new EnableSaveButtonDelegate(EnableGetStoryButton), enabled);
                return;
            }

            btnGetStory.Enabled = enabled;
        }

        private delegate void EnableSaveButtonDelegate(bool enabled);
        private void EnableSaveButton(bool enabled)
        {
            if (btnOk.InvokeRequired)
            {
                Invoke(new EnableSaveButtonDelegate(EnableSaveButton), enabled);
                return;
            }

            btnOk.Enabled = enabled;
        }

        private void SetStoryInfo(WdcInteractiveStory story)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Title: {story.Name}");
            sb.AppendLine();
            sb.AppendLine("Short description:");
            sb.AppendLine(story.ShortDescription);
            sb.AppendLine();
            sb.AppendLine(story.Description);

            SetStoryInfo(sb.ToString());
        }

        private delegate void SetStoryInfoDelegate(string storyInfo);
        private void SetStoryInfo(string storyInfo)
        {
            if (txtStoryInfo.InvokeRequired)
            {
                Invoke(new SetStoryInfoDelegate(SetStoryInfo), storyInfo);
                return;
            }

            txtStoryInfo.Text = storyInfo;
        }

        private delegate void EnableStoryParmInputDelegate(bool enabled);
        private void EnableStoryParmInput(bool enabled)
        {
            if (txtStoryParm.InvokeRequired)
            {
                Invoke(new EnableStoryParmInputDelegate(EnableStoryParmInput), enabled);
                return;
            }

            txtStoryParm.ReadOnly = !enabled;
        }

        private async void GetStory()
        {
            SetFormState(AddStoryFormState.GettingStory);

            try
            {
                var storyParm = txtStoryParm.Text;

                if (string.IsNullOrEmpty(storyParm)) throw new ArgumentNullException("storyParm");

                // If it's a URL, just get the parameter.
                Uri uriOutput;
                if (Uri.TryCreate(storyParm, UriKind.Absolute, out uriOutput))
                {
                    storyParm = uriOutput.Segments.Last(); // Just get the last part, should be the story ID
                }

                _log.Debug($"Getting story info: {storyParm}");

                var storyResult = await _wdcReader.GetInteractiveStory(storyParm, _wdcClient, _ctSourceWdcClient.Token);

                // Made it here, request was successfull
                _story = storyResult;
                SetStoryInfo(storyResult);
            }
            catch (OperationCanceledException ex)
            {
                // Do nothing
            }
            catch (Exception ex)
            {
                // TODO error handling
                _log.Warn("Error while trying to get story", ex);

                ShowError("Error trying to get story", "An error occurred while trying to get the story info.", ex);
            }
            finally
            {
                SetFormState(_story == null ? AddStoryFormState.ReadToGetStory : AddStoryFormState.ReadyToSave);
            }
            
        }

        private void ShowError(string caption, string message)
        {
            ShowError(caption, message, null);
        }

        private void ShowError(string caption, string message, Exception exception)
        {
            var sb = new StringBuilder();
            sb.AppendLine(message);
            sb.AppendLine();
            sb.AppendLine(exception.GetType().Name);
            sb.AppendLine(exception.Message);

            MessageBox.Show(
                sb.ToString(),
                caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning
                );
        }

        private enum AddStoryFormState
        {
            ReadToGetStory,
            GettingStory,
            ReadyToSave
        }

        private void AddStoryWdcForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            Cancel(); // Cancel any Wdc requests
        }

        private void btnGetStory_Click(object sender, EventArgs e)
        {
            GetStory();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            try
            {
                _wdcStoryContainer.AddStory(_story);
            }
            catch (ArgumentException ex)
            {
                _log.Warn("", ex);

                ShowError("Error adding story", "An error occurred while trying to add the story. Have you already added this story?", ex);
                return;
            }
            catch (Exception ex)
            {
                _log.Warn("", ex);

                ShowError("Error adding story", "An error occurred while trying to add the story.", ex);
                return;
            }
            
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
