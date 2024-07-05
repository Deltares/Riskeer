// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

using System.ComponentModel;
using System.Windows.Forms;

namespace Riskeer.Common.Forms.Controls
{
    partial class FailureMechanismSectionConfigurationControl
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
            this.components = new System.ComponentModel.Container();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.parameterALabel = new System.Windows.Forms.Label();
            this.lengthEffectNRoundedLabel = new System.Windows.Forms.Label();
            this.lengthEffectNRoundedTextBox = new System.Windows.Forms.TextBox();
            this.parameterATextBox = new System.Windows.Forms.TextBox();
            this.parameterBLabel = new System.Windows.Forms.Label();
            this.parameterBTextBox = new System.Windows.Forms.TextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.parameterAToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lengthEffectNRoundedToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.parameterBToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.parameterALabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lengthEffectNRoundedLabel, 0, 2);
            this.tableLayoutPanel.Controls.Add(this.lengthEffectNRoundedTextBox, 1, 2);
            this.tableLayoutPanel.Controls.Add(this.parameterATextBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.parameterBLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.parameterBTextBox, 1, 1);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 3;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(496, 150);
            this.tableLayoutPanel.TabIndex = 0;
            // 
            // parameterALabel
            // 
            this.parameterALabel.AutoSize = true;
            this.parameterALabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterALabel.Location = new System.Drawing.Point(0, 3);
            this.parameterALabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.parameterALabel.Name = "parameterALabel";
            this.parameterALabel.Size = new System.Drawing.Size(193, 20);
            this.parameterALabel.TabIndex = 0;
            this.parameterALabel.Text = global::Riskeer.Common.Forms.Properties.Resources.FailureMechanismSectionConfigurationControl_Parameter_A_DisplayName;
            this.parameterALabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectNRoundedLabel
            // 
            this.lengthEffectNRoundedLabel.AutoSize = true;
            this.lengthEffectNRoundedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectNRoundedLabel.Location = new System.Drawing.Point(0, 55);
            this.lengthEffectNRoundedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lengthEffectNRoundedLabel.Name = "lengthEffectNRoundedLabel";
            this.lengthEffectNRoundedLabel.Size = new System.Drawing.Size(193, 92);
            this.lengthEffectNRoundedLabel.TabIndex = 1;
            this.lengthEffectNRoundedLabel.Text = global::Riskeer.Common.Forms.Properties.Resources.FailureMechanismSectionConfigurationControl_LengthEffectNRounded_DisplayName;
            this.lengthEffectNRoundedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectNRoundedTextBox
            // 
            this.lengthEffectNRoundedTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.lengthEffectNRoundedTextBox.Enabled = false;
            this.lengthEffectNRoundedTextBox.Location = new System.Drawing.Point(199, 55);
            this.lengthEffectNRoundedTextBox.Name = "lengthEffectNRoundedTextBox";
            this.lengthEffectNRoundedTextBox.Size = new System.Drawing.Size(69, 20);
            this.lengthEffectNRoundedTextBox.TabIndex = 2;
            // 
            // parameterATextBox
            // 
            this.parameterATextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.parameterATextBox.Enabled = false;
            this.parameterATextBox.Location = new System.Drawing.Point(199, 3);
            this.parameterATextBox.Name = "parameterATextBox";
            this.parameterATextBox.Size = new System.Drawing.Size(69, 20);
            this.parameterATextBox.TabIndex = 3;
            // 
            // parameterBLabel
            // 
            this.parameterBLabel.AutoSize = true;
            this.parameterBLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.parameterBLabel.Location = new System.Drawing.Point(0, 29);
            this.parameterBLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.parameterBLabel.Name = "parameterBLabel";
            this.parameterBLabel.Size = new System.Drawing.Size(193, 20);
            this.parameterBLabel.TabIndex = 4;
            this.parameterBLabel.Text = global::Riskeer.Common.Forms.Properties.Resources.FailureMechanism_GeneralInput_B_DisplayName;
            this.parameterBLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // parameterBTextBox
            // 
            this.parameterBTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.parameterBTextBox.Enabled = false;
            this.parameterBTextBox.Location = new System.Drawing.Point(199, 29);
            this.parameterBTextBox.Name = "parameterBTextBox";
            this.parameterBTextBox.Size = new System.Drawing.Size(69, 20);
            this.parameterBTextBox.TabIndex = 5;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // parameterAToolTip
            // 
            this.parameterAToolTip.AutoPopDelay = 5000;
            this.parameterAToolTip.InitialDelay = 100;
            this.parameterAToolTip.ReshowDelay = 100;
            // 
            // lengthEffectNRoundedToolTip
            // 
            this.lengthEffectNRoundedToolTip.AutoPopDelay = 5000;
            this.lengthEffectNRoundedToolTip.InitialDelay = 100;
            this.lengthEffectNRoundedToolTip.ReshowDelay = 100;
            // 
            // parameterBToolTip
            // 
            this.parameterBToolTip.AutoPopDelay = 5000;
            this.parameterBToolTip.InitialDelay = 100;
            this.parameterBToolTip.ReshowDelay = 100;
            // 
            // FailureMechanismSectionConfigurationControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "FailureMechanismSectionConfigurationControl";
            this.Size = new System.Drawing.Size(496, 150);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.errorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolTip parameterBToolTip;

        private System.Windows.Forms.TextBox parameterBTextBox;

        private System.Windows.Forms.Label parameterBLabel;

        private System.Windows.Forms.ToolTip parameterAToolTip;
        private System.Windows.Forms.ToolTip lengthEffectNRoundedToolTip;

        private System.Windows.Forms.Label parameterALabel;
        private System.Windows.Forms.Label lengthEffectNRoundedLabel;
        private System.Windows.Forms.TextBox lengthEffectNRoundedTextBox;
        private System.Windows.Forms.TextBox parameterATextBox;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ErrorProvider errorProvider;

        #endregion
    }
}