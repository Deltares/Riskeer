// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System.Drawing;
using System.Windows.Forms;
using Core.Common.Controls;

namespace Riskeer.Integration.Forms.Controls
{
    partial class AssemblyResultWithProbabilityControl
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssemblyResultWithProbabilityControl));
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.resultLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.assessmentSectionAssemblyCategoryGroupLabel = new System.Windows.Forms.Label();
            this.assessmentSectionFailureProbabilityLabel = new System.Windows.Forms.Label();
            this.groupLabel = new Core.Common.Controls.BorderedLabel();
            this.probabilityLabel = new Core.Common.Controls.BorderedLabel();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.resultLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            this.errorProvider.Icon = ((System.Drawing.Icon)(resources.GetObject("errorProvider.Icon")));
            // 
            // resultLayoutPanel
            // 
            this.resultLayoutPanel.ColumnCount = 2;
            this.resultLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.resultLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.resultLayoutPanel.Controls.Add(this.assessmentSectionAssemblyCategoryGroupLabel, 0, 0);
            this.resultLayoutPanel.Controls.Add(this.assessmentSectionFailureProbabilityLabel, 0, 1);
            this.resultLayoutPanel.Controls.Add(this.groupLabel, 1, 0);
            this.resultLayoutPanel.Controls.Add(this.probabilityLabel, 1, 1);
            this.resultLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.resultLayoutPanel.Name = "resultLayoutPanel";
            this.resultLayoutPanel.RowCount = 2;
            this.resultLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.resultLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.resultLayoutPanel.Size = new System.Drawing.Size(1361, 796);
            this.resultLayoutPanel.AutoSize = true;
            this.resultLayoutPanel.TabIndex = 0;
            // 
            // assessmentSectionAssemblyCategoryGroupLabel
            // 
            this.assessmentSectionAssemblyCategoryGroupLabel.AutoSize = true;
            this.assessmentSectionAssemblyCategoryGroupLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.assessmentSectionAssemblyCategoryGroupLabel.Location = new System.Drawing.Point(3, 0);
            this.assessmentSectionAssemblyCategoryGroupLabel.Name = "assessmentSectionAssemblyCategoryGroupLabel";
            this.assessmentSectionAssemblyCategoryGroupLabel.Size = new System.Drawing.Size(92, 13);
            this.assessmentSectionAssemblyCategoryGroupLabel.Dock = DockStyle.Left;
            this.assessmentSectionAssemblyCategoryGroupLabel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.assessmentSectionAssemblyCategoryGroupLabel.Margin = new System.Windows.Forms.Padding(3);
            this.assessmentSectionAssemblyCategoryGroupLabel.TabIndex = 0;
            this.assessmentSectionAssemblyCategoryGroupLabel.Text = "Veiligheidsoordeel";
            // 
            // assessmentSectionFailureProbabilityLabel
            // 
            this.assessmentSectionFailureProbabilityLabel.AutoSize = true;
            this.assessmentSectionFailureProbabilityLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.assessmentSectionFailureProbabilityLabel.Location = new System.Drawing.Point(3, 398);
            this.assessmentSectionFailureProbabilityLabel.Name = "assessmentSectionFailureProbabilityLabel";
            this.assessmentSectionFailureProbabilityLabel.Size = new System.Drawing.Size(50, 13);
            this.assessmentSectionFailureProbabilityLabel.Dock = DockStyle.Left;
            this.assessmentSectionFailureProbabilityLabel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.assessmentSectionFailureProbabilityLabel.Margin = new System.Windows.Forms.Padding(3);
            this.assessmentSectionFailureProbabilityLabel.TabIndex = 1;
            this.assessmentSectionFailureProbabilityLabel.Text = "Faalkans";
            // 
            // groupLabel
            // 
            this.groupLabel.AutoSize = true;
            this.groupLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.groupLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.groupLabel.Location = new System.Drawing.Point(153, 0);
            this.groupLabel.MinimumSize = new System.Drawing.Size(50, 2);
            this.groupLabel.Name = "groupLabel";
            this.groupLabel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.groupLabel.Margin = new System.Windows.Forms.Padding(3);
            this.groupLabel.Size = new System.Drawing.Size(1205, 398);
            this.groupLabel.TabIndex = 2;
            this.groupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // probabilityLabel
            // 
            this.probabilityLabel.AutoSize = true;
            this.probabilityLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.probabilityLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.probabilityLabel.Location = new System.Drawing.Point(153, 398);
            this.probabilityLabel.MinimumSize = new System.Drawing.Size(50, 2);
            this.probabilityLabel.Name = "probabilityLabel";
            this.probabilityLabel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.probabilityLabel.Margin = new System.Windows.Forms.Padding(3);
            this.probabilityLabel.Size = new System.Drawing.Size(1205, 398);
            this.probabilityLabel.TabIndex = 3;
            this.probabilityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.probabilityLabel.BackColor = Color.White;
            // 
            // AssemblyResultWithProbabilityControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.resultLayoutPanel);
            this.Name = "AssemblyResultWithProbabilityControl";
            this.Size = new System.Drawing.Size(1361, 796);
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.resultLayoutPanel.ResumeLayout(false);
            this.resultLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ErrorProvider errorProvider;
        private TableLayoutPanel resultLayoutPanel;
        private Label assessmentSectionAssemblyCategoryGroupLabel;
        private Label assessmentSectionFailureProbabilityLabel;
        private BorderedLabel groupLabel;
        private BorderedLabel probabilityLabel;
    }
}
