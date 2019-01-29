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

using System.Windows.Forms;
using Core.Common.Controls;

namespace Riskeer.Common.Forms.Controls
{
     abstract partial class AssemblyResultControl   
     {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GroupPanel = new System.Windows.Forms.TableLayoutPanel();
            this.GroupLabel = new Core.Common.Controls.BorderedLabel();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.manualAssemblyWarningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.GroupPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // GroupPanel
            // 
            this.GroupPanel.AutoSize = true;
            this.GroupPanel.ColumnCount = 1;
            this.GroupPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.GroupPanel.Controls.Add(this.GroupLabel, 0, 0);
            this.GroupPanel.Dock = System.Windows.Forms.DockStyle.Left;
            this.GroupPanel.Location = new System.Drawing.Point(0, 0);
            this.GroupPanel.Name = "GroupPanel";
            this.GroupPanel.RowCount = 1;
            this.GroupPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.GroupPanel.Size = new System.Drawing.Size(56, 796);
            this.GroupPanel.TabIndex = 0;
            // 
            // GroupLabel
            // 
            this.GroupLabel.AutoSize = true;
            this.GroupLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GroupLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GroupLabel.Location = new System.Drawing.Point(3, 3);
            this.GroupLabel.Margin = new System.Windows.Forms.Padding(3);
            this.GroupLabel.MinimumSize = new System.Drawing.Size(50, 2);
            this.GroupLabel.Name = "GroupLabel";
            this.GroupLabel.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.GroupLabel.Size = new System.Drawing.Size(50, 790);
            this.GroupLabel.TabIndex = 1;
            this.GroupLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            this.errorProvider.Icon = global::Riskeer.Common.Forms.Properties.Resources.ErrorIcon;
            // 
            // warningProvider
            // 
            this.manualAssemblyWarningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.manualAssemblyWarningProvider.ContainerControl = this;
            this.manualAssemblyWarningProvider.Icon = global::Riskeer.Common.Forms.Properties.Resources.PencilWarning;
            // 
            // AssemblyResultControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.GroupPanel);
            this.Name = "AssemblyResultControl";
            this.Size = new System.Drawing.Size(1361, 796);
            this.GroupPanel.ResumeLayout(false);
            this.GroupPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.manualAssemblyWarningProvider)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        protected System.Windows.Forms.TableLayoutPanel GroupPanel;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ErrorProvider manualAssemblyWarningProvider;
        protected BorderedLabel GroupLabel;
    }
}
