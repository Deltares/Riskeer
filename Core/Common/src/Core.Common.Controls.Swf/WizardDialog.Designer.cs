using System;
using System.Linq;
using System.Windows.Forms;

namespace Core.Common.Controls.Swf
{
    partial class WizardDialog
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private Timer updateButtonsTimer = new Timer();
        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            KeyUp -= OnPageModified;

            if (disposing && (updateButtonsTimer != null))
            {
                updateButtonsTimer.Dispose();
            }

            if (disposing && (components != null))
            {
                foreach (var wizardPage in wizardPages.OfType<IDisposable>())
                {
                    wizardPage.Dispose();
                }
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WizardDialog));
            this.wizardControl1 = new DevExpress.XtraWizard.WizardControl();
            this.welcomeWizardPage1 = new DevExpress.XtraWizard.WelcomeWizardPage();
            this.richTextBoxWelcome = new System.Windows.Forms.RichTextBox();
            this.completionWizardPage1 = new DevExpress.XtraWizard.CompletionWizardPage();
            this.richTextBoxFinished = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).BeginInit();
            this.wizardControl1.SuspendLayout();
            this.welcomeWizardPage1.SuspendLayout();
            this.completionWizardPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // wizardControl1
            // 
            this.wizardControl1.Controls.Add(this.welcomeWizardPage1);
            this.wizardControl1.Controls.Add(this.completionWizardPage1);
            this.wizardControl1.LookAndFeel.UseDefaultLookAndFeel = false;
            this.wizardControl1.LookAndFeel.UseWindowsXPTheme = true;
            this.wizardControl1.Name = "wizardControl1";
            this.wizardControl1.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[] {
            this.welcomeWizardPage1,
            this.completionWizardPage1});
            // 
            // welcomeWizardPage1
            // 
            this.welcomeWizardPage1.Controls.Add(this.richTextBoxWelcome);
            this.welcomeWizardPage1.Name = "welcomeWizardPage1";
            resources.ApplyResources(this.welcomeWizardPage1, "welcomeWizardPage1");
            // 
            // richTextBoxWelcome
            // 
            this.richTextBoxWelcome.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxWelcome.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.richTextBoxWelcome, "richTextBoxWelcome");
            this.richTextBoxWelcome.Name = "richTextBoxWelcome";
            this.richTextBoxWelcome.ReadOnly = true;
            // 
            // completionWizardPage1
            // 
            this.completionWizardPage1.Controls.Add(this.richTextBoxFinished);
            this.completionWizardPage1.Name = "completionWizardPage1";
            resources.ApplyResources(this.completionWizardPage1, "completionWizardPage1");
            // 
            // richTextBoxFinished
            // 
            this.richTextBoxFinished.BackColor = System.Drawing.SystemColors.Window;
            this.richTextBoxFinished.BorderStyle = System.Windows.Forms.BorderStyle.None;
            resources.ApplyResources(this.richTextBoxFinished, "richTextBoxFinished");
            this.richTextBoxFinished.Name = "richTextBoxFinished";
            this.richTextBoxFinished.ReadOnly = true;
            // 
            // WizardDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.wizardControl1);
            this.Name = "WizardDialog";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            ((System.ComponentModel.ISupportInitialize)(this.wizardControl1)).EndInit();
            this.wizardControl1.ResumeLayout(false);
            this.welcomeWizardPage1.ResumeLayout(false);
            this.completionWizardPage1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected DevExpress.XtraWizard.WizardControl wizardControl1;
        private DevExpress.XtraWizard.WelcomeWizardPage welcomeWizardPage1;
        private DevExpress.XtraWizard.CompletionWizardPage completionWizardPage1;
        private System.Windows.Forms.RichTextBox richTextBoxWelcome;
        private System.Windows.Forms.RichTextBox richTextBoxFinished;
    }
}