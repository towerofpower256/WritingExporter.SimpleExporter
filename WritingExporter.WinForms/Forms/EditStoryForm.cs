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
using WritingExporter.Common.Models;

namespace WritingExporter.WinForms.Forms
{
    public partial class EditStoryForm : Form
    {
        IWdcStoryContainer _storyContainer;

        WdcInteractiveStory _story;
        bool _isNewStory = true;

        public EditStoryForm(IWdcStoryContainer storyContainer)
        {
            _storyContainer = storyContainer;

            _story = new WdcInteractiveStory();

            InitializeComponent();
        }

        public void SetStory(WdcInteractiveStory newStory)
        {
            _story = newStory;
            _isNewStory = false;
            UpdateFormFromStory(newStory);
        }

        private delegate void UpdateFormFromStoryDelegate(WdcInteractiveStory story);
        private void UpdateFormFromStory(WdcInteractiveStory story)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new UpdateFormFromStoryDelegate(UpdateFormFromStory), story);
                return;
            }

            txtStoryName.Text = _story.Name;
            txtStoryID.Text = _story.ID;
            txtStoryShortDescription.Text = _story.ShortDescription;
            txtStoryDescription.Text = _story.Description;
            dtpLastUpdated.Value = _story.LastUpdated;
        }

        private void AddChangesToStory()
        {
            _story.Name = txtStoryName.Text;
            _story.ID = txtStoryID.Text;
            _story.ShortDescription = txtStoryShortDescription.Text;
            _story.Description = txtStoryDescription.Text;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            AddChangesToStory();
            if (_isNewStory)
                _storyContainer.AddStory(_story);
            else
                _storyContainer.UpdateStory(_story);

            this.Close();
        }
    }
}
