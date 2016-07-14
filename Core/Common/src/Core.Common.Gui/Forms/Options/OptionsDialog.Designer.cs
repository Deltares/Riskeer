namespace Core.Common.Gui.Forms.Options
{
    partial class OptionsDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OptionsDialog));
            this.groupBoxUserSettings = new System.Windows.Forms.GroupBox();
            this.checkBoxStartPage = new System.Windows.Forms.CheckBox();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.groupBoxUserSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxUserSettings
            // 
            resources.ApplyResources(this.groupBoxUserSettings, "groupBoxUserSettings");
            this.groupBoxUserSettings.Controls.Add(this.checkBoxStartPage);
            this.groupBoxUserSettings.Name = "groupBoxUserSettings";
            this.groupBoxUserSettings.TabStop = false;
            // 
            // checkBoxStartPage
            // 
            resources.ApplyResources(this.checkBoxStartPage, "checkBoxStartPage");
            this.checkBoxStartPage.Checked = true;
            this.checkBoxStartPage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStartPage.Name = "checkBoxStartPage";
            this.checkBoxStartPage.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.ButtonOkClick);
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.buttonCancel);
            this.Controls.Add(this.buttonOk);
            this.Controls.Add(this.groupBoxUserSettings);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Name = "OptionsDialog";
            this.groupBoxUserSettings.ResumeLayout(false);
            this.groupBoxUserSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxUserSettings;
        private System.Windows.Forms.CheckBox checkBoxStartPage;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
    }
}