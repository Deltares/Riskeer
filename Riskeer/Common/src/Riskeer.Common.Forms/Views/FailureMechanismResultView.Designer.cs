// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Common.Forms.Views
{
    partial class FailureMechanismResultView<TSectionResult, TSectionResultRow, TFailureMechanism, TAssemblyResultControl>
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
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.failureMechanismAssemblyLabel = new System.Windows.Forms.Label();
            this.infoIcon = new System.Windows.Forms.PictureBox();
            this.DataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // toolTip
            // 
            this.toolTip.AutoPopDelay = 5000;
            this.toolTip.InitialDelay = 100;
            this.toolTip.ReshowDelay = 100;
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.ColumnCount = 3;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this.TableLayoutPanel.Controls.Add(this.failureMechanismAssemblyLabel, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.infoIcon, 2, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(764, 30);
            this.TableLayoutPanel.TabIndex = 1;
            // 
            // failureMechanismAssemblyLabel
            // 
            this.failureMechanismAssemblyLabel.AutoSize = true;
            this.failureMechanismAssemblyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismAssemblyLabel.Location = new System.Drawing.Point(3, 0);
            this.failureMechanismAssemblyLabel.Name = "failureMechanismAssemblyLabel";
            this.failureMechanismAssemblyLabel.Size = new System.Drawing.Size(197, 30);
            this.failureMechanismAssemblyLabel.TabIndex = 2;
            this.failureMechanismAssemblyLabel.Text = global::Riskeer.Common.Forms.Properties.Resources.FailureMechanismResultView_FailureMechanismAssemblyLabel;
            this.failureMechanismAssemblyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // infoIcon
            // 
            this.infoIcon.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.infoIcon.Dock = System.Windows.Forms.DockStyle.Right;
            this.infoIcon.Location = new System.Drawing.Point(741, 7);
            this.infoIcon.Margin = new System.Windows.Forms.Padding(7);
            this.infoIcon.Name = "infoIcon";
            this.infoIcon.Size = new System.Drawing.Size(16, 16);
            this.infoIcon.TabIndex = 0;
            this.infoIcon.TabStop = false;
            // 
            // DataGridViewControl
            // 
            this.DataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridViewControl.Location = new System.Drawing.Point(0, 30);
            this.DataGridViewControl.MultiSelect = true;
            this.DataGridViewControl.Name = "DataGridViewControl";
            this.DataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.DataGridViewControl.Size = new System.Drawing.Size(764, 52);
            this.DataGridViewControl.TabIndex = 0;
            // 
            // FailureMechanismResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(500, 0);
            this.Controls.Add(this.DataGridViewControl);
            this.Controls.Add(this.TableLayoutPanel);
            this.Name = "FailureMechanismResultView";
            this.Size = new System.Drawing.Size(764, 82);
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.infoIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl DataGridViewControl;
        private System.Windows.Forms.ToolTip toolTip;
        protected System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.PictureBox infoIcon;
        private System.Windows.Forms.Label failureMechanismAssemblyLabel;
    }
}
