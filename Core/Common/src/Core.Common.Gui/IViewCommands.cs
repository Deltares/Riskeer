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

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface declaring methods related to manipulating views within the application.
    /// </summary>
    public interface IViewCommands
    {
        /// <summary>
        /// Presents the user with a dialog to choose an editor for the selected dataitem
        /// </summary>
        void OpenSelectViewDialog();

        void OpenViewForSelection();

        void OpenView(object dataObject);

        void RemoveAllViewsForItem(object dataObject);

        /// <summary>
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>true if there is a default view for the current selection</returns>
        bool CanOpenViewFor(object obj);

        /// <summary>
        /// </summary>
        /// <returns>true if there are more supported views for the current selection</returns>
        bool CanOpenSelectViewDialog();

        object GetDataOfActiveView(); 
    }
}