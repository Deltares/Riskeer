using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Core.Common.Controls.Swf.Table
{
    public class TableViewColumn : ITableViewColumn
    {
        private readonly GridView dxGridView;
        private readonly GridColumn dxColumn;
        private readonly ITableView tableView;
        private readonly GridControl dxGridControl;
        private ITypeEditor editor;

        private string displayFormat = "";
        private int visibleIndex;
        private ICustomFormatter customFormatter;

        public TableViewColumn(GridView view, GridControl control, GridColumn column, ITableView tableView, bool unbound)
        {
            dxGridView = view;
            dxGridControl = control;
            dxColumn = column;
            this.tableView = tableView;
            IsUnbound = unbound;

            dxColumn.FilterMode = dxColumn.ColumnType == typeof(DateTime) ||
                                  dxColumn.ColumnType == typeof(double) ||
                                  dxColumn.ColumnType == typeof(int)
                                      ? ColumnFilterMode.Value
                                      : ColumnFilterMode.DisplayText;
            dxColumn.OptionsColumn.AllowMove = false;
        }

        public bool FilteringAllowed
        {
            get
            {
                return dxColumn.OptionsFilter.AllowFilter;
            }
            set
            {
                dxColumn.OptionsFilter.AllowFilter = value;
            }
        }

        /// <summary>
        /// The name of the column
        /// </summary>
        public string Name
        {
            // XtraGrid prefixes the Name with a "col", we need the FieldName for DataBinding
            get
            {
                return dxColumn.FieldName;
            }
        }

        public string Caption
        {
            get
            {
                return dxColumn.GetCaption();
            } //get actual displayed value
            set
            {
                dxColumn.Caption = value;
            } //set custom value
        }

        public int DisplayIndex
        {
            get
            {
                return dxColumn.VisibleIndex;
            }
            set
            {
                dxColumn.VisibleIndex = value;
            }
        }

        public object DefaultValue { get; set; }

        public bool IsUnbound { get; private set; }

        /// <summary>
        /// Index of the column in column collection of the gridview. 
        /// This collection includes invisible columns etc
        /// </summary>
        public int AbsoluteIndex
        {
            get
            {
                return dxColumn.AbsoluteIndex;
            }
            set
            {
                dxColumn.AbsoluteIndex = value;
            }
        }

        /// <summary>
        /// Allows to override the way cell text is rendered.
        /// 
        /// Will reset 
        /// </summary>
        public ICustomFormatter CustomFormatter
        {
            get
            {
                return customFormatter;
            }
            set
            {
                customFormatter = value;

                SetXtraGridCustomFormatterCore(dxColumn.DisplayFormat, value);
                if (dxColumn.ColumnEdit != null)
                {
                    SetXtraGridCustomFormatterCore(dxColumn.ColumnEdit.DisplayFormat, value);
                }
            }
        }

        ///<summary>
        /// Sets the displayformat of the column. For example c2, D or AA{0}
        /// If CustomFormatter is used then this property is skipped.
        ///</summary>
        public string DisplayFormat
        {
            get
            {
                return displayFormat;
            }
            set
            {
                displayFormat = value;
                SetXtraGridDisplayFormat(value);
            }
        }

        public bool Pinned
        {
            get
            {
                return dxColumn.Fixed == FixedStyle.Left;
            }
            set
            {
                dxColumn.Fixed = (value ? FixedStyle.Left : FixedStyle.None);
                if (!Pinned)
                {
                    // Restore original display index
                    var unPinnedColumns = tableView.Columns.Where(c => !c.Pinned).ToList();
                    var columnToTheLeft = unPinnedColumns.LastOrDefault(c => c.AbsoluteIndex < AbsoluteIndex);
                    DisplayIndex = columnToTheLeft != null
                                       ? columnToTheLeft.DisplayIndex + 1
                                       : tableView.Columns.Count - unPinnedColumns.Count;
                }
            }
        }

        /// <summary>
        /// The visibility of the column
        /// </summary>
        public bool Visible
        {
            get
            {
                return dxColumn.Visible;
            }
            set
            {
                dxColumn.Visible = value;
                // Table BestFitColumns ignores columns with AllowSize set to false; It does noet use the visible
                // property. For best performance disable AllowSize if column is hidden.
                dxColumn.OptionsColumn.AllowSize = dxColumn.Visible;
                if (!value)
                {
                    // remember old column index
                    if (dxColumn.VisibleIndex != -1)
                    {
                        visibleIndex = dxColumn.VisibleIndex;
                    }
                    dxColumn.VisibleIndex = -1;
                }
                else
                {
                    dxColumn.VisibleIndex = visibleIndex;
                }
            }
        }

        public ITypeEditor Editor
        {
            get
            {
                return editor;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                editor = value;

                var repositoryItem = XtraGridRepositoryItemBuilder.CreateFromTypeEditor(editor, dxGridControl, dxColumn, Caption);
                dxGridControl.RepositoryItems.Add(repositoryItem);
                dxColumn.ColumnEdit = repositoryItem;
            }
        }

        public int Width
        {
            get
            {
                return dxColumn.Width;
            }
            set
            {
                dxColumn.Width = value;
            }
        }

        public bool SortingAllowed
        {
            get
            {
                return dxColumn.OptionsColumn.AllowSort != DefaultBoolean.False;
            }
            set
            {
                dxColumn.OptionsColumn.AllowSort = (value) ? DefaultBoolean.True : DefaultBoolean.False;
            }
        }

        /// <summary>
        /// Get or set column filter. Use a syntax like "[Naam] = 'kees'"
        /// </summary>
        public string FilterString
        {
            get
            {
                return dxColumn.FilterInfo.FilterString;
            }
            set
            {
                dxColumn.FilterInfo = new ColumnFilterInfo(value);
            }
        }

        /// <summary>
        /// Set the column sortorder.
        /// </summary>
        public SortOrder SortOrder
        {
            get
            {
                //some conversion between sortorders :(
                //who needs more??
                switch (dxColumn.SortOrder)
                {
                    case ColumnSortOrder.Ascending:
                        return SortOrder.Ascending;
                    case ColumnSortOrder.Descending:
                        return SortOrder.Descending;
                    default:
                        return SortOrder.None;
                }
            }
            set
            {
                switch (value)
                {
                    case SortOrder.None:
                        dxColumn.SortOrder = ColumnSortOrder.None;
                        break;
                    case SortOrder.Ascending:
                        dxColumn.SortOrder = ColumnSortOrder.Ascending;
                        break;
                    case SortOrder.Descending:
                        dxColumn.SortOrder = ColumnSortOrder.Descending;
                        break;
                }
            }
        }

        public Type ColumnType
        {
            get
            {
                var type = typeof(object);
                try
                {
                    if (dxColumn.ColumnHandle != -1)
                    {
                        type = dxColumn.ColumnType;
                    }
                }
                catch (NullReferenceException)
                {
                    //gulp: can throw nullreference exception for some reason
                }
                return type;
            }
        }

        public string ToolTip
        {
            get
            {
                return dxColumn.ToolTip;
            }
            set
            {
                dxColumn.ToolTip = value;
            }
        }

        public bool ReadOnly
        {
            get
            {
                return dxColumn.OptionsColumn.ReadOnly;
            }
            set
            {
                dxColumn.OptionsColumn.AllowEdit = !value; // false;
                dxColumn.OptionsColumn.ReadOnly = value;
                //copy readonly style from tableview
                if (dxColumn.OptionsColumn.ReadOnly)
                {
                    dxColumn.AppearanceCell.ForeColor = tableView.ReadOnlyCellForeColor;
                    dxColumn.AppearanceCell.BackColor = tableView.ReadOnlyCellBackColor;
                }
                else
                {
                    //reset to defaults 
                    dxColumn.AppearanceCell.ForeColor = Color.Empty;
                    //'empty'
                    dxColumn.AppearanceCell.BackColor = Color.Empty;
                    // dxColumn.AppearanceCell.BackColor = Color.White;
                }
            }
        }

        public void Dispose()
        {
            if (editor != null)
            {
                editor.Dispose();
            }
        }

        internal GridColumn DxColumn
        {
            get
            {
                return dxColumn;
            }
        }

        private void SetXtraGridDisplayFormat(string value)
        {
            SetXtraGridDisplayFormatCore(dxColumn.DisplayFormat, value);
            if (dxColumn.ColumnEdit != null)
            {
                SetXtraGridDisplayFormatCore(dxColumn.ColumnEdit.DisplayFormat, value);
            }
        }

        private void SetXtraGridDisplayFormatCore(FormatInfo dxFormatInfo, string value)
        {
            dxFormatInfo.FormatType = GetFormatType(value);
            dxFormatInfo.FormatString = value;
        }

        private static void SetXtraGridCustomFormatterCore(FormatInfo dxFormatInfo, ICustomFormatter value)
        {
            if (value == null)
            {
                dxFormatInfo.FormatType = FormatType.None;
                return;
            }

            dxFormatInfo.FormatType = FormatType.Custom;
            dxFormatInfo.Format = new TableViewCellFormatterProvider(value);
            dxFormatInfo.FormatString = "<custom>"; //must be non null/empty for custom formatting to work
        }

        private FormatType GetFormatType(string value)
        {
            return string.IsNullOrEmpty(value)
                       ? FormatType.None
                       : (ColumnType == typeof(DateTime)
                              ? FormatType.DateTime
                              : FormatType.Numeric);
        }
    }
}