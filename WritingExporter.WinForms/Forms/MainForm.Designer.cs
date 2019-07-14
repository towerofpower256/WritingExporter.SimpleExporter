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
            this.tsStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.tlMain = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.cbConsoleAutoScroll = new System.Windows.Forms.CheckBox();
            this.txtConsoleOutput = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tlStoryPanel = new System.Windows.Forms.TableLayoutPanel();
            this.dgvStories = new System.Windows.Forms.DataGridView();
            this.flStoryButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.btnRemoveStory = new System.Windows.Forms.Button();
            this.btnAddStory = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.txtStoryInfo = new System.Windows.Forms.TextBox();
            this.menuStripMain = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.miAddStory = new System.Windows.Forms.ToolStripMenuItem();
            this.miAddStoryAdvanced = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.miExit = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1.SuspendLayout();
            this.tlMain.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tlStoryPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvStories)).BeginInit();
            this.flStoryButtons.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuStripMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 428);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(884, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tsStatus
            // 
            this.tsStatus.Name = "tsStatus";
            this.tsStatus.Size = new System.Drawing.Size(67, 17);
            this.tsStatus.Text = "Status label";
            // 
            // tlMain
            // 
            this.tlMain.ColumnCount = 3;
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlMain.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tlMain.Controls.Add(this.groupBox2, 2, 0);
            this.tlMain.Controls.Add(this.groupBox1, 0, 0);
            this.tlMain.Controls.Add(this.groupBox3, 1, 0);
            this.tlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlMain.Location = new System.Drawing.Point(0, 24);
            this.tlMain.Name = "tlMain";
            this.tlMain.RowCount = 2;
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlMain.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlMain.Size = new System.Drawing.Size(884, 404);
            this.tlMain.TabIndex = 2;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tableLayoutPanel1);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(591, 3);
            this.groupBox2.Name = "groupBox2";
            this.tlMain.SetRowSpan(this.groupBox2, 2);
            this.groupBox2.Size = new System.Drawing.Size(290, 398);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Console output";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.cbConsoleAutoScroll, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtConsoleOutput, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel1.Size = new System.Drawing.Size(284, 379);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // cbConsoleAutoScroll
            // 
            this.cbConsoleAutoScroll.AutoSize = true;
            this.cbConsoleAutoScroll.Checked = true;
            this.cbConsoleAutoScroll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbConsoleAutoScroll.Enabled = false;
            this.cbConsoleAutoScroll.Location = new System.Drawing.Point(3, 359);
            this.cbConsoleAutoScroll.Name = "cbConsoleAutoScroll";
            this.cbConsoleAutoScroll.Size = new System.Drawing.Size(75, 17);
            this.cbConsoleAutoScroll.TabIndex = 0;
            this.cbConsoleAutoScroll.Text = "Auto scroll";
            this.cbConsoleAutoScroll.UseVisualStyleBackColor = true;
            // 
            // txtConsoleOutput
            // 
            this.txtConsoleOutput.BackColor = System.Drawing.SystemColors.Window;
            this.txtConsoleOutput.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtConsoleOutput.Location = new System.Drawing.Point(3, 3);
            this.txtConsoleOutput.Multiline = true;
            this.txtConsoleOutput.Name = "txtConsoleOutput";
            this.txtConsoleOutput.ReadOnly = true;
            this.txtConsoleOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtConsoleOutput.Size = new System.Drawing.Size(278, 350);
            this.txtConsoleOutput.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tlStoryPanel);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.tlMain.SetRowSpan(this.groupBox1, 2);
            this.groupBox1.Size = new System.Drawing.Size(288, 398);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Stories";
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
            this.tlStoryPanel.Size = new System.Drawing.Size(282, 379);
            this.tlStoryPanel.TabIndex = 1;
            // 
            // dgvStories
            // 
            this.dgvStories.AllowUserToAddRows = false;
            this.dgvStories.AllowUserToDeleteRows = false;
            this.dgvStories.AllowUserToResizeColumns = false;
            this.dgvStories.AllowUserToResizeRows = false;
            this.dgvStories.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dgvStories.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvStories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dgvStories.Location = new System.Drawing.Point(3, 43);
            this.dgvStories.Name = "dgvStories";
            this.dgvStories.ReadOnly = true;
            this.dgvStories.RowHeadersVisible = false;
            this.dgvStories.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvStories.ShowEditingIcon = false;
            this.dgvStories.Size = new System.Drawing.Size(276, 357);
            this.dgvStories.TabIndex = 0;
            // 
            // flStoryButtons
            // 
            this.flStoryButtons.Controls.Add(this.btnRemoveStory);
            this.flStoryButtons.Controls.Add(this.btnAddStory);
            this.flStoryButtons.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flStoryButtons.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flStoryButtons.Location = new System.Drawing.Point(3, 3);
            this.flStoryButtons.Name = "flStoryButtons";
            this.flStoryButtons.Size = new System.Drawing.Size(276, 34);
            this.flStoryButtons.TabIndex = 1;
            // 
            // btnRemoveStory
            // 
            this.btnRemoveStory.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnRemoveStory.Location = new System.Drawing.Point(243, 3);
            this.btnRemoveStory.Name = "btnRemoveStory";
            this.btnRemoveStory.Size = new System.Drawing.Size(30, 30);
            this.btnRemoveStory.TabIndex = 3;
            this.btnRemoveStory.Text = "X";
            this.btnRemoveStory.UseVisualStyleBackColor = true;
            this.btnRemoveStory.Click += new System.EventHandler(this.btnRemoveStory_Click);
            // 
            // btnAddStory
            // 
            this.btnAddStory.Location = new System.Drawing.Point(207, 3);
            this.btnAddStory.Name = "btnAddStory";
            this.btnAddStory.Size = new System.Drawing.Size(30, 30);
            this.btnAddStory.TabIndex = 2;
            this.btnAddStory.Text = "+";
            this.btnAddStory.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtStoryInfo);
            this.groupBox3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox3.Location = new System.Drawing.Point(297, 3);
            this.groupBox3.Name = "groupBox3";
            this.tlMain.SetRowSpan(this.groupBox3, 2);
            this.groupBox3.Size = new System.Drawing.Size(288, 398);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Story info";
            // 
            // txtStoryInfo
            // 
            this.txtStoryInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtStoryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtStoryInfo.Location = new System.Drawing.Point(3, 16);
            this.txtStoryInfo.Multiline = true;
            this.txtStoryInfo.Name = "txtStoryInfo";
            this.txtStoryInfo.ReadOnly = true;
            this.txtStoryInfo.Size = new System.Drawing.Size(282, 379);
            this.txtStoryInfo.TabIndex = 0;
            // 
            // menuStripMain
            // 
            this.menuStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
            this.menuStripMain.Location = new System.Drawing.Point(0, 0);
            this.menuStripMain.Name = "menuStripMain";
            this.menuStripMain.Size = new System.Drawing.Size(884, 24);
            this.menuStripMain.TabIndex = 3;
            this.menuStripMain.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.miAddStory,
            this.miAddStoryAdvanced,
            this.toolStripSeparator2,
            this.settingsToolStripMenuItem,
            this.toolStripSeparator1,
            this.miExit});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // miAddStory
            // 
            this.miAddStory.Name = "miAddStory";
            this.miAddStory.Size = new System.Drawing.Size(187, 22);
            this.miAddStory.Text = "&Add story";
            this.miAddStory.Click += new System.EventHandler(this.miAddStory_Click);
            // 
            // miAddStoryAdvanced
            // 
            this.miAddStoryAdvanced.Name = "miAddStoryAdvanced";
            this.miAddStoryAdvanced.Size = new System.Drawing.Size(187, 22);
            this.miAddStoryAdvanced.Text = "Add story (ad&vanced)";
            this.miAddStoryAdvanced.Click += new System.EventHandler(this.miAddStoryAdvanced_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(184, 6);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.settingsToolStripMenuItem.Text = "&Settings";
            this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(184, 6);
            // 
            // miExit
            // 
            this.miExit.Name = "miExit";
            this.miExit.Size = new System.Drawing.Size(187, 22);
            this.miExit.Text = "E&xit";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 450);
            this.Controls.Add(this.tlMain);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStripMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStripMain;
            this.Name = "MainForm";
            this.Text = "Writing.com Exporter";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.tlMain.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.tlStoryPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvStories)).EndInit();
            this.flStoryButtons.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.menuStripMain.ResumeLayout(false);
            this.menuStripMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tsStatus;
        private System.Windows.Forms.TableLayoutPanel tlMain;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.DataGridView dgvStories;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtStoryInfo;
        private System.Windows.Forms.TableLayoutPanel tlStoryPanel;
        private System.Windows.Forms.FlowLayoutPanel flStoryButtons;
        private System.Windows.Forms.Button btnAddStory;
        private System.Windows.Forms.Button btnRemoveStory;
        private System.Windows.Forms.MenuStrip menuStripMain;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem miExit;
        private System.Windows.Forms.ToolStripMenuItem miAddStory;
        private System.Windows.Forms.ToolStripMenuItem miAddStoryAdvanced;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtConsoleOutput;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbConsoleAutoScroll;
    }
}