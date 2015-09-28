using System;
using System.Windows.Forms;

namespace DelftTools.Controls
{
    /// <summary>
    /// A column in a ITableView
    /// </summary>
    public interface ITableViewColumn : IDisposable
    {
        /// <summary>
        /// The name of the column
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The visibility of the column
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Editor used to edit values in the column.
        /// </summary>
        ITypeEditor Editor { get; set; }

        /// <summary>
        /// The caption of the column
        /// </summary>
        string Caption { get; set; }

        /// <summary>
        /// Index used for display
        /// </summary>
        int DisplayIndex { get; set; }

        /// <summary>
        /// Determines if the column can be modified
        /// </summary>
        bool ReadOnly { get; set; }

        /// <summary>
        /// Can this column be used for sorting
        /// </summary>
        bool SortingAllowed { get; set; }

        /// <summary>
        /// The type of ordering (<see cref="SortOrder"/>) that is used for this column
        /// </summary>
        SortOrder SortOrder { get; set; }

        /// <summary>
        /// Get or set column filter. Use a syntax like "[Naam] = 'kees'"
        /// </summary>
        string FilterString { get; set; }

        /// <summary>
        /// Index of the column in the datasouce
        /// </summary>
        int AbsoluteIndex { get; }

        /// <summary>
        /// The type of data that the column contains
        /// </summary>
        Type ColumnType { get; }

        /// <summary>
        /// String to display as tooltip
        /// </summary>
        string ToolTip { get; set; }

        /// <summary>
        /// Width of this column
        /// </summary>
        int Width { get; set; }

        /// <summary>
        /// Allows to override the way cell text is rendered.
        /// </summary>
        ICustomFormatter CustomFormatter { get; set; }

        ///<summary>
        /// Sets the display format of the column. For example c2, D or AA{0}
        /// If CustomFormatter is used then this property is skipped.
        ///</summary>
        string DisplayFormat { get; set; }

        /// <summary>
        /// Makes column frozen (column does not move during scrolling)
        /// </summary>
        bool Pinned { get; set; }

        bool IsUnbound { get; }

        object DefaultValue { get; set; }
    }
}