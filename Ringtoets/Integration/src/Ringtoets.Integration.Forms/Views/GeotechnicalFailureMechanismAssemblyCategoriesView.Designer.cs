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

using Ringtoets.AssemblyTool.Data;

namespace Ringtoets.Integration.Forms.Views
{
    partial class GeotechnicalFailureMechanismAssemblyCategoriesView
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
            this.failureMechanismSectionAssemblyCategoriesTable = new Ringtoets.Integration.Forms.Views.AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup>();
            this.failureMechanismSectionAssemblyGroupBox = new System.Windows.Forms.GroupBox();
            this.groupBoxPanel = new System.Windows.Forms.Panel();
            this.groupBoxPanel.SuspendLayout();
            this.failureMechanismSectionAssemblyGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // failureMechanismSectionAssemblyCategoriesTable
            // 
            this.failureMechanismSectionAssemblyCategoriesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismSectionAssemblyCategoriesTable.MultiSelect = true;
            this.failureMechanismSectionAssemblyCategoriesTable.Name = "failureMechanismSectionAssemblyCategoriesTable";
            this.failureMechanismSectionAssemblyCategoriesTable.Padding = new System.Windows.Forms.Padding(5);
            this.failureMechanismSectionAssemblyCategoriesTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            //
            // groupBoxPanel
            //
            this.groupBoxPanel.Controls.Add(this.failureMechanismSectionAssemblyGroupBox);
            this.groupBoxPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPanel.Name = "groupBoxPanel";
            this.groupBoxPanel.Padding = new System.Windows.Forms.Padding(3);
            //
            // failureMechanismSectionAssemblyGroupBox
            // 
            this.failureMechanismSectionAssemblyGroupBox.Controls.Add(this.failureMechanismSectionAssemblyCategoriesTable);
            this.failureMechanismSectionAssemblyGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismSectionAssemblyGroupBox.Name = "failureMechanismSectionAssemblyGroupBox";
            this.failureMechanismSectionAssemblyGroupBox.Text = global::Ringtoets.Integration.Forms.Properties.Resources.FailureMechanismSectionAssemblyCategories_DisplayName;
            // 
            // FailureMechanismAssemblyCategoriesView
            // 
            this.Controls.Add(this.groupBoxPanel);
            this.Name = "GeotechnicalFailureMechanismAssemblyCategoriesView";
            this.Size = new System.Drawing.Size(750, 420);
            this.AutoScrollMinSize = new System.Drawing.Size(400, 100);
            this.groupBoxPanel.ResumeLayout(false);
            this.failureMechanismSectionAssemblyGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);
        }


        #endregion

        private Ringtoets.Integration.Forms.Views.AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionAssemblyCategoriesTable;
        private System.Windows.Forms.GroupBox failureMechanismSectionAssemblyGroupBox;
        private System.Windows.Forms.Panel groupBoxPanel;
    }
}
