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

using System;
using Core.Components.Gis.Style;

namespace Core.Components.Gis.Data.Removable
{
    /// <summary>
    /// <see cref="MapPointData"/> that is marked as being removable.
    /// </summary>
    public class RemovableMapPointData : MapPointData, IRemovable
    {
        /// <summary>
        /// Creates a new instance of <see cref="MapPointData"/> with default styling.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapPointData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public RemovableMapPointData(string name) : base(name) {}

        /// <summary>
        /// Creates a new instance of <see cref="MapPointData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MapPointData"/>.</param>
        /// <param name="style">The style of the data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="style"/>
        /// is <c>null</c>.</exception>
        public RemovableMapPointData(string name, PointStyle style) : base(name, style) {}
    }
}