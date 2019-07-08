namespace WritingExporter.WinForms.Forms
{
    partial class AddStoryWdcForm
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
            this.txtStoryParm = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtStoryInfo = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnGetStory = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtStoryParm
            // 
            this.txtStoryParm.Location = new System.Drawing.Point(96, 6);
            this.txtStoryParm.Name = "txtStoryParm";
            this.txtStoryParm.Size = new System.Drawing.Size(290, 20);
            this.txtStoryParm.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(78, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Story URL / ID";
            // 
            // txtStoryInfo
            // 
            this.txtStoryInfo.BackColor = System.Drawing.SystemColors.Window;
            this.txtStoryInfo.Location = new System.Drawing.Point(15, 81);
            this.txtStoryInfo.Multiline = true;
            this.txtStoryInfo.Name = "txtStoryInfo";
            this.txtStoryInfo.ReadOnly = true;
            this.txtStoryInfo.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtStoryInfo.Size = new System.Drawing.Size(371, 262);
            this.txtStoryInfo.TabIndex = 2;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(230, 349);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 3;
            this.btnOk.Text = "Add story";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(311, 349);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnGetStory
            // 
            this.btnGetStory.Location = new System.Drawing.Point(15, 35);
            this.btnGetStory.Name = "btnGetStory";
            this.btnGetStory.Size = new System.Drawing.Size(371, 31);
            this.btnGetStory.TabIndex = 5;
            this.btnGetStory.Text = "Get story";
            this.btnGetStory.UseVisualStyleBackColor = true;
            this.btnGetStory.Click += new System.EventHandler(this.btnGetStory_Click);
            // 
            // AddStoryWdcForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(397, 378);
            this.Controls.Add(this.btnGetStory);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtStoryInfo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtStoryParm);
            this.Name = "AddStoryWdcForm";
            this.Text = "AddStoryForm";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.AddStoryWdcForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtStoryParm;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtStoryInfo;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnGetStory;
    }
}