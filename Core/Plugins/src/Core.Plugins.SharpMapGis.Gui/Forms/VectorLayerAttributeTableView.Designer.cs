using Core.Common.Controls;
using Core.Common.Controls.Table;

namespace Core.Plugins.SharpMapGis.Gui.Forms
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
            this.TableView = new TableView();
            ((System.ComponentModel.ISupportInitialize)(this.TableView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableView
            // 
            this.TableView.AllowAddNewRow = true;
            this.TableView.AllowColumnPinning = true;
            this.TableView.AllowColumnSorting = true;
            this.TableView.AllowDeleteRow = true;
            this.TableView.AutoGenerateColumns = true;
            this.TableView.ColumnAutoWidth = false;
            this.TableView.DisplayCellFilter = null;
            this.TableView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TableView.HeaderHeigth = -1;
            this.TableView.InputValidator = null;
            this.TableView.InvalidCellBackgroundColor = System.Drawing.Color.Tomato;
            this.TableView.InvalidCellFilter = null;
            this.TableView.IsEndEditOnEnterKey = false;
            this.TableView.Location = new System.Drawing.Point(0, 0);
            this.TableView.MultipleCellEdit = true;
            this.TableView.MultiSelect = true;
            this.TableView.Name = "TableView";
            this.TableView.ReadOnly = false;
            this.TableView.ReadOnlyCellBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(244)))), ((int)(((byte)(244)))));
            this.TableView.ReadOnlyCellFilter = null;
            this.TableView.ReadOnlyCellForeColor = System.Drawing.Color.Black;
            this.TableView.RowHeight = -1;
            this.TableView.RowSelect = false;
            this.TableView.RowValidator = null;
            this.TableView.EditButtons = true;
            this.TableView.ShowRowNumbers = false;
            this.TableView.Size = new System.Drawing.Size(644, 316);
            this.TableView.TabIndex = 0;
            this.TableView.UseCenteredHeaderText = false;
            this.TableView.SelectionChanged += new System.EventHandler<TableSelectionChangedEventArgs>(this.TableViewSelectionChanged);
            // 
            // VectorLayerAttributeTableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.TableView);
            this.Name = "VectorLayerAttributeTableView";
            this.Size = new System.Drawing.Size(644, 316);
            ((System.ComponentModel.ISupportInitialize)(this.TableView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
    }
}
