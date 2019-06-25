namespace WritingExporter.WinForms.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tsProgBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.dgvStories = new System.Windows.Forms.DataGridView();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbConsoleOutput = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tbStoryInfo = new System.Windows.Forms.TextBox();
            this.tlStoryPanel = new System.Windows.Forms.TableLayoutPanel();
            this.flStoryButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnAddStory = new System.Windows.Forms.Button();
            this.btnRemoveStory = new System.Windows.Forms.Button();
            this.statusStrip1.SuspendLayout();
            this.tlMain.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStories)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.tlStoryPanel.SuspendLayout();
            this.flStoryButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsProgBar,
            this.tsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(659, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsProgBar
            // 
            this.tsProgBar.Name = "tsProgBar";
            this.tsProgBar.Size = new System.Drawing.Size(100, 16);
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(67, 17);
            this.tsStatus.Text = "Status label";
            // 
            // tlMain
            // 
            this.tlMain.ColumnCount = 2;
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlMain.Controls.Add(this.groupBox2, 0, 1);
            this.tlMain.Controls.Add(this.groupBox1, 0, 0);
            this.tlMain.Controls.Add(this.groupBox3, 1, 0);
            this.tlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlMain.Location = new System.Drawing.Point(0, 0);
            this.tlMain.Name = "tlMain";
            this.tlMain.RowCount = 2;
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlMain.Size = new System.Drawing.Size(659, 428);
            this.tlMain.TabIndex = 2;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tlStoryPanel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.tlMain.SetRowSpan(this.groupBox1, 2);
            this.groupBox1.Size = new System.Drawing.Size(323, 422);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stories";
            // 
            // dgvStories
            // 
            this.dgvStories.AllowUserToAddRows = false;
            this.dgvStories.AllowUserToDeleteRows = false;
            this.dgvStories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStories.Location = new System.Drawing.Point(3, 43);
            this.dgvStories.Name = "dgvStories";
            this.dgvStories.ReadOnly = true;
            this.dgvStories.Size = new System.Drawing.Size(311, 357);
            this.dgvStories.TabIndex = 0;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbConsoleOutput);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(332, 217);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(324, 208);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Console output";
            // 
            // tbConsoleOutput
            // 
            this.tbConsoleOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbConsoleOutput.Location = new System.Drawing.Point(3, 16);
            this.tbConsoleOutput.Multiline = true;
            this.tbConsoleOutput.Name = "tbConsoleOutput";
            this.tbConsoleOutput.ReadOnly = true;
            this.tbConsoleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.tbConsoleOutput.Size = new System.Drawing.Size(318, 189);
            this.tbConsoleOutput.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.tbStoryInfo);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(332, 3);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(324, 208);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Story info";
            // 
            // tbStoryInfo
            // 
            this.tbStoryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStoryInfo.Location = new System.Drawing.Point(3, 16);
            this.tbStoryInfo.Multiline = true;
            this.tbStoryInfo.Name = "tbStoryInfo";
            this.tbStoryInfo.ReadOnly = true;
            this.tbStoryInfo.Size = new System.Drawing.Size(318, 189);
            this.tbStoryInfo.TabIndex = 0;
            // 
            // tlStoryPanel
            // 
            this.tlStoryPanel.ColumnCount = 1;
            this.tlStoryPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlStoryPanel.Controls.Add(this.dgvStories, 0, 1);
            this.tlStoryPanel.Controls.Add(this.flStoryButtons, 0, 0);
            this.tlStoryPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlStoryPanel.Location = new System.Drawing.Point(3, 16);
            this.tlStoryPanel.Name = "tlStoryPanel";
            this.tlStoryPanel.RowCount = 2;
            this.tlStoryPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tlStoryPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlStoryPanel.Size = new System.Drawing.Size(317, 403);
            this.tlStoryPanel.TabIndex = 1;
            // 
            // flStoryButtons
            // 
            this.flStoryButtons.Controls.Add(this.btnRemoveStory);
            this.flStoryButtons.Controls.Add(this.btnAddStory);
            this.flStoryButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flStoryButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flStoryButtons.Location = new System.Drawing.Point(3, 3);
            this.flStoryButtons.Name = "flStoryButtons";
            this.flStoryButtons.Size = new System.Drawing.Size(311, 34);
            this.flStoryButtons.TabIndex = 1;
            // 
            // btnAddStory
            // 
            this.btnAddStory.Location = new System.Drawing.Point(242, 3);
            this.btnAddStory.Name = "btnAddStory";
            this.btnAddStory.Size = new System.Drawing.Size(30, 30);
            this.btnAddStory.TabIndex = 2;
            this.btnAddStory.Text = "+";
            this.btnAddStory.UseVisualStyleBackColor = true;
            // 
            // btnRemoveStory
            // 
            this.btnRemoveStory.Location = new System.Drawing.Point(278, 3);
            this.btnRemoveStory.Name = "btnRemoveStory";
            this.btnRemoveStory.Size = new System.Drawing.Size(30, 30);
            this.btnRemoveStory.TabIndex = 3;
            this.btnRemoveStory.Text = "X";
            this.btnRemoveStory.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(659, 450);
            this.Controls.Add(this.tlMain);
            this.Controls.Add(this.statusStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Writing.com Exporter";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tlMain.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStories)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tlStoryPanel.ResumeLayout(false);
            this.flStoryButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar tsProgBar;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.TableLayoutPanel tlMain;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvStories;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbConsoleOutput;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox tbStoryInfo;
        private System.Windows.Forms.TableLayoutPanel tlStoryPanel;
        private System.Windows.Forms.FlowLayoutPanel flStoryButtons;
        private System.Windows.Forms.Button btnAddStory;
        private System.Windows.Forms.Button btnRemoveStory;
    }
}