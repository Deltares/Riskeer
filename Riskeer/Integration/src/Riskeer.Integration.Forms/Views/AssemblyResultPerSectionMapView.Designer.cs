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

namespace Riskeer.Integration.Forms.Views
{
    partial class AssemblyResultPerSectionMapView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AssemblyResultPerSectionMapView));
            this.ringtoetsMapControl = new Riskeer.Common.Forms.Views.RingtoetsMapControl();
            this.warningPanel = new System.Windows.Forms.Panel();
            this.warningIcon = new System.Windows.Forms.PictureBox();
            this.warningText = new System.Windows.Forms.Label();
            this.warningPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningIcon)).BeginInit();
            this.SuspendLayout();
            // 
            // ringtoetsMapControl
            // 
            this.ringtoetsMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ringtoetsMapControl.Location = new System.Drawing.Point(0, 0);
            this.ringtoetsMapControl.Margin = new System.Windows.Forms.Padding(0);
            this.ringtoetsMapControl.Name = "ringtoetsMapControl";
            this.ringtoetsMapControl.Size = new System.Drawing.Size(562, 150);
            this.ringtoetsMapControl.TabIndex = 0;
            // 
            // warningPanel
            // 
            this.warningPanel.BackColor = System.Drawing.SystemColors.Info;
            this.warningPanel.Controls.Add(this.warningText);
            this.warningPanel.Controls.Add(this.warningIcon);
            this.warningPanel.Location = new System.Drawing.Point(0, 25);
            this.warningPanel.Name = "warningPanel";
            this.warningPanel.Size = new System.Drawing.Size(562, 100);
            this.warningPanel.TabIndex = 1;
            this.warningPanel.AutoSize = true;
            this.warningPanel.Dock = DockStyle.Top;
            // 
            // warningIcon
            // 
            this.warningIcon.Image = global::Riskeer.Common.Forms.Properties.Resources.PencilWarning.ToBitmap();
            this.warningIcon.Location = new System.Drawing.Point(4, 4);
            this.warningIcon.MaximumSize = new System.Drawing.Size(16, 16);
            this.warningIcon.Name = "warningIcon";
            this.warningIcon.Size = new System.Drawing.Size(16, 16);
            this.warningIcon.TabIndex = 0;
            this.warningIcon.TabStop = false;
            this.warningIcon.SizeMode = PictureBoxSizeMode.StretchImage;
            this.warningIcon.Dock = DockStyle.Left;
            // 
            // warningText
            // 
            this.warningText.AutoSize = true;
            this.warningText.Location = new System.Drawing.Point(27, 6);
            this.warningText.Name = "warningText";
            this.warningText.Size = new System.Drawing.Size(35, 13);
            this.warningText.TabIndex = 1;
            this.warningText.Text = global::Riskeer.Common.Forms.Properties.Resources.ManualAssemblyWarning_FailureMechanismAssemblyResult_is_based_on_manual_assemblies;
            // 
            // AssemblyResultPerSectionMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ringtoetsMapControl);
            this.Controls.Add(this.warningPanel);
            this.Name = "AssemblyResultPerSectionMapView";
            this.Size = new System.Drawing.Size(562, 150);
            this.warningPanel.ResumeLayout(false);
            this.warningPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.warningIcon)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private Riskeer.Common.Forms.Views.RingtoetsMapControl ringtoetsMapControl;
        private Panel warningPanel;
        private PictureBox warningIcon;
        private Label warningText;
    }
}
