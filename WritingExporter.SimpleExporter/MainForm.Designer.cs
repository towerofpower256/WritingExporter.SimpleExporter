namespace WritingExporter.SimpleExporter
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
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.btnExport = new System.Windows.Forms.Button();
            this.btnSaveStory = new System.Windows.Forms.Button();
            this.btnReset = new System.Windows.Forms.Button();
            this.btnCancelExport = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFetchStory = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tbWritingUsername = new System.Windows.Forms.TextBox();
            this.tbWritingPassword = new System.Windows.Forms.TextBox();
            this.tbStoryUrl = new System.Windows.Forms.TextBox();
            this.tbStoryInfo = new System.Windows.Forms.TextBox();
            this.btnOpenStoryFromSource = new System.Windows.Forms.Button();
            this.btnOpenStoryFile = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tbOutputConsole = new System.Windows.Forms.TextBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.progExportProgress = new System.Windows.Forms.ToolStripProgressBar();
            this.lblExportStatusMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.flowLayoutPanel1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 2;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.groupBox2, 1, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(0, 0, 0, 20);
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(818, 584);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.tableLayoutPanel2);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(403, 558);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Settings";
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.Controls.Add(this.flowLayoutPanel1, 0, 7);
            this.tableLayoutPanel2.Controls.Add(this.label1, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.btnFetchStory, 0, 5);
            this.tableLayoutPanel2.Controls.Add(this.label2, 0, 1);
            this.tableLayoutPanel2.Controls.Add(this.label3, 0, 2);
            this.tableLayoutPanel2.Controls.Add(this.tbWritingUsername, 1, 0);
            this.tableLayoutPanel2.Controls.Add(this.tbWritingPassword, 1, 1);
            this.tableLayoutPanel2.Controls.Add(this.tbStoryUrl, 1, 2);
            this.tableLayoutPanel2.Controls.Add(this.tbStoryInfo, 0, 6);
            this.tableLayoutPanel2.Controls.Add(this.btnOpenStoryFromSource, 0, 3);
            this.tableLayoutPanel2.Controls.Add(this.btnOpenStoryFile, 0, 4);
            this.tableLayoutPanel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel2.Location = new System.Drawing.Point(3, 16);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 8;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(397, 539);
            this.tableLayoutPanel2.TabIndex = 0;
            // 
            // flowLayoutPanel1
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.flowLayoutPanel1, 2);
            this.flowLayoutPanel1.Controls.Add(this.btnExport);
            this.flowLayoutPanel1.Controls.Add(this.btnSaveStory);
            this.flowLayoutPanel1.Controls.Add(this.btnReset);
            this.flowLayoutPanel1.Controls.Add(this.btnCancelExport);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowLayoutPanel1.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(3, 507);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(391, 29);
            this.flowLayoutPanel1.TabIndex = 9;
            // 
            // btnExport
            // 
            this.btnExport.Location = new System.Drawing.Point(313, 3);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(75, 23);
            this.btnExport.TabIndex = 4;
            this.btnExport.Text = "Export story";
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // btnSaveStory
            // 
            this.btnSaveStory.Location = new System.Drawing.Point(232, 3);
            this.btnSaveStory.Name = "btnSaveStory";
            this.btnSaveStory.Size = new System.Drawing.Size(75, 23);
            this.btnSaveStory.TabIndex = 2;
            this.btnSaveStory.Text = "Save story";
            this.btnSaveStory.UseVisualStyleBackColor = true;
            this.btnSaveStory.Click += new System.EventHandler(this.btnSaveStory_Click);
            // 
            // btnReset
            // 
            this.btnReset.Location = new System.Drawing.Point(151, 3);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(75, 23);
            this.btnReset.TabIndex = 1;
            this.btnReset.Text = "Reset";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // btnCancelExport
            // 
            this.btnCancelExport.Location = new System.Drawing.Point(70, 3);
            this.btnCancelExport.Name = "btnCancelExport";
            this.btnCancelExport.Size = new System.Drawing.Size(75, 23);
            this.btnCancelExport.TabIndex = 3;
            this.btnCancelExport.Text = "Abort";
            this.btnCancelExport.UseVisualStyleBackColor = true;
            this.btnCancelExport.Click += new System.EventHandler(this.btnCancelExport_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(94, 26);
            this.label1.TabIndex = 0;
            this.label1.Text = "Writing username";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // btnFetchStory
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.btnFetchStory, 2);
            this.btnFetchStory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnFetchStory.Location = new System.Drawing.Point(3, 161);
            this.btnFetchStory.Name = "btnFetchStory";
            this.btnFetchStory.Size = new System.Drawing.Size(391, 34);
            this.btnFetchStory.TabIndex = 0;
            this.btnFetchStory.Text = "Update story from Writing.com";
            this.btnFetchStory.UseVisualStyleBackColor = true;
            this.btnFetchStory.Click += new System.EventHandler(this.btnFetchStory_Click_1);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label2.Location = new System.Drawing.Point(3, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 26);
            this.label2.TabIndex = 1;
            this.label2.Text = "Writing password";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.label3.Location = new System.Drawing.Point(3, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(94, 26);
            this.label3.TabIndex = 2;
            this.label3.Text = "Story URL";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // tbWritingUsername
            // 
            this.tbWritingUsername.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbWritingUsername.Location = new System.Drawing.Point(103, 3);
            this.tbWritingUsername.Name = "tbWritingUsername";
            this.tbWritingUsername.Size = new System.Drawing.Size(291, 20);
            this.tbWritingUsername.TabIndex = 3;
            // 
            // tbWritingPassword
            // 
            this.tbWritingPassword.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbWritingPassword.Location = new System.Drawing.Point(103, 29);
            this.tbWritingPassword.Name = "tbWritingPassword";
            this.tbWritingPassword.Size = new System.Drawing.Size(291, 20);
            this.tbWritingPassword.TabIndex = 4;
            this.tbWritingPassword.UseSystemPasswordChar = true;
            // 
            // tbStoryUrl
            // 
            this.tbStoryUrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStoryUrl.Location = new System.Drawing.Point(103, 55);
            this.tbStoryUrl.Name = "tbStoryUrl";
            this.tbStoryUrl.Size = new System.Drawing.Size(291, 20);
            this.tbStoryUrl.TabIndex = 5;
            // 
            // tbStoryInfo
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.tbStoryInfo, 2);
            this.tbStoryInfo.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStoryInfo.Location = new System.Drawing.Point(3, 201);
            this.tbStoryInfo.Multiline = true;
            this.tbStoryInfo.Name = "tbStoryInfo";
            this.tbStoryInfo.ReadOnly = true;
            this.tbStoryInfo.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbStoryInfo.Size = new System.Drawing.Size(391, 300);
            this.tbStoryInfo.TabIndex = 8;
            // 
            // btnOpenStoryFromSource
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.btnOpenStoryFromSource, 2);
            this.btnOpenStoryFromSource.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOpenStoryFromSource.Location = new System.Drawing.Point(3, 81);
            this.btnOpenStoryFromSource.Name = "btnOpenStoryFromSource";
            this.btnOpenStoryFromSource.Size = new System.Drawing.Size(391, 34);
            this.btnOpenStoryFromSource.TabIndex = 10;
            this.btnOpenStoryFromSource.Text = "Open story from Writing.com";
            this.btnOpenStoryFromSource.UseVisualStyleBackColor = true;
            this.btnOpenStoryFromSource.Click += new System.EventHandler(this.btnOpenStory_Click);
            // 
            // btnOpenStoryFile
            // 
            this.tableLayoutPanel2.SetColumnSpan(this.btnOpenStoryFile, 2);
            this.btnOpenStoryFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.btnOpenStoryFile.Location = new System.Drawing.Point(3, 121);
            this.btnOpenStoryFile.Name = "btnOpenStoryFile";
            this.btnOpenStoryFile.Size = new System.Drawing.Size(391, 34);
            this.btnOpenStoryFile.TabIndex = 11;
            this.btnOpenStoryFile.Text = "Open story from file";
            this.btnOpenStoryFile.UseVisualStyleBackColor = true;
            this.btnOpenStoryFile.Click += new System.EventHandler(this.btnOpenStoryFile_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.tbOutputConsole);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(412, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(403, 558);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Console";
            // 
            // tbOutputConsole
            // 
            this.tbOutputConsole.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbOutputConsole.Location = new System.Drawing.Point(3, 16);
            this.tbOutputConsole.Multiline = true;
            this.tbOutputConsole.Name = "tbOutputConsole";
            this.tbOutputConsole.ReadOnly = true;
            this.tbOutputConsole.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.tbOutputConsole.Size = new System.Drawing.Size(397, 539);
            this.tbOutputConsole.TabIndex = 0;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.progExportProgress,
            this.lblExportStatusMessage});
            this.statusStrip1.Location = new System.Drawing.Point(0, 562);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(818, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // progExportProgress
            // 
            this.progExportProgress.Name = "progExportProgress";
            this.progExportProgress.Size = new System.Drawing.Size(100, 16);
            this.progExportProgress.Value = 50;
            // 
            // lblExportStatusMessage
            // 
            this.lblExportStatusMessage.Name = "lblExportStatusMessage";
            this.lblExportStatusMessage.Size = new System.Drawing.Size(670, 17);
            this.lblExportStatusMessage.Spring = true;
            this.lblExportStatusMessage.Text = "Status message";
            this.lblExportStatusMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(818, 584);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "Writing.com Simple Exporter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.tableLayoutPanel2.PerformLayout();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tbWritingUsername;
        private System.Windows.Forms.TextBox tbWritingPassword;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox tbStoryUrl;
        private System.Windows.Forms.TextBox tbOutputConsole;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripProgressBar progExportProgress;
        private System.Windows.Forms.ToolStripStatusLabel lblExportStatusMessage;
        private System.Windows.Forms.TextBox tbStoryInfo;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button btnCancelExport;
        private System.Windows.Forms.Button btnSaveStory;
        private System.Windows.Forms.Button btnFetchStory;
        private System.Windows.Forms.Button btnReset;
        private System.Windows.Forms.Button btnOpenStoryFromSource;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Button btnOpenStoryFile;
    }
}