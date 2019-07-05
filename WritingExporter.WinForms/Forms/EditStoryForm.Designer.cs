namespace WritingExporter.WinForms.Forms
{
    partial class EditStoryForm
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
            this.lblWarning = new System.Windows.Forms.Label();
            this.txtStoryName = new System.Windows.Forms.TextBox();
            this.txtStoryID = new System.Windows.Forms.TextBox();
            this.txtStoryDescription = new System.Windows.Forms.TextBox();
            this.txtStoryShortDescription = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpLastUpdated = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblWarning
            // 
            this.lblWarning.AutoSize = true;
            this.lblWarning.Location = new System.Drawing.Point(12, 9);
            this.lblWarning.Name = "lblWarning";
            this.lblWarning.Size = new System.Drawing.Size(341, 26);
            this.lblWarning.TabIndex = 0;
            this.lblWarning.Text = "Warning: you should only be using this if you know what you are doing.\r\nIf you br" +
    "eak something, it sucks to be you.";
            // 
            // txtStoryName
            // 
            this.txtStoryName.Location = new System.Drawing.Point(106, 51);
            this.txtStoryName.Name = "txtStoryName";
            this.txtStoryName.Size = new System.Drawing.Size(214, 20);
            this.txtStoryName.TabIndex = 1;
            this.txtStoryName.Text = "Story name";
            // 
            // txtStoryID
            // 
            this.txtStoryID.Location = new System.Drawing.Point(106, 77);
            this.txtStoryID.Name = "txtStoryID";
            this.txtStoryID.Size = new System.Drawing.Size(214, 20);
            this.txtStoryID.TabIndex = 2;
            this.txtStoryID.Text = "Story ID";
            // 
            // txtStoryDescription
            // 
            this.txtStoryDescription.Location = new System.Drawing.Point(15, 196);
            this.txtStoryDescription.Multiline = true;
            this.txtStoryDescription.Name = "txtStoryDescription";
            this.txtStoryDescription.Size = new System.Drawing.Size(305, 168);
            this.txtStoryDescription.TabIndex = 3;
            this.txtStoryDescription.Text = "Story description";
            // 
            // txtStoryShortDescription
            // 
            this.txtStoryShortDescription.Location = new System.Drawing.Point(106, 103);
            this.txtStoryShortDescription.Name = "txtStoryShortDescription";
            this.txtStoryShortDescription.Size = new System.Drawing.Size(214, 20);
            this.txtStoryShortDescription.TabIndex = 4;
            this.txtStoryShortDescription.Text = "Story short description";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(65, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Name";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(82, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(18, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "ID";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 180);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Description";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 106);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Short description";
            // 
            // dtpLastUpdated
            // 
            this.dtpLastUpdated.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpLastUpdated.Enabled = false;
            this.dtpLastUpdated.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpLastUpdated.Location = new System.Drawing.Point(106, 129);
            this.dtpLastUpdated.Name = "dtpLastUpdated";
            this.dtpLastUpdated.Size = new System.Drawing.Size(214, 20);
            this.dtpLastUpdated.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(31, 129);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Last updated";
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(275, 492);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 11;
            this.btnSave.Text = "Save";
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // EditStoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 527);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.dtpLastUpdated);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStoryShortDescription);
            this.Controls.Add(this.txtStoryDescription);
            this.Controls.Add(this.txtStoryID);
            this.Controls.Add(this.txtStoryName);
            this.Controls.Add(this.lblWarning);
            this.Name = "EditStoryForm";
            this.Text = "EditStoryForm";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblWarning;
        private System.Windows.Forms.TextBox txtStoryName;
        private System.Windows.Forms.TextBox txtStoryID;
        private System.Windows.Forms.TextBox txtStoryDescription;
        private System.Windows.Forms.TextBox txtStoryShortDescription;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpLastUpdated;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnSave;
    }
}