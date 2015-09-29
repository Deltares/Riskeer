using System;
using DelftTools.Shell.Core;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Selection container allows object of any type to be selected.
    /// Can be implemented by any user interface - related class like control/window/app where managing a single selection is required.
    /// TODO: merge with IGui
    /// </summary>
    public interface ISelectionContainer
    {
        /// <summary>
        /// Gets or sets current selected object(s). 
        /// Visibility of the menus, toolbars and other controls should be updated when selected object is changed.
        /// Default implementation will also show it in the PropertyGrid.
        /// </summary>
        object Selection { get; set; }

        /// <summary>
        /// Fired when user changes selection by clicking on it or by setting it using Selection property.
        /// </summary>
        event EventHandler<SelectedItemChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Selected project item. 
        /// If non-IProjectItem is selected in project tree view then this item must be set from outside 
        /// (by a project tree view or any other IProjectItem navigation control).
        /// </summary>
        IProjectItem SelectedProjectItem { get; set; }
    }
}