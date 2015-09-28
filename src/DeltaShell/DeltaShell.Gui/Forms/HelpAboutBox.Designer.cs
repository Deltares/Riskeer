namespace DeltaShell.Gui.Forms
{
    partial class HelpAboutBox
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HelpAboutBox));
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.logoPictureBox = new System.Windows.Forms.PictureBox();
            this.okButton = new System.Windows.Forms.Button();
            this.textBoxDescription = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.labelSupportPhone = new System.Windows.Forms.Label();
            this.labelSupportEmail = new System.Windows.Forms.Label();
            this.labelCopyright = new System.Windows.Forms.Label();
            this.labelVersion = new System.Windows.Forms.Label();
            this.labelProductName = new System.Windows.Forms.Label();
            this.labelTel1 = new System.Windows.Forms.Label();
            this.labelEmail1 = new System.Windows.Forms.Label();
            this.labelCopyright1 = new System.Windows.Forms.Label();
            this.labelVersion1 = new System.Windows.Forms.Label();
            this.labelProductName1 = new System.Windows.Forms.Label();
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            resources.ApplyResources(this.tableLayoutPanel, "tableLayoutPanel");
            this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.okButton, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.textBoxDescription, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.panel1, 1, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            // 
            // logoPictureBox
            // 
            resources.ApplyResources(this.logoPictureBox, "logoPictureBox");
            this.logoPictureBox.Name = "logoPictureBox";
            this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 3);
            this.logoPictureBox.TabStop = false;
            // 
            // okButton
            // 
            resources.ApplyResources(this.okButton, "okButton");
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.okButton.Name = "okButton";
            // 
            // textBoxDescription
            // 
            resources.ApplyResources(this.textBoxDescription, "textBoxDescription");
            this.textBoxDescription.Name = "textBoxDescription";
            this.textBoxDescription.ReadOnly = true;
            this.textBoxDescription.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.textBoxDescription_LinkClicked);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelSupportPhone);
            this.panel1.Controls.Add(this.labelSupportEmail);
            this.panel1.Controls.Add(this.labelCopyright);
            this.panel1.Controls.Add(this.labelVersion);
            this.panel1.Controls.Add(this.labelProductName);
            this.panel1.Controls.Add(this.labelTel1);
            this.panel1.Controls.Add(this.labelEmail1);
            this.panel1.Controls.Add(this.labelCopyright1);
            this.panel1.Controls.Add(this.labelVersion1);
            this.panel1.Controls.Add(this.labelProductName1);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // labelSupportPhone
            // 
            resources.ApplyResources(this.labelSupportPhone, "labelSupportPhone");
            this.labelSupportPhone.Name = "labelSupportPhone";
            // 
            // labelSupportEmail
            // 
            resources.ApplyResources(this.labelSupportEmail, "labelSupportEmail");
            this.labelSupportEmail.Name = "labelSupportEmail";
            // 
            // labelCopyright
            // 
            resources.ApplyResources(this.labelCopyright, "labelCopyright");
            this.labelCopyright.Name = "labelCopyright";
            // 
            // labelVersion
            // 
            resources.ApplyResources(this.labelVersion, "labelVersion");
            this.labelVersion.Name = "labelVersion";
            // 
            // labelProductName
            // 
            resources.ApplyResources(this.labelProductName, "labelProductName");
            this.labelProductName.Name = "labelProductName";
            // 
            // labelTel1
            // 
            resources.ApplyResources(this.labelTel1, "labelTel1");
            this.labelTel1.Name = "labelTel1";
            // 
            // labelEmail1
            // 
            resources.ApplyResources(this.labelEmail1, "labelEmail1");
            this.labelEmail1.Name = "labelEmail1";
            // 
            // labelCopyright1
            // 
            resources.ApplyResources(this.labelCopyright1, "labelCopyright1");
            this.labelCopyright1.Name = "labelCopyright1";
            // 
            // labelVersion1
            // 
            resources.ApplyResources(this.labelVersion1, "labelVersion1");
            this.labelVersion1.Name = "labelVersion1";
            // 
            // labelProductName1
            // 
            resources.ApplyResources(this.labelProductName1, "labelProductName1");
            this.labelProductName1.Name = "labelProductName1";
            // 
            // HelpAboutBox
            // 
            this.AcceptButton = this.okButton;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "HelpAboutBox";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.tableLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.RichTextBox textBoxDescription;
        private System.Windows.Forms.PictureBox logoPictureBox;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label labelSupportPhone;
        private System.Windows.Forms.Label labelSupportEmail;
        private System.Windows.Forms.Label labelCopyright;
        private System.Windows.Forms.Label labelVersion;
        private System.Windows.Forms.Label labelProductName;
        private System.Windows.Forms.Label labelTel1;
        private System.Windows.Forms.Label labelEmail1;
        private System.Windows.Forms.Label labelCopyright1;
        private System.Windows.Forms.Label labelVersion1;
        private System.Windows.Forms.Label labelProductName1;
    }
}