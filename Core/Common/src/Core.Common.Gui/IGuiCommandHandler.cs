using System;
using System.Collections.Generic;

namespace Core.Common.Gui
{
    /// <summary>
    /// Handles all common high-level commands in the graphical user interface invoked via menu / toolbar.
    /// </summary>
    public interface IGuiCommandHandler
    {
        /// <summary>
        /// Presents the user with a dialog to choose an editor for the selected dataitem
        /// </summary>
        void OpenSelectViewDialog();

        void OpenViewForSelection();

        void OpenView(object dataObject);

        void RemoveAllViewsForItem(object dataObject);

        /// <summary>
        /// Presents the user with a dialog from which items can be selected and then created. The items are retrieved 
        /// using the DataItemInfo objects of plugins. The item is NOT added to the project or wrapped in a DataItem.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childItemTypes">The predicate which must evaluate to true for an item type to be included in the list</param>
        object AddNewChildItem(object parent, IEnumerable<Type> childItemTypes);

        void AddNewItem(object parent);

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if there is a default view for the current selection</returns>
        bool CanOpenViewFor(object obj);

        /// <summary>
        /// </summary>
        /// <returns>true if there are more supported views for the current selection</returns>
        bool CanOpenSelectViewDialog();

        void AddItemToProject(object item);

        /// <summary>
        /// Activates the propertyGrid toolbox
        /// </summary>
        /// <param name="obj"></param>
        void ShowPropertiesFor(object obj);

        /// <summary>
        /// Indicates if there is a property view object for the current <see cref="IGui.Selection"/>.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns><c>true</c> if a property view is defined, <c>false</c> otherwise.</returns>
        bool CanShowPropertiesFor(object obj);

        object GetDataOfActiveView();

        void OpenLogFileExternal();
    }
}