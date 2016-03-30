namespace Ringtoets.Piping.Forms.Views
{
    partial class PipingSurfaceLineSelectionView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PipingSurfaceLineSelectionView));
            this.panel1 = new System.Windows.Forms.Panel();
            this.SelectNoneButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.panel2 = new System.Windows.Forms.Panel();
            this.SurfaceLineDataGrid = new System.Windows.Forms.DataGridView();
            this.SurfaceLineNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UseColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SurfaceLineDataGrid)).BeginInit();
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
            this.panel2.Controls.Add(this.SurfaceLineDataGrid);
            resources.ApplyResources(this.panel2, "panel2");
            this.panel2.Name = "panel2";
            // 
            // SurfaceLineDataGrid
            // 
            this.SurfaceLineDataGrid.AllowUserToAddRows = false;
            this.SurfaceLineDataGrid.AllowUserToDeleteRows = false;
            this.SurfaceLineDataGrid.AllowUserToResizeColumns = false;
            this.SurfaceLineDataGrid.AllowUserToResizeRows = false;
            this.SurfaceLineDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.SurfaceLineDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.UseColumn,
            this.SurfaceLineNameColumn});
            resources.ApplyResources(this.SurfaceLineDataGrid, "SurfaceLineDataGrid");
            this.SurfaceLineDataGrid.Name = "SurfaceLineDataGrid";
            this.SurfaceLineDataGrid.RowHeadersVisible = false;
            this.SurfaceLineDataGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            // 
            // SurfaceLineNameColumn
            // 
            this.SurfaceLineNameColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.SurfaceLineNameColumn.DataPropertyName = "Name";
            resources.ApplyResources(this.SurfaceLineNameColumn, "SurfaceLineNameColumn");
            this.SurfaceLineNameColumn.Name = "SurfaceLineNameColumn";
            // 
            // UseColumn
            // 
            this.UseColumn.DataPropertyName = "Selected";
            resources.ApplyResources(this.UseColumn, "UseColumn");
            this.UseColumn.Name = "UseColumn";
            this.UseColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.UseColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // PipingSurfaceLineSelectionView
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Name = "PipingSurfaceLineSelectionView";
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SurfaceLineDataGrid)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridView SurfaceLineDataGrid;
        private System.Windows.Forms.Button SelectNoneButton;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.DataGridViewCheckBoxColumn UseColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn SurfaceLineNameColumn;
    }
}
