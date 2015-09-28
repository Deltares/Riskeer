namespace DeltaShell.Plugins.SharpMapGis.Gui.Forms
{
    partial class VectorLayerAttributeTableView
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
            OpenViewMethod = null;
            ZoomToFeature = null;
            DeleteSelectedFeatures = null;
            createFeatureRowObject = null;

            CleanUpFeatureRows();

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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VectorLayerAttributeTableView));
            this.tableView = new DelftTools.Controls.Swf.Table.TableView();
            ((System.ComponentModel.ISupportInitialize)(this.tableView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableView
            // 
            this.tableView.AllowAddNewRow = true;
            this.tableView.AllowColumnPinning = true;
            this.tableView.AllowColumnSorting = true;
            this.tableView.AllowDeleteRow = true;
            this.tableView.AutoGenerateColumns = true;
            this.tableView.ColumnAutoWidth = false;
            this.tableView.DisplayCellFilter = null;
            this.tableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableView.HeaderHeigth = -1;
            this.tableView.InputValidator = null;
            this.tableView.InvalidCellBackgroundColor = System.Drawing.Color.Tomato;
            this.tableView.InvalidCellFilter = null;
            this.tableView.IsEndEditOnEnterKey = false;
            this.tableView.Location = new System.Drawing.Point(0, 0);
            this.tableView.MultipleCellEdit = true;
            this.tableView.MultiSelect = true;
            this.tableView.Name = "tableView";
            this.tableView.ReadOnly = false;
            this.tableView.ReadOnlyCellBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.tableView.ReadOnlyCellFilter = null;
            this.tableView.ReadOnlyCellForeColor = System.Drawing.Color.Black;
            this.tableView.RowHeight = -1;
            this.tableView.RowSelect = false;
            this.tableView.RowValidator = null;
            this.tableView.EditButtons = true;
            this.tableView.ShowRowNumbers = false;
            this.tableView.Size = new System.Drawing.Size(644, 316);
            this.tableView.TabIndex = 0;
            this.tableView.UseCenteredHeaderText = false;
            this.tableView.SelectionChanged += new System.EventHandler<DelftTools.Controls.TableSelectionChangedEventArgs>(this.TableViewSelectionChanged);
            // 
            // VectorLayerAttributeTableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tableView);
            this.Name = "VectorLayerAttributeTableView";
            this.Size = new System.Drawing.Size(644, 316);
            ((System.ComponentModel.ISupportInitialize)(this.tableView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
