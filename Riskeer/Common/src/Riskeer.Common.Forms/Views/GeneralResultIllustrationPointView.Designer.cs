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

namespace Riskeer.Common.Forms.Views
{
    partial class GeneralResultIllustrationPointView<TTopLevelIllustrationPoint>
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        protected IllustrationPointsControl IllustrationPointsControl => illustrationPointsControl;

        protected SplitContainer SplitContainer => splitContainer;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.illustrationPointsControl = new Riskeer.Common.Forms.Views.IllustrationPointsControl();
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
            this.splitContainer.Panel1.Controls.Add(this.IllustrationPointsControl);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Size = new System.Drawing.Size(487, 357);
            this.splitContainer.SplitterDistance = 261;
            this.splitContainer.TabIndex = 0;
            // 
            // illustrationPointsControl
            // 
            this.IllustrationPointsControl.Data = null;
            this.IllustrationPointsControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.IllustrationPointsControl.Location = new System.Drawing.Point(0, 0);
            this.IllustrationPointsControl.Name = "illustrationPointsControl";
            this.IllustrationPointsControl.Size = new System.Drawing.Size(261, 357);
            this.IllustrationPointsControl.TabIndex = 0;
            // 
            // GeneralResultFaultTreeIllustrationPointView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer);
            this.Name = "GeneralResultIllustrationPointView";
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
    }
}
