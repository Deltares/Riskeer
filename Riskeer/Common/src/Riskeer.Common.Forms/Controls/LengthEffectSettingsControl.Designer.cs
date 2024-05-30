// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
    partial class LengthEffectSettingsControl
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
            this.lengthEffectErrorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.parameterAToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.lengthEffectNRoundedToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.tableLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize) (this.lengthEffectErrorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.AutoSize = true;
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel.Controls.Add(this.parameterALabel, 0, 0);
            this.tableLayoutPanel.Controls.Add(this.lengthEffectNRoundedLabel, 0, 1);
            this.tableLayoutPanel.Controls.Add(this.lengthEffectNRoundedTextBox, 1, 1);
            this.tableLayoutPanel.Controls.Add(this.parameterATextBox, 1, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 2;
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
            this.parameterALabel.Size = new System.Drawing.Size(166, 20);
            this.parameterALabel.TabIndex = 0;
            this.parameterALabel.Text = global::Riskeer.Common.Forms.Properties.Resources.Parameter_A_DisplayName;
            this.parameterALabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectNRoundedLabel
            // 
            this.lengthEffectNRoundedLabel.AutoSize = true;
            this.lengthEffectNRoundedLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lengthEffectNRoundedLabel.Location = new System.Drawing.Point(0, 29);
            this.lengthEffectNRoundedLabel.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lengthEffectNRoundedLabel.Name = "lengthEffectNRoundedLabel";
            this.lengthEffectNRoundedLabel.Size = new System.Drawing.Size(166, 118);
            this.lengthEffectNRoundedLabel.TabIndex = 1;
            this.lengthEffectNRoundedLabel.Text = global::Riskeer.Common.Forms.Properties.Resources.LengthEffect_RoundedNSection_DisplayName;
            this.lengthEffectNRoundedLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lengthEffectNRoundedTextBox
            // 
            this.lengthEffectNRoundedTextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.lengthEffectNRoundedTextBox.Enabled = false;
            this.lengthEffectNRoundedTextBox.Location = new System.Drawing.Point(172, 29);
            this.lengthEffectNRoundedTextBox.Name = "lengthEffectNRoundedTextBox";
            this.lengthEffectNRoundedTextBox.Size = new System.Drawing.Size(69, 20);
            this.lengthEffectNRoundedTextBox.TabIndex = 2;
            // 
            // parameterATextBox
            // 
            this.parameterATextBox.Dock = System.Windows.Forms.DockStyle.Left;
            this.parameterATextBox.Enabled = false;
            this.parameterATextBox.Location = new System.Drawing.Point(172, 3);
            this.parameterATextBox.Name = "parameterATextBox";
            this.parameterATextBox.Size = new System.Drawing.Size(69, 20);
            this.parameterATextBox.TabIndex = 3;
            this.parameterATextBox.KeyDown += ParameterATextBoxKeyDown;
            this.parameterATextBox.Leave += ParameterATextBoxLeave;
            // 
            // lengthEffectErrorProvider
            // 
            this.lengthEffectErrorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.lengthEffectErrorProvider.ContainerControl = this;
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
            // LengthEffectSettingsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "LengthEffectSettingsControl";
            this.Size = new System.Drawing.Size(496, 150);
            this.tableLayoutPanel.ResumeLayout(false);
            this.tableLayoutPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize) (this.lengthEffectErrorProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private System.Windows.Forms.ToolTip parameterAToolTip;
        private System.Windows.Forms.ToolTip lengthEffectNRoundedToolTip;

        private System.Windows.Forms.Label parameterALabel;
        private System.Windows.Forms.Label lengthEffectNRoundedLabel;
        private System.Windows.Forms.TextBox lengthEffectNRoundedTextBox;
        private System.Windows.Forms.TextBox parameterATextBox;

        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.ErrorProvider lengthEffectErrorProvider;

        #endregion
    }
}