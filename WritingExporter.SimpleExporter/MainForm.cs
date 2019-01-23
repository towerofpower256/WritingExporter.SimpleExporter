using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;
using System.Diagnostics;
using WritingExporter.SimpleExporter.Models;
using WritingExporter.SimpleExporter.Exceptions;

namespace WritingExporter.SimpleExporter
{
    public partial class MainForm : Form
    {
        private static ILogger log = LogManager.GetLogger(typeof(MainForm));
        private static string STORY_FILE_DIALOG_SUFFIX = "json";
        private static string STORY_FILE_DIALOG_FILTER = "Story JSON (*.json)|*.json";
        private static string STORY_EXPORT_DIALOG_SUFFIX = "html";
        private static string STORY_EXPORT_DIALOG_FILTER = "Story export (*.html)|*.html";

        private string LastFileDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        private WInteractiveStory _story;
        private Task ExportTask;
        private WStoryExporter StoryExporter;

        public MainForm()
        {
            InitializeComponent();

            ResetForm(); // Reset the form

            // Some hooks
            // Log hook
            LogManager.OnAppend += new EventHandler<AppendEventArgs>((sender, e) =>
            {
                var sb = new StringBuilder();
                sb.AppendLine($"{e.Level} - {e.Message}");
                if (e.Exception != null)
                {
                    sb.AppendLine($"{e.Exception.GetType().ToString()}");
                    sb.AppendLine($"{e.Exception.Message}");
                }

                UpdateOutputConsole(sb.ToString());
            });

            log.Info("Main window ready");
        }

        private void ResetForm()
        {
            log.Debug("Resetting form");

            // Cancel an export, if one is running
            StoryExporter?.Cancel();

            tbStoryUrl.ReadOnly = false;
            EnableCancelButton(false);
            EnableSaveButton(false);
            EnableFetchStoryButton(false);
            EnableOpenStoryButton(true);

            UpdateStatusProgress(0);
            UpdateStatusMessage("Open a story to get started");

            tbStoryInfo.Clear();

            _story = null;
        }

        private void OnExportUpdate(object sender, WStoryExporterUpdateEventArgs e)
        {
            //log.Debug("Processing exporter update");

            if (e.MessageUpdated)
                UpdateStatusMessage(e.Message);

            if (e.ProgressUpdated)
                UpdateStatusProgress(e.ProgressValue, e.ProgressMax);
        }

        private void _OpenStory(WInteractiveStory story)
        {
            _story = story;

            // Update the Story info textbox
            // Update the Story Info textbox with the story's info
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Name: {story.Name}");
            sb.AppendLine();
            if (!string.IsNullOrWhiteSpace(story.ShortDescription))
            {
                sb.AppendLine(story.ShortDescription);
                sb.AppendLine();
            }
            sb.AppendLine("Description:");
            sb.AppendLine(story.Description);

            UpdateStoryInfo(sb.ToString());

            // Change the form
            tbStoryUrl.Text = story.Url;
            tbStoryUrl.ReadOnly = true;
            EnableFetchStoryButton(true);

            log.InfoFormat("Story opened: {0}", story.UrlID);
        }

        private async void OpenStoryFromSource()
        {
            try
            {
                UpdateStatusMessage("Getting basic story information");
                log.InfoFormat("Opening story from Writing.com");

                WritingClient wc = GetWritingClient();

                var story = await wc.GetInteractive(new Uri(tbStoryUrl.Text));
                story.HasChanged = true;

                // Got the story, lets ready the form
                UpdateStatusMessage("Got the basic story information, press \'Update story from Writing.com\' to get the rest");

                _OpenStory(story);
            }
            catch (Exception ex)
            {
                log.Error("An error occured when trying to get the story info.", ex);
            }
        }

        private void OpenStoryFromFile()
        {
            try
            {
                WInteractiveStory story;

                var ofd = new OpenFileDialog();
                ofd.Filter = STORY_FILE_DIALOG_FILTER;
                ofd.InitialDirectory = LastFileDir;
                ofd.ValidateNames = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    log.Info("Opening story from file");
                    
                    story = StoryFileHelperJson.DeserializeInteractiveStory(File.ReadAllText(ofd.FileName));
                    LastFileDir = Path.GetDirectoryName(ofd.FileName);

                    _OpenStory(story);

                    UpdateStatusMessage("Story opened from file");
                }
            }
            catch (Exception ex)
            {
                log.Error("An error occured when trying open the story from a file.", ex);
            }
        }

