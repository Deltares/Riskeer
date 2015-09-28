using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace DelftTools.Controls.Swf.Table
{
    public class TableViewArgumentBasedPasteController : TableViewPasteController
    {
        private string sorted;

        public TableViewArgumentBasedPasteController(TableView tableView, IList<int> argumentColumns) : base(tableView)
        {
            ArgumentColumns = argumentColumns;
            DataIsSorted = true;
        }

        public bool SkipRowsWithMissingArgumentValues { get; set; }

        /// <summary>
        /// Indicates if the paste logic should use a more efficient algorithm
        /// that assumes data is and remains sorted, or should assume unsorted
        /// data in <see cref="TableView"/>.
        /// </summary>
        public bool DataIsSorted { get; set; }

        private IList<int> ArgumentColumns { get; set; }

        protected override bool CanPaste(string[] clipboardLines, out string errorMessage)
        {
            var canPaste = base.CanPaste(clipboardLines, out errorMessage);

            if (!canPaste)
            {
                return false;
            }

            var targetSelection = GetPasteTargetSelection(out errorMessage);
            if (targetSelection == null)
            {
                return false;
            }

            //assume pasted data is square
            var targetColumns = GetTargetColumns(clipboardLines, targetSelection).ToList();

            var argumentOverlap = targetColumns.Count(c => ArgumentColumns.Contains(c));
            if (argumentOverlap == 0)
            {
                return true; //base can handle this
            }

            if (targetColumns.Count() != TableView.Columns.Count(c => c.Visible))
            {
                errorMessage = string.Format(
                    "This table contains multiple argument columns ({0}). When pasting, please paste the full row width, or paste into non-argument columns only.",
                    string.Join(",", TableView.Columns.Where(c => c.Visible).Where((c, i) => ArgumentColumns.Contains(i)).Select(c => c.Name).ToArray()));
                return false;
            }

            return true;
        }

        private static IEnumerable<int> GetTargetColumns(string[] clipboardLines, RectangleSelection targetSelection)
        {
            var pastedDataWidth = clipboardLines.Length > 0 ? SplitToCells(clipboardLines[0]).Length : 0;

            //can only paste if either none or all arguments are included in the paste
            return Enumerable.Range(targetSelection.Left, pastedDataWidth);
        }

        protected override void PasteValues(string[] clipboardLines, RectangleSelection targetSelection, bool allowNewRows, bool wrapInEditAction=true)
        {
            if (wrapInEditAction)
            {
                TableView.DoActionInEditAction(TableView, "Pasting values",
                                               () => PasteValuesCore(clipboardLines, targetSelection, allowNewRows));
            }
            else
            {
                PasteValuesCore(clipboardLines, targetSelection, allowNewRows);
            }
        }

        private void PasteValuesCore(string[] clipboardLines, RectangleSelection targetSelection, bool allowNewRows)
        {
            var table = TableView.Data as DataTable;
            var constraints = new List<Constraint>();
            if (table != null && table.DataSet != null)
            {
                table.DataSet.EnforceConstraints = false;
                constraints.AddRange(table.Constraints.Cast<Constraint>());
                table.Constraints.Clear();
                sorted = table.DefaultView.Sort;
                table.DefaultView.Sort = string.Empty;
            }

            try
            {
                var targetColumns = GetTargetColumns(clipboardLines, targetSelection);
                var argumentOverlap = targetColumns.Count(c => ArgumentColumns.Contains(c));
                bool allowNewRowsCore = (!SkipRowsWithMissingArgumentValues || argumentOverlap >= ArgumentColumns.Count) &&
                                        allowNewRows;

                if (argumentOverlap == 0) //if not pasting arguments at all, use base
                {
                    base.PasteValues(clipboardLines, targetSelection, allowNewRowsCore, false);
                }
                else
                {
                    foreach (var line in clipboardLines)
                    {
                        PasteLine(line, allowNewRowsCore);
                    }
                }
            }
            finally
            {
                if (table != null && table.DataSet != null)
                {
                    constraints.ForEach(c => table.Constraints.Add(c));
                    table.DataSet.EnforceConstraints = true;
                    table.DefaultView.Sort = sorted;
                }
            }
        }

        private void PasteLine(string line, bool allowNewRows)
        {
            //todo: I don't think this works well with columns out of order..

            var cellValues = SplitToCells(line);
            var argumentValues = cellValues.Where((f, i) => ArgumentColumns.Contains(i)).ToList();
            
            var rowIndex = GetExistingRowForArguments(argumentValues);
            var addNewRow = allowNewRows && (rowIndex == -1);

            if (rowIndex == -1)
            {
                //paste all
                PasteCellsToRow(cellValues, TableView.RowCount, 0, TableView.Columns.Count, addNewRow, false);
            }
            else
            {
                //only paste non-argument values
                var nonArgumentColumn = ArgumentColumns.Count;
                var nonArgumentValues = cellValues.Skip(nonArgumentColumn).ToArray();
                PasteCellsToRow(nonArgumentValues, rowIndex, nonArgumentColumn, nonArgumentValues.Length, addNewRow, false);
            }

        }

        private int GetExistingRowForArguments(List<string> argumentValues)
        {
            var dataTable = TableView.Data as DataTable;
            return dataTable != null
                       ? GetExistingRowForDataTable(argumentValues, !dataTable.DefaultView.Sort.EndsWith(" DESC"))
                       : GetExistingRowForFunctionArguments(argumentValues);
        }

        private int GetExistingRowForDataTable(List<string> argumentValues, bool isAscending)
        {
            //search in a datable with sorting off..no optimization here..
            var lastRow = TableView.RowCount - 1;
            var start = isAscending ? lastRow : 0;
            var end = isAscending ? -1 : TableView.RowCount; //out-of-bounds
            var delta = isAscending ? -1 : 1;

            for (int i = start; i != end; i += delta)
            {
                var equal = true;

                for (int j = 0; j < ArgumentColumns.Count(); j++)
                {
                    var cellValue = TableView.GetCellValue(i, ArgumentColumns[j]);
                    var pasteValue = TableView.ConvertStringValueToDataValue(j, argumentValues[j]);

                    equal &= CheckEquals(cellValue,pasteValue);

                    if (!equal)
                        break;
                }
                if (equal)
                    return i;
            }
            return -1;
        }

        private int GetExistingRowForFunctionArguments(List<string> argumentValues)
        {
            if (DataIsSorted)
            {
                // We make the assumption everything is (and remains) sorted according to arguments 
                // (not allowed to paste in sorted/filtered table). This allows a huge speedup. 
                // Next we optimize towards the user pasted in sorted order as well. 
                // Todo: do a binary search?

                for (int i = TableView.RowCount - 1; i >= 0; i--)
                {
                    switch (IsValueInSortedRowCollection(i, argumentValues))
                    {
                        case 1: //found correct row
                            return i;
                        case -1: //not in the sorted collection
                            return -1;
                        case 0: //maybe in collection -> continue
                            continue;
                    }
                }
                return -1;
            }

            return GetExistingRowForUnsortedRowCollection(argumentValues);
        }

        /// <summary>
        /// Reads whole row collection to return the index of a row
        /// that matches the specified argument values.
        /// </summary>
        /// <param name="argumentValues">Argument values to be matched.</param>
        /// <returns>The index of the row, if found. -1 in case there is no match.</returns>
        private int GetExistingRowForUnsortedRowCollection(List<string> argumentValues)
        {
            for (int rowIndex = 0; rowIndex < TableView.RowCount; rowIndex++)
            {
                var rowArgumentsMatch = true;
                for (int j = 0; j < ArgumentColumns.Count(); j++)
                {
                    var cellValue = TableView.GetCellValue(rowIndex, ArgumentColumns[j]);
                    if (cellValue == null) continue;

                    var pasteValue = TableView.ConvertStringValueToDataValue(j, argumentValues[j]);

                    if (!CheckEquals(cellValue, pasteValue))
                    {
                        // Not all arguments match: Check next row.
                        rowArgumentsMatch = false;
                        break;
                    }
                }

                if (rowArgumentsMatch) return rowIndex;
            }

            // Given argument-values cannot be found in the table:
            return -1;
        }

        /// <summary>
        /// IsValueInSortedRowCollection: return -1 = no, 0 = maybe -> continue please, 1 = yep, in this row
        /// </summary>
        /// <param name="rowIndex"></param>
        /// <param name="argumentValues"></param>
        /// <returns></returns>
        private int IsValueInSortedRowCollection(int rowIndex, IList<string> argumentValues)
        {
            int returnValue = 0;
            for (int j = 0; j < ArgumentColumns.Count(); j++)
            {
                var cellValue = TableView.GetCellValue(rowIndex, ArgumentColumns[j]);

                if (cellValue != null)
                {
                    var pasteStr = argumentValues[j];
                    var pasteValue = TableView.ConvertStringValueToDataValue(j, pasteStr);

                    if (CheckEquals(cellValue, pasteValue))
                    {
                        returnValue = 1;
                        continue;
                    }
                    
                    //we couldn't convert the pasted value, so we convert the original value
                    if (pasteValue is string && !(cellValue is string))
                    {
                        cellValue = cellValue.ToString();
                    }

                    if (IsLarger(pasteValue, cellValue))
                    {
                        return -1;
                    }

                    return 0;
                }
            }

            return returnValue;
        }

        private bool CheckEquals(object cellValue, object pasteValue)
        {
            if (IsDecimalNumber(cellValue) && IsDecimalNumber(pasteValue))
            {
                var cellValueDouble = Convert.ToDouble(cellValue);
                var pasteValueDouble = Convert.ToDouble(pasteValue);

                return Math.Abs(cellValueDouble - pasteValueDouble) < 1e-9;
            }

            return cellValue.Equals(pasteValue);
        }

        private static bool IsDecimalNumber(object value)
        {
            return value is float
                   || value is double
                   || value is decimal;
        }

        private static bool IsLarger(object obj1, object obj2)
        {
            if (obj1 is IComparable)
            {
                var comparableRowObject = obj1 as IComparable;
                return comparableRowObject.CompareTo(obj2) > 0;
            }

            return false;
        }
    }
}