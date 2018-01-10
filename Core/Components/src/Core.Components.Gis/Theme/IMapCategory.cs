// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.Generic;
using System.Drawing;
using Core.Components.Gis.Theme.Criteria;

namespace Core.Components.Gis.Theme
{
    /// <summary>
    /// Interface for map categories.
    /// </summary>
    public interface IMapCategory // TODO: IThemeCategory --> make concrete class?
    {
        /// <summary>
        /// The color of the map category.
        /// </summary>
        Color Color { get; }

        /// <summary>
        /// The criteria that is associated with the map category.
        /// </summary>
        IEnumerable<IMapCriteria> Criteria { get; } // TODO: IThemeCriteria
    }
}