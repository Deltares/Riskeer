using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Controls.Swf.Editors;
using DelftTools.Controls.Swf.Properties;
using DelftTools.Controls.Swf.Table.Filtering;
using DelftTools.Utils;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Collections.Generic;
using DelftTools.Utils.Editing;
using DelftTools.Utils.Reflection;
using log4net;
using IEditableObject = DelftTools.Utils.Editing.IEditableObject;
using Image = System.Drawing.Image;
using UserControl = System.Windows.Forms.UserControl;

namespace DelftTools.Controls.Swf.Table
{
    public partial class TableView2 : UserControl , ITableView
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TableView2));
        private readonly EventedList<ITableViewColumn> columns;
        private readonly EventedList<TableViewCell> selectedCells;

        private int mouseOverColumnHeaderIndex = -2;
        private bool updatingSelection;
        internal bool ColumnSyncDisabled; 
        private object dataSource;
        private readonly FilterTableColumnControl filterColumnControl = new FilterTableColumnControl{Visible = false};

        public TableView2()
        {
            InitializeComponent();

            columns = new EventedList<ITableViewColumn>();
            selectedCells = new EventedList<TableViewCell>();

            SetStandardSettings();

            Controls.Add(filterColumnControl);
            filterColumnControl.BringToFront();
            
            dataGridView.ColumnAdded += DataGridViewOnColumnAdded;
            dataGridView.ColumnRemoved += DataGridViewOnColumnRemoved;
            dataGridView.SelectionChanged += DataGridViewSelectionChanged;
            dataGridView.DataError += DataGridViewDataError;
            dataGridView.KeyDown += DataGridViewKeyDown;
            dataGridView.CellFormatting += DataGridViewCellFormatting;
            dataGridView.CellPainting += DataGridViewCellPainting;
            dataGridView.CellMouseClick += DataGridViewOnCellMouseClick;
            dataGridView.CellMouseEnter += DataGridViewCellMouseEnter;
            dataGridView.CellMouseLeave += DataGridViewOnCellMouseLeave;
            dataGridView.CellValueChanged += DataGridViewCellValueChanged;
            dataGridView.Resize += DataGridViewResize;
            dataGridView.Click += DataGridViewClick;
            dataGridView.RowPostPaint += DataGridViewRowPostPaint;
            dataGridView.ColumnHeaderMouseClick += DataGridViewColumnHeaderMouseClick;
            dataGridView.CellParsing += DataGridViewCellParsing;
        }

        #region public properties

        public DataGridView DataGridView { get { return dataGridView; }}

        public object Data
        {
            get { return dataSource; }
            set
            {
                dataGridView.CancelEdit();

                dataSource = value;

                ColumnSyncDisabled = true;
                
                bindingSource.DataSource = dataSource;

                BuildColumnWrappers();
                AddEnumCellEditors();

                dataGridView.ClearSelection();

                ColumnSyncDisabled = false;
            }
        }

        public Image Image { get; set; }

        public ViewInfo ViewInfo { get; set; }

        public bool AllowDeleteRow
        {
            get { return bindingSource.AllowRemove && dataGridView.AllowUserToDeleteRows; }
            set { dataGridView.AllowUserToDeleteRows = value; }
        }

        public bool AllowAddNewRow
        {
            get { return bindingSource.AllowNew && dataGridView.AllowUserToAddRows; } 
            set { dataGridView.AllowUserToAddRows = value; }
        }

        public bool AllowColumnSorting { get; set; }

        public bool ColumnAutoWidth
        {
            get { return dataGridView.AutoSizeColumnsMode == DataGridViewAutoSizeColumnsMode.Fill; } 
            set
            {
                dataGridView.AutoSizeColumnsMode = value
                    ? DataGridViewAutoSizeColumnsMode.Fill
                    : DataGridViewAutoSizeColumnsMode.None;
            }
        }

        public bool AutoGenerateColumns
        {
            get { return dataGridView.AutoGenerateColumns; } 
            set { dataGridView.AutoGenerateColumns = value; }
        }

        public bool ReadOnly
        {
            get { return dataGridView.ReadOnly; }
            set { dataGridView.ReadOnly = value; }
        }

        public bool IsEditing
        {
            get { return dataGridView.IsCurrentCellInEditMode; }
        }
        
        public bool MultipleCellEdit { get; set; }

        public bool RowSelect
        {
            get { return dataGridView.SelectionMode == DataGridViewSelectionMode.FullRowSelect; } 
            set
            {
                dataGridView.SelectionMode = value
                    ? DataGridViewSelectionMode.FullRowSelect
                    : DataGridViewSelectionMode.CellSelect;
            }
        }

        public bool MultiSelect
        {
            get { return dataGridView.MultiSelect; } 
            set { dataGridView.MultiSelect = value; }
        }

        public bool IncludeHeadersOnCopy
        {
            get { return dataGridView.ClipboardCopyMode == DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText; }
            set
            {
                dataGridView.ClipboardCopyMode = value
                    ? DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText
                    : DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            }
        }

        public int RowCount
        {
            get { return dataGridView.RowCount; }
        }

        public int FocusedRowIndex
        {
            get { return dataGridView.CurrentCellAddress.Y; }
            set { dataGridView.CurrentCell = dataGridView[0, value]; }
        }
        
        public int[] SelectedRowsIndices 
        {
            get { return dataGridView.SelectedRows.Cast<DataGridViewRow>().Select(r => r.Index).ToArray(); } 
        }
        
        public object CurrentFocusedRowObject 
        { 
            get
            {
                var dataGridViewRow = dataGridView.CurrentRow;
                return dataGridViewRow != null ? dataGridViewRow.DataBoundItem : null;
            }
        }
        
        public Color ReadOnlyCellForeColor { get; set; }
        
        public Color ReadOnlyCellBackColor { get; set; }
        
        public Color InvalidCellBackgroundColor { get; set; }
        
        public ITableViewPasteController PasteController { get; set; }

        public IList<ITableViewColumn> Columns
        {
            get { return columns; }
        }

        public IList<TableViewCell> SelectedCells
        {
            get { return selectedCells; }
        }
        
        /// <summary>
        /// Gets or sets the function for deleting selected rows (return value indicates if the deleting is handled)
        /// When null or returning false the default delete will be executed
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<bool> RowDeleteHandler { get; set; }

        /// <summary>
        /// Gets or sets the function that checks if the current selection can be deleted.
        /// When null or returning true, deletion is allowed.
        /// </summary>
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<bool> CanDeleteCurrentSelection { get; set; }
        
        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<TableViewCell, object, Utils.Tuple<string, bool>> InputValidator { get; set; }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public Func<int, object[], IRowValidationResult> RowValidator { get; set; }

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

        public bool AllowColumnPinning { get; set; }
        
        public bool IsEndEditOnEnterKey { get; set; }
        
        public IEditableObject EditableObject { get; set; }

        public int[] SelectedColumnsIndices
        {
            get 
            {
                return selectedCells.Where(c => c.Column != null)
                .Select(c => c.Column.AbsoluteIndex)
                .OrderBy(i => i)
                .Distinct().ToArray(); 
            }
        }

        public bool ShowRowNumbers { get; set; }

        public bool UseCenteredHeaderText
        {
            get
            {
                return dataGridView.ColumnHeadersDefaultCellStyle.Alignment == DataGridViewContentAlignment.MiddleCenter;
            }
            set
            {
                dataGridView.ColumnHeadersDefaultCellStyle.Alignment = value
                    ? DataGridViewContentAlignment.MiddleCenter
                    : DataGridViewContentAlignment.MiddleLeft;
            }
        }

        #endregion

        #region public methods

        public new void ResetBindings()
        {
            dataGridView.ResetBindings();
            bindingSource.ResetBindings(true);
            if (AutoGenerateColumns)
            {
                BuildColumnWrappers();
                BestFitColumns();
            }

            base.ResetBindings();
        }

        public void EnsureVisible(object item) {}

        public void BestFitColumns(bool useOnlyFirstWordOfHeader = true)
        {
            dataGridView.AutoResizeColumns(DataGridViewAutoSizeColumnsMode.AllCells);
        }

        public void SelectRow(int index, bool clearPreviousSelection = true)
        {
            SelectRows(new []{index}, clearPreviousSelection);
        }

        public void SelectRows(int[] indices, bool clearPreviousSelection = true)
        {
            updatingSelection = true;

            if (clearPreviousSelection)
            {
                dataGridView.ClearSelection();
            }

            foreach (var index in indices)
            {
                dataGridView.Rows[index].Cells.OfType<DataGridViewCell>().ForEach(c => c.Selected = true);                
            }

            updatingSelection = false;

            UpdateSelectionFromGridControl();
        }

        public void SelectCells(int top, int left, int bottom, int right, bool clearOldSelection = true)
        {
            if (RowSelect)
            {
                throw new InvalidOperationException("Unable to select cells when tableView has RowSelect enabled. Use SelectRow instead.");
            }

            updatingSelection = true;

            if (clearOldSelection)
            {
                dataGridView.ClearSelection();
            }

            var rowIndices = Enumerable.Range(top, bottom - top + 1);
            var columnIndices = Enumerable.Range(left, right - left + 1).Select(i => i + GetNumberOfInvisibleColumnsBefore(i));
            var dataGridViewCells = dataGridView.Rows.Cast<DataGridViewRow>()
                                        .Where(r => rowIndices.Contains(r.Index))
                                        .SelectMany(row => row.Cells.Cast<DataGridViewCell>()
                                                                .Where(c => columnIndices.Contains(c.ColumnIndex)));
            foreach (var cell in dataGridViewCells)
            {
                cell.Selected = true;
            }

            updatingSelection = false;
            UpdateSelectionFromGridControl();
        }

        public void ClearSelection()
        {
            dataGridView.ClearSelection();
        }

        public void DeleteCurrentSelection()
        {
            if (ReadOnly)
            {
                return;
            }

            dataGridView.CancelEdit(); // cancel any open changes (as that may trigger validation)

            // Nothing to delete?
            if (dataGridView.SelectedCells.Count == 0) return;

            // Is deletion allowed?
            if (CanDeleteCurrentSelection != null && !CanDeleteCurrentSelection()) return;

            DoActionInEditAction(this, "Deleting selection", () =>
            {
                //delete rows in case entire rows are selected
                var groupBy = SelectedCells.GroupBy(c => c.RowIndex);
                var count = Columns.Count(c => c.Visible);

                if (AllowDeleteRow && (RowSelect || groupBy.All(g => g.Count() == count)))
                {
                    var deletedUsingRowDeleteHandler = RowDeleteHandler != null && RowDeleteHandler();
                    if (deletedUsingRowDeleteHandler)
                    {
                        ResetBindings();
                        return;
                    }
                    
                    var dataGridViewRows = dataGridView.SelectedCells.OfType<DataGridViewCell>()
                        .Select(c => c.OwningRow).Distinct().Where(r => !r.IsNewRow).ToList();
                    foreach (var row in dataGridViewRows)
                    {
                        dataGridView.Rows.Remove(row);
                    }

                    return;
                }

                var selectedGridCells = SelectedCells.ToList();
                foreach (var c in selectedGridCells)
                {
                    var defaultValue = c.Column.DefaultValue ?? TypeUtils.GetDefaultValue(c.Column.ColumnType);
                    SetCellValue(c.RowIndex, c.Column.AbsoluteIndex, defaultValue);
                }
            });
        }

        public bool SetCellValue(int rowIndex, int columnIndex, object value)
        {
            var realDisplayColumnIndex = columnIndex + GetNumberOfInvisibleColumnsBefore(columnIndex);
            if (CellIsReadOnly(rowIndex, Columns.First(c => c.DisplayIndex == realDisplayColumnIndex))) return false;
            
            dataGridView.Rows[rowIndex].Cells[realDisplayColumnIndex].Value = value;
            return true;
        }

        public bool SetRowCellValues(int rowIndex, int columnDisplayStartIndex, object[] cellValues)
        {
            throw new NotImplementedException();
        }

        public string GetCellDisplayText(int rowIndex, int absoluteColumnIndex)
        {
            var formattedValue = dataGridView.Rows[rowIndex].Cells[absoluteColumnIndex].FormattedValue;
            return formattedValue != null ? formattedValue.ToString() : "";
        }

        public void RefreshData()
        {
            dataGridView.ResetBindings();
        }

        public void ScheduleRefresh()
        {
            
        }

        public int GetDataSourceIndexByRowIndex(int rowIndex)
        {
            var dataObject = dataGridView.Rows[rowIndex].DataBoundItem;
            return bindingSource.CurrencyManager.List.IndexOf(dataObject);
        }

        public int GetRowIndexByDataSourceIndex(int dataSourceIndex)
        {
            var dataObject = bindingSource.CurrencyManager.List[dataSourceIndex];
            return bindingSource.IndexOf(dataObject);
        }

        public object GetCellValue(int rowIndex, int absoluteColumnIndex)
        {
            return dataGridView.Rows[rowIndex].Cells[absoluteColumnIndex].Value;
        }

        public object GetCellValue(TableViewCell cell)
        {
            return GetCellValue(cell.RowIndex,cell.Column.AbsoluteIndex);
        }

        public ITableViewColumn GetColumnByName(string columnName)
        {
            return Columns.FirstOrDefault(c => c.Name == columnName);
        }

        public ITableViewColumn AddUnboundColumn(string columnName, Type columnType, int index = -1, ITypeEditor editor = null)
        {
            var column = new DataGridViewColumn(new DataGridViewTextBoxCell()) {Name = columnName, HeaderText = columnName, ValueType = columnType};

            if (index == -1)
            {
                dataGridView.Columns.Add(column);
            }
            else
            {
                dataGridView.Columns.Insert(index, column);
            }

            var tableviewColumn = Columns.OfType<TableViewColumn2>().First(c => c.Column == column);
            tableviewColumn.Editor = editor;
            tableviewColumn.IsUnbound = true;

            return tableviewColumn;
        }

        public ITableViewColumn AddColumn(string dataSourcePropertyName, string columnCaption)
        {
            return AddColumn(dataSourcePropertyName, columnCaption, false, 100);
        }

        public ITableViewColumn AddColumn(string dataSourcePropertyName, string columnCaption, bool readOnly, int width, Type columnType = null, string displayFormat = null)
        {
            var columnIndex = dataGridView.Columns.Add(dataSourcePropertyName,columnCaption);
            var tableViewColumn = GetColumnByIndex(columnIndex);

            tableViewColumn.ReadOnly = readOnly;
            tableViewColumn.Column.DataPropertyName = dataSourcePropertyName;
            tableViewColumn.Column.Width = width;
            tableViewColumn.Column.ValueType = columnType;
            tableViewColumn.DisplayFormat = displayFormat;

            return tableViewColumn;
        }

        #endregion

        public event EventHandler<EventArgs<TableViewCell>> CellChanged;

        public event EventHandler<TableSelectionChangedEventArgs> SelectionChanged;

        public event EventHandler FocusedRowChanged;

        public event EventHandler<EventArgs<ITableViewColumn>> ColumnFilterChanged;

        internal void RaiseColumnFilterChanged(ITableViewColumn tableViewColumn)
        {
            if (ColumnFilterChanged == null) return;

            ColumnFilterChanged(this, new EventArgs<ITableViewColumn>(tableViewColumn));
        }

        private int GetNumberOfInvisibleColumnsBefore(int index)
        {
            var count = 0;
            for (int i = 0; i < index + 1; i++)
            {
                if (!columns[i].Visible)
                {
                    index++;
                    count++;
                }
            }
            return count;
        }

        public bool CellIsReadOnly(int rowIndex, ITableViewColumn column)
        {
            // Tableview readonly?
            if (ReadOnly) return true;

            // Column does not exist or cannot be found (not visible for example)
            if (column == null) return true;

            // Column readonly?
            if (column.ReadOnly) return true;

            // Cell readonly?
            if (ReadOnlyCellFilter != null)
            {
                return ReadOnlyCellFilter(new TableViewCell(rowIndex, column));
            }

            // Cell is not read only
            return false;
        }

        private void AddEnumCellEditors()
        {
            foreach (var column in Columns.Where(c => c.ColumnType.IsEnum && c.Editor == null))
            {
                column.Editor = new ComboBoxTypeEditor
                    {
                        Items = Enum.GetValues(column.ColumnType),
                        ItemsMandatory = true,
                        CustomFormatter = new EnumFormatter(column.ColumnType)
                    };
            }
        }

        private void SetStandardSettings()
        {
            AllowColumnPinning = true;
            AllowDeleteRow = true;
            AutoGenerateColumns = true;
            AllowColumnSorting = true;
            MultipleCellEdit = true;
            dataGridView.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridView.AutoGenerateColumns = true;
            ReadOnlyCellForeColor = Color.Black; //just use black as  a default to increase readability
            ReadOnlyCellBackColor = Color.FromArgb(255, 244, 244, 244);
            InvalidCellBackgroundColor = Color.Tomato;
        }

        private void DataGridViewCellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (CellChanged == null) return;

            CellChanged(this, new EventArgs<TableViewCell>(new TableViewCell(e.RowIndex, GetColumnByIndex(e.ColumnIndex))));
        }

        private void DataGridViewClick(object sender, EventArgs e)
        {
            filterColumnControl.Visible = false;
        }

        private void DataGridViewResize(object sender, EventArgs e)
        {
            if (filterColumnControl.Visible && filterColumnControl.Right > Right)
            {
                filterColumnControl.Location = new Point(Right - 5 - filterColumnControl.Width, filterColumnControl.Location.Y);
            }
        }

        private void DataGridViewCellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            var column = GetColumnByIndex(e.ColumnIndex);
            if (column == null) return;
            
            if (column.IsUnbound)
            {
                e.Value = UnboundColumnData != null
                    ? UnboundColumnData(column.AbsoluteIndex, e.RowIndex, true, false,null)
                    : null;
            }

            if (column.CustomFormatter != null)
            {
                e.Value = column.CustomFormatter.Format(null, e.Value, null);
                e.FormattingApplied = true;
            }

            if (!e.FormattingApplied && column.DisplayFormat != null && column.Column.ValueType == typeof(string))
            {
                e.Value = string.Format(column.DisplayFormat, e.Value);
                e.FormattingApplied = true;
            }
        }

        private void DataGridViewCellParsing(object sender, DataGridViewCellParsingEventArgs e)
        {
            var column = GetColumnByIndex(e.ColumnIndex);
            if (column == null) return;

            if (column.IsUnbound)
            {
                UnboundColumnData(column.AbsoluteIndex, e.RowIndex, false, true, e.Value);
                e.ParsingApplied = true;
            }
        }

        private void DataGridViewOnCellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = GetColumnByIndex(e.ColumnIndex);
            if (column == null) return;

            var buttonTypeEditor = column.Editor as ButtonTypeEditor;
            if (buttonTypeEditor != null)
            {
                buttonTypeEditor.ButtonClickAction();
            }
            
            if (e.RowIndex != -1 && e.Button == MouseButtons.Right)
            {
                contextMenuStrip1.Show(this, e.Location);
            }
        }

        private void DataGridViewColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            var column = GetColumnByIndex(e.ColumnIndex);
            if (column != null && e.Button == MouseButtons.Left && OnHeaderFilterButton(e.Location, e.ColumnIndex, e.RowIndex))
            {
                ShowFilterControl(column);
                return;
            }

            if (!AllowColumnSorting || (column != null && !column.SortingAllowed) || e.Button != MouseButtons.Left) return;

            var gridViewColumn = dataGridView.Columns[e.ColumnIndex];
            var sameColumn = dataGridView.SortedColumn == gridViewColumn;

            var sortOrder = (sameColumn && dataGridView.SortOrder == SortOrder.Ascending
                ? SortOrder.Descending
                : (sameColumn && dataGridView.SortOrder == SortOrder.Descending
                    ? SortOrder.None
                    : SortOrder.Ascending));

            if (sortOrder == SortOrder.Ascending || sortOrder == SortOrder.Descending)
            {
                var listSortDirection = sortOrder == SortOrder.Ascending
                    ? ListSortDirection.Ascending
                    : ListSortDirection.Descending;
                
                dataGridView.Sort(gridViewColumn,listSortDirection);
            }
            else
            {
                bindingSource.RemoveSort();
            }

            gridViewColumn.HeaderCell.SortGlyphDirection = sortOrder;
        }

        private void DataGridViewSelectionChanged(object sender, EventArgs e)
        {
            UpdateSelectionFromGridControl();
        }

        private void DataGridViewOnColumnAdded(object sender, DataGridViewColumnEventArgs e)
        {
            if (ColumnSyncDisabled) return;
            Columns.Add(CreateTableViewColumn(e.Column));
        }

        private void DataGridViewOnColumnRemoved(object sender, DataGridViewColumnEventArgs e)
        {
            if (ColumnSyncDisabled) return;

            var column = columns.OfType<TableViewColumn2>().FirstOrDefault(c => c.Column == e.Column);
            if (column == null) return;
            columns.Remove(column);
        }

        private void DataGridViewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && !IsEditing)
            {
                DeleteCurrentSelection();
                e.Handled = true;
            }
        }

        private void DataGridViewDataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            Log.ErrorFormat("Error occurred : {0}", e.Exception.Message);
            e.ThrowException = false;
        }

        private void DataGridViewCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (CellIsReadOnly(e.RowIndex, GetColumnByIndex(e.ColumnIndex)))
            {
                e.CellStyle.BackColor = ReadOnlyCellBackColor;
                e.CellStyle.ForeColor = ReadOnlyCellForeColor;
            }

            if (DisplayCellFilter != null)
            {
                var selected = e.RowIndex != -1 && e.ColumnIndex != -1 && dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Selected;

                var tableViewCellStyle = new TableViewCellStyle(e.RowIndex, GetColumnByIndex(e.ColumnIndex), selected)
                    {
                        ForeColor = e.CellStyle.ForeColor,
                        BackColor = e.CellStyle.BackColor
                    };

                if (DisplayCellFilter(tableViewCellStyle))
                {
                    e.CellStyle.ForeColor = tableViewCellStyle.ForeColor;
                    e.CellStyle.BackColor = tableViewCellStyle.BackColor;
                }
                return;
            }

            if (!bindingSource.SupportsFiltering || e.RowIndex != -1 || (!HasFilter(e.ColumnIndex) && mouseOverColumnHeaderIndex != e.ColumnIndex)) return;

            e.Paint(e.CellBounds, DataGridViewPaintParts.All);

            // Add filter Glyph
            using (var image = Resources.funnel_small)
            {
                e.Graphics.DrawImage(image, GetFilterImageBounds(e.CellBounds, e.ColumnIndex));
            }
            
            e.Handled = true;
        }

        private void DataGridViewRowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            if (!ShowRowNumbers) return;

            // Add row numbers
            using (var brush = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString((e.RowIndex).ToString(CultureInfo.InvariantCulture.NumberFormat),
                    e.InheritedRowStyle.Font, brush, e.RowBounds.Location.X + 15,
                    e.RowBounds.Location.Y + 4);
            }
        }

        private void DataGridViewOnCellMouseLeave(object sender, DataGridViewCellEventArgs dataGridViewCellEventArgs)
        {
            mouseOverColumnHeaderIndex = -2;
        }

        private void DataGridViewCellMouseEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex != -1 || e.ColumnIndex == -1) return;
            mouseOverColumnHeaderIndex = e.ColumnIndex;
        }

        private void DoActionInEditAction(ITableView tableView, string actionName, Action action)
        {
            var editableObject = tableView.EditableObject /*?? tableView.Data as IEditableObject*/;
            if (editableObject != null)
            {
                editableObject.BeginEdit(new DefaultEditAction(actionName));
            }

            try
            {
                action();
            }
            finally
            {
                if (editableObject != null)
                {
                    editableObject.EndEdit();
                }
            }
        }

        private TableViewColumn2 GetColumnByIndex(int columnIndex)
        {
            return Columns.OfType<TableViewColumn2>().FirstOrDefault(c => c.AbsoluteIndex == columnIndex);
        }

        private bool OnHeaderFilterButton(Point location, int columnIndex, int rowIndex)
        {
            var cellBounds = dataGridView.GetCellDisplayRectangle(columnIndex, rowIndex, false);
            var filterImageBounds = GetFilterImageBounds(cellBounds, columnIndex);

            var point = new Point(location.X + cellBounds.X, location.Y + cellBounds.Y);

            return filterImageBounds.Contains(point);
        }

        private void ShowFilterControl(ITableViewColumn tableViewColumn)
        {
            if (!bindingSource.SupportsFiltering) return;

            var cellDisplayRectangle = dataGridView.GetCellDisplayRectangle(tableViewColumn.AbsoluteIndex,-1,false);

            filterColumnControl.TableViewColumn = tableViewColumn;
            filterColumnControl.Location = new Point(cellDisplayRectangle.Right - 5, cellDisplayRectangle.Top + cellDisplayRectangle.Height/2);

            if (filterColumnControl.Right > Right)
            {
                filterColumnControl.Location = new Point(Right -5 - filterColumnControl.Width, cellDisplayRectangle.Top + cellDisplayRectangle.Height/2);
            }

            filterColumnControl.Visible = true;
        }

        private bool HasFilter(int columnIndex)
        {
            return !string.IsNullOrEmpty(bindingSource.Filter) && columnIndex != -1 && !string.IsNullOrEmpty(Columns.First(c => c.AbsoluteIndex == columnIndex).FilterString);
        }

        private Rectangle GetFilterImageBounds(Rectangle cellBounds, int columnIndex)
        {
            var sorted = dataGridView.SortedColumn != null && dataGridView.SortedColumn.Index == columnIndex;
            var offset = !sorted ? cellBounds.Right - 19 : cellBounds.Right - 33;
            return new Rectangle(offset, cellBounds.Top + 1, 16, 16);
        }

        private void UpdateSelectionFromGridControl()
        {
            if (updatingSelection)
            {
                return;
            }

            updatingSelection = true;

            selectedCells.Clear();

            var gridViewSelectedCells = dataGridView.SelectedCells.OfType<DataGridViewCell>();
            foreach (var cell in gridViewSelectedCells)
            {
                selectedCells.Add(new TableViewCell(cell.RowIndex , Columns.FirstOrDefault(c => c.AbsoluteIndex == cell.ColumnIndex)));
            }

            if (!IsEditing /*&& !isPasting*/)
            {
                if (SelectionChanged != null)
                {
                    SelectionChanged(this, new TableSelectionChangedEventArgs(selectedCells.ToArray()));
                }
            }

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

            foreach (DataGridViewColumn dataGridViewColumn in dataGridView.Columns)
            {
                Columns.Add(CreateTableViewColumn(dataGridViewColumn));
            }

            columns.CollectionChanged += ColumnsOnCollectionChanged;
        }

        private TableViewColumn2 CreateTableViewColumn(DataGridViewColumn dataGridViewColumn)
        {
            dataGridViewColumn.SortMode = DataGridViewColumnSortMode.Programmatic;
            return new TableViewColumn2(dataGridViewColumn, this)
                {
                    Caption = GetColumnCaption(dataGridViewColumn)
                };
        }

        private string GetColumnCaption(DataGridViewColumn tableViewColumn)
        {
            // copy caption for DataTable columns
            var dataTable = bindingSource.DataSource as DataTable;
            return dataTable == null 
                ? tableViewColumn.HeaderText 
                : dataTable.Columns[tableViewColumn.Name].Caption;
        }

        private void ColumnsOnCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (e.Action == NotifyCollectionChangeAction.Remove && dataGridView.Columns.Count > e.Index)
            {
                dataGridView.Columns.RemoveAt(e.Index);
            }
        }

        private void pinColumnToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var hi = dataGridView.HitTest(contextMenuStrip1.Location.X - dataGridView.Location.X, contextMenuStrip1.Location.Y - dataGridView.Location.Y);
            var column = columns.FirstOrDefault(c => c.AbsoluteIndex == hi.ColumnIndex);
            if (column == null) return;
            
            column.Pinned = !column.Pinned;
        }

        internal void UpdateFilter()
        {
            if (bindingSource == null) return;
            bindingSource.Filter = string.Join(" AND ", Columns.Where(c => !string.IsNullOrEmpty(c.FilterString)).Select(c => c.FilterString));
        }

        private void dataGridView_AllowUserToAddRowsChanged(object sender, EventArgs e)
        {
            bindingNavigatorAddNewItem.Enabled = dataGridView.AllowUserToAddRows;
        }

        private void dataGridView_AllowUserToDeleteRowsChanged(object sender, EventArgs e)
        {
            bindingNavigatorDeleteItem.Enabled = dataGridView.AllowUserToDeleteRows;
        }

        public void SetFocus(int rowIndex, int columnIndex)
        {
            dataGridView.CurrentCell = dataGridView.Rows[rowIndex].Cells[columnIndex];
        }
    }
}
