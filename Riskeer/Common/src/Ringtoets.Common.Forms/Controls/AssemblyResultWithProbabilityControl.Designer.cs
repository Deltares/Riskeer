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

using Core.Common.Controls;

namespace Ringtoets.Common.Forms.Controls
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
            this.probabilityPanel = new System.Windows.Forms.TableLayoutPanel();
            this.ProbabilityLabel = new BorderedLabel();
            this.probabilityPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // probabilityPanel
            // 
            this.probabilityPanel.ColumnCount = 1;
            this.probabilityPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.probabilityPanel.Controls.Add(this.ProbabilityLabel, 0, 0);
            this.probabilityPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.probabilityPanel.Location = new System.Drawing.Point(56, 0);
            this.probabilityPanel.Name = "probabilityPanel";
            this.probabilityPanel.AutoSize = true;
            this.probabilityPanel.RowCount = 1;
            this.probabilityPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.probabilityPanel.TabIndex = 1;
            // 
            // ProbabilityLabel
            // 
            this.ProbabilityLabel.AutoSize = true;
            this.ProbabilityLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ProbabilityLabel.Dock = System.Windows.Forms.DockStyle.Left;
            this.ProbabilityLabel.Location = new System.Drawing.Point(3, 3);
            this.ProbabilityLabel.Margin = new System.Windows.Forms.Padding(3);
            this.ProbabilityLabel.MinimumSize = new System.Drawing.Size(50, 2);
            this.ProbabilityLabel.Name = "ProbabilityLabel";
            this.ProbabilityLabel.BackColor = System.Drawing.Color.White;
            this.ProbabilityLabel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.ProbabilityLabel.Size = new System.Drawing.Size(50, 790);
            this.ProbabilityLabel.TabIndex = 0;
            this.ProbabilityLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // FailureMechanismAssemblyControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.probabilityPanel);
            this.Name = "FailureMechanismAssemblyControl";
            this.Controls.SetChildIndex(this.probabilityPanel, 0);
            this.probabilityPanel.ResumeLayout(false);
            this.probabilityPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel probabilityPanel;
        protected BorderedLabel ProbabilityLabel;
    }
}
