using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Swf.Properties;
using Core.Common.Utils;
using log4net;

namespace Core.Common.Controls.Swf.Table
{
    /// <summary>
    /// Class add copy paste functionality to a tableview. Based on ITableView
    /// </summary>
    public class TableViewPasteController : ITableViewPasteController
    {
        public event EventHandler<EventArgs<string>> PasteFailed;

        public event EventHandler<EventArgs> PasteFinished;

        private const int NewRowSelectedIndex = int.MinValue + 1;
        private static readonly ILog Log = LogManager.GetLogger(typeof(TableViewPasteController));

        public TableViewPasteController(TableView tableView)
        {
            TableView = tableView;
            PastedRows = new List<int>();
            PastedBlocks = new List<RectangleSelection>();
        }

        /// <summary>
        /// Gets or sets the paste behaviour value
        /// </summary>
        public TableViewPasteBehaviourOptions PasteBehaviour { get; set; }

        public bool IsPasting { get; private set; }

        /// <summary>
        /// This method does most of the work. It Pastes Clipboard content to the XtraGrid. 
        /// If OptionsSelection.EnableAppearanceFocusedRow == true then it will paste row (if 
        /// any row is present in the clipboard) to the currently selected row. It will not 
        /// replace Readonly or Not Editable Fields as well as key fields for the child tables 
        /// and details. It works not on the XtraGrid level but on the DataTable level.
        /// If the currently selected row in the XtraGrid is the New Row then it will populate 
        /// it with the values from the row in the Clipboard.
        /// </summary>
        public void PasteClipboardContents()
        {
            // for now only allow to paste text 
            if (!Clipboard.ContainsText())
            {
                Log.Debug(Resources.TableViewPasteController_PasteClipboardContents_Clipboard_does_not_contain_text__so_it_cannot_be_pasted_to_the_grid_);
                return;
            }

            string[] clipboardLines = GetClipboardLines();
            //nothing to paste.
            if (clipboardLines.Length == 0)
            {
                return;
            }

            PasteLines(clipboardLines);
        }

        /// <summary>
        /// Paste value string into tableview at current selection
        /// </summary>
        /// <param name="lines"></param>
        public void PasteLines(string[] lines)
        {
            IsPasting = true;

            lines = RemoveHeaderIfPresent(lines);

            //get the selection in which we are to paste
            var targetSelection = GetPasteTargetSelection();
            //check if we can paste or fail
            string message;
            if (!CanPaste(lines, out message))
            {
                OnPasteFailed(message);

                IsPasting = false;

                return; //don't paste
            }

            //pastevalues returns the pasted selection

            var tableViewImpl = TableView as TableView;

            if (tableViewImpl != null)
            {
                tableViewImpl.BeginInit();
            }

            PastedRows.Clear();
            PastedBlocks.Clear();

            PasteValues(lines, targetSelection, (!TableView.IsSorted()) && (TableView.AllowAddNewRow));

            if (tableViewImpl != null)
            {
                tableViewImpl.EndInit();
            }

            SetSelection();

            PastedRows.Clear();
            PastedBlocks.Clear();

            IsPasting = false;

            OnPasteFinished();
        }

        protected TableView TableView { get; private set; }
        protected List<RectangleSelection> PastedBlocks { private get; set; }

        /// <summary>
        /// Overload without message parameter.
        /// </summary>
        /// <returns></returns>
        protected RectangleSelection GetPasteTargetSelection()
        {
            string message;
            return GetPasteTargetSelection(out message);
        }

        /// <summary>
        /// Gets the target selection from tableview or raises paste failed event in case
        /// the target selection is invalid (non-rectangular)
        /// </summary>
        /// <returns></returns>
        protected RectangleSelection GetPasteTargetSelection(out string errorMessage)
        {
            errorMessage = "";

            var topLeft = new TableViewCell(0, null);
            //in RowSelect don't used selected cells because they don't work
            if (TableView.RowSelect)
            {
                return GetRowSelectSelection();
            }

            var cells = TableView.SelectedCells;
            if (cells.Count == 0)
            {
                //base selection on focused cell
                var focusedCell = TableView.GetFocusedCell();
                if (focusedCell != null)
                {
                    return new RectangleSelection(focusedCell, focusedCell);
                }
                //no focused cell must mean empty table..start pasting left,top
                return new RectangleSelection(topLeft, topLeft);
            }

            //return a selection based on the first and last cell in the selection. Tricky..should check the most upper etc..
            RectangleSelection selection = GetRectangleSelection(cells);

            if (selection == null)
            {
                errorMessage = Resources.TableViewPasteController_GetPasteTargetSelection_Cannot_paste_into_non_rectangular_selection;
            }

            return selection;
        }