        private void GetStory()
        {
            EnableSaveButton(false);
            EnableOpenStoryButton(false);
            EnableCancelButton(true);
            EnableFetchStoryButton(false);

            var newTask = new Task(async () =>
            {
                log.Info("Updating story content from Writing.com");

                try
                {
                    var wc = GetWritingClient();
                    var storyExporter = new WStoryExporter(wc);
                    StoryExporter = storyExporter;
                    storyExporter.OnStatusUpdate += OnExportUpdate;

                    await storyExporter.ExportStory(_story, new WStoryExporterExportSettings());

                    EnableSaveButton(true); // Only enable the SAVE button if no exception was thrown

                    log.Info("Story update complete");
                }
                catch (Exception ex)
                {
                    log.Error("An error occurred while trying to export the story.", ex);
                    UpdateStatusMessage("ERROR");
                    UpdateStatusProgress(0, 1);
                }
                finally
                {
                    EnableOpenStoryButton(true);
                    EnableFetchStoryButton(true);
                    EnableCancelButton(false);
                    StoryExporter = null;

                    Beep(); // Should we always beep, or just when it completes successfully?
                } 
            });

            newTask.Start();
            ExportTask = newTask;
        }

        private WritingClient GetWritingClient()
        {
            string username = tbWritingUsername.Text;
            string password = tbWritingPassword.Text;
            Uri writingRoot = new Uri("https://www.writing.com");

            if (string.IsNullOrEmpty(username))
                throw new ArgumentNullException("username", "Writing username cannot be left empty");

            if (string.IsNullOrEmpty(password))
                throw new ArgumentNullException("password", "Writing password cannot be left empty");

            WritingClient wc = new WritingClient(new WritingClientSettings()
            {
                WritingCredentials = new System.Net.NetworkCredential(tbWritingUsername.Text, tbWritingPassword.Text),
                WritingUrlRoot = writingRoot
            });

            return wc;
        }

