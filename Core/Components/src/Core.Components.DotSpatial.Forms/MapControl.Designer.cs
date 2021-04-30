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

namespace Core.Components.DotSpatial.Forms
{
    partial class MapControl
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
            this.zoomToAllVisibleLayersToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.showCoordinatesToolStripButton = new System.Windows.Forms.ToolStripButton();
            this.toolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.CanOverflow = false;
            this.toolStrip.Dock = System.Windows.Forms.DockStyle.Right;
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.panToolStripButton,
            this.zoomToRectangleToolStripButton,
            this.toolStripSeparator1,
            this.zoomToAllVisibleLayersToolStripButton,
            this.toolStripSeparator2,
            this.showCoordinatesToolStripButton});
            this.toolStrip.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.VerticalStackWithOverflow;
            this.toolStrip.Location = new System.Drawing.Point(649, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(48, 611);
            this.toolStrip.TabIndex = 2;
            // 
            // panToolStripButton
            // 
            this.panToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.panToolStripButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.Pan;
            this.panToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.panToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.panToolStripButton.Name = "panToolStripButton";
            this.panToolStripButton.Size = new System.Drawing.Size(39, 36);
            this.panToolStripButton.ToolTipText = "Verschuiven";
            this.panToolStripButton.Click += new System.EventHandler(this.PanToolStripButtonClick);
            // 
            // zoomToRectangleToolStripButton
            // 
            this.zoomToRectangleToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomToRectangleToolStripButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.ZoomToRectangle;
            this.zoomToRectangleToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomToRectangleToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.zoomToRectangleToolStripButton.Name = "zoomToRectangleToolStripButton";
            this.zoomToRectangleToolStripButton.Size = new System.Drawing.Size(39, 36);
            this.zoomToRectangleToolStripButton.ToolTipText = "Zoom door rechthoek";
            this.zoomToRectangleToolStripButton.Click += new System.EventHandler(this.ZoomToRectangleToolStripButtonClick);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(43, 6);
            // 
            // zoomToAllVisibleLayersToolStripButton
            // 
            this.zoomToAllVisibleLayersToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.zoomToAllVisibleLayersToolStripButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.ZoomToExtents;
            this.zoomToAllVisibleLayersToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.zoomToAllVisibleLayersToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.zoomToAllVisibleLayersToolStripButton.Name = "zoomToAllVisibleLayersToolStripButton";
            this.zoomToAllVisibleLayersToolStripButton.Size = new System.Drawing.Size(39, 36);
            this.zoomToAllVisibleLayersToolStripButton.ToolTipText = "Zoom naar alles";
            this.zoomToAllVisibleLayersToolStripButton.Click += new System.EventHandler(this.ZoomToAllVisibleLayersToolStripButtonClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(43, 6);
            // 
            // showCoordinatesToolStripButton
            // 
            this.showCoordinatesToolStripButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.showCoordinatesToolStripButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.ShowCoordinates;
            this.showCoordinatesToolStripButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.showCoordinatesToolStripButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 3);
            this.showCoordinatesToolStripButton.Name = "showCoordinatesToolStripButton";
            this.showCoordinatesToolStripButton.Size = new System.Drawing.Size(39, 36);
            this.showCoordinatesToolStripButton.ToolTipText = "Toon coördinaten";
            this.showCoordinatesToolStripButton.Click += new System.EventHandler(this.ShowCoordinatesToolStripButtonClick);
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.toolStrip);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(697, 611);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton panToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomToRectangleToolStripButton;
        private System.Windows.Forms.ToolStripButton zoomToAllVisibleLayersToolStripButton;
        private System.Windows.Forms.ToolStripButton showCoordinatesToolStripButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
    }
}