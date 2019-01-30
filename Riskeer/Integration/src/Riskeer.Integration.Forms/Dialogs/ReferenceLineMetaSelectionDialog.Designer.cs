// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Riskeer.Integration.Forms.Dialogs
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
            this.LowLimitValueRadioButton = new System.Windows.Forms.RadioButton();
            this.SignallingValueRadioButton = new System.Windows.Forms.RadioButton();
            this.ReferenceLineMetaDataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.SelectAssessmentSectionLabel = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.flowLayoutPanel1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.groupBox1.SuspendLayout();
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
            this.tableLayoutPanel1.Controls.Add(this.SelectAssessmentSectionLabel, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.ReferenceLineMetaDataGridViewControl, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.flowLayoutPanel1, 0, 3);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            // 
            // LowLimitValueRadioButton
            // 
            resources.ApplyResources(this.LowLimitValueRadioButton, "LowLimitValueRadioButton");
            this.LowLimitValueRadioButton.Name = "LowLimitValueRadioButton";
            this.LowLimitValueRadioButton.UseVisualStyleBackColor = true;
            // 
            // SignallingValueRadioButton
            // 
            resources.ApplyResources(this.SignallingValueRadioButton, "SignallingValueRadioButton");
            this.SignallingValueRadioButton.Checked = true;
            this.SignallingValueRadioButton.Name = "SignallingValueRadioButton";
            this.SignallingValueRadioButton.TabStop = true;
            this.SignallingValueRadioButton.UseVisualStyleBackColor = true;
            // 
            // ReferenceLineMetaDataGridViewControl
            // 
            resources.ApplyResources(this.ReferenceLineMetaDataGridViewControl, "ReferenceLineMetaDataGridViewControl");
            this.ReferenceLineMetaDataGridViewControl.MultiSelect = false;
            this.ReferenceLineMetaDataGridViewControl.Name = "ReferenceLineMetaDataGridViewControl";
            this.ReferenceLineMetaDataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // SelectAssessmentSectionLabel
            // 
            resources.ApplyResources(this.SelectAssessmentSectionLabel, "SelectAssessmentSectionLabel");
            this.SelectAssessmentSectionLabel.Name = "SelectAssessmentSectionLabel";
            // 
            // groupBox1
            // 
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Controls.Add(this.SignallingValueRadioButton);
            this.groupBox1.Controls.Add(this.LowLimitValueRadioButton);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
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
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
        private System.Windows.Forms.Button Ok;
        private System.Windows.Forms.Button Cancel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private Core.Common.Controls.DataGrid.DataGridViewControl ReferenceLineMetaDataGridViewControl;
        private System.Windows.Forms.RadioButton LowLimitValueRadioButton;
        private System.Windows.Forms.RadioButton SignallingValueRadioButton;
        private System.Windows.Forms.Label SelectAssessmentSectionLabel;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}