using Core.Common.Controls.Swf.Properties;

namespace Core.Common.Controls.Swf.WizardPages
{
    partial class SelectFileWizardPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SelectFileWizardPage));
            this.buttonOpenFile = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.lblDescription = new System.Windows.Forms.Label();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.labelErrorMessage = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // buttonOpenFile
            // 
            resources.ApplyResources(this.buttonOpenFile, "buttonOpenFile");
            this.buttonOpenFile.BackColor = System.Drawing.SystemColors.Control;
            this.buttonOpenFile.Name = "buttonOpenFile";
            this.buttonOpenFile.UseVisualStyleBackColor = false;
            this.buttonOpenFile.Click += new System.EventHandler(this.buttonOpenFile_Click);
            // 
            // textBox1
            // 
            resources.ApplyResources(this.textBox1, "textBox1");
            this.textBox1.Name = "textBox1";
            this.textBox1.ReadOnly = true;
            // 
            // lblDescription
            // 
            resources.ApplyResources(this.lblDescription, "lblDescription");
            this.lblDescription.Name = "lblDescription";
            // 
            // openFileDialog
            // 
            this.openFileDialog.Filter = global::Core.Common.Controls.Swf.Properties.Resources.Shape_files_filter;
            // 
            // labelErrorMessage
            // 
            resources.ApplyResources(this.labelErrorMessage, "labelErrorMessage");
            this.labelErrorMessage.Name = "labelErrorMessage";
            // 
            // SelectFileWizardPage
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Transparent;
            this.Controls.Add(this.buttonOpenFile);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.labelErrorMessage);
            this.Controls.Add(this.lblDescription);
            this.Name = "SelectFileWizardPage";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonOpenFile;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label lblDescription;
        private System.Windows.Forms.OpenFileDialog openFileDialog;
        private System.Windows.Forms.Label labelErrorMessage;
    }
}