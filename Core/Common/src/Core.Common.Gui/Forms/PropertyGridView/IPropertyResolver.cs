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

namespace Core.Common.Gui.Forms.PropertyGridView
{
    /// <summary>
    /// Interface for an object that is used to retrieve property objects corresponding to a data-object.
    /// </summary>
    public interface IPropertyResolver
    {
        /// <summary>
        /// Returns object properties based on the provided <paramref name="sourceData"/>.
        /// </summary>
        /// <param name="sourceData">The source data to get the object properties for.</param>
        /// <returns>An object properties object, or null when no relevant properties object is found
        /// or multiple matches are found.</returns>
        object GetObjectProperties(object sourceData);
    }
}