        protected RectangleSelection GetRowSelectSelection()
        {
            var selectedRows = TableView.SelectedRowsIndices;
            var topLeft = selectedRows.Length == 0 
                                        ? new TableViewCell(TableView.RowCount, TableView.GetColumnByDisplayIndex(0)) 
                                        : new TableViewCell(selectedRows[0], TableView.GetColumnByDisplayIndex(0));

            return new RectangleSelection(topLeft, topLeft);
        }

        /// <summary>
        /// Returns rectangle for a collection of cells. Or null if cells don't make up a rectangle
        /// </summary>
        /// <param name="cells"></param>
        /// <returns></returns>
        protected RectangleSelection GetRectangleSelection(IList<TableViewCell> cells)
        {
            int left = cells.Min(cell => cell.Column.DisplayIndex);
            int right = cells.Max(cell => cell.Column.DisplayIndex);
            int top = cells.Min(cell => cell.RowIndex);
            int bottom = cells.Max(cell => cell.RowIndex);

            //if the selection is on 'new' row change to rowcount..
            if ((bottom == NewRowSelectedIndex) && (top == NewRowSelectedIndex))
            {
                bottom = TableView.RowCount;
                top = TableView.RowCount;
            }
            //do a check if we have enought cells...
            if (cells.Count != (right + 1 - left)*(bottom + 1 - top))
            {
                return null;
            }

            return new RectangleSelection(new TableViewCell(top, TableView.GetColumnByDisplayIndex(left)), new TableViewCell(bottom, TableView.GetColumnByDisplayIndex(right)));
        }

        /// <summary>
        /// Checks whether the target tableView is filtered or the target selection contains a sorted column
        /// </summary>
        /// <param name="clipboardLines"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        protected virtual bool CanPaste(string[] clipboardLines, out string errorMessage)
        {
            //get a targetselection. Can fail if target is non square. Then error message is filled
            var targetSelection = GetPasteTargetSelection(out errorMessage);
            if (targetSelection == null)
            {
                return false;
            }

            if (clipboardLines.Length == 0)
            {
                errorMessage = Resources.TableViewPasteController_CanPaste_There_are_no_values_to_paste__headers_are_skipped__;
                return false;
            }

            //how many columns do if hit when we paste?
            var pasteColumnSpan = SplitToCells(clipboardLines[0]).Length;

            int[] pastedColumnIndexes = Enumerable.Range(targetSelection.Left, pasteColumnSpan).ToArray();

            //is there sorted column in our pasted columns?
            var sortedColumnExists = (from col in TableView.Columns
                                      where col.SortOrder != SortOrder.None && pastedColumnIndexes.Contains(col.DisplayIndex)
                                      select col).Any();
            if (sortedColumnExists)
            {
                errorMessage = Resources.TableViewPasteController_CanPaste_Cannot_paste_into_sorted_column; //todo: add name of column here?
                return false;
            }
            if (TableView.Columns.Any(col => !string.IsNullOrEmpty(col.FilterString)))
            {
                errorMessage = Resources.TableViewPasteController_CanPaste_Cannot_paste_into_filtered_tableview_; //todo: add name of column here?
                return false;
            }
            return true;
        }

        /// <summary>
        /// Pastes values into target selection.
        /// </summary>
        protected virtual void PasteValues(string[] clipboardLines, RectangleSelection targetSelection,
                                           bool allowNewRows, bool wrapInEditAction = true)
        {
            if (wrapInEditAction)
            {
                PasteValuesCore(clipboardLines, targetSelection, allowNewRows);
            }
            else
            {
                PasteValuesCore(clipboardLines, targetSelection, allowNewRows);
            }
        }

