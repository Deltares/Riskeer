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

namespace Ringtoets.Integration.Forms.Views
{
    partial class AssemblyResultPerSectionView
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
            this.refreshAssemblyResultsButton = new System.Windows.Forms.Button();
            this.buttonGroupBox = new System.Windows.Forms.GroupBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.warningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.manualAssemblyWarningProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.buttonGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.manualAssemblyWarningProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 43);
            this.dataGridViewControl.MultiSelect = true;
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            this.dataGridViewControl.Size = new System.Drawing.Size(789, 373);
            this.dataGridViewControl.TabIndex = 3;
            // 
            // refreshAssemblyResultsButton
            // 
            this.refreshAssemblyResultsButton.AutoSize = true;
            this.refreshAssemblyResultsButton.Enabled = false;
            this.refreshAssemblyResultsButton.Location = new System.Drawing.Point(3, 14);
            this.refreshAssemblyResultsButton.Name = "refreshAssemblyResultsButton";
            this.refreshAssemblyResultsButton.Size = new System.Drawing.Size(164, 23);
            this.refreshAssemblyResultsButton.TabIndex = 0;
            this.refreshAssemblyResultsButton.Text = global::Ringtoets.Integration.Forms.Properties.Resources.RefreshAssemblyResultsButton_Text;
            this.refreshAssemblyResultsButton.UseVisualStyleBackColor = true;
            this.refreshAssemblyResultsButton.Click += new System.EventHandler(this.RefreshAssemblyResults_Click);
            // 
            // buttonGroupBox
            // 
            this.buttonGroupBox.Controls.Add(this.refreshAssemblyResultsButton);
            this.buttonGroupBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.buttonGroupBox.Location = new System.Drawing.Point(0, 0);
            this.buttonGroupBox.MinimumSize = new System.Drawing.Size(180, 43);
            this.buttonGroupBox.Name = "buttonGroupBox";
            this.buttonGroupBox.Size = new System.Drawing.Size(789, 43);
            this.buttonGroupBox.TabIndex = 2;
            this.buttonGroupBox.TabStop = false;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            this.errorProvider.Icon = global::Ringtoets.Common.Forms.Properties.Resources.ErrorIcon;
            this.errorProvider.SetIconPadding(this.refreshAssemblyResultsButton, 4);
            // 
            // warningProvider
            // 
            this.warningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.warningProvider.ContainerControl = this;
            this.warningProvider.Icon = global::Ringtoets.Common.Forms.Properties.Resources.warning;
            this.warningProvider.SetIconPadding(this.refreshAssemblyResultsButton, 4);
            // 
            // manualAssemblyWarningProvider
            // 
            this.manualAssemblyWarningProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.manualAssemblyWarningProvider.ContainerControl = this;
            this.manualAssemblyWarningProvider.Icon = global::Ringtoets.Common.Forms.Properties.Resources.PencilWarning;
            this.manualAssemblyWarningProvider.SetIconPadding(this.refreshAssemblyResultsButton, 4);
            // 
            // AssemblyResultPerSectionView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScrollMinSize = new System.Drawing.Size(300, 250);
            this.Controls.Add(this.dataGridViewControl);
            this.Controls.Add(this.buttonGroupBox);
            this.Name = "AssemblyResultPerSectionView";
            this.Size = new System.Drawing.Size(789, 416);
            this.buttonGroupBox.ResumeLayout(false);
            this.buttonGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.warningProvider)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.manualAssemblyWarningProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Core.Common.Controls.DataGrid.DataGridViewControl dataGridViewControl;
        private System.Windows.Forms.Button refreshAssemblyResultsButton;
        private System.Windows.Forms.GroupBox buttonGroupBox;
        private System.Windows.Forms.ErrorProvider errorProvider;
        private System.Windows.Forms.ErrorProvider warningProvider;
        private System.Windows.Forms.ErrorProvider manualAssemblyWarningProvider;
    }
}
