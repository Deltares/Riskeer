namespace DelftTools.Controls.Swf.Table
{
    partial class TableView
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
            this.dxGridControl = new DevExpress.XtraGrid.GridControl();
            this.dxGridView = new DevExpress.XtraGrid.Views.Grid.GridView();
            this.repositoryItemTimeEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit();
            ((System.ComponentModel.ISupportInitialize)(this.dxGridControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTimeEdit1)).BeginInit();
            this.SuspendLayout();
            // 
            // dxGridControl
            // 
            this.dxGridControl.AllowRestoreSelectionAndFocusedRow = DevExpress.Utils.DefaultBoolean.True;
            this.dxGridControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dxGridControl.Location = new System.Drawing.Point(0, 0);
            this.dxGridControl.LookAndFeel.SkinName = "Blue";
            this.dxGridControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
            this.dxGridControl.LookAndFeel.UseDefaultLookAndFeel = false;
            this.dxGridControl.MainView = this.dxGridView;
            this.dxGridControl.Name = "dxGridControl";
            this.dxGridControl.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[] {
            this.repositoryItemTimeEdit1});
            this.dxGridControl.Size = new System.Drawing.Size(564, 408);
            this.dxGridControl.TabIndex = 3;
            this.dxGridControl.UseEmbeddedNavigator = true;
            this.dxGridControl.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[] {
            this.dxGridView});
            // 
            // dxGridView
            // 
            this.dxGridView.GridControl = this.dxGridControl;
            this.dxGridView.Name = "dxGridView";
            this.dxGridView.OptionsCustomization.AllowColumnMoving = false;
            this.dxGridView.OptionsCustomization.AllowGroup = false;
            this.dxGridView.OptionsDetail.EnableDetailToolTip = true;
            this.dxGridView.OptionsSelection.MultiSelect = true;
            this.dxGridView.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
            this.dxGridView.OptionsView.ColumnAutoWidth = false;
            this.dxGridView.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
            this.dxGridView.OptionsView.ShowFooter = true;
            this.dxGridView.OptionsView.ShowGroupPanel = false;
            this.dxGridView.PaintStyleName = "Flat";
            // 
            // repositoryItemTimeEdit1
            // 
            this.repositoryItemTimeEdit1.AutoHeight = false;
            this.repositoryItemTimeEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton()});
            this.repositoryItemTimeEdit1.DisplayFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.repositoryItemTimeEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemTimeEdit1.EditFormat.FormatString = "yyyy-MM-dd HH:mm:ss";
            this.repositoryItemTimeEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
            this.repositoryItemTimeEdit1.EditValueChangedFiringMode = DevExpress.XtraEditors.Controls.EditValueChangedFiringMode.Default;
            this.repositoryItemTimeEdit1.Mask.EditMask = "yyyy-MM-dd HH:mm:ss";
            this.repositoryItemTimeEdit1.Name = "repositoryItemTimeEdit1";
            this.repositoryItemTimeEdit1.ValidateOnEnterKey = true;
            // 
            // TableView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dxGridControl);
            this.Name = "TableView";
            this.Size = new System.Drawing.Size(564, 408);
            ((System.ComponentModel.ISupportInitialize)(this.dxGridControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dxGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.repositoryItemTimeEdit1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DevExpress.XtraEditors.Repository.RepositoryItemTimeEdit repositoryItemTimeEdit1;
        private DevExpress.XtraGrid.GridControl dxGridControl;
        private DevExpress.XtraGrid.Views.Grid.GridView dxGridView;
    }
}