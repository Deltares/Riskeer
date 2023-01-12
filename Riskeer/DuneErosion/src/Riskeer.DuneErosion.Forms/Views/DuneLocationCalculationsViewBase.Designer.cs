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

namespace Riskeer.DuneErosion.Forms.Views
{
    partial class DuneLocationCalculationsViewBase
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
            this.components = new System.ComponentModel.Container();
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.CalculateForSelectedButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.DeselectAllButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.SelectAllButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.CalculateForSelectedButtonErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.ButtonGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CalculateForSelectedButtonErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(533, 85);
            this.dataGridViewControl.TabIndex = 2;
            // 
            // CalculateForSelectedButton
            // 
            this.CalculateForSelectedButton.Enabled = false;
            this.CalculateForSelectedButton.Location = new System.Drawing.Point(227, 29);
            this.CalculateForSelectedButton.Name = "CalculateForSelectedButton";
            this.CalculateForSelectedButton.Size = new System.Drawing.Size(207, 23);
            this.CalculateForSelectedButton.TabIndex = 2;
            this.CalculateForSelectedButton.UseVisualStyleBackColor = true;
            this.CalculateForSelectedButton.Click += new System.EventHandler(this.CalculateForSelectedButton_Click);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.Location = new System.Drawing.Point(110, 29);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(111, 23);
            this.DeselectAllButton.TabIndex = 1;
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Location = new System.Drawing.Point(6, 29);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(98, 23);
            this.SelectAllButton.TabIndex = 0;
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // ButtonGroupBox
            // 
            this.ButtonGroupBox.Controls.Add(this.CalculateForSelectedButton);
            this.ButtonGroupBox.Controls.Add(this.DeselectAllButton);
            this.ButtonGroupBox.Controls.Add(this.SelectAllButton);
            this.ButtonGroupBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.ButtonGroupBox.Location = new System.Drawing.Point(0, 85);
            this.ButtonGroupBox.MinimumSize = new System.Drawing.Size(445, 61);
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.Size = new System.Drawing.Size(533, 61);
            this.ButtonGroupBox.TabIndex = 3;
            this.ButtonGroupBox.TabStop = false;
            // 
            // CalculateForSelectedButtonErrorProvider
            // 
            this.CalculateForSelectedButtonErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.CalculateForSelectedButtonErrorProvider.ContainerControl = this;
            this.CalculateForSelectedButtonErrorProvider.Icon = Core.Gui.Properties.Resources.warning;
            this.CalculateForSelectedButtonErrorProvider.SetIconPadding(CalculateForSelectedButton, 2);
            // 
            // DuneLocationCalculationsViewBase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(526, 85);
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.ButtonGroupBox);
            this.Name = "DuneLocationCalculationsViewBase";
            this.Size = new System.Drawing.Size(533, 146);
            this.ButtonGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CalculateForSelectedButtonErrorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private Core.Common.Controls.Forms.EnhancedButton CalculateForSelectedButton;
        private Core.Common.Controls.Forms.EnhancedButton DeselectAllButton;
        private Core.Common.Controls.Forms.EnhancedButton SelectAllButton;
        protected System.Windows.Forms.GroupBox ButtonGroupBox;
        private System.Windows.Forms.ErrorProvider CalculateForSelectedButtonErrorProvider;
    }
}