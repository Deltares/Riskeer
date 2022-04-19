﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Views
{
    partial class MacroStabilityInwardsOutputChartControl
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
            this.chartControl = new Core.Components.OxyPlot.Forms.ChartControl();
            this.SuspendLayout();
            // 
            // chartControl
            // 
            this.chartControl.BottomAxisTitle = RiskeerCommonFormsResources.View_Distance_DisplayName;
            this.chartControl.ChartTitle = null;
            this.chartControl.Data = null;
            this.chartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.chartControl.LeftAxisTitle = RiskeerCommonFormsResources.View_Height_DisplayName;
            this.chartControl.Location = new System.Drawing.Point(0, 0);
            this.chartControl.MinimumSize = new System.Drawing.Size(100, 100);
            this.chartControl.Name = "chartControl";
            this.chartControl.Size = new System.Drawing.Size(150, 150);
            this.chartControl.TabIndex = 0;
            this.chartControl.Text = "chartControl";
            // 
            // MacroStabilityInwardsOutputChartControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.chartControl);
            this.Name = "MacroStabilityInwardsOutputChartControl";
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Components.OxyPlot.Forms.ChartControl chartControl;
    }
}
