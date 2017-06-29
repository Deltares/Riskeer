// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Drawing;

namespace Ringtoets.Common.Forms.Views
{
    partial class LocationsView<T>
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
            this.CalculateForSelectedButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.CalculateForSelectedButtonErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.verticalSplitContainer = new System.Windows.Forms.SplitContainer();
            this.horizontalSplitContainer = new System.Windows.Forms.SplitContainer();
            this.illustrationPointsChartControl = new Ringtoets.Common.Forms.Views.IllustrationPointsChartControl();
            this.ButtonGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.CalculateForSelectedButtonErrorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.verticalSplitContainer)).BeginInit();
            this.verticalSplitContainer.Panel1.SuspendLayout();
            this.verticalSplitContainer.Panel2.SuspendLayout();
            this.verticalSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.horizontalSplitContainer)).BeginInit();
            this.horizontalSplitContainer.Panel1.SuspendLayout();
            this.horizontalSplitContainer.SuspendLayout();
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
            this.dataGridViewControl.Size = new System.Drawing.Size(443, 480);
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
            this.ButtonGroupBox.Location = new System.Drawing.Point(0, 480);
            this.ButtonGroupBox.MinimumSize = new System.Drawing.Size(445, 61);
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.Size = new System.Drawing.Size(445, 61);
            this.ButtonGroupBox.TabIndex = 3;
            this.ButtonGroupBox.TabStop = false;
            // 
            // CalculateForSelectedButtonErrorProvider
            // 
            this.CalculateForSelectedButtonErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.CalculateForSelectedButtonErrorProvider.ContainerControl = this;
            this.CalculateForSelectedButtonErrorProvider.Icon = global::Ringtoets.Common.Forms.Properties.Resources.warning;
            // 
            // verticalSplitContainer
            // 
            this.verticalSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.verticalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.verticalSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.verticalSplitContainer.Name = "verticalSplitContainer";
            // 
            // verticalSplitContainer.Panel1
            // 
            this.verticalSplitContainer.Panel1.AutoScroll = true;
            this.verticalSplitContainer.Panel1.AutoScrollMinSize = new System.Drawing.Size(440, 0);
            this.verticalSplitContainer.Panel1.Controls.Add(this.dataGridViewControl);
            this.verticalSplitContainer.Panel1.Controls.Add(this.ButtonGroupBox);
            // 
            // verticalSplitContainer.Panel2
            // 
            this.verticalSplitContainer.Panel2.Controls.Add(this.horizontalSplitContainer);
            this.verticalSplitContainer.Size = new System.Drawing.Size(822, 543);
            this.verticalSplitContainer.SplitterDistance = 445;
            this.verticalSplitContainer.TabIndex = 1;
            // 
            // horizontalSplitContainer
            // 
            this.horizontalSplitContainer.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.horizontalSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.horizontalSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.horizontalSplitContainer.Name = "horizontalSplitContainer";
            this.horizontalSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // horizontalSplitContainer.Panel1
            // 
            this.horizontalSplitContainer.Panel1.Controls.Add(this.illustrationPointsChartControl);
            this.horizontalSplitContainer.Size = new System.Drawing.Size(373, 543);
            this.horizontalSplitContainer.SplitterDistance = 261;
            this.horizontalSplitContainer.TabIndex = 0;
            // 
            // illustrationPointsChartControl
            // 
            this.illustrationPointsChartControl.Data = null;
            this.illustrationPointsChartControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.illustrationPointsChartControl.Location = new System.Drawing.Point(0, 0);
            this.illustrationPointsChartControl.Name = "illustrationPointsChartControl";
            this.illustrationPointsChartControl.Size = new System.Drawing.Size(371, 259);
            this.illustrationPointsChartControl.TabIndex = 0;
            // 
            // LocationsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(526, 85);
            this.Controls.Add(this.verticalSplitContainer);
            this.Name = "LocationsView";
            this.Size = new System.Drawing.Size(822, 543);
            this.ButtonGroupBox.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.CalculateForSelectedButtonErrorProvider)).EndInit();
            this.verticalSplitContainer.Panel1.ResumeLayout(false);
            this.verticalSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.verticalSplitContainer)).EndInit();
            this.verticalSplitContainer.ResumeLayout(false);
            this.horizontalSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.horizontalSplitContainer)).EndInit();
            this.horizontalSplitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button CalculateForSelectedButton;
        private System.Windows.Forms.Button DeselectAllButton;
        private System.Windows.Forms.Button SelectAllButton;
        protected System.Windows.Forms.GroupBox ButtonGroupBox;
        private System.Windows.Forms.ErrorProvider CalculateForSelectedButtonErrorProvider;
        private System.Windows.Forms.SplitContainer verticalSplitContainer;
        private System.Windows.Forms.SplitContainer horizontalSplitContainer;
        private IllustrationPointsChartControl illustrationPointsChartControl;
    }
}