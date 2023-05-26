﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
            this.flowLayoutPanelButtons = new System.Windows.Forms.FlowLayoutPanel();
            this.cancelButton = new System.Windows.Forms.Button();
            this.importButton = new System.Windows.Forms.Button();
            this.infoIcon = new System.Windows.Forms.PictureBox();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.tableLayoutPanelForForm = new System.Windows.Forms.TableLayoutPanel();
            this.panelForLabel = new System.Windows.Forms.Panel();
            this.failureMechanismSelectLabel = new System.Windows.Forms.Label();
            this.flowLayoutPanelButtons.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.infoIcon)).BeginInit();
            this.tableLayoutPanelForForm.SuspendLayout();
            this.panelForLabel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // flowLayoutPanelButtons
            // 
            resources.ApplyResources(this.flowLayoutPanelButtons, "flowLayoutPanelButtons");
            this.flowLayoutPanelButtons.Controls.Add(this.cancelButton);
            this.flowLayoutPanelButtons.Controls.Add(this.importButton);
            this.flowLayoutPanelButtons.Controls.Add(this.infoIcon);
            this.flowLayoutPanelButtons.Name = "flowLayoutPanelButtons";
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.cancelButton, "cancelButton");
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // importButton
            // 
            this.importButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            resources.ApplyResources(this.importButton, "importButton");
            this.importButton.Name = "importButton";
            this.importButton.UseVisualStyleBackColor = true;
            // 
            // infoIcon
            // 
            resources.ApplyResources(this.infoIcon, "infoIcon");
            this.infoIcon.Name = "infoIcon";
            this.infoIcon.TabStop = false;
            // 
            // dataGridViewControl
            // 
            resources.ApplyResources(this.dataGridViewControl, "dataGridViewControl");
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // tableLayoutPanelForForm
            // 
            resources.ApplyResources(this.tableLayoutPanelForForm, "tableLayoutPanelForForm");
            this.tableLayoutPanelForForm.Controls.Add(this.panelForLabel, 0, 1);
            this.tableLayoutPanelForForm.Controls.Add(this.dataGridViewControl, 0, 2);
            this.tableLayoutPanelForForm.Controls.Add(this.flowLayoutPanelButtons, 0, 3);
            this.tableLayoutPanelForForm.Name = "tableLayoutPanelForForm";
            // 
            // panelForLabel
            // 
            this.panelForLabel.Controls.Add(this.failureMechanismSelectLabel);
            resources.ApplyResources(this.panelForLabel, "panelForLabel");
            this.panelForLabel.Name = "panelForLabel";
            // 
            // failureMechanismSelectLabel
            // 
            resources.ApplyResources(this.failureMechanismSelectLabel, "failureMechanismSelectLabel");
            this.failureMechanismSelectLabel.Name = "failureMechanismSelectLabel";
            // 
            // AssessmentSectionMergeDataProviderDialog
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanelForForm);
            this.Name = "AssessmentSectionMergeDataProviderDialog";
            this.flowLayoutPanelButtons.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.infoIcon)).EndInit();
            this.tableLayoutPanelForForm.ResumeLayout(false);
            this.tableLayoutPanelForForm.PerformLayout();
            this.panelForLabel.ResumeLayout(false);
            this.panelForLabel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanelForForm;
        private System.Windows.Forms.Panel panelForLabel;
        private System.Windows.Forms.ToolTip toolTip;
        private System.Windows.Forms.PictureBox infoIcon;
        private System.Windows.Forms.Label failureMechanismSelectLabel;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanelButtons;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button importButton;
        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
    }
}