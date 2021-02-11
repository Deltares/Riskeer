// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Riskeer.Piping.Primitives;

namespace Riskeer.Piping.Forms
{
    partial class PipingSurfaceLineSelectionDialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        protected System.ComponentModel.IContainer components = null;

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
        
        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.DataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.ButtonGroupBox = new System.Windows.Forms.GroupBox();
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CheckBoxesPanel = new System.Windows.Forms.Panel();
            this.ProbabilisticCheckBox = new System.Windows.Forms.CheckBox();
            this.SemiProbabilisticCheckBox = new System.Windows.Forms.CheckBox();
            this.SelectDeselectPanel = new System.Windows.Forms.Panel();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.DeselectAllButton = new System.Windows.Forms.Button();
            this.DoForSelectedButton = new System.Windows.Forms.Button();
            this.CustomCancelButton = new System.Windows.Forms.Button();
            this.OkCancelButtonPanel = new System.Windows.Forms.Panel();
            this.ButtonGroupBox.SuspendLayout();
            this.TableLayoutPanel.SuspendLayout();
            this.CheckBoxesPanel.SuspendLayout();
            this.SelectDeselectPanel.SuspendLayout();
            this.OkCancelButtonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // DataGridViewControl
            // 
            this.DataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridViewControl.Location = new System.Drawing.Point(3, 58);
            this.DataGridViewControl.MultiSelect = true;
            this.DataGridViewControl.Name = "DataGridViewControl";
            this.DataGridViewControl.Padding = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.DataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.DataGridViewControl.Size = new System.Drawing.Size(261, 66);
            this.DataGridViewControl.TabIndex = 0;
            // 
            // ButtonGroupBox
            // 
            this.ButtonGroupBox.Controls.Add(this.TableLayoutPanel);
            this.ButtonGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ButtonGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ButtonGroupBox.Name = "ButtonGroupBox";
            this.ButtonGroupBox.Size = new System.Drawing.Size(354, 476);
            this.ButtonGroupBox.TabIndex = 7;
            this.ButtonGroupBox.TabStop = false;
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.AutoSize = true;
            this.TableLayoutPanel.ColumnCount = 1;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Controls.Add(this.CheckBoxesPanel, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.DataGridViewControl, 0, 1);
            this.TableLayoutPanel.Controls.Add(this.SelectDeselectPanel, 0, 2);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableLayoutPanel.Location = new System.Drawing.Point(3, 16);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 3;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 40F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(348, 457);
            this.TableLayoutPanel.TabIndex = 7;
            // 
            // CheckBoxesPanel
            // 
            this.CheckBoxesPanel.AutoSize = true;
            this.CheckBoxesPanel.Controls.Add(this.ProbabilisticCheckBox);
            this.CheckBoxesPanel.Controls.Add(this.SemiProbabilisticCheckBox);
            this.CheckBoxesPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.CheckBoxesPanel.Location = new System.Drawing.Point(3, 3);
            this.CheckBoxesPanel.Name = "CheckBoxesPanel";
            this.CheckBoxesPanel.Size = new System.Drawing.Size(342, 49);
            this.CheckBoxesPanel.TabIndex = 8;
            // 
            // ProbabilisticCheckBox
            // 
            this.ProbabilisticCheckBox.AutoSize = true;
            this.ProbabilisticCheckBox.Location = new System.Drawing.Point(4, 28);
            this.ProbabilisticCheckBox.Name = "ProbabilisticCheckBox";
            this.ProbabilisticCheckBox.Size = new System.Drawing.Size(80, 17);
            this.ProbabilisticCheckBox.TabIndex = 1;
            this.ProbabilisticCheckBox.UseVisualStyleBackColor = true;
            this.ProbabilisticCheckBox.CheckedChanged += new System.EventHandler(this.ProbabilisticCheckbox_CheckedChanged);
            // 
            // SemiProbabilisticCheckBox
            // 
            this.SemiProbabilisticCheckBox.AutoSize = true;
            this.SemiProbabilisticCheckBox.Location = new System.Drawing.Point(4, 4);
            this.SemiProbabilisticCheckBox.Name = "SemiProbabilisticCheckBox";
            this.SemiProbabilisticCheckBox.Size = new System.Drawing.Size(80, 17);
            this.SemiProbabilisticCheckBox.TabIndex = 0;
            this.SemiProbabilisticCheckBox.UseVisualStyleBackColor = true;
            this.SemiProbabilisticCheckBox.CheckedChanged += new System.EventHandler(this.SemiProbabilisticCheckbox_CheckedChanged);
            // 
            // SelectDeselectPanel
            // 
            this.SelectDeselectPanel.Controls.Add(this.SelectAllButton);
            this.SelectDeselectPanel.Controls.Add(this.DeselectAllButton);
            this.SelectDeselectPanel.Location = new System.Drawing.Point(3, 420);
            this.SelectDeselectPanel.Name = "SelectDeselectPanel";
            this.SelectDeselectPanel.Size = new System.Drawing.Size(342, 34);
            this.SelectDeselectPanel.TabIndex = 6;
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.SelectAllButton.Location = new System.Drawing.Point(9, 6);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(111, 23);
            this.SelectAllButton.TabIndex = 5;
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // DeselectAllButton
            // 
            this.DeselectAllButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DeselectAllButton.Location = new System.Drawing.Point(126, 6);
            this.DeselectAllButton.Name = "DeselectAllButton";
            this.DeselectAllButton.Size = new System.Drawing.Size(111, 23);
            this.DeselectAllButton.TabIndex = 4;
            this.DeselectAllButton.UseVisualStyleBackColor = true;
            this.DeselectAllButton.Click += new System.EventHandler(this.DeselectAllButton_Click);
            // 
            // DoForSelectedButton
            // 
            this.DoForSelectedButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.DoForSelectedButton.Enabled = false;
            this.DoForSelectedButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.DoForSelectedButton.Location = new System.Drawing.Point(167, 6);
            this.DoForSelectedButton.Name = "DoForSelectedButton";
            this.DoForSelectedButton.Size = new System.Drawing.Size(86, 23);
            this.DoForSelectedButton.TabIndex = 3;
            this.DoForSelectedButton.UseVisualStyleBackColor = true;
            this.DoForSelectedButton.Click += new System.EventHandler(this.DoForSelectedButton_Click);
            // 
            // CustomCancelButton
            // 
            this.CustomCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CustomCancelButton.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.CustomCancelButton.Location = new System.Drawing.Point(259, 6);
            this.CustomCancelButton.Name = "CustomCancelButton";
            this.CustomCancelButton.Size = new System.Drawing.Size(86, 23);
            this.CustomCancelButton.TabIndex = 6;
            this.CustomCancelButton.UseVisualStyleBackColor = true;
            this.CustomCancelButton.Click += new System.EventHandler(this.CustomCancelButton_Click);
            // 
            // OkCancelButtonPanel
            // 
            this.OkCancelButtonPanel.Controls.Add(this.DoForSelectedButton);
            this.OkCancelButtonPanel.Controls.Add(this.CustomCancelButton);
            this.OkCancelButtonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.OkCancelButtonPanel.Location = new System.Drawing.Point(0, 476);
            this.OkCancelButtonPanel.Name = "OkCancelButtonPanel";
            this.OkCancelButtonPanel.Size = new System.Drawing.Size(354, 35);
            this.OkCancelButtonPanel.TabIndex = 8;
            // 
            // PipingSurfaceLineSelectionDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(240, 90);
            this.ClientSize = new System.Drawing.Size(354, 511);
            this.Controls.Add(this.ButtonGroupBox);
            this.Controls.Add(this.OkCancelButtonPanel);
            this.MinimumSize = new System.Drawing.Size(370, 550);
            this.Name = "PipingSurfaceLineSelectionDialog";
            this.Text = "SelectionDialogBase";
            this.ButtonGroupBox.ResumeLayout(false);
            this.ButtonGroupBox.PerformLayout();
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            this.CheckBoxesPanel.ResumeLayout(false);
            this.CheckBoxesPanel.PerformLayout();
            this.SelectDeselectPanel.ResumeLayout(false);
            this.OkCancelButtonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        
        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl DataGridViewControl;
        private System.Windows.Forms.GroupBox ButtonGroupBox;
        private System.Windows.Forms.Panel SelectDeselectPanel;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.Button DeselectAllButton;
        private System.Windows.Forms.Button DoForSelectedButton;
        private System.Windows.Forms.Button CustomCancelButton;
        private System.Windows.Forms.Panel OkCancelButtonPanel;
        private TableLayoutPanel TableLayoutPanel;
        private Panel CheckBoxesPanel;
        private CheckBox ProbabilisticCheckBox;
        private CheckBox SemiProbabilisticCheckBox;
    }
}
