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
using System.Drawing;
using Core.Common.Controls.Forms;
using Core.Common.Controls.Style;

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
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.panToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.zoomToRectangleToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.zoomToVisibleSeriesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.BackColor = ColorDefinitions.ControlBackgroundColor;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.panToolStripButton,
            this.zoomToRectangleToolStripButton,
            this.toolStripSeparator1,
            this.zoomToVisibleSeriesToolStripButton});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(438, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(28, 387);
            this.toolStrip.TabIndex = 0;
            this.toolStrip.Renderer = new CustomToolStripRenderer();
            // 
            // panToolStripButton
            // 
            this.panToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.panToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.panToolStripButton.Name = "panToolStripButton";
            this.panToolStripButton.Size = new System.Drawing.Size(22, 19);
            this.panToolStripButton.Text = "\uE900";
            this.panToolStripButton.ToolTipText = global::Core.Components.OxyPlot.Forms.Properties.Resources.ChartControl_Pan;
            this.panToolStripButton.Click += new System.EventHandler(this.PanToolStripButtonClick);
            // 
            // zoomToRectangleToolStripButton
            // 
            this.zoomToRectangleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomToRectangleToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.zoomToRectangleToolStripButton.Name = "zoomToRectangleToolStripButton";
            this.zoomToRectangleToolStripButton.Size = new System.Drawing.Size(22, 19);
            this.zoomToRectangleToolStripButton.Text = "\uE901";
            this.zoomToRectangleToolStripButton.ToolTipText = global::Core.Components.OxyPlot.Forms.Properties.Resources.ChartControl_ZoomToRectangle;
            this.zoomToRectangleToolStripButton.Click += new System.EventHandler(this.ZoomToRectangleToolStripButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(26, 6);
            // 
            // zoomToVisibleSeriesToolStripButton
            // 
            this.zoomToVisibleSeriesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomToVisibleSeriesToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.zoomToVisibleSeriesToolStripButton.Name = "zoomToVisibleSeriesToolStripButton";
            this.zoomToVisibleSeriesToolStripButton.Size = new System.Drawing.Size(22, 19);
            this.zoomToVisibleSeriesToolStripButton.Text = "\uE902";
            this.zoomToVisibleSeriesToolStripButton.ToolTipText = global::Core.Components.OxyPlot.Forms.Properties.Resources.ChartControl_ZoomToVisibleSeries;
            this.zoomToVisibleSeriesToolStripButton.Click += new System.EventHandler(this.ZoomToVisibleSeriesToolStripButtonClick);
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.toolStrip, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(466, 387);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // ChartControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "ChartControl";
            this.Size = new System.Drawing.Size(466, 387);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton panToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomToRectangleToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton zoomToVisibleSeriesToolStripButton;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
    }
}