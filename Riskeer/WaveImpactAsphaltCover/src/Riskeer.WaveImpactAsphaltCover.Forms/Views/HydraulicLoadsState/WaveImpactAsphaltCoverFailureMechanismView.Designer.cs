﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

namespace Riskeer.WaveImpactAsphaltCover.Forms.Views.HydraulicLoadsState
{
    partial class WaveImpactAsphaltCoverFailureMechanismView
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
            this.riskeerMapControl = new Riskeer.Common.Forms.Views.RiskeerMapControl();
            this.SuspendLayout();
            // 
            // riskeerMapControl
            // 
            this.riskeerMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.riskeerMapControl.Location = new System.Drawing.Point(0, 0);
            this.riskeerMapControl.Name = "riskeerMapControl";
            this.riskeerMapControl.Size = new System.Drawing.Size(150, 150);
            this.riskeerMapControl.TabIndex = 0;
            // 
            // WaveImpactAsphaltCoverFailureMechanismView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.riskeerMapControl);
            this.Name = "WaveImpactAsphaltCoverFailureMechanismView";
            this.ResumeLayout(false);

        }

        #endregion

        private Common.Forms.Views.RiskeerMapControl riskeerMapControl;
    }
}