        // debug function, just save the story as a binary file.
        // I'm going to use this file to test the Story2Html converter
        [Obsolete]
        public void SaveStoryFileBinary()
        {
            var sfd = new SaveFileDialog();
            sfd.Filter = "Binary (*.bin)|*.bin";
            sfd.InitialDirectory = LastFileDir;
            sfd.FileName = $"{_story.UrlID}.bin";
            sfd.ValidateNames = true;
            sfd.OverwritePrompt = true;
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                using (var fs = new FileStream(sfd.FileName, FileMode.Create))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Serialize(fs, _story);
                    UpdateStatusMessage("Story saved to file");

                    LastFileDir = Path.GetDirectoryName(sfd.FileName);
                }
            }
        }

        public void SaveStoryFile()
        {
            try
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = STORY_FILE_DIALOG_FILTER;
                sfd.InitialDirectory = LastFileDir;
                sfd.FileName = $"{_story.UrlID}.{STORY_FILE_DIALOG_SUFFIX}";
                sfd.ValidateNames = true;
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StoryFileHelperJson.SaveInteractiveStory(sfd.FileName, _story);
                    _story.HasChanged = false;
                    LastFileDir = Path.GetDirectoryName(sfd.FileName);
                    UpdateStatusMessage("Story saved to file");
                    log.Info("Story saved to file");
                }
            }
            catch (Exception ex)
            {
                UpdateStatusMessage("Error saving story to file");
                log.Error("Error saving story to file", ex);
            }
            
        }

        public void ExportStory(WInteractiveStory story)
        {
            UpdateStatusMessage("Exporting story to HTML");

            try
            {
                var sfd = new SaveFileDialog();
                sfd.Filter = STORY_EXPORT_DIALOG_FILTER;
                sfd.InitialDirectory = LastFileDir;
                sfd.FileName = $"{story.UrlID}.{STORY_EXPORT_DIALOG_SUFFIX}";
                sfd.ValidateNames = true;
                sfd.OverwritePrompt = true;
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    var htmlPayload = new WStoryToHtml().ConvertStory(story);
                    File.WriteAllText(sfd.FileName, htmlPayload);

                    if (MessageBox.Show(
                        "The story has been exported.\n\nWould you like to see it?",
                        "Story exported",
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Information
                        ) == DialogResult.Yes)
                    {
                        string args = $"/select,\"{sfd.FileName}\"";
                        Process.Start("explorer", args);
                    }

                    LastFileDir = Path.GetDirectoryName(sfd.FileName);
                    UpdateStatusMessage("Story exported to HTML");
                    log.Info("Story exported to HTML");
                }

                
            }
            catch (Exception ex)
            {
                log.Error("Error while exporting story to HTML", ex);
                UpdateStatusMessage("Failed to export story to HTML");
            }
            
        }

        // Open a story file, then export it
        // Instead of having to re-download the whole story again
        [Obsolete]
        public void ExportStoryFromFile()
        {
            UpdateStatusMessage("Exporting story from file to HTML");
            
            WInteractiveStory story;

            try
            {
                var ofd = new OpenFileDialog();
                ofd.Filter = STORY_FILE_DIALOG_FILTER;
                ofd.InitialDirectory = LastFileDir;
                ofd.ValidateNames = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    using (var fs = new FileStream(ofd.FileName, FileMode.Open))
                    {
                        BinaryFormatter bf = new BinaryFormatter();
                        story = (WInteractiveStory)bf.Deserialize(fs);
                    }

                    ExportStory(_story);

                    LastFileDir = Path.GetDirectoryName(ofd.FileName);
                }
            }
            catch (Exception ex)
            {
                log.Error("Error while exporting story to HTML from file", ex);
                UpdateStatusMessage("Failed to export story to HTML");
            }
        }

        private void Beep()
        {
            System.Media.SystemSounds.Beep.Play();
        }

        // Form update functions

        public delegate void UpdateOutputConsoleDelegate(string msg);
        public void UpdateOutputConsole(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateOutputConsoleDelegate(UpdateOutputConsole), new object[] { msg });
                return;
            }

            tbOutputConsole.AppendText($"> {msg}\n");
            tbOutputConsole.ScrollToCaret(); // Scroll to the bottom every time we update it
        }

        public delegate void UpdateStoryInfoDelegate(string info);
        public void UpdateStoryInfo(string info)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateStoryInfoDelegate(UpdateStoryInfo), new object[] { info });
                return;
            }
                

            tbStoryInfo.Text = info;
        }

        public delegate void EnableOpenStoryButtonDelegate(bool enable);
        public void EnableOpenStoryButton(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EnableOpenStoryButtonDelegate(EnableOpenStoryButton), new object[] { enable });
                return;
            }

            btnOpenStoryFile.Enabled = enable;
            btnOpenStoryFromSource.Enabled = enable;
        }

        public delegate void EnableCancelButtonDelegate(bool enable);
        public void EnableCancelButton(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EnableCancelButtonDelegate(EnableCancelButton), new object[] { enable });
                return;
            }

            btnCancelExport.Enabled = enable;
        }

        public delegate void EnableSaveButtonDelegate(bool enable);
        public void EnableSaveButton(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EnableCancelButtonDelegate(EnableSaveButton), new object[] { enable });
                return;
            }

            btnSaveStory.Enabled = enable;
            btnExport.Enabled = enable;
        }

        public delegate void EnableFetchStoryButtonDelegate(bool enable);
        public void EnableFetchStoryButton(bool enable)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new EnableCancelButtonDelegate(EnableFetchStoryButton), new object[] { enable });
                return;
            }

            btnFetchStory.Enabled = enable;
        }

        public delegate void UpdateStatusProgressDelegate(double value, double max);
        public void UpdateStatusProgress(double value, double max = 100)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateStatusProgressDelegate(UpdateStatusProgress), new object[] { value, max });
                return;
            }

            //Update the progress bar on the form
            int valueNormalized;
            if (value == 0) valueNormalized = 0;
            else valueNormalized = (int)Math.Floor((value / max) * 100); 
            progExportProgress.Value = valueNormalized;

            // Update the taskbar icon progress bar (if supported)
            // TODO
        }

        public delegate void UpdateStatusMessageDelegate(string msg);
        public void UpdateStatusMessage(string msg)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateStatusMessageDelegate(UpdateStatusMessage), new object[] { msg });
                return;
            }

            lblExportStatusMessage.Text = msg;
            //log.Info(msg);
        }

        // Events
        private void btnReset_Click(object sender, EventArgs e)
        {
            ResetForm();
        }

        private void btnFetchStory_Click(object sender, EventArgs e)
        {
            
        }

        private void btnOpenStory_Click(object sender, EventArgs e)
        {
            OpenStoryFromSource();
        }

        private void btnFetchStory_Click_1(object sender, EventArgs e)
        {
            GetStory();
        }

        private void btnSaveStory_Click(object sender, EventArgs e)
        {
            SaveStoryFile();
        }

        private void btnCancelExport_Click(object sender, EventArgs e)
        {
            StoryExporter?.Cancel();
        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            ExportStory(_story);
        }

        private void btnExportFile_Click(object sender, EventArgs e)
        {
            ExportStoryFromFile();
        }

        private void btnOpenStoryFile_Click(object sender, EventArgs e)
        {
            OpenStoryFromFile();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (_story != null && _story.HasChanged)
            {
                if (MessageBox.Show(
                    "There are unsaved changes. Are you sure that you would like to exit without saving the changes?",
                    "Unsaved changes",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    e.Cancel = true; // Prevent the form from closing
                }
            }
        }
    }
}