        /// <summary>
        /// Pastes the given contents to the table at row startRowIndex, increasing the number of rows if necessary and allowed.
        /// </summary>
        /// <returns>The row index at which the pasting effectively occured, -1 if no pasting was done.</returns>
        protected int PasteCellsToRow(string[] content, int startRowIndex, int startColumnIndex, int pasteWidth, bool addNewRow, bool cellBased)
        {
            var index = startRowIndex;
            if (addNewRow)
            {
                TableView.AddNewRowToDataSource();
            }
            else
            {
                if (startRowIndex >= TableView.RowCount)
                {
                    return -1;
                }
            }
            var exceptionMode = TableView.ExceptionMode;
            TableView.ExceptionMode = TableView.ValidationExceptionMode.ThrowException; //throw exception
            try
            {
                if (cellBased)
                {
                    var contentWidth = content.Length;
                    for (var i = 0; i < pasteWidth; i++)
                    {
                        if (!SafeSetCellValue(index, startColumnIndex + i, content[i%contentWidth]))
                        {
                            Log.ErrorFormat(Resources.TableViewPasteController_PasteCellsToRow_Can_not_paste_value_into_cell___0____1____Row__0__will_be_skipped,
                                            startRowIndex, startColumnIndex + i);
                            if (addNewRow)
                            {
                                TableView.SelectRow(index);
                                TableView.DeleteCurrentSelection();
                            }
                            return -1;
                        }
                    }
                    UpdatePastedBlocks(startColumnIndex, pasteWidth, index);
                }
                else
                {
                    var contentWidth = content.Length;
                    var values = new List<string>();
                    for (var i = 0; i < pasteWidth; i++)
                    {
                        values.Add(content[i%contentWidth]);
                    }
                    if (!SafeSetRowCellValues(index, startColumnIndex, values))
                    {
                        Log.ErrorFormat(Resources.TableViewPasteController_PasteCellsToRow_Skipping_invalid_row__0__from_pasting,
                                        startRowIndex);
                        if (addNewRow)
                        {
                            TableView.SelectRow(index);
                            TableView.DeleteCurrentSelection();
                        }
                        return -1;
                    }
                    PastedRows.Add(index);
                }
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Resources.TableViewPasteController_PasteCellsToRow_Pasting_values_failed___0_, e.Message);
            }
            finally
            {
                TableView.ExceptionMode = exceptionMode;
            }
            return index;
        }

        protected static string[] SplitToCells(string p)
        {
            //tab delimited data: excel, and xtragrid copy (todo think about how to implement pasting of space delimited data)
            return p.Split(new[]
            {
                "\t"
            }, StringSplitOptions.None);
        }

        private List<int> PastedRows { get; set; }

        private string[] RemoveHeaderIfPresent(string[] lines)
        {
            if (lines.Length > 0 && string.Equals(lines[0], GetTableHeaderString()))
            {
                return lines.Skip(1).ToArray(); //skip header
            }
            return lines;
        }

        private string GetTableHeaderString()
        {
            //The function xtragrid uses to get the header string is protected and uses dxColumn.GetTextCaption(), 
            //which may differ from dxColumn.GetCaption we use. In those cases this may break. Also if the ordering
            //of columns is out of sync (can that happen?)
            return String.Join("\t", TableView.Columns.Where(c => c.Visible).Select(c => c.Caption).ToArray());
        }

        private void OnPasteFailed(string message)
        {
            Log.Warn(message);

            if (PasteFailed != null)
            {
                PasteFailed(this, new EventArgs<string>(message));
            }
        }

        private void OnPasteFinished()
        {
            if (PasteFinished != null)
            {
                PasteFinished(this, new EventArgs());
            }
        }

