﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    partial class GrassCoverErosionInwardsDikeProfileSelectionView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GrassCoverErosionInwardsDikeProfileSelectionView));
            this.panel1 = new System.Windows.Forms.Panel();
            this.SelectNoneButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.DikeProfileDataGrid = new System.Windows.Forms.DataGridView();
            this.UseColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DikeProfileNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DikeProfileDataGrid)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.SelectNoneButton);
            this.panel1.Controls.Add(this.SelectAllButton);
            resources.ApplyResources(this.panel1, "panel1");
            this.panel1.Name = "panel1";
            // 
            // SelectNoneButton
            // 
            resources.ApplyResources(this.SelectNoneButton, "SelectNoneButton");
            this.SelectNoneButton.Name = "SelectNoneButton";
            this.SelectNoneButton.UseVisualStyleBackColor = true;
            this.SelectNoneButton.Click += new System.EventHandler(this.OnSelectNoneClick);
            // 
            // SelectAllButton
            // 
            resources.ApplyResources(this.SelectAllButton, "SelectAllButton");
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.OnSelectAllClick);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.DikeProfileDataGrid);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // DikeProfileDataGrid
            // 
            this.DikeProfileDataGrid.AllowUserToAddRows = false;
            this.DikeProfileDataGrid.AllowUserToDeleteRows = false;
            this.DikeProfileDataGrid.AllowUserToResizeColumns = false;
            this.DikeProfileDataGrid.AllowUserToResizeRows = false;
            this.DikeProfileDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DikeProfileDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UseColumn,
            this.DikeProfileNameColumn});
            resources.ApplyResources(this.DikeProfileDataGrid, "DikeProfileDataGrid");
            this.DikeProfileDataGrid.Name = "DikeProfileDataGrid";
            this.DikeProfileDataGrid.RowHeadersVisible = false;
            this.DikeProfileDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // UseColumn
            // 
            this.UseColumn.DataPropertyName = "Selected";
            resources.ApplyResources(this.UseColumn, "UseColumn");
            this.UseColumn.Name = "UseColumn";
            this.UseColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UseColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // SurfaceLineNameColumn
            // 
            this.DikeProfileNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DikeProfileNameColumn.DataPropertyName = "Name";
            resources.ApplyResources(this.DikeProfileNameColumn, "DikeProfileNameColumn");
            this.DikeProfileNameColumn.Name = "DikeProfileNameColumn";
            // 
            // GrassCoverErosionInwardsDikeProfileSelectionView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "GrassCoverErosionInwardsDikeProfileSelectionView";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DikeProfileDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView DikeProfileDataGrid;
        private System.Windows.Forms.Button SelectNoneButton;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn DikeProfileNameColumn;
    }
}
