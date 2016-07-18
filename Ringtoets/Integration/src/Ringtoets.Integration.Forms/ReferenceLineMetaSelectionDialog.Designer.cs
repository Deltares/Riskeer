namespace Ringtoets.Integration.Forms
{
    partial class ReferenceLineMetaSelectionDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ReferenceLineMetaSelectionDialog));
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.Cancel = new System.Windows.Forms.Button();
            this.Ok = new System.Windows.Forms.Button();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SelectValueLabel = new System.Windows.Forms.Label();
            this.SignalingLowerLimitComboBox = new System.Windows.Forms.ComboBox();
            this.ReferenceLineMetaDataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            resources.ApplyResources(this.flowLayoutPanel1, "flowLayoutPanel1");
            this.flowLayoutPanel1.Controls.Add(this.Cancel);
            this.flowLayoutPanel1.Controls.Add(this.Ok);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            // 
            // Cancel
            // 
            this.Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.Cancel, "Cancel");
            this.Cancel.Name = "Cancel";
            this.Cancel.UseVisualStyleBackColor = true;
            this.Cancel.Click += new System.EventHandler(this.CancelButtonOnClick);
            // 
            // Ok
            // 
            this.Ok.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.Ok, "Ok");
            this.Ok.Name = "Ok";
            this.Ok.UseVisualStyleBackColor = true;
            this.Ok.Click += new System.EventHandler(this.OkButtonOnClick);
            // 
            // tableLayoutPanel1
            // 
            resources.ApplyResources(this.tableLayoutPanel1, "tableLayoutPanel1");
            this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ReferenceLineMetaDataGridViewControl, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 2);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.Paint += new System.Windows.Forms.PaintEventHandler(this.tableLayoutPanel1_Paint);
            // 
            // panel1
            // 
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Controls.Add(this.SelectValueLabel);
            this.panel1.Controls.Add(this.SignalingLowerLimitComboBox);
            this.panel1.Name = "panel1";
            // 
            // SelectValueLabel
            // 
            resources.ApplyResources(this.SelectValueLabel, "SelectValueLabel");
            this.SelectValueLabel.Name = "SelectValueLabel";
            // 
            // SignalingLowerLimitComboBox
            // 
            this.SignalingLowerLimitComboBox.FormattingEnabled = true;
            resources.ApplyResources(this.SignalingLowerLimitComboBox, "SignalingLowerLimitComboBox");
            this.SignalingLowerLimitComboBox.Name = "SignalingLowerLimitComboBox";
            // 
            // ReferenceLineMetaDataGridViewControl
            // 
            resources.ApplyResources(this.ReferenceLineMetaDataGridViewControl, "ReferenceLineMetaDataGridViewControl");
            this.ReferenceLineMetaDataGridViewControl.MultiSelect = true;
            this.ReferenceLineMetaDataGridViewControl.Name = "ReferenceLineMetaDataGridViewControl";
            this.ReferenceLineMetaDataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // ReferenceLineMetaSelectionDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel1);
            this.Name = "ReferenceLineMetaSelectionDialog";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ComboBox SignalingLowerLimitComboBox;
        private System.Windows.Forms.Label SelectValueLabel;
        private Core.Common.Controls.DataGrid.DataGridViewControl ReferenceLineMetaDataGridViewControl;
    }
}