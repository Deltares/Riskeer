// Copyright (C) Stichting Deltares 2016. All rights preserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

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