        private void PasteValuesCore(string[] clipboardLines, RectangleSelection targetSelection, bool allowNewRows)
        {
            var startRowIndex = targetSelection.Top;
            var startColumnIndex = targetSelection.Left;
            if (startRowIndex < 0)
            {
                throw new ArgumentException(string.Format(Resources.TableViewPasteController_PasteValuesCore_Invalid_row_number__0_, startRowIndex),
                                            "targetSelection");
            }

            //paste all the lines once...
            int lastRowIndex = Math.Max(startRowIndex + clipboardLines.Length - 1, targetSelection.Bottom);

            int currentClipBoardLineIndex = 0;
            int canceledNewRows = 0;
            for (int j = startRowIndex; j <= lastRowIndex; j++)
            {
                string[] cols = SplitToCells(clipboardLines[currentClipBoardLineIndex]);
                //Simply set values as text.
                var pasteWidth = Math.Max(cols.Length, targetSelection.Right - targetSelection.Left + 1);
                //paste cannot be wider than tablewidth - startColumnIndex
                pasteWidth = Math.Min(pasteWidth, TableView.Columns.Count(c => c.Visible) - startColumnIndex);

                //do the pasting
                bool checkCells = (PasteBehaviour == TableViewPasteBehaviourOptions.SkipRowWhenValueIsInvalid);
                bool addNewRow = allowNewRows && (j >= TableView.RowCount);
                var pasteStartRowIndex = j - canceledNewRows;

                var pasteResult = PasteCellsToRow(cols, pasteStartRowIndex, startColumnIndex, pasteWidth, addNewRow,
                                                  checkCells);

                if (addNewRow && pasteResult == -1)
                {
                    canceledNewRows++;
                }

                //update clipboard line index..loop
                currentClipBoardLineIndex++;
                if (currentClipBoardLineIndex == clipboardLines.Length)
                {
                    //break;//do really want repeat data like below?
                    currentClipBoardLineIndex = 0;
                }
            }
        }

        private bool SafeSetRowCellValues(int index, int startColumnIndex, List<string> values)
        {
            try
            {
                return TableView.SetRowCellValues(index, startColumnIndex, values.ToArray());
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Resources.TableViewPasteController_SafeSetCellValue_Invalid_row_reason_0_, e.Message);
                return false;
            }
        }

        private bool SafeSetCellValue(int index, int columnIndex, string value)
        {
            try
            {
                return TableView.SetCellValue(index, columnIndex, value);
            }
            catch (Exception e)
            {
                Log.ErrorFormat(Resources.TableViewPasteController_SafeSetCellValue_Invalid_row_reason_0_, e.Message);
                return false;
            }
        }

        private void UpdatePastedBlocks(int startColumnIndex, int pasteWidth, int index)
        {
            var topLeftCell = new TableViewCell(index, TableView.GetColumnByDisplayIndex(startColumnIndex));
            var bottomRightCell = new TableViewCell(index, TableView.GetColumnByDisplayIndex(startColumnIndex + pasteWidth - 1));
            if (PastedBlocks.Count == 0)
            {
                PastedBlocks.Add(new RectangleSelection(topLeftCell, bottomRightCell));
            }
            else
            {
                // if we can extend a rectangular block, do it...
                var selectBlock = PastedBlocks.Where(b => b.Bottom + 1 == index);
                if (selectBlock.Any())
                {
                    selectBlock.First().Bottom++;
                }
                else
                {
                    PastedBlocks.Add(new RectangleSelection(topLeftCell, bottomRightCell));
                }
            }
        }

        private void SetSelection()
        {
            if (PastedRows.Count != 0)
            {
                TableView.ClearSelection();
                TableView.SelectRows(PastedRows.ToArray());
            }

            if (PastedBlocks.Count != 0)
            {
                TableView.ClearSelection();
                foreach (var selection in PastedBlocks)
                {
                    TableView.SelectCells(selection.Top, selection.Left, selection.Bottom, selection.Right, false);
                }
            }
        }

        private static string[] GetClipboardLines()
        {
            var strPasteText = Clipboard.GetText();
            var lines = strPasteText.Split(new[]
            {
                "\r\n"
            }, StringSplitOptions.None);
            return RemoveLastLineIfEmpty(lines);
        }

        private static string[] RemoveLastLineIfEmpty(string[] lines)
        {
            if (lines[lines.Length - 1] == "")
            {
                return lines.ToList().Take(lines.Length - 1).ToArray();
            }
            return lines;
        }

        /// <summary>
        /// Rectangular selection on a tableview. 
        /// </summary>
        protected class RectangleSelection
        {
            public RectangleSelection(TableViewCell topLeft, TableViewCell bottomRight)
            {
                Top = topLeft.RowIndex;
                Left = topLeft.Column == null ? 0 : topLeft.Column.DisplayIndex;
                Bottom = bottomRight.RowIndex;
                Right = bottomRight.Column == null ? 0 : bottomRight.Column.DisplayIndex;
            }

            public int Top { get; set; }

            public int Left { get; set; }

            public int Bottom { get; set; }

            public int Right { get; set; }
        }
    }
}