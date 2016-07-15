// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Integration.Forms.Views
{
    partial class ReferenceLineMetaSelectionView : UserControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ReferenceLineMetaDataGrid = new System.Windows.Forms.DataGridView();
            this.AssessmentSectionId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SignalingValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LowerLimitValue = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.eventLog1 = new System.Diagnostics.EventLog();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.SignalingLowerLimitLabel = new System.Windows.Forms.Label();
            this.SignalingLowerLimitComboBox = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.ReferenceLineMetaDataGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).BeginInit();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // ReferenceLineMetaDataGrid
            // 
            this.ReferenceLineMetaDataGrid.AllowUserToAddRows = false;
            this.ReferenceLineMetaDataGrid.AllowUserToDeleteRows = false;
            this.ReferenceLineMetaDataGrid.AllowUserToResizeColumns = false;
            this.ReferenceLineMetaDataGrid.AllowUserToResizeRows = false;
            this.ReferenceLineMetaDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ReferenceLineMetaDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.AssessmentSectionId,
            this.SignalingValue,
            this.LowerLimitValue});
            this.ReferenceLineMetaDataGrid.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ReferenceLineMetaDataGrid.Location = new System.Drawing.Point(0, 34);
            this.ReferenceLineMetaDataGrid.MultiSelect = false;
            this.ReferenceLineMetaDataGrid.Name = "ReferenceLineMetaDataGrid";
            this.ReferenceLineMetaDataGrid.RowHeadersVisible = false;
            this.ReferenceLineMetaDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.ReferenceLineMetaDataGrid.Size = new System.Drawing.Size(338, 167);
            this.ReferenceLineMetaDataGrid.TabIndex = 1;
            this.ReferenceLineMetaDataGrid.TabStop = false;
            // 
            // AssessmentSectionId
            // 
            this.AssessmentSectionId.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.AssessmentSectionId.DataPropertyName = "AssessmentSectionId";
            this.AssessmentSectionId.HeaderText = "Identificatiecode ";
            this.AssessmentSectionId.Name = "AssessmentSectionId";
            this.AssessmentSectionId.ReadOnly = true;
            // 
            // SignalingValue
            // 
            this.SignalingValue.DataPropertyName = "SignalingValue";
            this.SignalingValue.FillWeight = 200F;
            this.SignalingValue.HeaderText = "Signaleringswaarde";
            this.SignalingValue.Name = "SignalingValue";
            this.SignalingValue.ReadOnly = true;
            this.SignalingValue.Width = 110;
            // 
            // LowerLimitValue
            // 
            this.LowerLimitValue.DataPropertyName = "LowerLimitValue";
            this.LowerLimitValue.FillWeight = 200F;
            this.LowerLimitValue.HeaderText = "Ondergrens";
            this.LowerLimitValue.Name = "LowerLimitValue";
            this.LowerLimitValue.ReadOnly = true;
            this.LowerLimitValue.Width = 110;
            // 
            // eventLog1
            // 
            this.eventLog1.SynchronizingObject = this;
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this.SignalingLowerLimitLabel);
            this.flowLayoutPanel1.Controls.Add(this.SignalingLowerLimitComboBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(338, 27);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // SignalingLowerLimitLabel
            // 
            this.SignalingLowerLimitLabel.AutoSize = true;
            this.SignalingLowerLimitLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SignalingLowerLimitLabel.Location = new System.Drawing.Point(3, 3);
            this.SignalingLowerLimitLabel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 0);
            this.SignalingLowerLimitLabel.Name = "SignalingLowerLimitLabel";
            this.SignalingLowerLimitLabel.Size = new System.Drawing.Size(65, 13);
            this.SignalingLowerLimitLabel.TabIndex = 0;
            this.SignalingLowerLimitLabel.Text = "Kies waarde";
            // 
            // SignalingLowerLimitComboBox
            // 
            this.SignalingLowerLimitComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SignalingLowerLimitComboBox.FormattingEnabled = true;
            this.SignalingLowerLimitComboBox.Location = new System.Drawing.Point(74, 3);
            this.SignalingLowerLimitComboBox.Name = "SignalingLowerLimitComboBox";
            this.SignalingLowerLimitComboBox.Size = new System.Drawing.Size(121, 21);
            this.SignalingLowerLimitComboBox.TabIndex = 0;
            // 
            // ReferenceLineMetaSelectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.flowLayoutPanel1);
            this.Controls.Add(this.ReferenceLineMetaDataGrid);
            this.Name = "ReferenceLineMetaSelectionView";
            this.Size = new System.Drawing.Size(338, 201);
            ((System.ComponentModel.ISupportInitialize)(this.ReferenceLineMetaDataGrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.eventLog1)).EndInit();
            this.flowLayoutPanel1.ResumeLayout(false);
            this.flowLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private DataGridView ReferenceLineMetaDataGrid;
        private DataGridViewTextBoxColumn AssessmentSectionId;
        private DataGridViewTextBoxColumn SignalingValue;
        private DataGridViewTextBoxColumn LowerLimitValue;
        private System.Diagnostics.EventLog eventLog1;
        private FlowLayoutPanel flowLayoutPanel1;
        private Label SignalingLowerLimitLabel;
        private ComboBox SignalingLowerLimitComboBox;


    }
}
