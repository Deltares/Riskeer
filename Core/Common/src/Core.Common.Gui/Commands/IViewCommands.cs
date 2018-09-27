// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// Interface declaring methods related to manipulating views within the application.
    /// </summary>
    public interface IViewCommands
    {
        /// <summary>
        /// Open the view for the application's selection.
        /// </summary>
        /// <remarks>If multiple views are available, the user is asked which view to use.</remarks>
        /// <seealso cref="OpenView"/>
        void OpenViewForSelection();

        /// <summary>
        /// Open the view for the given data object.
        /// </summary>
        /// <param name="viewData">The data object for which a view must be opened.</param>
        /// <remarks>If multiple views are available, the user is asked which view to use.</remarks>
        /// <seealso cref="OpenViewForSelection"/>
        void OpenView(object viewData);

        /// <summary>
        /// Removes all views that are associated to the given data object and/or its children.
        /// </summary>
        /// <param name="viewData">The root data object for which all views must be closed.</param>
        void RemoveAllViewsForItem(object viewData);

        /// <summary>
        /// Indicates if a there are any views available for the application's selection.
        /// </summary>
        /// <param name="viewData">The data object to open views for.</param>
        /// <returns><c>true</c> if there are any views for <paramref name="viewData"/>, <c>false</c> otherwise.</returns>
        bool CanOpenViewFor(object viewData);
    }
}