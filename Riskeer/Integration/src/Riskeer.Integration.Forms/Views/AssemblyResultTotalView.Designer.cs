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

using System.Windows.Forms;
using Riskeer.Integration.Forms.Controls;

namespace Riskeer.Integration.Forms.Views
{
    partial class AssemblyResultTotalView
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
            this.refreshAssemblyResultsButton = new Core.Common.Controls.Forms.EnhancedButton();
            this.assessmentSectionAssemblyControl = new AssessmentSectionAssemblyResultControl();
            this.refreshButtonPanel = new System.Windows.Forms.Panel();
            this.warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.refreshButtonPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 145);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(789, 271);
            this.dataGridViewControl.TabIndex = 3;
            // 
            // refreshAssemblyResultsButton
            // 
            this.refreshAssemblyResultsButton.AutoSize = true;
            this.refreshAssemblyResultsButton.Dock = System.Windows.Forms.DockStyle.Left;
            this.refreshAssemblyResultsButton.Enabled = false;
            this.refreshAssemblyResultsButton.Location = new System.Drawing.Point(5, 5);
            this.refreshAssemblyResultsButton.Name = "refreshAssemblyResultsButton";
            this.refreshAssemblyResultsButton.Size = new System.Drawing.Size(164, 25);
            this.refreshAssemblyResultsButton.TabIndex = 0;
            this.refreshAssemblyResultsButton.Text = global::Riskeer.Integration.Forms.Properties.Resources.RefreshAssemblyResultsButton_Text;
            this.refreshAssemblyResultsButton.UseVisualStyleBackColor = true;
            this.refreshAssemblyResultsButton.Click += new System.EventHandler(this.RefreshAssemblyResults_Click);
            // 
            // assessmentSectionAssemblyControl
            // 
            this.assessmentSectionAssemblyControl.AutoSize = true;
            this.assessmentSectionAssemblyControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.assessmentSectionAssemblyControl.Location = new System.Drawing.Point(90, 3);
            this.assessmentSectionAssemblyControl.Name = "assessmentSectionAssemblyControl";
            this.assessmentSectionAssemblyControl.Size = new System.Drawing.Size(690, 24);
            this.assessmentSectionAssemblyControl.TabIndex = 3;
            // 
            // refreshButtonPanel
            // 
            this.refreshButtonPanel.Controls.Add(this.refreshAssemblyResultsButton);
            this.refreshButtonPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.refreshButtonPanel.Location = new System.Drawing.Point(0, 0);
            this.refreshButtonPanel.Name = "refreshButtonPanel";
            this.refreshButtonPanel.Padding = new System.Windows.Forms.Padding(5);
            this.refreshButtonPanel.Size = new System.Drawing.Size(789, 35);
            this.refreshButtonPanel.TabIndex = 1;
            // 
            // warningProvider
            // 
            this.warningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.warningProvider.ContainerControl = this;
            this.warningProvider.Icon = Core.Gui.Properties.Resources.warning;
            this.warningProvider.SetIconPadding(this.refreshAssemblyResultsButton, 4);
            // 
            // AssemblyResultTotalView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScrollMinSize = new System.Drawing.Size(350, 250);
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.assessmentSectionAssemblyControl);
            this.Controls.Add(this.refreshButtonPanel);
            this.Name = "AssemblyResultTotalView";
            this.Size = new System.Drawing.Size(789, 416);
            this.refreshButtonPanel.ResumeLayout(false);
            this.refreshButtonPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private Core.Common.Controls.Forms.EnhancedButton refreshAssemblyResultsButton;
        private Controls.AssessmentSectionAssemblyResultControl assessmentSectionAssemblyControl;
        private System.Windows.Forms.Panel refreshButtonPanel;
        private System.Windows.Forms.ErrorProvider warningProvider;
    }
}
