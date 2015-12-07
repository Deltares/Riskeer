namespace Core.Common.Gui.Forms.OptionsDialog
{
    partial class GeneralOptionsControl
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralOptionsControl));
            this.groupBoxUserSettings = new System.Windows.Forms.GroupBox();
            this.comboBoxTheme = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.checkBoxStartPage = new System.Windows.Forms.CheckBox();
            this.groupBoxUserSettings.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxUserSettings
            // 
            resources.ApplyResources(this.groupBoxUserSettings, "groupBoxUserSettings");
            this.groupBoxUserSettings.Controls.Add(this.comboBoxTheme);
            this.groupBoxUserSettings.Controls.Add(this.label1);
            this.groupBoxUserSettings.Controls.Add(this.checkBoxStartPage);
            this.groupBoxUserSettings.Name = "groupBoxUserSettings";
            this.groupBoxUserSettings.TabStop = false;
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
            // GeneralOptionsControl
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.groupBoxUserSettings);
            this.Name = "GeneralOptionsControl";
            this.groupBoxUserSettings.ResumeLayout(false);
            this.groupBoxUserSettings.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxUserSettings;
        private System.Windows.Forms.CheckBox checkBoxStartPage;
        private System.Windows.Forms.ComboBox comboBoxTheme;
        private System.Windows.Forms.Label label1;
    }
}