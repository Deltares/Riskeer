using System;
using System.Linq;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace DelftTools.Controls.Swf.Table.Validation
{
    internal class TableViewValidator
    {
        private readonly TableView tableView;

        public TableViewValidator(TableView tableView)
        {
            this.tableView = tableView;
            AutoValidation = true;
        }

        private object[] originalRowValues;
        private bool editingRow;
        private int currentRowIndex;

        private void BeginEditRow(int rowIndex)
        {
            if (currentRowIndex >= 0 && rowIndex != currentRowIndex)
            {
                EndEditRow();
            }

            if (editingRow) return;

            currentRowIndex = rowIndex;

            editingRow = true;
            
            originalRowValues = GetRowValues();
        }

        private object[] GetRowValues()
        {
            return tableView.Columns
                .Where(column => column.Visible)
                .Select(c => tableView.GetCellValue(currentRowIndex, c.AbsoluteIndex))
                .ToArray();
        }

        private void EndEditRow()
        {
            if (!editingRow) return;

            editingRow = false;
            currentRowIndex = -1;
            originalRowValues = null;
        }
        
        private object[] GetOriginalRowValues()
        {
            if (editingRow)
            {
                return originalRowValues;
            }

            return GetRowValues(); //otherwise just get the values directly
        }

        public bool ValidateRow(int rowIndex, out string firstError)
        {
            firstError = "";
            BeginEditRow(rowIndex);

            if (tableView.RowValidator != null)
            {
                //clear all errors first
                foreach (var column in tableView.Columns)
                {
                    tableView.SetColumnError(column, "");
                }

                tableView.SetColumnError(null, ""); //clear row error

                var result = tableView.RowValidator(rowIndex, GetRowValues());
                
                if (!result.Valid)
                {
                    firstError = "Validation of row failed: " + result.ErrorText;
                    tableView.SetColumnError(result.ColumnIndex == -1 ? null : tableView.Columns[result.ColumnIndex],
                                        result.ErrorText);
                    return false;
                }
            }

            return true;
        }

        public bool ValidateCell(TableViewCell cell, object newValue, out string error)
        {
            BeginEditRow(cell.RowIndex);

            error = "";

            if (tableView.InputValidator != null)
            {
                var result = tableView.InputValidator(cell, newValue);
                if (!result.Second)
                {
                    error = "Validation of cell failed: " + result.First;
                    return false;
                }
            }
            return true;
        }

        public void RefreshRowData()
        {
            EndEditRow();
        }

        #region Event Handlers
        
        public void RowLostFocus(object sender, FocusedRowChangedEventArgs e)
        {
            if (e.FocusedRowHandle != int.MinValue && e.FocusedRowHandle != currentRowIndex)
            {
                EndEditRow();
            }
        }

        public bool AutoValidation { get; set; }

        public void OnCellValueChanged(object sender, CellValueChangedEventArgs e) //after cell value changed, check the entire row
        {
            if (AutoValidation)
            {
                string error;
                ValidateRow(e.RowHandle, out error);
            }
        }

        public void OnValidateCell(TableViewCell cell, BaseContainerValidateEditorEventArgs e) //before cell value changed
        {
            string error;
            e.Valid = ValidateCell(cell, e.Value, out error);
            e.ErrorText = error;
                
            tableView.SetColumnError(cell.Column, error);
        }

        public void OnValidateRow(object sender, ValidateRowEventArgs e) //all values might have changed, do a full validation
        {
            string error;
            e.Valid = ValidateRow(e.RowHandle, out error);
        }

        public void HiddenEditor(object sender, EventArgs e)
        {
            var view = sender as GridView;
            if (view != null)
            {
                view.ClearColumnErrors();
            }
        }

        #endregion
    }
}
