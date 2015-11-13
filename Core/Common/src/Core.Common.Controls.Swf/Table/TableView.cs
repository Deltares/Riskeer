using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Editors;
using Core.Common.Controls.Swf.Properties;
using Core.Common.Controls.Swf.Table.Validation;
using Core.Common.Utils;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Collections.Generic;
using Core.Common.Utils.Globalization;
using Core.Common.Utils.Reflection;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using log4net;
using TypeConverter = Core.Common.Utils.TypeConverter;

namespace Core.Common.Controls.Swf.Table
{
    ///<summary>
    /// Graphical representation of tabular data.
    ///</summary>
    public partial class TableView : UserControl, ITableView, ISupportInitialize
    {
        public enum ValidationExceptionMode
        {
            Ignore,
            NoAction,
            DisplayError,
            ThrowException
        }

        private static readonly ILog Log = LogManager.GetLogger(typeof(TableView));
        private readonly EventedList<TableViewCell> selectedCells;
        private readonly TableViewValidator tableViewValidator;
        private readonly EventedList<ITableViewColumn> columns;
        private bool isPasting;
        private bool isSelectionChanging;
        private bool updatingSelection;
        private bool showRowNumbers;
        private bool autoGenerateColumns;
        private bool refreshRequired;
        private bool allowColumnSorting = true;
        private bool f2Pressed;
        private Timer refreshTimer;
        private BindingSource bindingSource;
        private GridCell[] cellsToFill;

        public TableView()
        {
            InitializeComponent();

            columns = new EventedList<ITableViewColumn>();
            ColumnMenuItems = new List<TableViewColumnMenuItem>();
            selectedCells = new EventedList<TableViewCell>();
            tableViewValidator = new TableViewValidator(this);
            PasteController = new TableViewPasteController(this)
            {
                PasteBehaviour = TableViewPasteBehaviourOptions.SkipCellWhenValueIsInvalid
            };

            AllowColumnPinning = true;
            AllowDeleteRow = true;
            AutoGenerateColumns = true;
            MultipleCellEdit = true;
            dxGridView.OptionsView.ShowFooter = false;
            dxGridView.OptionsBehavior.CopyToClipboardWithColumnHeaders = false; //mimic behavior of 8.2
            GridLocalizer.Active = new TableViewExceptionMessageController();

            Text = Resources.TableView_TableView_new_Table;

            ReadOnlyCellForeColor = Color.Black; //just use black as  a default to increase readability
            ReadOnlyCellBackColor = Color.FromArgb(255, 244, 244, 244);
            InvalidCellBackgroundColor = Color.Tomato;
            ExceptionMode = ValidationExceptionMode.DisplayError;
            ConfigureContextMenu();
            SubscribeEvents();
            CreateRefreshTimer();
        }

        public void ExportAsCsv(string fileName, string delimiter = ", ")
        {
            // the build-in export to file method depends on a devexpress dll we haven't included so far
            using (var writer = new StreamWriter(fileName))
            {
                var visibleColumns = Columns.Where(c => c.Visible).OrderBy(c => c.DisplayIndex);
                var lastColumn = visibleColumns.Last();

                foreach (var visibleColumn in visibleColumns)
                {
                    writer.Write(visibleColumn.Caption);
                    if (visibleColumn != lastColumn)
                    {
                        writer.Write(delimiter);
                    }
                }

                writer.WriteLine();

                var shownColumns = visibleColumns.Select(c => c.AbsoluteIndex).ToArray();

                //writing the data
                for (int x = 0; x < RowCount; x++)
                {
                    for (int y = 0; y < shownColumns.Length; y++)
                    {
                        writer.Write(GetCellDisplayText(x, shownColumns[y]));
                        if (y != Columns.Count - 1)
                        {
                            writer.Write(delimiter);
                        }
                    }
                    writer.WriteLine();
                }
                writer.Close();
            }
        }

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            UnSubscribeFromDataSource();
            RegionalSettingsManager.FormatChanged -= RegionalSettingsManagerFormatChanged;

            if (refreshTimer != null)
            {
                refreshTimer.Stop();
                refreshTimer.Tick -= OnRefreshTimerOnTick;
                refreshTimer.Dispose();
                refreshTimer = null;
            }

            if (disposing)
            {
                //only dispose if disposing managed resource (otherwise called from other thread)
                // prevents memory leaks
                foreach (var view in dxGridControl.Views.Cast<BaseView>().ToList())
                {
                    view.Dispose();
                }
                dxGridControl.Dispose();
            }

            if (Columns != null)
            {
                foreach (var column in Columns)
                {
                    column.Dispose();
                }
            }

            if (disposing && (components != null))
            {
                components.Dispose();
            }

            try
            {
                /*
                   Localize bug in XtraEditors:
              
                   System.InvalidOperationException : Value Dispose() cannot be called while doing CreateHandle().
                        at System.Windows.Forms.Control.Dispose(Boolean disposing)
                        at DevExpress.XtraEditors.ScrollBarBase.Dispose(Boolean disposing)
                        at System.ComponentModel.Component.Dispose()
                        at DevExpress.XtraGrid.Scrolling.ScrollInfo.Dispose()
                        at DevExpress.XtraGrid.Views.Grid.GridView.Dispose(Boolean disposing)
                        at System.ComponentModel.Component.Dispose()
                        at DevExpress.XtraGrid.GridControl.RemoveView(BaseView gv, Boolean disposeView)
                        at DevExpress.XtraGrid.GridControl.RemoveView(BaseView gv)
                        at DevExpress.XtraGrid.GridControl.Dispose(Boolean disposing)
                        at System.ComponentModel.Component.Dispose()
                        at System.Windows.Forms.Control.Dispose(Boolean disposing)
                        at System.Windows.Forms.ContainerControl.Dispose(Boolean disposing)
                        at Core.Common.Controls.Swf.Table.TableView.Dispose(Boolean disposing) in TableView.Designer.cs: line 22
                 */
                base.Dispose(disposing);
            }
            catch (InvalidOperationException e)
            {
                Log.Debug("Strange bug in XtraGrid control, from time to time crashes", e);
            }
        }

        protected override void OnParentVisibleChanged(EventArgs e)
        {
            base.OnParentVisibleChanged(e);

            // unsubscribe from static events
            RegionalSettingsManager.FormatChanged -= RegionalSettingsManagerFormatChanged;
            if (Parent.Visible)
            {
                RegionalSettingsManager.FormatChanged += RegionalSettingsManagerFormatChanged;
            }
        }

        /// <summary>
        /// Converts value pasted from clipboard, currently always a string, to a value of the type
        /// required by the underlying datasource.
        /// In addition to default conversion for e.g string to int by
        ///     TypeConverter.ConvertValueToTargetType
        /// There is added support for the RepositoryItemImageComboBox where the description (will default be 
        /// copied to clipboard) will be converted to value.
        ///    - NB the base class RepositoryItemComboBox does not have items with value and description
        /// </summary>
        /// <param name="columnIndex"></param>
        /// <param name="cellValue"></param>
        /// <returns></returns>
        internal object ConvertStringValueToDataValue(int columnIndex, string cellValue)
        {
            object value = null;
            RepositoryItem editor = dxGridView.Columns[columnIndex].ColumnEdit;
            Type columnType = dxGridView.Columns[columnIndex].ColumnType;
            if (null != editor)
            {
                var imageComboBoxEditor = editor as RepositoryItemImageComboBox;
                if (imageComboBoxEditor != null)
                {
                    var repositoryItemImageComboBox = imageComboBoxEditor;
                    var map = new Dictionary<string, object>();
                    for (int j = 0; j < repositoryItemImageComboBox.Items.Count; j++)
                    {
                        map[repositoryItemImageComboBox.Items[j].Description] =
                            repositoryItemImageComboBox.Items[j].Value;
                    }
                    if (map.ContainsKey(cellValue))
                    {
                        value = map[cellValue];
                    }
                }
                else if (editor is RepositoryItemComboBox || editor is RepositoryItemLookUpEdit)
                {
                    var valueLookUp = new Dictionary<string, object>();

                    var comboBoxEditor = editor as RepositoryItemComboBox;
                    if (comboBoxEditor != null)
                    {
                        var repositoryItemComboBox = comboBoxEditor;
                        valueLookUp = repositoryItemComboBox.Items.OfType<TableViewComboBoxItem>().ToDictionary(i => i.DisplayText, i => i.Value);
                    }
                    else
                    {
                        var lookUpEdit = (RepositoryItemLookUpEdit) editor;
                        var comboBoxItems = lookUpEdit.DataSource as IEnumerable<TableViewComboBoxItem>;
                        if (comboBoxItems != null)
                        {
                            valueLookUp = comboBoxItems.ToDictionary(i => i.DisplayText, i => i.Value);
                        }
                    }

                    if (valueLookUp.Any() && valueLookUp.ContainsKey(cellValue))
                    {
                        value = valueLookUp[cellValue];
                    }
                    else if (comboBoxEditor != null) // items are not mandatory
                    {
                        value = cellValue;
                    }
                }
                else
                {
                    value = ConvertToColumnValue(cellValue, columnType);
                }
            }
            else if (dxGridView.Columns[columnIndex].RealColumnEdit is RepositoryItemCheckEdit)
            {
                return cellValue == "Checked";
            }
            else
            {
                value = ConvertToColumnValue(cellValue, columnType);
            }
            return value;
        }

