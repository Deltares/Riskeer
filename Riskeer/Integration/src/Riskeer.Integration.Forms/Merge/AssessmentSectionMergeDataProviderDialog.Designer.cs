// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Windows.Forms;

namespace Riskeer.Integration.Forms.Merge
{
    partial class AssessmentSectionMergeDataProviderDialog
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssessmentSectionMergeDataProviderDialog));
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.infoIcon = new System.Windows.Forms.PictureBox();
            this.tableLayoutPanelForForm = new System.Windows.Forms.TableLayoutPanel();
            this.assessmentSectionSelectLabel = new System.Windows.Forms.Label();
            this.assessmentSectionComboBox = new System.Windows.Forms.ComboBox();
            this.tableLayoutPanelForLabel = new System.Windows.Forms.TableLayoutPanel();
            this.failureMechanismSelectLabel = new System.Windows.Forms.Label();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.infoIcon)).BeginInit();
            this.tableLayoutPanelForForm.SuspendLayout();
            this.tableLayoutPanelForLabel.SuspendLayout();
            this.flowLayoutPanelButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // infoIcon
            // 
            resources.ApplyResources(this.infoIcon, "infoIcon");
            this.infoIcon.Name = "infoIcon";
            this.infoIcon.TabStop = false;
            // 
            // tableLayoutPanelForForm
            // 
            resources.ApplyResources(this.tableLayoutPanelForForm, "tableLayoutPanelForForm");
            this.tableLayoutPanelForForm.Controls.Add(this.assessmentSectionSelectLabel, 0, 0);
            this.tableLayoutPanelForForm.Controls.Add(this.assessmentSectionComboBox, 0, 1);
            this.tableLayoutPanelForForm.Controls.Add(this.tableLayoutPanelForLabel, 0, 2);
            this.tableLayoutPanelForForm.Controls.Add(this.dataGridViewControl, 0, 3);
            this.tableLayoutPanelForForm.Controls.Add(this.flowLayoutPanelButtons, 0, 4);
            this.tableLayoutPanelForForm.Name = "tableLayoutPanelForForm";
            // 
            // assessmentSectionSelectLabel
            // 
            resources.ApplyResources(this.assessmentSectionSelectLabel, "assessmentSectionSelectLabel");
            this.assessmentSectionSelectLabel.Name = "assessmentSectionSelectLabel";
            // 
            // assessmentSectionComboBox
            // 
            resources.ApplyResources(this.assessmentSectionComboBox, "assessmentSectionComboBox");
            this.assessmentSectionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.assessmentSectionComboBox.Name = "assessmentSectionComboBox";
            // 
            // tableLayoutPanelForLabel
            // 
            resources.ApplyResources(this.tableLayoutPanelForLabel, "tableLayoutPanelForLabel");
            this.tableLayoutPanelForLabel.Controls.Add(this.infoIcon, 1, 0);
            this.tableLayoutPanelForLabel.Controls.Add(this.failureMechanismSelectLabel, 0, 0);
            this.tableLayoutPanelForLabel.Name = "tableLayoutPanelForLabel";
            // 
            // failureMechanismSelectLabel
            // 
            resources.ApplyResources(this.failureMechanismSelectLabel, "failureMechanismSelectLabel");
            this.failureMechanismSelectLabel.Name = "failureMechanismSelectLabel";
            // 
            // dataGridViewControl
            // 
            resources.ApplyResources(this.dataGridViewControl, "dataGridViewControl");
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // flowLayoutPanelButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelButtons, "flowLayoutPanelButtons");
            this.flowLayoutPanelButtons.Controls.Add(this.cancelButton);
            this.flowLayoutPanelButtons.Controls.Add(this.importButton);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            // 
            // cancelButton
            // 
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.DialogResult = DialogResult.Cancel;
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // importButton
            // 
            resources.ApplyResources(this.importButton, "importButton");
            this.importButton.DialogResult = DialogResult.OK;
            this.importButton.Name = "importButton";
            this.importButton.UseVisualStyleBackColor = true;
            // 
            // AssessmentSectionMergeDataProviderDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelForForm);
            this.Name = "AssessmentSectionMergeDataProviderDialog";
            ((System.ComponentModel.ISupportInitialize)(this.infoIcon)).EndInit();
            this.tableLayoutPanelForForm.ResumeLayout(false);
            this.tableLayoutPanelForForm.PerformLayout();
            this.tableLayoutPanelForLabel.ResumeLayout(false);
            this.tableLayoutPanelForLabel.PerformLayout();
            this.flowLayoutPanelButtons.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelForForm;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelForLabel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox infoIcon;
        private System.Windows.Forms.Label assessmentSectionSelectLabel;
        private System.Windows.Forms.Label failureMechanismSelectLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button importButton;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.ComboBox assessmentSectionComboBox;
    }
}