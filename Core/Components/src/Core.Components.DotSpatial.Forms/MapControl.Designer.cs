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
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.panToggleButton = new System.Windows.Forms.CheckBox();
            this.zoomToRectangleToggleButton = new System.Windows.Forms.CheckBox();
            this.zoomToExtentsButton = new System.Windows.Forms.CheckBox();
            this.showCoordinatesToggleButton = new System.Windows.Forms.CheckBox();
            this.tableLayoutPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tableLayoutPanel.BackColor = System.Drawing.Color.Transparent;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel.Controls.Add(this.panToggleButton, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.zoomToRectangleToggleButton, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.zoomToExtentsButton, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.showCoordinatesToggleButton, 1, 1);
            this.tableLayoutPanel.Location = new System.Drawing.Point(618, 3);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 38F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(76, 76);
            this.tableLayoutPanel.TabIndex = 1;
            // 
            // panToggleButton
            // 
            this.panToggleButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.panToggleButton.AutoSize = true;
            this.panToggleButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panToggleButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.MapPanZoomImage;
            this.panToggleButton.Location = new System.Drawing.Point(3, 3);
            this.panToggleButton.Name = "panToggleButton";
            this.panToggleButton.Size = new System.Drawing.Size(32, 32);
            this.panToggleButton.TabIndex = 0;
            this.panToggleButton.UseVisualStyleBackColor = true;
            // 
            // zoomToRectangleToggleButton
            // 
            this.zoomToRectangleToggleButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.zoomToRectangleToggleButton.AutoSize = true;
            this.zoomToRectangleToggleButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomToRectangleToggleButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.zoomrectangle;
            this.zoomToRectangleToggleButton.Location = new System.Drawing.Point(41, 3);
            this.zoomToRectangleToggleButton.Name = "zoomToRectangleToggleButton";
            this.zoomToRectangleToggleButton.Size = new System.Drawing.Size(32, 32);
            this.zoomToRectangleToggleButton.TabIndex = 1;
            this.zoomToRectangleToggleButton.UseVisualStyleBackColor = true;
            // 
            // zoomToExtentsButton
            // 
            this.zoomToExtentsButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.zoomToExtentsButton.AutoSize = true;
            this.zoomToExtentsButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.zoomToExtentsButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.zoomextents;
            this.zoomToExtentsButton.Location = new System.Drawing.Point(3, 41);
            this.zoomToExtentsButton.Name = "zoomToExtentsButton";
            this.zoomToExtentsButton.Size = new System.Drawing.Size(32, 32);
            this.zoomToExtentsButton.TabIndex = 2;
            this.zoomToExtentsButton.UseVisualStyleBackColor = true;
            // 
            // showCoordinatesToggleButton
            // 
            this.showCoordinatesToggleButton.Appearance = System.Windows.Forms.Appearance.Button;
            this.showCoordinatesToggleButton.AutoSize = true;
            this.showCoordinatesToggleButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.showCoordinatesToggleButton.Image = global::Core.Components.DotSpatial.Forms.Properties.Resources.map_pin;
            this.showCoordinatesToggleButton.Location = new System.Drawing.Point(41, 41);
            this.showCoordinatesToggleButton.Name = "showCoordinatesToggleButton";
            this.showCoordinatesToggleButton.Size = new System.Drawing.Size(32, 32);
            this.showCoordinatesToggleButton.TabIndex = 3;
            this.showCoordinatesToggleButton.UseVisualStyleBackColor = true;
            // 
            // MapControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "MapControl";
            this.Size = new System.Drawing.Size(697, 611);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.CheckBox panToggleButton;
        private System.Windows.Forms.CheckBox zoomToRectangleToggleButton;
        private System.Windows.Forms.CheckBox zoomToExtentsButton;
        private System.Windows.Forms.CheckBox showCoordinatesToggleButton;
    }
}