namespace DeltaShell.Gui.Forms.OptionsDialog
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.upDownNumberOfDecimals = new System.Windows.Forms.NumericUpDown();
            this.lblDecimalsOrSignificants = new System.Windows.Forms.Label();
            this.radioButtonCompactNotation = new System.Windows.Forms.RadioButton();
            this.radioButtonNumberNotation = new System.Windows.Forms.RadioButton();
            this.radioButtonScientificNotation = new System.Windows.Forms.RadioButton();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.textBoxRealFormatSample = new System.Windows.Forms.TextBox();
            this.checkBoxStartPage = new System.Windows.Forms.CheckBox();
            this.groupBoxUserSettings.SuspendLayout();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNumberOfDecimals)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBoxUserSettings
            // 
            resources.ApplyResources(this.groupBoxUserSettings, "groupBoxUserSettings");
            this.groupBoxUserSettings.Controls.Add(this.comboBoxTheme);
            this.groupBoxUserSettings.Controls.Add(this.label1);
            this.groupBoxUserSettings.Controls.Add(this.groupBox1);
            this.groupBoxUserSettings.Controls.Add(this.checkBoxStartPage);
            this.groupBoxUserSettings.Name = "groupBoxUserSettings";
            this.groupBoxUserSettings.TabStop = false;
            // 
            // comboBoxTheme
            // 
            this.comboBoxTheme.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxTheme.FormattingEnabled = true;
            this.comboBoxTheme.Items.AddRange(new object[] {
            resources.GetString("comboBoxTheme.Items"),
            resources.GetString("comboBoxTheme.Items1"),
            resources.GetString("comboBoxTheme.Items2"),
            resources.GetString("comboBoxTheme.Items3"),
            resources.GetString("comboBoxTheme.Items4"),
            resources.GetString("comboBoxTheme.Items5")});
            resources.ApplyResources(this.comboBoxTheme, "comboBoxTheme");
            this.comboBoxTheme.Name = "comboBoxTheme";
            this.comboBoxTheme.SelectedIndexChanged += new System.EventHandler(this.comboBoxTheme_SelectedIndexChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.upDownNumberOfDecimals);
            this.groupBox1.Controls.Add(this.lblDecimalsOrSignificants);
            this.groupBox1.Controls.Add(this.radioButtonCompactNotation);
            this.groupBox1.Controls.Add(this.radioButtonNumberNotation);
            this.groupBox1.Controls.Add(this.radioButtonScientificNotation);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // upDownNumberOfDecimals
            // 
            resources.ApplyResources(this.upDownNumberOfDecimals, "upDownNumberOfDecimals");
            this.upDownNumberOfDecimals.Name = "upDownNumberOfDecimals";
            this.upDownNumberOfDecimals.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.upDownNumberOfDecimals.ValueChanged += new System.EventHandler(this.UpDownNumberOfDecimalsValueChanged);
            // 
            // lblDecimalsOrSignificants
            // 
            resources.ApplyResources(this.lblDecimalsOrSignificants, "lblDecimalsOrSignificants");
            this.lblDecimalsOrSignificants.Name = "lblDecimalsOrSignificants";
            // 
            // radioButtonCompactNotation
            // 
            resources.ApplyResources(this.radioButtonCompactNotation, "radioButtonCompactNotation");
            this.radioButtonCompactNotation.Name = "radioButtonCompactNotation";
            this.radioButtonCompactNotation.UseVisualStyleBackColor = true;
            this.radioButtonCompactNotation.CheckedChanged += new System.EventHandler(this.RadioButtonCompactNotationCheckedChanged);
            // 
            // radioButtonNumberNotation
            // 
            resources.ApplyResources(this.radioButtonNumberNotation, "radioButtonNumberNotation");
            this.radioButtonNumberNotation.Name = "radioButtonNumberNotation";
            this.radioButtonNumberNotation.UseVisualStyleBackColor = true;
            this.radioButtonNumberNotation.CheckedChanged += new System.EventHandler(this.RadioButtonNumberNotationCheckedChanged);
            // 
            // radioButtonScientificNotation
            // 
            resources.ApplyResources(this.radioButtonScientificNotation, "radioButtonScientificNotation");
            this.radioButtonScientificNotation.Name = "radioButtonScientificNotation";
            this.radioButtonScientificNotation.UseVisualStyleBackColor = true;
            this.radioButtonScientificNotation.CheckedChanged += new System.EventHandler(this.RadioButtonScientificNotationCheckedChanged);
            // 
            // groupBox2
            // 
            resources.ApplyResources(this.groupBox2, "groupBox2");
            this.groupBox2.Controls.Add(this.textBoxRealFormatSample);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.TabStop = false;
            // 
            // textBoxRealFormatSample
            // 
            this.textBoxRealFormatSample.BackColor = System.Drawing.SystemColors.Window;
            this.textBoxRealFormatSample.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.textBoxRealFormatSample, "textBoxRealFormatSample");
            this.textBoxRealFormatSample.Name = "textBoxRealFormatSample";
            this.textBoxRealFormatSample.ReadOnly = true;
            this.textBoxRealFormatSample.TabStop = false;
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.upDownNumberOfDecimals)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBoxUserSettings;
        private System.Windows.Forms.CheckBox checkBoxStartPage;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton radioButtonNumberNotation;
        private System.Windows.Forms.RadioButton radioButtonScientificNotation;
        private System.Windows.Forms.RadioButton radioButtonCompactNotation;
        private System.Windows.Forms.NumericUpDown upDownNumberOfDecimals;
        private System.Windows.Forms.Label lblDecimalsOrSignificants;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox textBoxRealFormatSample;
        private System.Windows.Forms.ComboBox comboBoxTheme;
        private System.Windows.Forms.Label label1;
    }
}