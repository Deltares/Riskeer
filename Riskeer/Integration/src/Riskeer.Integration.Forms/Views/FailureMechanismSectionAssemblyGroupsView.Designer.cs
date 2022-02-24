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

using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.Forms;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Integration.Forms.Views
{
    partial class FailureMechanismSectionAssemblyGroupsView
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
            this.assemblyGroupsTable = new AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup>();
            this.groupBoxPanel = new System.Windows.Forms.Panel();
            this.groupBoxPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // failureMechanismAssemblyCategoriesTable
            // 
            this.assemblyGroupsTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.assemblyGroupsTable.MultiSelect = true;
            this.assemblyGroupsTable.Name = "assemblyGroupsTable";
            this.assemblyGroupsTable.Padding = new System.Windows.Forms.Padding(5);
            this.assemblyGroupsTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            //
            // groupBoxPanel
            //
            this.groupBoxPanel.Controls.Add(this.assemblyGroupsTable);
            this.groupBoxPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBoxPanel.Name = "groupBoxPanel";
            this.groupBoxPanel.Padding = new System.Windows.Forms.Padding(3);
            // 
            // FailureMechanismSectionAssemblyGroupsView
            // 
            this.Name = "FailureMechanismSectionAssemblyGroupsView";
            this.Controls.Add(this.groupBoxPanel);
            this.Size = new System.Drawing.Size(750, 420);
            this.AutoScrollMinSize = new System.Drawing.Size(400, 100);
            this.groupBoxPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private AssemblyGroupsTable<DisplayFailureMechanismSectionAssemblyGroup> assemblyGroupsTable;
        private System.Windows.Forms.Panel groupBoxPanel;
    }
}
