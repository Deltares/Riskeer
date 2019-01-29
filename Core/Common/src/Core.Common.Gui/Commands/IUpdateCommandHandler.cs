// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
    /// Interface declaring commands/methods related to updating data.
    /// </summary>
    public interface IUpdateCommandHandler
    {
        /// <summary>
        /// Indicates if there are updaters for the given target object.
        /// </summary>
        /// <param name="target">The target object to check updater availability for.</param>
        /// <returns><c>true</c> if there are updaters available, <c>false</c> otherwise.</returns>
        bool CanUpdateOn(object target);

        /// <summary>
        /// Perform the update workflow by the following steps:
        /// <list type="number">
        /// <item>If multiple updaters are available for the target object, determine
        /// which updater to use;</item>
        /// <item>Create the updater;</item>
        /// <item>Obtain the data from the updater;</item>
        /// <item>Update the data on the target object.</item>
        /// </list>
        /// </summary>
        /// <param name="target">The data object to update.</param>
        void UpdateOn(object target);
    }
}