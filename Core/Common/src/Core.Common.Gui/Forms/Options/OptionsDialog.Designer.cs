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
            this.buttonCancel = new System.Windows.Forms.Button();
            this.buttonOk = new System.Windows.Forms.Button();
            this.comboBoxTheme = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxStartPage = new System.Windows.Forms.CheckBox();
            this.groupBoxUserSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxUserSettings
            // 
            resources.ApplyResources(this.groupBoxUserSettings, "groupBoxUserSettings");
            this.groupBoxUserSettings.Controls.Add(this.buttonCancel);
            this.groupBoxUserSettings.Controls.Add(this.buttonOk);
            this.groupBoxUserSettings.Controls.Add(this.comboBoxTheme);
            this.groupBoxUserSettings.Controls.Add(this.label1);
            this.groupBoxUserSettings.Controls.Add(this.checkBoxStartPage);
            this.groupBoxUserSettings.Name = "groupBoxUserSettings";
            this.groupBoxUserSettings.TabStop = false;
            // 
            // buttonCancel
            // 
            resources.ApplyResources(this.buttonCancel, "buttonCancel");
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
            // 
            // buttonOk
            // 
            resources.ApplyResources(this.buttonOk, "buttonOk");
            this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOk.Name = "buttonOk";
            this.buttonOk.UseVisualStyleBackColor = true;
            this.buttonOk.Click += new System.EventHandler(this.buttonOk_Click);
            // 
            // comboBoxTheme
            // 
            this.comboBoxTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTheme.FormattingEnabled = true;
            resources.ApplyResources(this.comboBoxTheme, "comboBoxTheme");
            this.comboBoxTheme.Name = "comboBoxTheme";
            this.comboBoxTheme.SelectedIndexChanged += new System.EventHandler(this.comboBoxTheme_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // checkBoxStartPage
            // 
            resources.ApplyResources(this.checkBoxStartPage, "checkBoxStartPage");
            this.checkBoxStartPage.Checked = true;
            this.checkBoxStartPage.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxStartPage.Name = "checkBoxStartPage";
            this.checkBoxStartPage.UseVisualStyleBackColor = true;
            // 
            // OptionsDialog
            // 
            this.AcceptButton = this.buttonOk;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.Controls.Add(this.groupBoxUserSettings);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "OptionsDialog";
            this.ShowInTaskbar = false;
            this.groupBoxUserSettings.ResumeLayout(false);
            this.groupBoxUserSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxUserSettings;
        private System.Windows.Forms.CheckBox checkBoxStartPage;
        private System.Windows.Forms.ComboBox comboBoxTheme;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.Button buttonOk;
    }
}