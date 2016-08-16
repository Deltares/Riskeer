// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Core.Common.Controls.DataGrid;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    partial class GrassCoverErosionInwardsScenariosView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        private DataGridViewControl dataGridViewControl;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.dataGridViewControl = new Core.Common.Controls.DataGrid.DataGridViewControl();
            this.SuspendLayout();
            // 
            // dataGridViewControl
            // 
            this.dataGridViewControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewControl.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewControl.Margin = new System.Windows.Forms.Padding(5);
            this.dataGridViewControl.MinimumSize = new System.Drawing.Size(667, 369);
            this.dataGridViewControl.Name = "dataGridViewControl";
            this.dataGridViewControl.Size = new System.Drawing.Size(667, 555);
            this.dataGridViewControl.TabIndex = 0;
            // 
            // GrassCoverErosionInwardsScenariosView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewControl);
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "GrassCoverErosionInwardsScenariosView";
            this.Size = new System.Drawing.Size(664, 555);
            this.ResumeLayout(false);

        }

        #endregion
    }
}
