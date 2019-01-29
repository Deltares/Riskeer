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

namespace Riskeer.Common.Forms.Views
{
    partial class GeneralResultFaultTreeIllustrationPointView
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
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.illustrationPointsControl = new Riskeer.Common.Forms.Views.IllustrationPointsControl();
            this.illustrationPointsFaultTreeControl = new Riskeer.Common.Forms.Views.IllustrationPointsFaultTreeControl();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 0);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.illustrationPointsControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.illustrationPointsFaultTreeControl);
            this.splitContainer.Size = new System.Drawing.Size(487, 357);
            this.splitContainer.SplitterDistance = 261;
            this.splitContainer.TabIndex = 0;
            // 
            // illustrationPointsControl
            // 
            this.illustrationPointsControl.Data = null;
            this.illustrationPointsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.illustrationPointsControl.Location = new System.Drawing.Point(0, 0);
            this.illustrationPointsControl.Name = "illustrationPointsControl";
            this.illustrationPointsControl.Size = new System.Drawing.Size(261, 357);
            this.illustrationPointsControl.TabIndex = 0;
            // 
            // illustrationPointsFaultTreeControl
            // 
            this.illustrationPointsFaultTreeControl.Data = null;
            this.illustrationPointsFaultTreeControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.illustrationPointsFaultTreeControl.Location = new System.Drawing.Point(0, 0);
            this.illustrationPointsFaultTreeControl.Name = "illustrationPointsFaultTreeControl";
            this.illustrationPointsFaultTreeControl.Size = new System.Drawing.Size(222, 357);
            this.illustrationPointsFaultTreeControl.TabIndex = 0;
            // 
            // GeneralResultFaultTreeIllustrationPointView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "GeneralResultFaultTreeIllustrationPointView";
            this.Size = new System.Drawing.Size(487, 357);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer;
        private IllustrationPointsControl illustrationPointsControl;
        private IllustrationPointsFaultTreeControl illustrationPointsFaultTreeControl;
    }
}
