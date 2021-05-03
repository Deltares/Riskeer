// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.ComponentModel;
using Core.Components.OxyPlot.Forms.Properties;

namespace Core.Components.OxyPlot.Forms
{
    partial class ChartControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChartControl));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.panToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomToRectangleToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToVisibleLayersToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.panToolStripButton,
            this.zoomToRectangleToolStripButton,
            this.toolStripSeparator1,
            this.zoomToVisibleLayersToolStripButton});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(651, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(48, 596);
            this.toolStrip.TabIndex = 0;
            // 
            // panToolStripButton
            // 
            this.panToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.panToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.panToolStripButton.Name = "panToolStripButton";
            this.panToolStripButton.Size = new System.Drawing.Size(39, 29);
            this.panToolStripButton.Text = "";
            this.panToolStripButton.ToolTipText = Resources.ChartControl_Pan;
            this.panToolStripButton.Click += new System.EventHandler(this.PanToolStripButtonClick);
            // 
            // zoomToRectangleToolStripButton
            // 
            this.zoomToRectangleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomToRectangleToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.zoomToRectangleToolStripButton.Name = "zoomToRectangleToolStripButton";
            this.zoomToRectangleToolStripButton.Size = new System.Drawing.Size(39, 29);
            this.zoomToRectangleToolStripButton.Text = "";
            this.zoomToRectangleToolStripButton.ToolTipText = Resources.ChartControl_ZoomToRectangle;
            this.zoomToRectangleToolStripButton.Click += new System.EventHandler(this.ZoomToRectangleToolStripButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(43, 6);
            // 
            // zoomToAllVisibleLayersToolStripButton
            // 
            this.zoomToVisibleLayersToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomToVisibleLayersToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.zoomToVisibleLayersToolStripButton.Name = "zoomToAllVisibleLayersToolStripButton";
            this.zoomToVisibleLayersToolStripButton.Size = new System.Drawing.Size(39, 29);
            this.zoomToVisibleLayersToolStripButton.Text = "";
            this.zoomToVisibleLayersToolStripButton.ToolTipText = Resources.ChartControl_ZoomToVisibleLayers;
            this.zoomToVisibleLayersToolStripButton.Click += new System.EventHandler(this.ZoomToAllVisibleLayersToolStripButtonClick);
            // 
            // ChartControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip);
            this.Name = "ChartControl";
            this.Size = new System.Drawing.Size(699, 596);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton panToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomToRectangleToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton zoomToVisibleLayersToolStripButton;
    }
}