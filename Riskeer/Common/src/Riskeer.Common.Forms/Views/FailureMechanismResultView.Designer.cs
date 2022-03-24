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
using System.ComponentModel;
using System.Windows.Forms;

namespace Riskeer.Common.Forms.Views
{
    partial class FailureMechanismResultView<TSectionResult, TSectionResultRow, TFailureMechanism>
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
            this.TableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.failureMechanismAssemblyLabel = new System.Windows.Forms.Label();
            this.probabilityResultTypeComboBox = new System.Windows.Forms.ComboBox();
            this.failureMechanismAssemblyProbabilityTextBox = new System.Windows.Forms.TextBox();
            this.DataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.TableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // TableLayoutPanel
            // 
            this.TableLayoutPanel.ColumnCount = 4;
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.AutoSize));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 150F));
            this.TableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 363F));
            this.TableLayoutPanel.Controls.Add(this.failureMechanismAssemblyLabel, 0, 0);
            this.TableLayoutPanel.Controls.Add(this.probabilityResultTypeComboBox, 1, 0);
            this.TableLayoutPanel.Controls.Add(this.failureMechanismAssemblyProbabilityTextBox, 2, 0);
            this.TableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.TableLayoutPanel.Location = new System.Drawing.Point(0, 42);
            this.TableLayoutPanel.Name = "TableLayoutPanel";
            this.TableLayoutPanel.RowCount = 1;
            this.TableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.TableLayoutPanel.Size = new System.Drawing.Size(764, 40);
            this.TableLayoutPanel.TabIndex = 1;
            // 
            // failureMechanismAssemblyLabel
            // 
            this.failureMechanismAssemblyLabel.AutoSize = true;
            this.failureMechanismAssemblyLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismAssemblyLabel.Location = new System.Drawing.Point(3, 0);
            this.failureMechanismAssemblyLabel.Name = "failureMechanismAssemblyLabel";
            this.failureMechanismAssemblyLabel.Size = new System.Drawing.Size(162, 40);
            this.failureMechanismAssemblyLabel.TabIndex = 2;
            this.failureMechanismAssemblyLabel.Text = global::Riskeer.Common.Forms.Properties.Resources.FailureMechanismResultView_FailureMechanismAssemblyLabel;
            this.failureMechanismAssemblyLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // probabilityResultTypeComboBox
            // 
            this.probabilityResultTypeComboBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.probabilityResultTypeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.probabilityResultTypeComboBox.FormattingEnabled = true;
            this.probabilityResultTypeComboBox.Location = new System.Drawing.Point(171, 10);
            this.probabilityResultTypeComboBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.probabilityResultTypeComboBox.Name = "probabilityResultTypeComboBox";
            this.probabilityResultTypeComboBox.Size = new System.Drawing.Size(150, 21);
            this.probabilityResultTypeComboBox.TabIndex = 3;
            this.probabilityResultTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.ProbabilityResultTypeComboBoxSelectedIndexChanged);
            // 
            // failureMechanismAssemblyProbabilityTextBox
            // 
            this.failureMechanismAssemblyProbabilityTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismAssemblyProbabilityTextBox.Location = new System.Drawing.Point(298, 10);
            this.failureMechanismAssemblyProbabilityTextBox.Margin = new System.Windows.Forms.Padding(3, 10, 3, 10);
            this.failureMechanismAssemblyProbabilityTextBox.Name = "failureMechanismAssemblyProbabilityTextBox";
            this.failureMechanismAssemblyProbabilityTextBox.Size = new System.Drawing.Size(100, 20);
            this.failureMechanismAssemblyProbabilityTextBox.KeyDown += new KeyEventHandler(this.FailureMechanismAssemblyProbabilityTextBoxKeyDown);
            this.failureMechanismAssemblyProbabilityTextBox.Leave += new EventHandler(this.FailureMechanismAssemblyProbabilityTextBoxLeave);
            this.failureMechanismAssemblyProbabilityTextBox.TabIndex = 4;
            // 
            // DataGridViewControl
            // 
            this.DataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridViewControl.Location = new System.Drawing.Point(0, 0);
            this.DataGridViewControl.MultiSelect = true;
            this.DataGridViewControl.Name = "DataGridViewControl";
            this.DataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.DataGridViewControl.Size = new System.Drawing.Size(764, 42);
            this.DataGridViewControl.TabIndex = 0;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            this.errorProvider.Icon = global::Riskeer.Common.Forms.Properties.Resources.ErrorIcon;
            // 
            // FailureMechanismResultView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.AutoScrollMinSize = new System.Drawing.Size(500, 0);
            this.Controls.Add(this.DataGridViewControl);
            this.Controls.Add(this.TableLayoutPanel);
            this.errorProvider.SetIconPadding(this, 10);
            this.Name = "FailureMechanismResultView";
            this.Size = new System.Drawing.Size(764, 82);
            this.TableLayoutPanel.ResumeLayout(false);
            this.TableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        protected Core.Common.Controls.DataGrid.DataGridViewControl DataGridViewControl;
        protected System.Windows.Forms.TableLayoutPanel TableLayoutPanel;
        private System.Windows.Forms.Label failureMechanismAssemblyLabel;
        private System.Windows.Forms.ComboBox probabilityResultTypeComboBox;
        private System.Windows.Forms.TextBox failureMechanismAssemblyProbabilityTextBox;
        private ErrorProvider errorProvider;
    }
}
