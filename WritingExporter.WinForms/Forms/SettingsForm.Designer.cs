namespace WritingExporter.WinForms.Forms
{
    partial class SettingsForm
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
            this.groupWdcSettings = new System.Windows.Forms.GroupBox();
            this.tlpWdcSettings = new System.Windows.Forms.TableLayoutPanel();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtWdcUsername = new System.Windows.Forms.TextBox();
            this.txtWdcPassword = new System.Windows.Forms.TextBox();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupWdcSettings.SuspendLayout();
            this.tlpWdcSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupWdcSettings
            // 
            this.groupWdcSettings.Controls.Add(this.tlpWdcSettings);
            this.groupWdcSettings.Location = new System.Drawing.Point(13, 13);
            this.groupWdcSettings.Name = "groupWdcSettings";
            this.groupWdcSettings.Size = new System.Drawing.Size(252, 100);
            this.groupWdcSettings.TabIndex = 0;
            this.groupWdcSettings.TabStop = false;
            this.groupWdcSettings.Text = "Writing.com";
            // 
            // tlpWdcSettings
            // 
            this.tlpWdcSettings.ColumnCount = 2;
            this.tlpWdcSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpWdcSettings.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tlpWdcSettings.Controls.Add(this.label1, 0, 0);
            this.tlpWdcSettings.Controls.Add(this.label2, 0, 1);
            this.tlpWdcSettings.Controls.Add(this.txtWdcUsername, 1, 0);
            this.tlpWdcSettings.Controls.Add(this.txtWdcPassword, 1, 1);
            this.tlpWdcSettings.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tlpWdcSettings.Location = new System.Drawing.Point(3, 16);
            this.tlpWdcSettings.Name = "tlpWdcSettings";
            this.tlpWdcSettings.RowCount = 3;
            this.tlpWdcSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpWdcSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tlpWdcSettings.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tlpWdcSettings.Size = new System.Drawing.Size(246, 81);
            this.tlpWdcSettings.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Username";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 38);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "Password";
            // 
            // txtWdcUsername
            // 
            this.txtWdcUsername.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWdcUsername.Location = new System.Drawing.Point(64, 5);
            this.txtWdcUsername.Name = "txtWdcUsername";
            this.txtWdcUsername.Size = new System.Drawing.Size(179, 20);
            this.txtWdcUsername.TabIndex = 2;
            // 
            // txtWdcPassword
            // 
            this.txtWdcPassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.txtWdcPassword.Location = new System.Drawing.Point(64, 35);
            this.txtWdcPassword.Name = "txtWdcPassword";
            this.txtWdcPassword.PasswordChar = '*';
            this.txtWdcPassword.Size = new System.Drawing.Size(179, 20);
            this.txtWdcPassword.TabIndex = 3;
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(632, 415);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 1;
            this.btnOk.Text = "Save";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(713, 415);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // SettingsForm
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.groupWdcSettings);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.groupWdcSettings.ResumeLayout(false);
            this.tlpWdcSettings.ResumeLayout(false);
            this.tlpWdcSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupWdcSettings;
        private System.Windows.Forms.TableLayoutPanel tlpWdcSettings;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtWdcUsername;
        private System.Windows.Forms.TextBox txtWdcPassword;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}