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

namespace Ringtoets.Integration.Forms.Views
{
    partial class AssemblyResultPerSectionMapView
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
            this.ringtoetsMapControl = new Ringtoets.Common.Forms.Views.RingtoetsMapControl();
            this.SuspendLayout();
            // 
            // ringtoetsMapControl1
            // 
            this.ringtoetsMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ringtoetsMapControl.Location = new System.Drawing.Point(0, 0);
            this.ringtoetsMapControl.Name = "ringtoetsMapControl";
            this.ringtoetsMapControl.Size = new System.Drawing.Size(150, 150);
            this.ringtoetsMapControl.TabIndex = 0;
            // 
            // AssemblyResultPerSectionMapView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ringtoetsMapControl);
            this.Name = "AssemblyResultPerSectionMapView";
            this.ResumeLayout(false);

        }

        #endregion

        private Common.Forms.Views.RingtoetsMapControl ringtoetsMapControl;
    }
}
