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

namespace Riskeer.Common.Forms.Views
{
    partial class IllustrationPointsControl
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.illustrationPointsChartControl = new Riskeer.Common.Forms.Views.IllustrationPointsChartControl();
            this.illustrationPointsTableControl = new Riskeer.Common.Forms.Views.IllustrationPointsTableControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            this.splitContainer.Size = new System.Drawing.Size(150, 150);
            this.splitContainer.SplitterDistance = 72;
            this.splitContainer.TabIndex = 0;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.illustrationPointsChartControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.illustrationPointsTableControl);
            // 
            // illustrationPointsChartControl
            // 
            this.illustrationPointsChartControl.Data = null;
            this.illustrationPointsChartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.illustrationPointsChartControl.Location = new System.Drawing.Point(0, 0);
            this.illustrationPointsChartControl.Name = "illustrationPointsChartControl";
            this.illustrationPointsChartControl.Size = new System.Drawing.Size(131, 230);
            this.illustrationPointsChartControl.TabIndex = 0;
            // 
            // illustrationPointsTableControl
            // 
            this.illustrationPointsTableControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.illustrationPointsTableControl.Location = new System.Drawing.Point(0, 0);
            this.illustrationPointsTableControl.Name = "illustrationPointsTableControl";
            this.illustrationPointsTableControl.Size = new System.Drawing.Size(148, 72);
            this.illustrationPointsTableControl.TabIndex = 0;
            // 
            // IllustrationPointsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "IllustrationPointsControl";
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private IllustrationPointsChartControl illustrationPointsChartControl;
        private IllustrationPointsTableControl illustrationPointsTableControl;
    }
}
