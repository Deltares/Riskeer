// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
// All rights reserved.

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface that declares application feature manipulation.
    /// </summary>
    public interface IApplicationFeatureCommands
    {
        /// <summary>
        /// Activates the propertyGrid toolbox
        /// </summary>
        /// <param name="obj"></param>
        void ShowPropertiesFor(object obj);

        /// <summary>
        /// Indicates if there is a property view object for some object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns><c>true</c> if a property view is defined, <c>false</c> otherwise.</returns>
        bool CanShowPropertiesFor(object obj);

        void OpenLogFileExternal();
    }
}