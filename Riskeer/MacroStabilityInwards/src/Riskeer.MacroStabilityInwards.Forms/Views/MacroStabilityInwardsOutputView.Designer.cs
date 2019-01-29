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

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    partial class MacroStabilityInwardsOutputView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.macroStabilityInwardsOutputChartControl = new Riskeer.MacroStabilityInwards.Forms.Views.MacroStabilityInwardsOutputChartControl(data, getNormativeAssessmentLevelFunc);
            this.slicesTable = new Riskeer.MacroStabilityInwards.Forms.Views.MacroStabilityInwardsSlicesTable();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.macroStabilityInwardsOutputChartControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.slicesTable);
            this.splitContainer.Size = new System.Drawing.Size(150, 150);
            this.splitContainer.SplitterDistance = 100;
            this.splitContainer.TabIndex = 0;
            // 
            // macroStabilityInwardsOutputChartControl
            // 
            this.macroStabilityInwardsOutputChartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.macroStabilityInwardsOutputChartControl.Location = new System.Drawing.Point(0, 0);
            this.macroStabilityInwardsOutputChartControl.Name = "macroStabilityInwardsOutputChartControl";
            this.macroStabilityInwardsOutputChartControl.Size = new System.Drawing.Size(148, 98);
            this.macroStabilityInwardsOutputChartControl.TabIndex = 0;
            // 
            // slicesTable
            // 
            this.slicesTable.AutoSize = true;
            this.slicesTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.slicesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.slicesTable.Location = new System.Drawing.Point(0, 0);
            this.slicesTable.Margin = new System.Windows.Forms.Padding(0);
            this.slicesTable.MultiSelect = true;
            this.slicesTable.Name = "slicesTable";
            this.slicesTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.slicesTable.Size = new System.Drawing.Size(148, 44);
            this.slicesTable.TabIndex = 0;
            // 
            // MacroStabilityInwardsOutputView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "MacroStabilityInwardsOutputView";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            this.splitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private MacroStabilityInwardsOutputChartControl macroStabilityInwardsOutputChartControl;
        private MacroStabilityInwardsSlicesTable slicesTable;
    }
}