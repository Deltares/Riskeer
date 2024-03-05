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

namespace Riskeer.Common.Forms.Views
{
    partial class LocationCalculationsView<T>
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.HideHydraulicBoundaryDatabaseColumnCheckBox = new System.Windows.Forms.CheckBox();
            this.illustrationPointsControl = new Riskeer.Common.Forms.Views.IllustrationPointsControl();
            this.ButtonGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.CalculateForSelectedButtonErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
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
            this.dataGridViewControl.Size = new System.Drawing.Size(536, 347);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // CalculateForSelectedButton
            // 
            this.CalculateForSelectedButton.Enabled = false;
            this.CalculateForSelectedButtonErrorProvider.SetIconPadding(this.CalculateForSelectedButton, 2);
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
            this.ButtonGroupBox.Location = new System.Drawing.Point(0, 364);
            this.ButtonGroupBox.MinimumSize = new System.Drawing.Size(445, 61);
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.Size = new System.Drawing.Size(536, 61);
            this.ButtonGroupBox.TabIndex = 3;
            this.ButtonGroupBox.TabStop = false;
            // 
            // CalculateForSelectedButtonErrorProvider
            // 
            this.CalculateForSelectedButtonErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.CalculateForSelectedButtonErrorProvider.ContainerControl = this;
            this.CalculateForSelectedButtonErrorProvider.Icon = Core.Gui.Properties.Resources.warning;
            // 
            // splitContainer
            // 
            this.splitContainer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            this.splitContainer.Size = new System.Drawing.Size(822, 543);
            this.splitContainer.SplitterDistance = 445;
            this.splitContainer.TabIndex = 1;
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.AutoScroll = true;
            this.splitContainer.Panel1.AutoScrollMinSize = new System.Drawing.Size(535, 0);
            this.splitContainer.Panel1.Controls.Add(this.dataGridViewControl);
            this.splitContainer.Panel1.Controls.Add(this.HideHydraulicBoundaryDatabaseColumnCheckBox);
            this.splitContainer.Panel1.Controls.Add(this.ButtonGroupBox);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.illustrationPointsControl);
            this.splitContainer.Size = new System.Drawing.Size(991, 425);
            this.splitContainer.SplitterDistance = 536;
            this.splitContainer.TabIndex = 1;
            // 
            // HideHydraulicBoundaryDatabaseColumnCheckBox
            // 
            this.HideHydraulicBoundaryDatabaseColumnCheckBox.AutoSize = true;
            this.HideHydraulicBoundaryDatabaseColumnCheckBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.HideHydraulicBoundaryDatabaseColumnCheckBox.Location = new System.Drawing.Point(0, 347);
            this.HideHydraulicBoundaryDatabaseColumnCheckBox.Name = "HideHydraulicBoundaryDatabaseColumnCheckBox";
            this.HideHydraulicBoundaryDatabaseColumnCheckBox.Size = new System.Drawing.Size(536, 17);
            this.HideHydraulicBoundaryDatabaseColumnCheckBox.TabIndex = 3;
            this.HideHydraulicBoundaryDatabaseColumnCheckBox.UseVisualStyleBackColor = true;
            // 
            // illustrationPointsControl
            // 
            this.illustrationPointsControl.Data = null;
            this.illustrationPointsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.illustrationPointsControl.Location = new System.Drawing.Point(0, 0);
            this.illustrationPointsControl.Name = "illustrationPointsControl";
            this.illustrationPointsControl.Size = new System.Drawing.Size(451, 425);
            this.illustrationPointsControl.TabIndex = 0;
            // 
            // LocationCalculationsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "LocationCalculationsView";
            this.Size = new System.Drawing.Size(991, 425);
            this.ButtonGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.CalculateForSelectedButtonErrorProvider)).EndInit();
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel1.PerformLayout();
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize) (this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private Core.Common.Controls.Forms.EnhancedButton CalculateForSelectedButton;
        private Core.Common.Controls.Forms.EnhancedButton DeselectAllButton;
        private Core.Common.Controls.Forms.EnhancedButton SelectAllButton;
        protected System.Windows.Forms.GroupBox ButtonGroupBox;
        private System.Windows.Forms.CheckBox HideHydraulicBoundaryDatabaseColumnCheckBox;
        private System.Windows.Forms.ErrorProvider CalculateForSelectedButtonErrorProvider;
        private System.Windows.Forms.SplitContainer splitContainer;
        private IllustrationPointsControl illustrationPointsControl;
    }
}