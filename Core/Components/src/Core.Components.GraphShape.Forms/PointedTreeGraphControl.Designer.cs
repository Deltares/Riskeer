// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Core.Components.GraphShape.Forms
{
    partial class PointedTreeGraphControl
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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.wpfElementHost = new System.Windows.Forms.Integration.ElementHost();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.zoomToGraphExtentsToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.tableLayoutPanel.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.wpfElementHost, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.toolStrip, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(150, 150);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // wpfElementHost
            // 
            this.wpfElementHost.Dock = System.Windows.Forms.DockStyle.Fill;
            this.wpfElementHost.Location = new System.Drawing.Point(0, 0);
            this.wpfElementHost.Name = "wpfElementHost";
            this.wpfElementHost.Size = new System.Drawing.Size(122, 150);
            this.wpfElementHost.TabIndex = 1;
            this.wpfElementHost.Text = "elementHost1";
            this.wpfElementHost.Child = null;
            // 
            // toolStrip
            // 
            this.toolStrip.BackColor = Core.Common.Controls.Style.ColorDefinitions.ControlBackgroundColor;
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.zoomToGraphExtentsToolStripButton});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(122, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Renderer = new Core.Common.Controls.Forms.CustomToolStripRenderer();
            this.toolStrip.Size = new System.Drawing.Size(28, 150);
            this.toolStrip.TabIndex = 2;
            // 
            // zoomToGraphExtentsToolStripButton
            // 
            this.zoomToGraphExtentsToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.zoomToGraphExtentsToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.zoomToGraphExtentsToolStripButton.Name = "zoomToGraphExtentsToolStripButton";
            this.zoomToGraphExtentsToolStripButton.Size = new System.Drawing.Size(22, 19);
            this.zoomToGraphExtentsToolStripButton.Text = "\uE902";
            this.zoomToGraphExtentsToolStripButton.ToolTipText = global::Core.Components.GraphShape.Forms.Properties.Resources.PointedTreeGraphControl_ZoomToGraphExtents;
            this.zoomToGraphExtentsToolStripButton.Click += new System.EventHandler(this.ZoomToGraphExtentsToolStripButtonClick);
            // 
            // PointedTreeGraphControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "PointedTreeGraphControl";
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.Integration.ElementHost wpfElementHost;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton zoomToGraphExtentsToolStripButton;
    }
}
