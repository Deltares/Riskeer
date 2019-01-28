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

namespace Riskeer.Integration.Forms.Views
{
    partial class FailureMechanismAssemblyCategoriesView
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
            this.failureMechanismSectionAssemblyCategoriesTable = new AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup>();
            this.failureMechanismAssemblyCategoriesTable = new AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup>();
            this.failureMechanismAssemblyGroupBox = new System.Windows.Forms.GroupBox();
            this.failureMechanismSectionAssemblyGroupBox = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
            this.failureMechanismAssemblyGroupBox.SuspendLayout();
            this.failureMechanismSectionAssemblyGroupBox.SuspendLayout();
            this.tableLayoutPanel.SuspendLayout();
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
            // failureMechanismAssemblyCategoriesTable
            // 
            this.failureMechanismAssemblyCategoriesTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismAssemblyCategoriesTable.MultiSelect = true;
            this.failureMechanismAssemblyCategoriesTable.Name = "failureMechanismAssemblyCategoriesTable";
            this.failureMechanismAssemblyCategoriesTable.Padding = new System.Windows.Forms.Padding(5);
            this.failureMechanismAssemblyCategoriesTable.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.RowHeaderSelect;
            // 
            // failureMechanismAssemblyGroupBox
            // 
            this.failureMechanismAssemblyGroupBox.Controls.Add(this.failureMechanismAssemblyCategoriesTable);
            this.failureMechanismAssemblyGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismAssemblyGroupBox.Name = "failureMechanismAssemblyGroupBox";
            this.failureMechanismAssemblyGroupBox.Text = global::Riskeer.Integration.Forms.Properties.Resources.FailureMechanismAssemblyCategories_DisplayName;
            // 
            // failureMechanismSectionAssemblyGroupBox
            // 
            this.failureMechanismSectionAssemblyGroupBox.Controls.Add(this.failureMechanismSectionAssemblyCategoriesTable);
            this.failureMechanismSectionAssemblyGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.failureMechanismSectionAssemblyGroupBox.Name = "failureMechanismSectionAssemblyGroupBox";
            this.failureMechanismSectionAssemblyGroupBox.Text = global::Riskeer.Integration.Forms.Properties.Resources.FailureMechanismSectionAssemblyCategories_DisplayName;
            // 
            // tableLayoutPanel
            // 
            this.tableLayoutPanel.ColumnCount = 2;
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel.Controls.Add(this.failureMechanismSectionAssemblyGroupBox, 1, 0);
            this.tableLayoutPanel.Controls.Add(this.failureMechanismAssemblyGroupBox, 0, 0);
            this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel.Name = "tableLayoutPanel";
            this.tableLayoutPanel.RowCount = 1;
            this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel.Size = new System.Drawing.Size(750, 420);
            this.tableLayoutPanel.TabIndex = 2;
            // 
            // FailureMechanismAssemblyCategoriesView
            // 
            this.Controls.Add(this.tableLayoutPanel);
            this.Name = "FailureMechanismAssemblyCategoriesView";
            this.Size = new System.Drawing.Size(750, 420);
            this.AutoScrollMinSize = new System.Drawing.Size(400, 100);
            this.failureMechanismAssemblyGroupBox.ResumeLayout(false);
            this.failureMechanismSectionAssemblyGroupBox.ResumeLayout(false);
            this.tableLayoutPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }


        #endregion

        private AssemblyCategoriesTable<FailureMechanismSectionAssemblyCategoryGroup> failureMechanismSectionAssemblyCategoriesTable;
        private AssemblyCategoriesTable<FailureMechanismAssemblyCategoryGroup> failureMechanismAssemblyCategoriesTable;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
        private System.Windows.Forms.GroupBox failureMechanismAssemblyGroupBox;
        private System.Windows.Forms.GroupBox failureMechanismSectionAssemblyGroupBox;
    }
}
