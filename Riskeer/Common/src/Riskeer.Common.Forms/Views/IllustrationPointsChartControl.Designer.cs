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
using Riskeer.Common.Forms.Properties;

namespace Riskeer.Common.Forms.Views
{
    partial class IllustrationPointsChartControl
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
            this.stackChartControl = new Core.Components.OxyPlot.Forms.StackChartControl();
            this.SuspendLayout();
            // 
            // stackChartControl
            // 
            this.stackChartControl.ChartTitle = null;
            this.stackChartControl.VerticalAxisTitle = Resources.IllustrationPointsChartControl_StackChartControl_VerticalAxisTitle;
            this.stackChartControl.Data = null;
            this.stackChartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.stackChartControl.Location = new System.Drawing.Point(0, 0);
            this.stackChartControl.Name = "stackChartControl";
            this.stackChartControl.Size = new System.Drawing.Size(150, 150);
            this.stackChartControl.TabIndex = 0;
            this.stackChartControl.Text = "stackChartControl";
            // 
            // IllustrationPointsChartControl
            // 
            this.BorderStyle = BorderStyle.FixedSingle;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(400, 320);
            this.Controls.Add(this.stackChartControl);
            this.Name = "IllustrationPointsChartControl";
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Components.OxyPlot.Forms.StackChartControl stackChartControl;
    }
}
