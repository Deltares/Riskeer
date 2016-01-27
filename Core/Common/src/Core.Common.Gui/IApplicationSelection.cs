using System;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface for keeping and notifying changes to the application selection.
    /// </summary>
    public interface IApplicationSelection
    {
        /// <summary>
        /// Gets or sets current selected object(s). Visibility of the menus, toolbars and 
        /// other controls should be updated when selected object is changed. Default 
        /// implementation will also show it in the PropertyGrid.
        /// </summary>
        object Selection { get; set; }

        /// <summary>
        /// Fired when user changes selection by clicking on it or by setting it using <see cref="Selection"/> property.
        /// </summary>
        event EventHandler<SelectedItemChangedEventArgs> SelectionChanged; 
    }
}