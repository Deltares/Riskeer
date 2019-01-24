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

using Riskeer.AssemblyTool.Data;

namespace Ringtoets.Integration.Forms.Views
{
    partial class AssemblyResultCategoriesView
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
            this.assemblyCategoriesTable = new Ringtoets.Integration.Forms.Views.AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup>();
            this.groupBoxPanel = new System.Windows.Forms.Panel();
            this.groupBoxPanel.SuspendLayout();
            this.groupBox = new System.Windows.Forms.GroupBox();
            this.groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // failureMechanismAssemblyCategoriesTable
            // 
            this.assemblyCategoriesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assemblyCategoriesTable.MultiSelect = true;
            this.assemblyCategoriesTable.Name = "assemblyCategoriesTable";
            this.assemblyCategoriesTable.Padding = new System.Windows.Forms.Padding(5);
            this.assemblyCategoriesTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            //
            // groupBoxPanel
            //
            this.groupBoxPanel.Controls.Add(this.groupBox);
            this.groupBoxPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPanel.Name = "groupBoxPanel";
            this.groupBoxPanel.Padding = new System.Windows.Forms.Padding(3);
            // 
            // groupBox
            // 
            this.groupBox.Controls.Add(assemblyCategoriesTable);
            this.groupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox.Location = new System.Drawing.Point(0, 0);
            this.groupBox.Margin = new System.Windows.Forms.Padding(5);
            this.groupBox.Name = "groupBox";
            this.groupBox.Size = new System.Drawing.Size(150, 150);
            this.groupBox.TabIndex = 0;
            this.groupBox.TabStop = false;
            this.groupBox.Text = global::Ringtoets.Integration.Forms.Properties.Resources.AssemblyResultCategories_DisplayName;
            // 
            // AssemblyResultCategoriesView
            // 
            this.Name = "AssemblyResultCategoriesView";
            this.Controls.Add(this.groupBoxPanel);
            this.Size = new System.Drawing.Size(750, 420);
            this.AutoScrollMinSize = new System.Drawing.Size(400, 100);
            this.groupBoxPanel.ResumeLayout(false);
            this.groupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Ringtoets.Integration.Forms.Views.AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> assemblyCategoriesTable;
        private System.Windows.Forms.GroupBox groupBox;
        private System.Windows.Forms.Panel groupBoxPanel;
    }
}