        private ITableViewColumn GetColumnByDxColumn(GridColumn dxGridColumn)
        {
            return Columns.FirstOrDefault(c => c.AbsoluteIndex == dxGridColumn.AbsoluteIndex);
        }

        private void BestFitColumnsWithOnlyFirstWordOfHeader()
        {
            var oldHeaders = Columns.ToDictionary(c => c, c => c.Caption);

            dxGridView.BeginUpdate();
            try
            {
                foreach (var column in Columns)
                {
                    string caption = column.Caption;

                    //Say the caption is 'Discharge on Lateral (m3/s)'. We're trying to do 
                    //something a bit smarter than just ignore the header entirely: we try 
                    //to get the first word (eg: "Discharge"), add room for the ellipsis 
                    //and keep that into view

                    if (!String.IsNullOrEmpty(caption))
                    {
                        var indexOfWhitespace = caption.IndexOfAny(new[]
                        {
                            ' ',
                            '\t'
                        });
                        if (indexOfWhitespace >= 0)
                        {
                            //add space for ellipsis
                            column.Caption = caption.Substring(0, indexOfWhitespace) + "...";
                        }
                    }
                }

                dxGridView.BestFitColumns();

                //restore the original columns
                foreach (var column in Columns)
                {
                    column.Caption = oldHeaders[column];
                }
            }
            finally
            {
                dxGridView.EndUpdate();
            }
        }

