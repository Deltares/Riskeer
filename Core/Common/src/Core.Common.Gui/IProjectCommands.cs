using System;
using System.Collections.Generic;

using Core.Common.Base.Data;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring a set of methods related to adding data to an instance of <see cref="Project"/>.
    /// </summary>
    public interface IProjectCommands
    {
        /// <summary>
        /// Presents the user with a dialog from which items can be selected and then created. The items are retrieved 
        /// using the DataItemInfo objects of plugins. The item is NOT added to the project or wrapped in a DataItem.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="childItemTypes">The predicate which must evaluate to true for an item type to be included in the list</param>
        object AddNewChildItem(object parent, IEnumerable<Type> childItemTypes);

        void AddNewItem(object parent);

        void AddItemToProject(object item); 
    }
}