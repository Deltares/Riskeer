using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using IEditableObject = Core.Common.Utils.Editing.IEditableObject;

namespace Core.Common.Controls
{
    /// <summary>
    /// Graphical representation of tabular data
    /// </summary>
    public interface ITableView : IView, ISynchronizeInvoke
    {
        event EventHandler<TableSelectionChangedEventArgs> SelectionChanged;

        event EventHandler FocusedRowChanged;

        /// <summary>
        /// Specifies whether it is possible to remove rows 
        /// </summary>
        bool AllowDeleteRow { get; set; }

        /// <summary>
        /// Specifies whether it is possible to add new rows
        /// </summary>
        bool AllowAddNewRow { get; set; }

        /// <summary>
        /// Specifies whether it is possible to sort the columns
        /// </summary>
        bool AllowColumnSorting { get; set; }

        bool ColumnAutoWidth { get; set; }

        /// <summary>
        /// Determines whether the table view generates columns when 
        /// setting data
        /// </summary>
        bool AutoGenerateColumns { get; set; }

        /// <summary>
        /// Grid will be read only: just for displaying the data
        /// </summary>
        bool ReadOnly { get; set; }

        ///<summary>
        /// Gets a Boolean value indicating that the view is being edited or not
        ///</summary>
        bool IsEditing { get; }

        /// <summary>
        /// Allows the user to edit multiple cells simultaneously using CTRL-ENTER like Excel.
        /// </summary>
        bool MultipleCellEdit { get; set; }

        ///<summary>
        /// TODO: If RowSelect is true, gridView.GetSelectedCells etc will give an empty array.... SelectedRows will still work.
        ///</summary>
        bool RowSelect { get; set; }

        /// <summary>
        /// Gets or sets whether multiple rows can be selected
        /// </summary>
        bool MultiSelect { get; set; }

        /// <summary>
        /// Gets or sets whether column captions are copied to the clipboard, when CTRL+C is pressed
        /// </summary>
        bool IncludeHeadersOnCopy { get; set; }

        /// <summary>
        /// Number of rows in the grid excluding the newrow and filtered rows.
        /// </summary>
        int RowCount { get; }

        ///<summary>
        /// Sets and gets the zero-based index of the focused row in the visualized table.
        ///</summary>
        ///<example>A value of 1 corresponds to the 2nd entry as shown in the table (sorted, filtered, etc).</example>
        int FocusedRowIndex { get; set; }

        ///<summary>
        /// Retrieve selected row indexes (when using multi-select). 
        /// TODO: Turn this into a method!
        ///</summary>
        int[] SelectedRowsIndices { get; }

        /// <summary>
        /// Returns the focused row (also works if the focused row is new or deleted)
        /// </summary>
        object CurrentFocusedRowObject { get; }

        ///<summary>
        /// Gets or sets the disabled cell fore color
        ///</summary>
        Color ReadOnlyCellForeColor { get; set; }

        ///<summary>
        /// Gets or sets the disabled cell fore color
        ///</summary>
        Color ReadOnlyCellBackColor { get; set; }

        ///<summary>
        /// Gets or sets the background color of the invalid cell (value)
        ///</summary>
        Color InvalidCellBackgroundColor { get; set; }

        ITableViewPasteController PasteController { get; set; }

        /// <summary>
        /// Return the <see cref="ITableViewColumn"/> for the given index. 
        /// </summary>
        IList<ITableViewColumn> Columns { get; }

        /// <summary>
        /// Contains collection of selected cells. Allows to add, remove selected cells.
        /// </summary>
        IList<TableViewCell> SelectedCells { get; }

        Func<TableViewCell, object, Utils.Tuple<string, bool>> InputValidator { get; set; }

        /// <summary>
        /// Gets or sets the row values validator. The validator should return a RowValidationResult
        /// </summary>
        Func<int, object[], IRowValidationResult> RowValidator { get; set; }

        /// <summary>
        /// Allow the pinning of columns (default is true)
        /// </summary>
        bool AllowColumnPinning { get; set; }

        /// <summary>
        /// Commit row at enter.
        /// </summary>
        bool IsEndEditOnEnterKey { get; set; }

        /// <summary>
        /// If data is not editable object it's owner is, this should be set to it.
        /// </summary>
        IEditableObject EditableObject { get; set; }

        /// <summary>
        /// Allows to fit all columns to their contents.
        /// </summary>
        void BestFitColumns(bool useOnlyFirstWordOfHeader = true);

        /// <summary>
        ///  Select row on index
        /// </summary>
        /// <param name="index">index</param>
        /// <param name="clearPreviousSelection"></param>
        void SelectRow(int index, bool clearPreviousSelection = true);

        ///<summary>
        /// Select multiple rows on index
        ///</summary>
        void SelectRows(int[] indices, bool clearPreviousSelection = true);

        /// <summary>
        /// Updates the SelectedCells to a rectangle top,left,bottom,right
        /// </summary>
        void SelectCells(int top, int left, int bottom, int right, bool clearOldSelection = true);

        /// <summary>
        /// Clears the current selection
        /// </summary>
        void ClearSelection();

        /// <summary>
        /// Deletes the content of the current selection
        /// </summary>
        void DeleteCurrentSelection();

        /// <summary>
        /// Set the value of a certain cell
        /// </summary>
        bool SetCellValue(int rowIndex, int columnIndex, object value);

        /// <summary>
        /// Sets a number of values in a certain row. Can be faster than SetCellValue because validation and end edit is done after 
        /// all values are set.
        /// </summary>
        bool SetRowCellValues(int rowIndex, int columnDisplayStartIndex, object[] cellValues);

        /// <summary>
        /// Updates grid cells using current data source.
        /// </summary>
        void RefreshData();

        /// <summary>
        /// Sets <c>render required </c>to true. Updates are done by timer
        /// </summary>
        void ScheduleRefresh();

        ///<summary>
        /// Converts the displayRowIndex (used in events) to the dataSourceRowIndex
        ///</summary>
        ///<param name="rowIndex">The zero-based index of row in the table</param>
        ///<returns>The matching zero-based index in the data source</returns>
        int GetDataSourceIndexByRowIndex(int rowIndex);

        ///<summary>
        /// Converts the zero-based data source index to the zero-based index of the row in the table
        ///</summary>
        ///<param name="dataSourceIndex">The zero based index in the data source</param>
        ///<returns>The zero-based index as visualized in the table</returns>
        int GetRowIndexByDataSourceIndex(int dataSourceIndex);

        /// <summary>
        /// Gets value of a certain cell.
        /// </summary>
        /// <param name="rowIndex">Row index of the cell</param>
        /// <param name="absoluteColumnIndex">Absolute index of the column</param>
        object GetCellValue(int rowIndex, int absoluteColumnIndex);

        /// <summary>
        /// Gets value of a certain cell.
        /// </summary>
        object GetCellValue(TableViewCell cell);

        ITableViewColumn GetColumnByName(string columnName);
    }
}