        private bool DeselectSelectCells(int top, int left, int bottom, int right)
        {
            var cellsDeselected = false;
            // dxGridControl.SuspendLayout();
            dxGridView.BeginSelection();
            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    var selectedCell = selectedCells.FirstOrDefault(c => c.Column == GetColumnByDisplayIndex(x) && c.RowIndex == y);
                    if (selectedCell != null)
                    {
                        selectedCells.Remove(selectedCell);
                        cellsDeselected = true;
                    }
                }
            }
            dxGridView.EndSelection();
            //dxGridControl.ResumeLayout();

            return cellsDeselected;
        }

        private void AddEnumCellEditors()
        {
            foreach (var column in Columns.Where(c => c.ColumnType.IsEnum && c.Editor == null))
            {
                column.Editor = new ComboBoxTypeEditor
                {
                    Items = Enum.GetValues(column.ColumnType),
                    ItemsMandatory = false,
                    CustomFormatter = new EnumFormatter(column.ColumnType)
                };
            }
        }

        private void ColumnsOnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (e.Action == NotifyCollectionChangeAction.Remove && dxGridView.Columns.Count > e.Index)
            {
                dxGridView.Columns.RemoveAt(e.Index);
            }
        }

        private static void CopyPasteControllerPasteFailed(object sender, EventArgs<string> e)
        {
            System.Windows.Forms.MessageBox.Show(e.Value);
        }

        private void SelectedCellsCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (updatingSelection)
            {
                return;
            }

            var cell = (TableViewCell) e.Item;

            var gridColumn = GetDxColumnByDisplayIndex(cell.Column.DisplayIndex);

            switch (e.Action)
            {
                case NotifyCollectionChangeAction.Add:
                    dxGridView.SelectCell(cell.RowIndex, gridColumn);
                    break;
                case NotifyCollectionChangeAction.Remove:
                    dxGridView.UnselectCell(cell.RowIndex, gridColumn);
                    break;
                default:
                    throw new NotSupportedException(string.Format(Resources.TableView_SelectedCellsCollectionChanged_Action__0__is_not_supported_by_the_TableView, e.Action));
            }
        }

        private void BindingListListChanged(object sender, ListChangedEventArgs e)
        {
            if (!AutoGenerateColumns || e.ListChangedType != ListChangedType.PropertyDescriptorChanged || e.PropertyDescriptor == null)
            {
                return;
            }

            // has column name changed
            var column = dxGridView.Columns.ColumnByName(e.PropertyDescriptor.DisplayName);
            if (column != null)
            {
                return;
            }

            column = dxGridView.Columns[e.NewIndex];

            column.FieldName = e.PropertyDescriptor.Name;
            column.Caption = e.PropertyDescriptor.DisplayName;
        }

        private void DataSourceDataTableColumnsCollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            if (e.Action == CollectionChangeAction.Add || e.Action == CollectionChangeAction.Remove)
            {
                if (AutoGenerateColumns)
                {
                    dxGridView.PopulateColumns();
                    BuildColumnWrappers();
                    UpdateColumnsFormatting();
                    BestFitColumns();
                }
            }
        }

        private void DataSourcePropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            refreshRequired = true;
        }

        [InvokeRequired]
        private void DataSourceCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            refreshRequired = true;
            UpdateHeaderColumnSize();
        }

        /// <summary>
        /// Checks if the mouse is clicked at the Header Panel Button in the grid.
        /// If yes select entire grid (TOOLS-2834)
        /// cfr: http://documentation.devexpress.com/#WindowsForms/CustomDocument532
        /// "The header panel button belongs to both the column header panel and row 
        ///  indicator panel elements. By default, the header panel button has no special 
        ///  functionality. When the View is used to represent detail data, the header 
        ///  panel button serves as a zoom button." 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void HandleHeaderPanelButton(object sender, MouseEventArgs e)
        {
            var view = sender as GridView;
            if (view == null)
            {
                return;
            }
            var hitInfo = view.CalcHitInfo(new Point(e.X, e.Y));
            if ((hitInfo.HitTest == GridHitTest.ColumnButton) && (hitInfo.InColumn == false))
            {
                view.SelectAll();
            }
        }

        private static DXMenuItem GetItemByStringId(DXPopupMenu menu, GridStringId id)
        {
            return menu.Items.Cast<DXMenuItem>().FirstOrDefault(item => ((GridStringId) item.Tag) == id);
        }

        private void SubscribeToDataSource()
        {
            if (dxGridControl.DataSource == null)
            {
                return;
            }

            var propertyChanged = dxGridControl.DataSource as INotifyPropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged.PropertyChanged += DataSourcePropertyChanged;
            }

            var collectionChanged = dxGridControl.DataSource as INotifyCollectionChanged;
            if (collectionChanged != null)
            {
                collectionChanged.CollectionChanged += DataSourceCollectionChanged;
            }

            var dataTable = dxGridControl.DataSource as DataTable;
            if (dataTable != null)
            {
                var table = dataTable;
                table.Columns.CollectionChanged += DataSourceDataTableColumnsCollectionChanged;
            }

            var bindingList = dxGridControl.DataSource as IBindingList;
            if (bindingList != null)
            {
                bindingList.ListChanged += BindingListListChanged;
            }
        }

        private void UnSubscribeFromDataSource()
        {
            if (dxGridControl == null || dxGridControl.DataSource == null)
            {
                return;
            }

            var propertyChanged = dxGridControl.DataSource as INotifyPropertyChanged;
            if (propertyChanged != null)
            {
                propertyChanged.PropertyChanged -= DataSourcePropertyChanged;
            }

            var notifyCollectionChanged = dxGridControl.DataSource as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
            {
                notifyCollectionChanged.CollectionChanged -= DataSourceCollectionChanged;
            }

            var dataTable = dxGridControl.DataSource as DataTable;
            if (dataTable != null)
            {
                var table = dataTable;
                table.Columns.CollectionChanged -= DataSourceDataTableColumnsCollectionChanged;
            }

            var bindingSource = dxGridControl.DataSource as BindingSource;
            if (bindingSource != null)
            {
                bindingSource.ListChanged -= BindingListListChanged;
            }
        }

        private void RegionalSettingsManagerFormatChanged()
        {
            dxGridView.Invalidate();
            UpdateColumnsFormatting();
        }

        /// <summary>
        /// Calculate width of the header column based on grid font
        /// </summary>
        private void UpdateHeaderColumnSize()
        {
            var indicatorWidth = (int) dxGridControl.Font.SizeInPoints*RowCount.ToString(CultureInfo.InvariantCulture).Length + 15;
            dxGridView.IndicatorWidth = showRowNumbers ? indicatorWidth : -1;
            Refresh();
        }

        /// <summary>
        /// Returns true if the whole selection is readonly. This can be on cell,column or table level If a cell is not it returns false.
        /// </summary>
        /// <returns></returns>
        private bool GetSelectionIsReadonly()
        {
            //no selection use tableview readonly
            if (SelectedCells.Count == 0)
            {
                return ReadOnly;
            }

            //A selection is readonly if ALL cells are readonly (otherwise i can change the not readonly part)
            return SelectedCells.All(cell => CellIsReadOnly(cell.RowIndex, cell.Column));
        }

        private void UpdateSelectionFromGridControl()
        {
            if (updatingSelection)
            {
                return;
            }

            updatingSelection = true;

            selectedCells.Clear();

            var gridViewSelectedCells = dxGridView.GetSelectedCells();
            foreach (var cell in gridViewSelectedCells)
            {
                selectedCells.Add(new TableViewCell(cell.RowHandle, GetColumnByDxColumn(cell.Column)));
            }

            if (!IsEditing && !isPasting)
            {
                if (SelectionChanged != null)
                {
                    Log.DebugFormat("Firing selection changed event");
                    SelectionChanged(this, new TableSelectionChangedEventArgs(selectedCells.ToArray()));
                }
            }

            // Update the row context menu
            RowContextMenu.Items.OfType<ToolStripMenuItem>().ForEach(mi => mi.Available = true);
            RowContextMenu.Items.OfType<ToolStripMenuItem>()
                          .Where(mi => mi.Name == "btnDelete" || mi.Name == "btnPaste")
                          .ForEach(mi => mi.Available = !GetSelectionIsReadonly());

            updatingSelection = false;
        }

        private void BuildColumnWrappers()
        {
            if (!AutoGenerateColumns)
            {
                return;
            }

            columns.CollectionChanged -= ColumnsOnCollectionChanged;
            columns.Clear();

            foreach (GridColumn dxColumn in dxGridView.Columns)
            {
                Columns.Add(new TableViewColumn(dxGridView, dxGridControl, dxColumn, this, false)
                {
                    SortingAllowed = AllowColumnSorting
                });
            }

            columns.CollectionChanged += ColumnsOnCollectionChanged;
        }

        private void UpdateColumnsFormatting()
        {
            foreach (var column in Columns.OfType<TableViewColumn>().Where(tvc => tvc.CustomFormatter == null))
            {
                if (column.ColumnType == typeof(DateTime))
                {
                    column.DxColumn.ColumnEdit = (RepositoryItem) repositoryItemTimeEdit1.Clone();

                    if (string.IsNullOrEmpty(column.DisplayFormat))
                    {
                        column.DisplayFormat = RegionalSettingsManager.DateTimeFormat;
                    }
                }
                else if (column.ColumnType.IsNumericalType())
                {
                    if (string.IsNullOrEmpty(column.DisplayFormat))
                    {
                        column.DisplayFormat = RegionalSettingsManager.RealNumberFormat;
                    }
                }
            }
        }

        private void ShowEditorIfRowSelect()
        {
            if (dxGridView.FocusedColumn != null && (RowSelect) && (dxGridView.ActiveEditor == null))
            {
                dxGridView.ShowEditor();
            }
        }

        private GridColumn GetDxColumnByDisplayIndex(int displayIndex)
        {
            return dxGridView.Columns[Columns.First(c => c.DisplayIndex == displayIndex).AbsoluteIndex];
        }

        private void UpdateColumnHeaderMenu(GridMenuEventArgs e, ITableViewColumn viewColumn)
        {
            //show grid menu is handled to remove menu-items.  For grouping etc.
            //No way to do this in a setting :(
            //see http://community.devexpress.com/forums/t/61316.aspx

            var ids = new[]
            {
                GridStringId.MenuColumnGroupBox,
                GridStringId.MenuColumnGroup,
                GridStringId.MenuColumnRemoveColumn
            };

            var dxMenuItems = ids.Select(id => GetItemByStringId(e.Menu, id));
            var dxMenuItemsToHide = dxMenuItems.Where(item => item != null);
            foreach (var item in dxMenuItemsToHide)
            {
                item.Visible = false;
            }

            if (AllowColumnPinning && viewColumn != null)
            {
                var pinColumnMenuItem = new DXMenuCheckItem
                {
                    Caption = viewColumn.Pinned ? "Unpin Column" : "Pin Column",
                    Checked = viewColumn.Pinned,
                    Image = Resources.pin
                };

                pinColumnMenuItem.CheckedChanged += (sender, args) => { viewColumn.Pinned = pinColumnMenuItem.Checked; };

                e.Menu.Items.Add(pinColumnMenuItem);
            }

            var copyHeadersColumnMenuItem = new DXMenuItem
            {
                Caption = "Copy all headers",
                Image = Resources.CopyHS
            };

            copyHeadersColumnMenuItem.Click += (sender, args) =>
            {
                var sb = new StringBuilder();

                CopyHeader(sb);

                Clipboard.Clear();
                Clipboard.SetData(DataFormats.Text, sb.ToString());
            };

            e.Menu.Items.Add(copyHeadersColumnMenuItem);
        }

        private static bool IsNumberType(Type type)
        {
            if (type == typeof(double)
                || type == typeof(float)
                || type == typeof(decimal))
            {
                return true;
            }
            return false;
        }

        private bool ValidateAndCommitRow(int rowIndex)
        {
            string errorText;
            if (!tableViewValidator.ValidateRow(rowIndex, out errorText))
            {
                Log.ErrorFormat("Can not set value for row {0}, reason: {1}", rowIndex, errorText);
                dxGridView.CancelUpdateCurrentRow();
                tableViewValidator.RefreshRowData();
                dxGridView.DeleteRow(rowIndex);
                return false;
            }

            dxGridView.FocusedRowHandle = rowIndex;
            //this line is needed for the changes to be 'commited' and the row
            //to leave an updating state. http://community.devexpress.com/forums/p/30892/106718.aspx
            //unfortunately hard to write a test without exposing/hacking too much
            dxGridView.UpdateCurrentRow();
            tableViewValidator.RefreshRowData();
            return true;
        }

        private bool SetCellValueInternal(int rowIndex, int columnDisplayIndex, object value)
        {
            var col = GetDxColumnByDisplayIndex(columnDisplayIndex);

            if (CellIsReadOnly(rowIndex, GetColumnByDxColumn(col)))
            {
                return false;
            }

            //check if the value can be converted to the column type
            object objectValue = value is string
                                     ? ConvertStringValueToDataValue(col.AbsoluteIndex, (string) value)
                                     : value;
            if (objectValue == null)
            {
                Log.ErrorFormat("Can not set value into cell [{0}, {1}] reason:{2}", rowIndex, col.AbsoluteIndex, "No conversion from string possible");
                return false;
            }

            string error;
            if (!tableViewValidator.ValidateCell(new TableViewCell(rowIndex, GetColumnByDxColumn(col)), objectValue, out error))
            {
                Log.ErrorFormat("Can not set value into cell [{0}, {1}] reason:{2}", rowIndex, col.AbsoluteIndex, error);
                return false;
            }

            // (Gijs) We are mixing the row handlers and indices throughout this wrapper: should be fixed, as 

            dxGridView.FocusedRowHandle = rowIndex;

            dxGridView.SetRowCellValue(rowIndex, col, objectValue);
            return true;
        }

        private static object ConvertToColumnValue(string cellValue, Type columnType)
        {
            try
            {
                return TypeConverter.ConvertValueToTargetType(columnType, cellValue);
            }
            catch (Exception)
            {
                if (string.IsNullOrEmpty(cellValue.TrimStart()))
                {
                    if (columnType.IsValueType)
                    {
                        return Activator.CreateInstance(columnType);
                    }
                    return null;
                }

                // still try to parse double or float numbers
                if (columnType == typeof(double) || columnType == typeof(float))
                {
                    double value;
                    if (double.TryParse(cellValue, out value))
                    {
                        return value;
                    }
                }

                Log.WarnFormat("Unable to convert string {0} to {1} for paste", cellValue, columnType);
                return null;
            }
        }

        #region Public properties

        /// <summary>
        /// Specifies whether the edit buttons on the bottom of the table view is shown
        /// </summary>
        public bool EditButtons
        {
            get
            {
                return dxGridControl.UseEmbeddedNavigator;
            }
            set
            {
                dxGridControl.UseEmbeddedNavigator = value;
            }
        }

        public bool AllowDeleteRow
        {
            get
            {
                if (bindingSource == null) // lazy initialize
                {
                    bindingSource = new BindingSource
                    {
                        DataSource = dxGridView.DataSource
                    };
                }

                var tableAllowRemove = dxGridView.OptionsBehavior.AllowDeleteRows;
                var dataAllowsRemove = bindingSource.AllowRemove;

                // better code, but doesn't work for List (hacked through BindingSource):
                // dataAllowsNew = xGridView.DataController != null && dxGridView.DataController.AllowNew;

                return (tableAllowRemove == DefaultBoolean.True || tableAllowRemove == DefaultBoolean.Default) &&
                       dataAllowsRemove;
            }
            set
            {
                dxGridView.OptionsBehavior.AllowDeleteRows = value ? DefaultBoolean.True : DefaultBoolean.False;
                if (dxGridControl.EmbeddedNavigator != null)
                {
                    dxGridControl.EmbeddedNavigator.Buttons.Remove.Visible = value;
                }
            }
        }

        public bool AllowAddNewRow
        {
            get
            {
                if (bindingSource == null) // lazy initialize
                {
                    bindingSource = new BindingSource
                    {
                        DataSource = dxGridView.DataSource
                    };
                }

                var tableAllowNew = dxGridView.OptionsBehavior.AllowAddRows;
                var dataAllowsNew = bindingSource.AllowNew;

                // better code, but doesn't work for List (hacked through BindingSource):
                // dataAllowsNew = xGridView.DataController != null && dxGridView.DataController.AllowNew;

                return (tableAllowNew == DefaultBoolean.True || tableAllowNew == DefaultBoolean.Default) &&
                       dataAllowsNew;
            }
            set
            {
                dxGridView.OptionsBehavior.AllowAddRows = value ? DefaultBoolean.True : DefaultBoolean.False;
                dxGridView.OptionsView.NewItemRowPosition = value ? NewItemRowPosition.Bottom : NewItemRowPosition.None;
                if (dxGridControl.EmbeddedNavigator != null)
                {
                    dxGridControl.EmbeddedNavigator.Buttons.Append.Enabled = value;
                }
            }
        }

        public bool AllowColumnSorting
        {
            get
            {
                return allowColumnSorting;
            }
            set
            {
                if (allowColumnSorting == value)
                {
                    return;
                }

                allowColumnSorting = value;
                foreach (var column in Columns)
                {
                    column.SortingAllowed = allowColumnSorting;
                }
            }
        }

        public bool AllowColumnPinning { get; set; }

        /// <summary>
        /// Gets or sets whether multiple rows can be selected
        /// </summary>
        public bool MultiSelect
        {
            get
            {
                return dxGridView.OptionsSelection.MultiSelect;
            }
            set
            {
                dxGridView.OptionsSelection.MultiSelect = value;
            }
        }

        public bool IncludeHeadersOnCopy { get; set; }

        ///<summary>
        /// Gets a boolean value indicating that the view is being edited or not
        ///</summary>
        [Browsable(false)]
        public bool IsEditing
        {
            get
            {
                return dxGridView.IsEditing || dxGridView.FocusedRowModified;
            }
        }

        ///<summary>
        /// TODO: If RowSelect is true, dxGridView.GetSelectedCells etc will give an empty array.... SelectedRows will stiull work.
        ///</summary>
        public bool RowSelect
        {
            get
            {
                return dxGridView.OptionsSelection.MultiSelectMode == GridMultiSelectMode.RowSelect;
            }
            set
            {
                dxGridView.OptionsSelection.EnableAppearanceFocusedCell = false;
                dxGridView.FocusRectStyle = DrawFocusRectStyle.None;
                dxGridView.OptionsSelection.MultiSelectMode = (value) ? GridMultiSelectMode.RowSelect : GridMultiSelectMode.CellSelect;
            }
        }

        public bool ShowRowNumbers
        {
            get
            {
                return showRowNumbers;
            }
            set
            {
                showRowNumbers = value;
                UpdateHeaderColumnSize();
            }
        }

        public bool ColumnAutoWidth
        {
            get
            {
                return dxGridView.OptionsView.ColumnAutoWidth;
            }
            set
            {
                dxGridView.OptionsView.ColumnAutoWidth = value;
            }
        }

        public bool UseCenteredHeaderText
        {
            get
            {
                return dxGridView.Appearance.HeaderPanel.TextOptions.HAlignment == HorzAlignment.Center;
            }
            set
            {
                dxGridView.Appearance.HeaderPanel.TextOptions.HAlignment = (value)
                                                                               ? HorzAlignment.Center
                                                                               : HorzAlignment.Default;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return !dxGridView.Editable;
            }
            set
            {
                dxGridView.OptionsBehavior.Editable = !value;
            }
        }

        public bool MultipleCellEdit { get; set; }

        /// <summary>
        /// Determines whether the tableview generates columns when 
        /// setting data
        /// </summary>
        public bool AutoGenerateColumns
        {
            get
            {
                return autoGenerateColumns;
            }
            set
            {
                autoGenerateColumns = value;
                //update gridview behavior
                dxGridView.OptionsBehavior.AutoPopulateColumns = value;
            }
        }

        public bool AutoSizeRows
        {
            get
            {
                return dxGridView.OptionsView.RowAutoHeight;
            }
            set
            {
                dxGridView.OptionsView.RowAutoHeight = value;
            }
        }

        public int HeaderHeigth
        {
            get
            {
                return dxGridView.ColumnPanelRowHeight;
            }
            set
            {
                if (value > 0)
                {
                    dxGridView.Appearance.HeaderPanel.TextOptions.WordWrap = WordWrap.Wrap;
                }
                dxGridView.ColumnPanelRowHeight = value;
            }
        }

        public int RowHeight
        {
            get
            {
                return dxGridView.RowHeight;
            }
            set
            {
                dxGridView.RowHeight = value;
            }
        }

        /// <summary>
        /// Number of rows in the grid excluding the newrow and filtered rows.
        /// </summary>
        [Browsable(false)]
        public int RowCount
        {
            get
            {
                return dxGridView.DataController.VisibleCount;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int FocusedRowIndex
        {
            get
            {
                return dxGridView.FocusedRowHandle;
            }
            set
            {
                if (dxGridView.FocusedRowHandle != value && dxGridView.FocusedRowHandle != int.MinValue && value > -1)
                {
                    dxGridView.FocusedRowHandle = value;
                }
            }
        }

        [Browsable(false)]
        public int[] SelectedRowsIndices
        {
            get
            {
                return dxGridView.GetSelectedRows();
            }
        }

        [Browsable(false)]
        public int[] SelectedColumnsIndices
        {
            get
            {
                return selectedCells.Select(c => c.Column.AbsoluteIndex).OrderBy(i => i).Distinct().ToArray();
            }
        }

        ///<summary>
        /// Gets or sets the disabled cell fore color
        ///</summary>
        public Color ReadOnlyCellForeColor { get; set; }

        ///<summary>
        /// Gets or sets the disabled cell fore color
        ///</summary>
        public Color ReadOnlyCellBackColor { get; set; }

        ///<summary>
        /// Gets or sets the background color of the invalid cell (value)
        ///</summary>
        public Color InvalidCellBackgroundColor { get; set; }

        /// <summary>
        /// Returns the focused row (also works if the focused row is new or deleted)
        /// </summary>
        [Browsable(false)]
        public object CurrentFocusedRowObject
        {
            get
            {
                return dxGridView.GetFocusedRow();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public object Data
        {
            get
            {
                return dxGridControl.DataSource;
            }
            set
            {
                dxGridView.CancelUpdateCurrentRow();
                UnSubscribeFromDataSource();
                bindingSource = null;
                ClearSelection();

                // clear before binding enables rebinding to new function; will remove old columns
                if (dxGridControl.DataSource != value)
                {
                    // do not explicitly clear columns; this will also remove user added column editors

                    dxGridControl.DataSource = value;

                    if (AutoGenerateColumns)
                    {
                        dxGridView.PopulateColumns();
                    }
                }

                RegionalSettingsManager.FormatChanged -= RegionalSettingsManagerFormatChanged;

                if (value == null)
                {
                    return;
                }

                RegionalSettingsManager.FormatChanged += RegionalSettingsManagerFormatChanged;

                SubscribeToDataSource();

                BeginInit();

                BuildColumnWrappers();
                AddEnumCellEditors();
                UpdateColumnsFormatting();
                UpdateHeaderColumnSize();

                EndInit();
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Image Image { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ITableViewPasteController PasteController { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ValidationExceptionMode ExceptionMode { get; set; }

        [Browsable(false)]
        public IList<TableViewCell> SelectedCells
        {
            get
            {
                return selectedCells;
            }
        }

        // avoid serialization of Columns into resx files, 
        // when something changes in columns designers becomes mad because of broken resx files
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<ITableViewColumn> Columns
        {
            get
            {
                return columns;
            }
        }

        public override ContextMenuStrip ContextMenuStrip
        {
            get
            {
                return RowContextMenu;
            }
            set
            {
                RowContextMenu = value;
            }
        }

        public ContextMenuStrip RowContextMenu
        {
            get
            {
                return dxGridControl.ContextMenuStrip;
            }
            private set
            {
                dxGridControl.ContextMenuStrip = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public IList<TableViewColumnMenuItem> ColumnMenuItems { get; set; }

        ///<summary>
        /// Gets or sets the filter to evaluate if the value of the cell is invalid
        ///</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<TableViewCell, bool> InvalidCellFilter { get; set; }

        ///<summary>
        /// Gets or sets the filter that can be used for making cells read-only
        ///</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<TableViewCell, bool> ReadOnlyCellFilter { get; set; }

        ///<summary>
        /// Gets or sets the filter that can be used for styling a cell
        ///</summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<TableViewCellStyle, bool> DisplayCellFilter { get; set; }

        /// <summary>
        /// Function for getting or setting unbound column values
        /// Parameters : column index, datasource row index, is getter, is setter, value
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<int, int, bool, bool, object, object> UnboundColumnData { get; set; }

        /// <summary>
        /// Gets or sets the row values validator. The validator should return a RowValidationResult
        /// </summary>
        /// <remarks>?Does not validate before commit to data source?</remarks>
        [Browsable(false)]
        public Func<int, object[], IRowValidationResult> RowValidator { get; set; }

        /// <summary>
        /// Gets or sets the cell value editor validator. The validator should return true on valid values
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<TableViewCell, object, Tuple<string, bool>> InputValidator { get; set; }

        /// <summary>
        /// Gets or sets the function that checks if the current selection can be deleted.
        /// When null or returning true, deletion is allowed.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<bool> CanDeleteCurrentSelection { get; set; }

        /// <summary>
        /// Gets or sets the function for deleting selected rows (return value indicates if the deleting is handled)
        /// When null or returning false the default delete will be executed
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<bool> RowDeleteHandler { get; set; }

        public bool IsEndEditOnEnterKey { get; set; }

        ///<summary>
        /// Enable filtering for all columns
        ///</summary>
        public bool AllowColumnFiltering
        {
            get
            {
                return Columns.OfType<TableViewColumn>().Any(c => c.FilteringAllowed);
            }
            set
            {
                foreach (var column in Columns.OfType<TableViewColumn>())
                {
                    column.FilteringAllowed = value;
                }
            }
        }

        public ViewInfo ViewInfo { get; set; }

        #endregion

        #region Public functions

        public new void ResetBindings()
        {
            dxGridControl.ResetBindings();
            if (AutoGenerateColumns)
            {
                dxGridView.PopulateColumns();
                BuildColumnWrappers();
                UpdateColumnsFormatting();
                BestFitColumns();
            }

            base.ResetBindings();
        }

        public void EnsureVisible(object item) {}

        ///<summary>
        /// Function to check if the current focused row is a 
        /// new row (row that's not committed to the datasource)
        ///</summary>
        ///<returns>If the focused row is a new row</returns>
        public bool OnNewRow()
        {
            return dxGridView.FocusedRowHandle.Equals(GridControl.NewItemRowHandle);
        }

        ///<summary>
        /// Clear selected rows
        ///</summary>
        public void ClearSelection()
        {
            dxGridView.ClearSelection();
        }

        /// <summary>
        /// Allows to fit all columns to their contents.
        /// </summary>
        public void BestFitColumns(bool useOnlyFirstWordOfHeader = false)
        {
            dxGridView.BestFitMaxRowCount = Columns.Count > 50 ? 10 : 50;

            if (useOnlyFirstWordOfHeader)
            {
                BestFitColumnsWithOnlyFirstWordOfHeader();
            }
            else
            {
                dxGridView.BestFitColumns();
            }
        }

        public int GetRowIndexByDataSourceIndex(int datarowIndex)
        {
            return dxGridView.GetRowHandle(datarowIndex);
        }

        /// <summary>
        /// Sets a display filter on a column. Note that it filters values on it's display text rather than the actual value.
        /// </summary>
        /// <param name="columnName">The name of the column to apply a display filter to</param>
        /// <param name="filter">The SQL-like filter expression to apply</param>
        public void SetColumnFilter(string columnName, string filter)
        {
            if (filter == string.Empty)
            {
                // Remove the filter rather than add or change it
                dxGridView.ActiveFilter.Remove(dxGridView.Columns[columnName]);
                return;
            }
            var columnFilterInfo = new ColumnFilterInfo(filter);
            dxGridView.ActiveFilter.Add(dxGridView.Columns[columnName], columnFilterInfo);
        }

        public void BeginInit()
        {
            dxGridView.BeginUpdate();
            dxGridControl.BeginUpdate();
        }

        public void EndInit()
        {
            dxGridControl.EndUpdate();
            dxGridView.EndUpdate();
        }

        public ITableViewColumn GetColumnByName(string columnName)
        {
            return Columns.FirstOrDefault(c => c.Name == columnName);
        }

        public void SelectRow(int index, bool clearPreviousSelection = true)
        {
            SelectRows(new[]
            {
                index
            }, clearPreviousSelection);
        }

        public void SelectRows(int[] indices, bool clearPreviousSelection = true)
        {
            dxGridView.BeginSelection();

            if (clearPreviousSelection)
            {
                dxGridView.ClearSelection(); //clear any previous selection
            }

            dxGridView.HideEditor(); //close any open editor

            for (int i = 0; i < indices.Length; i++)
            {
                if (i == 0)
                {
                    dxGridView.SelectionChanged -= DxGridViewSelectionChanged;
                }

                if (i == indices.Length - 1)
                {
                    dxGridView.SelectionChanged += DxGridViewSelectionChanged;
                }

                dxGridView.SelectRow(indices[i]);
            }
            dxGridView.EndSelection();
        }

        /// <summary>
        ///   Returns the index of the data source record which the specified row handle corresponds to.
        /// </summary>
        /// <param name="rowIndex">The visualized, zero-based row-index in the table</param>
        /// <returns>
        ///   An integer value representing the zero-based index of the data record to which the specified row handle corresponds
        /// </returns>
        public int GetDataSourceIndexByRowIndex(int rowIndex)
        {
            RefreshIfRequired();
            return dxGridView.GetDataSourceRowIndex(rowIndex);
        }

        private void RefreshIfRequired()
        {
            if (refreshRequired)
            {
                RefreshData();
                refreshRequired = false;
            }
        }

        public void RefreshData()
        {
            dxGridControl.RefreshDataSource();
            dxGridView.LayoutChanged();
        }

        public void ScheduleRefresh()
        {
            refreshRequired = true;
        }

        public bool CellIsReadOnly(int rowHandle, ITableViewColumn column)
        {
            // Tableview readonly?
            if (ReadOnly)
            {
                return true;
            }

            // Column does not exist or cannot be found (not visible for example)
            if (column == null)
            {
                return true;
            }

            // Column readonly?
            if (column.ReadOnly)
            {
                return true;
            }

            // Cell readonly?
            if (ReadOnlyCellFilter != null)
            {
                return ReadOnlyCellFilter(new TableViewCell(rowHandle, column));
            }

            // Cell is not read only
            return false;
        }

        /// <summary>
        /// Assign a certain value to cell. 
        /// </summary>
        /// <param name="rowIndex">Display index of the row</param>
        /// <param name="columnIndex">Display index of the column</param>
        /// <param name="value">String value to be set</param>
        public bool SetCellValue(int rowIndex, int columnIndex, object value)
        {
            tableViewValidator.AutoValidation = false;

            try
            {
                if (!SetCellValueInternal(rowIndex, columnIndex, value))
                {
                    return false;
                }

                if (!ValidateAndCommitRow(rowIndex))
                {
                    return false;
                }
            }
            finally
            {
                tableViewValidator.AutoValidation = true;
            }

            return true;
        }

        public bool SetRowCellValues(int rowIndex, int columnDisplayStartIndex, object[] cellValues)
        {
            tableViewValidator.AutoValidation = false;
            var success = true;

            try
            {
                for (int i = 0; i < cellValues.Length; i++)
                {
                    if (CellIsReadOnly(rowIndex, GetColumnByDisplayIndex(i + columnDisplayStartIndex))) // skip readonly cells, does not affect success.
                    {
                        continue;
                    }
                    success &= SetCellValueInternal(rowIndex, i + columnDisplayStartIndex, cellValues[i]);
                }

                if (!success)
                {
                    return false;
                }

                success = ValidateAndCommitRow(rowIndex);
            }
            finally
            {
                tableViewValidator.AutoValidation = true;
            }

            return success;
        }

        public object GetRowObjectAt(int rowIndex)
        {
            return dxGridView.RowCount > rowIndex && rowIndex >= 0 ? dxGridView.GetRow(rowIndex) : null;
        }

        /// <summary>
        /// Gets value of a certain cell.
        /// </summary>
        /// <param name="rowIndex">Index of the row</param>
        /// <param name="absoluteColumnIndex">Absolute index of the column</param>
        public object GetCellValue(int rowIndex, int absoluteColumnIndex)
        {
            return dxGridView.GetRowCellValue(rowIndex, dxGridView.Columns[absoluteColumnIndex]);
        }

        /// <summary>
        /// Gets value of a certain cell.
        /// </summary>
        public object GetCellValue(TableViewCell cell)
        {
            return GetCellValue(cell.RowIndex, cell.Column.AbsoluteIndex);
        }

        /// <summary>
        /// Selects cells in a square described by top,left -> bottom,right
        /// </summary>
        /// <param name="top"></param>
        /// <param name="left"></param>
        /// <param name="bottom"></param>
        /// <param name="right"></param>
        /// <param name="clearOldSelection"></param>
        public void SelectCells(int top, int left, int bottom, int right, bool clearOldSelection = true)
        {
            // dxGridControl.SuspendLayout();
            dxGridView.BeginSelection();
            if (RowSelect)
            {
                throw new InvalidOperationException("Unable to select cells when tableView has RowSelect enabled. Use SelectRow instead.");
            }

            if (clearOldSelection)
            {
                selectedCells.Clear();
            }

            for (int y = top; y <= bottom; y++)
            {
                for (int x = left; x <= right; x++)
                {
                    selectedCells.Add(new TableViewCell(y, GetColumnByDisplayIndex(x)));
                }
            }
            dxGridView.EndSelection();
            //dxGridControl.ResumeLayout();
        }

        ///<summary>
        /// Pastes clipboard contens into current selection
        ///</summary>
        public void PasteClipboardContents()
        {
            isPasting = true;
            PasteController.PasteClipboardContents();
            isPasting = false;
        }

        /// <summary>
        /// Copy original (not the display) values to the clipboard as strings
        /// </summary>
        public void CopySelectionToClipboard()
        {
            var sb = new StringBuilder();
            var areAllCellsSelected = dxGridView.GetSelectedCells().Length == (dxGridView.DataRowCount*dxGridView.VisibleColumns.Count);

            if (areAllCellsSelected && IncludeHeadersOnCopy)
            {
                CopyHeader(sb);
            }

            var selectedGridCells = dxGridView.GetSelectedCells();
            var rowIndex = -1;
            foreach (var cell in selectedGridCells)
            {
                if (rowIndex != cell.RowHandle)
                {
                    if (rowIndex != -1)
                    {
                        sb.AppendLine();
                    }
                    rowIndex = cell.RowHandle;
                }
                else
                {
                    sb.Append("\t");
                }

                sb.Append(RegionalSettingsManager.ConvertToString(
                    dxGridView.GetRowCellValue(cell.RowHandle, cell.Column), false));
            }

            if (rowIndex != -1)
            {
                sb.AppendLine();
            }

            Clipboard.Clear();
            Clipboard.SetData(DataFormats.Text, sb.ToString());
        }

        private void CopyHeader(StringBuilder sb)
        {
            var header = "";

            for (var i = 0; i < dxGridView.Columns.Count; i++)
            {
                if (header != "")
                {
                    header += "\t";
                }

                header += dxGridView.Columns[i].GetTextCaption().Replace("\n", " ");
            }

            sb.AppendLine(header);
        }

        public void SetFocus(int rowIndex, int columnIndex)
        {
            var column = GetDxColumnByDisplayIndex(columnIndex);

            dxGridView.FocusedRowHandle = rowIndex;
            dxGridView.FocusedColumn = column;

            //this is needed to get the focus visible
            dxGridView.ShowEditor();
            dxGridView.HideEditor();
        }

        public void DeleteCurrentSelection()
        {
            if (ReadOnly)
            {
                return;
            }

            dxGridView.CancelUpdateCurrentRow(); // cancel any open changes (as that may trigger validation)

            // Nothing to delete?
            if (dxGridView.GetSelectedRows().Length == 0)
            {
                return;
            }

            // Is deletion allowed?
            if (CanDeleteCurrentSelection != null && !CanDeleteCurrentSelection())
            {
                return;
            }

            //delete rows in case entire rows are selected
            var groupBy = SelectedCells.GroupBy(c => c.RowIndex);
            var count = Columns.Count(c => c.Visible);
            if (AllowDeleteRow && (RowSelect || groupBy.All(g => g.Count() == count)))
            {
                if (RowDeleteHandler == null || !RowDeleteHandler())
                {
                    dxGridView.DeleteSelectedRows();
                }
                return;
            }

            var selectedGridCells = SelectedCells.ToList();
            foreach (var c in selectedGridCells)
            {
                var defaultValue = c.Column.DefaultValue ??
                                   TypeUtils.GetDefaultValue(c.Column.ColumnType);

                SetCellValue(c.RowIndex, c.Column.DisplayIndex, Convert.ToString(defaultValue));
            }
        }

        public void AddColumn(string dataSourcePropertyName, string columnCaption, bool readOnly = false, int width = 100, Type columnType = null, string displayFormat = null)
        {
            var dxColumn = dxGridView.Columns.AddField(dataSourcePropertyName);
            dxColumn.Caption = columnCaption;
            dxColumn.VisibleIndex = dxGridView.Columns.Count - 1;
            dxColumn.OptionsColumn.ReadOnly = readOnly;
            dxColumn.Width = width;

            var column = new TableViewColumn(dxGridView, dxGridControl, dxColumn, this, false);

            if (displayFormat != null)
            {
                column.DisplayFormat = displayFormat;
            }

            Columns.Add(column);

            UpdateColumnsFormatting();
        }

        /// <summary>
        /// Adds an unbound column to the table. (Use <see cref="UnboundColumnData"/> to set values for the column)
        /// </summary>
        /// <param name="columnName">Name of the column</param>
        /// <param name="columnType">Type of the data the column is going to display</param>
        /// <param name="index">Index of the column</param>
        /// <param name="editor">Editor to use for editing the values of this column</param>
        public void AddUnboundColumn(string columnName, Type columnType, int index = -1, ITypeEditor editor = null)
        {
            var unbColumn = dxGridView.Columns.AddField(columnName);
            var column = new TableViewColumn(dxGridView, dxGridControl, unbColumn, this, true);
            Columns.Add(column);

            unbColumn.VisibleIndex = index != -1 ? index : dxGridView.Columns.Count;

            if (columnType == typeof(double))
            {
                unbColumn.UnboundType = UnboundColumnType.Decimal;
                unbColumn.DisplayFormat.FormatType = FormatType.Numeric;
            }
            else if (columnType == typeof(int))
            {
                unbColumn.UnboundType = UnboundColumnType.Integer;
                unbColumn.DisplayFormat.FormatType = FormatType.Numeric;
            }
            else if (columnType == typeof(string))
            {
                unbColumn.UnboundType = UnboundColumnType.String;
                unbColumn.DisplayFormat.FormatType = FormatType.None;
            }
            else if (columnType.IsEnum)
            {
                unbColumn.UnboundType = UnboundColumnType.Object;
            }
            else if (columnType == typeof(DateTime))
            {
                unbColumn.UnboundType = UnboundColumnType.DateTime;
                unbColumn.DisplayFormat.FormatType = FormatType.Custom;
                UpdateColumnsFormatting();
            }
            else
            {
                throw new ArgumentException(string.Format(Resources.TableView_AddUnboundColumn_Unbound_columns_of_type__0__not_supported_, columnType));
            }
            if (editor != null)
            {
                column.Editor = editor;
            }

            dxGridView.CustomUnboundColumnData -= DxGridViewCustomUnboundColumnData;
            dxGridView.CustomUnboundColumnData += DxGridViewCustomUnboundColumnData;
        }

        public string GetCellDisplayText(int rowIndex, int absoluteColumnIndex)
        {
            return dxGridView.GetDisplayTextByColumnValue(dxGridView.Columns[absoluteColumnIndex], GetCellValue(rowIndex, absoluteColumnIndex));
        }

        #endregion

        #region Public events

        public event EventHandler FocusedRowChanged;

        public event EventHandler<TableSelectionChangedEventArgs> SelectionChanged;

        public event EventHandler<EventArgs<TableViewCell>> CellChanged;

        public event EventHandler<EventArgs<ITableViewColumn>> ColumnFilterChanged;

        #endregion

        #region Internal functions

        /// <summary>
        /// Returns true when any column is sorted
        /// </summary>
        internal bool IsSorted()
        {
            return Columns.Any(c => c.SortOrder != SortOrder.None);
        }

        internal TableViewCell GetFocusedCell()
        {
            //no selection
            if (dxGridView.FocusedColumn == null)
            {
                return null;
            }
            //no row selected
            if (dxGridView.FocusedRowHandle == int.MinValue)
            {
                return null;
            }
            //'new' row..select the bottom
            if (dxGridView.FocusedRowHandle == (int.MinValue + 1))
            {
                var rowIndex = Math.Max(0, dxGridView.RowCount - 1);
                return new TableViewCell(rowIndex, GetColumnByDxColumn(dxGridView.FocusedColumn));
            }
            return new TableViewCell(dxGridView.FocusedRowHandle, GetColumnByDxColumn(dxGridView.FocusedColumn));
        }

        internal void SetColumnError(ITableViewColumn tableColumn, string errorText)
        {
            var dxGridColumn = tableColumn != null ? dxGridView.Columns[tableColumn.AbsoluteIndex] : null;
            dxGridView.SetColumnError(dxGridColumn, errorText);
        }

        internal void AddNewRowToDataSource()
        {
            if (bindingSource == null)
            {
                bindingSource = new BindingSource
                {
                    DataSource = dxGridView.DataSource
                };
            }

            if (!bindingSource.AllowNew)
            {
                Log.Debug("Adding rows to this table is not allowed");
                return;
            }
            bindingSource.AddNew();

            //end edit is needed when we bind to datatable
            if (bindingSource.DataSource is DataTable || bindingSource.DataSource is DataView)
            {
                bindingSource.EndEdit();
            }
        }

        internal ITableViewColumn GetColumnByDisplayIndex(int i)
        {
            return Columns.FirstOrDefault(c => c.DisplayIndex == i);
        }

        #endregion

        #region Gridview event handlers

        private void EmbeddedNavigatorButtonClick(object sender, NavigatorButtonClickEventArgs e)
        {
            if (e.Button != dxGridControl.EmbeddedNavigator.Buttons.Remove)
            {
                return;
            }

            // reroute delete event to "our" delete function
            e.Handled = true;
            DeleteCurrentSelection();
        }

        /// <summary>
        /// Do keyboard cursor interaction like in Excel (StackOverflow: (c)Jakob Möllås)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DxGridControlEditorKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && UserIsHoldingDownControlForCellFill())
            {
                // grab selected cells here because changing the first value may cause the 
                // data source to refresh and the selection to be lost
                cellsToFill = dxGridView.GetSelectedCells();
                return;
            }

            if (e.KeyData != Keys.Left && e.KeyData != Keys.Right)
            {
                return;
            }

            var gridControl = sender as GridControl;
            if (gridControl == null)
            {
                return;
            }

            var view = gridControl.FocusedView as GridView;
            if (view == null)
            {
                return;
            }

            var textEdit = view.ActiveEditor as TextEdit;
            if (textEdit == null)
            {
                return;
            }

            var left = e.KeyData == Keys.Left;
            var right = e.KeyData == Keys.Right;

            // Handle initial case - everything selected in control
            if ((left || right) &&
                textEdit.SelectionLength == textEdit.Text.Length &&
                textEdit.SelectionStart == 0)
            {
                textEdit.SelectionStart = left ? 0 : textEdit.Text.Length; //minor adjustment
                textEdit.SelectionLength = 0;
                e.Handled = true;
                return;
            }

            // Handle left & rightmost positions (prevent focus change)
            e.Handled = left && textEdit.SelectionStart == 0 ||
                        right && textEdit.SelectionStart == textEdit.Text.Length;
        }

        private void DxGridViewDoubleClick(object sender, EventArgs e)
        {
            OnDoubleClick(e);
        }

        private void DxGridViewCustomDrawCell(object sender, RowCellCustomDrawEventArgs e)
        {
            var tableViewColumn = GetColumnByDxColumn(e.Column);

            var buttonControl = tableViewColumn.Editor as ButtonTypeEditor;
            if (buttonControl == null || !buttonControl.HideOnReadOnly)
            {
                return;
            }

            var cellReadOnly = CellIsReadOnly(e.RowHandle, tableViewColumn);
            if (!cellReadOnly)
            {
                return;
            }

            var buttonEditViewInfo = ((GridCellInfo) e.Cell).ViewInfo as ButtonEditViewInfo;
            if ((buttonEditViewInfo == null) || (buttonEditViewInfo.RightButtons.Count != 1))
            {
                return;
            }

            e.Appearance.FillRectangle(new GraphicsCache(e.Graphics), e.Bounds);
            e.Handled = true;
        }

        private void DxGridViewColumnFilterChanged(object sender, EventArgs e)
        {
            if (ColumnFilterChanged == null)
            {
                return;
            }

            var selectedColumn = Columns.OfType<TableViewColumn>()
                                        .FirstOrDefault(c => c.DxColumn == dxGridView.FocusedColumn);

            ColumnFilterChanged(sender, new EventArgs<ITableViewColumn>(selectedColumn));
        }

        private void DxGridViewCellValueChanging(object sender, CellValueChangedEventArgs e)
        {
            Trace.WriteLine(string.Format(Resources.TableView_DxGridViewCellValueChanging_Value_Changing__0_, e.Value));
        }

        private void DxGridViewCustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
        {
            if (!showRowNumbers)
            {
                return;
            }

            // check whether the indicator cell belongs to a data row 
            if (e.Info.IsRowIndicator && e.RowHandle >= 0)
            {
                e.Info.DisplayText = (e.RowHandle + 1).ToString(CultureInfo.InvariantCulture);
                e.Info.ImageIndex = -1;
                e.Info.Appearance.TextOptions.HAlignment = HorzAlignment.Far;

                if (FocusedRowIndex == e.RowHandle)
                {
                    e.Info.ImageIndex = 0;
                }
            }
        }

        private void DxGridViewShowDxGridMenu(object sender, GridMenuEventArgs e)
        {
            bool first = true;

            if (e.MenuType == GridMenuType.Column)
            {
                ITableViewColumn tableViewColumn = null;

                if (e.HitInfo.Column != null) //on a column
                {
                    tableViewColumn = Columns.FirstOrDefault(c => c.Caption == e.HitInfo.Column.GetTextCaption());
                }

                UpdateColumnHeaderMenu(e, tableViewColumn);

                foreach (var menuItem in ColumnMenuItems.Where(menuItem => menuItem.ShouldShow(tableViewColumn)))
                {
                    if (first)
                    {
                        menuItem.InternalItem.BeginGroup = true;
                        first = false;
                    }
                    else
                    {
                        menuItem.InternalItem.BeginGroup = false;
                    }

                    e.Menu.Items.Add(menuItem.InternalItem);
                }
            }
        }

        private void DxGridView2RowCellStyle(object sender, RowCellStyleEventArgs e)
        {
            var selected = ((!RowSelect && dxGridView.IsCellSelected(e.RowHandle, e.Column)) ||
                            (RowSelect && dxGridView.IsRowSelected(e.RowHandle)));

            if (DisplayCellFilter != null)
            {
                var tableViewCellStyle = new TableViewCellStyle(e.RowHandle, GetColumnByDxColumn(e.Column), selected)
                {
                    ForeColor = e.Appearance.ForeColor,
                    BackColor = e.Appearance.BackColor
                };

                if (DisplayCellFilter(tableViewCellStyle))
                {
                    e.Appearance.ForeColor = tableViewCellStyle.ForeColor;
                    e.Appearance.BackColor = tableViewCellStyle.BackColor;
                    return;
                }
            }

            if (selected)
            {
                e.Appearance.ForeColor = Color.White;
                e.Appearance.BackColor = SystemColors.Highlight;
                return;
            }

            var isReadOnly = CellIsReadOnly(e.RowHandle, GetColumnByDxColumn(e.Column));
            if (isReadOnly)
            {
                e.Appearance.ForeColor = ReadOnlyCellForeColor;
                e.Appearance.BackColor = ReadOnlyCellBackColor;
            }

            if (InvalidCellFilter == null)
            {
                return;
            }
            if (InvalidCellFilter(new TableViewCell(e.RowHandle, GetColumnByDxColumn(e.Column))))
            {
                e.Appearance.BackColor = InvalidCellBackgroundColor;
            }
        }

        private void DxGridViewShownEditor(object sender, EventArgs e)
        {
            var view = sender as GridView;
            if (view == null)
            {
                return;
            }

            var textEditor = view.ActiveEditor as TextEdit;
            if (f2Pressed && textEditor != null && textEditor.IsEditorActive)
            {
                textEditor.SelectionStart = textEditor.Text.Length;
            }

            var buttonEditor = view.ActiveEditor as ButtonEdit;
            if (buttonEditor != null)
            {
                buttonEditor.PerformClick(null);
            }

            dxGridControl.EmbeddedNavigator.Buttons.Append.Enabled = AllowAddNewRow && !OnNewRow();

            f2Pressed = false;
        }

        private void DxGridViewShowingEditor(object sender, CancelEventArgs e)
        {
            var view = sender as GridView;
            if (view == null)
            {
                return;
            }

            e.Cancel = CellIsReadOnly(view.FocusedRowHandle, GetColumnByDxColumn(view.FocusedColumn));
        }

        private void DxGridControlProcessDxGridKey(object sender, KeyEventArgs e)
        {
            f2Pressed = e.KeyCode == Keys.F2;

            //TODO: get to switch like stament
            if (((e.Shift) && (e.KeyCode == Keys.Insert)) ||
                ((e.Control) && (e.KeyCode == Keys.V)))
            {
                //paste the glipboard if we are not editing a cell
                if ((dxGridView.OptionsBehavior.Editable) && (dxGridView.State != GridState.Editing))
                {
                    PasteClipboardContents();
                    //we need to suppres ctrl + v for being passing to child 
                    //controls. Others wise the column goes into an editing state
                    e.SuppressKeyPress = true;
                    e.Handled = true;
                }
            }
            if (((e.Control) && (e.KeyCode == Keys.Insert)) ||
                ((e.Control) && (e.KeyCode == Keys.C)))
            {
                CopySelectionToClipboard();
                e.Handled = true; //prevent XtraGrid from doing another copy resulting in an exception
            }
            if ((e.KeyCode == Keys.Delete) && (dxGridView.State == GridState.Normal))
            {
                DeleteCurrentSelection();
            }

            if (e.KeyCode == Keys.Enter && IsEndEditOnEnterKey && (dxGridView.IsEditing || OnNewRow()))
            {
                dxGridControl.EmbeddedNavigator.Buttons.DoClick(dxGridControl.EmbeddedNavigator.Buttons.EndEdit);
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void DxGridViewFocusedColumnChanged(object sender, FocusedColumnChangedEventArgs e)
        {
            if (e.FocusedColumn != null && !dxGridView.Columns.Contains(e.FocusedColumn))
            {
                // This is an awkward situation that can arise when a user clicks in the grid somewhere, after entering a value without Enter. 
                dxGridView.FocusedColumn = null;
            }

            ShowEditorIfRowSelect();
        }

        private void DxGridViewClick(object sender, EventArgs e)
        {
            var gridHitInfo = dxGridView.CalcHitInfo(PointToClient(MousePosition));

            if (gridHitInfo.Column != null && gridHitInfo.Column.ColumnEdit is RepositoryItemButtonEdit)
            {
                dxGridView.ShowEditor(); // Reduces the number of clicks needed to actually click buttons by one
            }

            if (gridHitInfo.InColumn && (!gridHitInfo.InColumnPanel && RowSelect) && gridHitInfo.Column != null)
            {
                var columnIndex = gridHitInfo.Column.VisibleIndex;
                if (((ModifierKeys & Keys.Control) == Keys.Control))
                {
                    if (!DeselectSelectCells(0, columnIndex, dxGridView.RowCount - 1, columnIndex))
                    {
                        SelectCells(0, columnIndex, dxGridView.RowCount - 1, columnIndex, false);
                    }
                }
                else
                {
                    SelectCells(0, columnIndex, dxGridView.RowCount - 1, columnIndex, true);
                }
            }

            //bubble click..
            OnClick(e);
        }

        /// <summary>
        /// Called when a cell value is validated and the new value is set to the current cell.
        /// If the user presses CTRL+ENTER the new value is copied to all selected cells. This
        /// mimics the behaviour in Excel XP. 
        /// TODO: when the user presses cancel after the CTRL+ENTER only the current cell is reset.
        /// </summary>
        private void DxGridViewCellValueChanged(object sender, CellValueChangedEventArgs e)
        {
            if (UserIsHoldingDownControlForCellFill())
            {
                // temporarily disable value changed event
                dxGridView.CellValueChanged -= DxGridViewCellValueChanged;

                try
                {
                    if (cellsToFill == null)
                    {
                        return;
                    }

                    foreach (GridCell gridcell in cellsToFill)
                    {
                        // only set new value to cell of same type
                        // todo add support for mixing double, float and int?
                        if (gridcell.Column.ColumnType == e.Value.GetType())
                        {
                            dxGridView.SetRowCellValue(gridcell.RowHandle, gridcell.Column, e.Value);
                        }
                    }
                }
                finally
                {
                    dxGridView.CellValueChanged += DxGridViewCellValueChanged;
                    cellsToFill = null;
                }
            }
            if (CellChanged != null)
            {
                CellChanged(this, new EventArgs<TableViewCell>(new TableViewCell(e.RowHandle, GetColumnByDxColumn(e.Column))));
            }
        }

        private bool UserIsHoldingDownControlForCellFill()
        {
            return MultipleCellEdit && ModifierKeys == Keys.Control && !isPasting;
        }

        private void DxGridViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!isSelectionChanging)
            {
                isSelectionChanging = true;
            }
            UpdateSelectionFromGridControl();
        }

        private void DxGridViewFocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
        {
            if (FocusedRowChanged != null)
            {
                FocusedRowChanged(sender, EventArgs.Empty);
            }
        }

        private void DxGridViewCustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
        {
            if (UnboundColumnData == null)
            {
                return;
            }

            var result = UnboundColumnData(e.Column.AbsoluteIndex, e.ListSourceRowIndex, e.IsGetData, e.IsSetData, e.Value);
            if (result != null)
            {
                e.Value = result;
            }
        }

        private void DxGridViewMouseEnter(object sender, EventArgs e)
        {
            OnMouseEnter(e);
        }

        private void DxGridViewMouseDown(object sender, MouseEventArgs e)
        {
            // Check edit single click when in cell select mode
            // http://www1.devexpress.com/Support/Center/p/Q144046.aspx
            if ((ModifierKeys & Keys.Control) != Keys.Control && (ModifierKeys & Keys.Shift) != Keys.Shift)
            {
                var hi = dxGridView.CalcHitInfo(e.Location);
                if (hi.InRowCell && hi.Column.RealColumnEdit.GetType() == typeof(RepositoryItemCheckEdit))
                {
                    dxGridView.FocusedRowHandle = hi.RowHandle;
                    dxGridView.FocusedColumn = hi.Column;
                    dxGridView.ShowEditor();

                    // get active editor or create one (when cell is readonly and no ActiveEditor is set)
                    var checkEdit = (dxGridView.ActiveEditor ?? hi.Column.RealColumnEdit.CreateEditor()) as CheckEdit;
                    if (checkEdit != null)
                    {
                        var checkInfo = (CheckEditViewInfo) checkEdit.GetViewInfo();
                        var glyphRect = checkInfo.CheckInfo.GlyphRect;

                        var viewInfo = dxGridView.GetViewInfo() as GridViewInfo;
                        if (viewInfo != null)
                        {
                            var gridGlyphRect = new Rectangle(viewInfo.GetGridCellInfo(hi).Bounds.X + glyphRect.X,
                                                              viewInfo.GetGridCellInfo(hi).Bounds.Y + glyphRect.Y,
                                                              glyphRect.Width,
                                                              glyphRect.Height);

                            dxGridView.ClearSelection();

                            if (!gridGlyphRect.Contains(e.Location))
                            {
                                dxGridView.CloseEditor();
                                if (!RowSelect)
                                {
                                    if (!dxGridView.IsCellSelected(hi.RowHandle, hi.Column))
                                    {
                                        dxGridView.SelectCell(hi.RowHandle, hi.Column);
                                    }
                                    else
                                    {
                                        dxGridView.UnselectCell(hi.RowHandle, hi.Column);
                                    }
                                }
                            }
                            else
                            {
                                checkEdit.Checked = !checkEdit.Checked;
                                dxGridView.CloseEditor();
                            }

                            if (RowSelect)
                            {
                                dxGridView.SelectRow(hi.RowHandle);
                            }
                        }
                    }

                    var dxMouseEventArgs = e as DXMouseEventArgs;
                    if (dxMouseEventArgs != null)
                    {
                        dxMouseEventArgs.Handled = true;
                    }
                }
            }

            HandleHeaderPanelButton(sender, e);
            OnMouseDown(e);
        }

        private void DxGridViewInvalidRowException(object sender, InvalidRowExceptionEventArgs e)
        {
            switch (ExceptionMode)
            {
                case ValidationExceptionMode.Ignore:
                    e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.Ignore;
                    break;
                case ValidationExceptionMode.NoAction:
                    e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.NoAction;
                    break;
                case ValidationExceptionMode.DisplayError:
                    e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.DisplayError;
                    break;
                case ValidationExceptionMode.ThrowException:
                    e.ExceptionMode = DevExpress.XtraEditors.Controls.ExceptionMode.ThrowException;
                    break;
            }
        }

        private static void DxGridControlPreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.IsInputKey = true; //do no use special processing
            }
        }

        private void DxGridViewHiddenEditor(object sender, EventArgs e)
        {
            dxGridControl.EmbeddedNavigator.Buttons.Append.Enabled = AllowAddNewRow;
        }

        private void OnDxGridViewOnFocusedRowChanged(object o, FocusedRowChangedEventArgs args)
        {
            DxGridViewFocusedRowChanged(o, args);
        }

        #endregion

        #region Initialization functions

        private void ConfigureContextMenu()
        {
            var btnCopy = new ToolStripMenuItem
            {
                Name = "btnCopy", Image = Resources.CopyHS, Size = new Size(116, 22), Text = Resources.TableView_ConfigureContextMenu_Copy, Tag = "btnCopy"
            };
            var btnPaste = new ToolStripMenuItem
            {
                Name = "btnPaste", Image = Resources.PasteHS, Size = new Size(116, 22), Text = Resources.TableView_ConfigureContextMenu_Paste, Tag = "btnPaste"
            };
            var btnDelete = new ToolStripMenuItem
            {
                Name = "btnDelete", Image = Resources.DeleteHS1, Size = new Size(116, 22), Text = Resources.TableView_ConfigureContextMenu_Delete, Tag = "btnDelete"
            };

            btnCopy.Click += delegate { CopySelectionToClipboard(); };
            btnPaste.Click += delegate { PasteClipboardContents(); };
            btnDelete.Click += delegate { DeleteCurrentSelection(); };

            var viewContextMenu = new ContextMenuStrip();
            viewContextMenu.Items.AddRange(new ToolStripItem[]
            {
                btnCopy,
                btnPaste,
                btnDelete
            });

            RowContextMenu = viewContextMenu;
        }

        private void CreateRefreshTimer()
        {
            refreshTimer = new Timer();
            refreshTimer.Tick += OnRefreshTimerOnTick;

            refreshTimer.Interval = 300;
            refreshTimer.Enabled = true;
            refreshTimer.Start();
        }

        private void OnRefreshTimerOnTick(object sender, EventArgs e)
        {
            RefreshIfRequired();
        }

        private void SubscribeEvents()
        {
            // mouse events
            dxGridView.Click += DxGridViewClick;
            dxGridView.DoubleClick += DxGridViewDoubleClick;
            dxGridView.MouseEnter += DxGridViewMouseEnter;
            dxGridView.MouseDown += DxGridViewMouseDown;

            dxGridView.InvalidRowException += DxGridViewInvalidRowException;

            // keyboard events
            dxGridControl.PreviewKeyDown += DxGridControlPreviewKeyDown;
            dxGridControl.ProcessGridKey += DxGridControlProcessDxGridKey;

            // change of focus
            dxGridView.FocusedRowChanged += OnDxGridViewOnFocusedRowChanged;
            dxGridView.FocusedRowChanged += tableViewValidator.RowLostFocus;
            dxGridView.FocusedColumnChanged += DxGridViewFocusedColumnChanged;

            // value changes
            dxGridView.CellValueChanged += tableViewValidator.OnCellValueChanged;
            dxGridView.CellValueChanged += DxGridViewCellValueChanged;
            dxGridView.CellValueChanging += DxGridViewCellValueChanging;

            // filtering events
            dxGridView.ColumnFilterChanged += DxGridViewColumnFilterChanged;

            // drawing events
            dxGridView.CustomDrawRowIndicator += DxGridViewCustomDrawRowIndicator;
            dxGridView.CustomDrawCell += DxGridViewCustomDrawCell;
            dxGridView.RowCellStyle += DxGridView2RowCellStyle;

            // editor events
            dxGridView.ShowingEditor += DxGridViewShowingEditor;
            dxGridView.ShownEditor += DxGridViewShownEditor;
            dxGridView.HiddenEditor += tableViewValidator.HiddenEditor;
            dxGridView.HiddenEditor += DxGridViewHiddenEditor;

            dxGridView.ValidatingEditor += (s, e) =>
            {
                var cell = new TableViewCell(FocusedRowIndex, GetColumnByDxColumn(dxGridView.FocusedColumn));
                tableViewValidator.OnValidateCell(cell, e);
            };
            dxGridControl.EditorKeyDown += DxGridControlEditorKeyDown;

            // selection events
            dxGridView.SelectionChanged += DxGridViewSelectionChanged;
            selectedCells.CollectionChanged += SelectedCellsCollectionChanged;

            dxGridView.ShowGridMenu += DxGridViewShowDxGridMenu;
            dxGridView.ValidateRow += tableViewValidator.OnValidateRow;
            dxGridControl.EmbeddedNavigator.ButtonClick += EmbeddedNavigatorButtonClick;

            PasteController.PasteFailed += CopyPasteControllerPasteFailed;
            columns.CollectionChanged += ColumnsOnCollectionChanged;
        }

        #endregion
    }
}