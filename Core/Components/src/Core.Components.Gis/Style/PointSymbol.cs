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

using Core.Common.Util.Attributes;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Style
{
    /// <summary>
    /// All symbols supported by <see cref="PointStyle"/>.
    /// </summary>
    public enum PointSymbol
    {
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Circle_DisplayName))]
        Circle,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Square_DisplayName))]
        Square,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Triangle_DisplayName))]
        Triangle,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Diamond_DisplayName))]
        Diamond,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Star_DisplayName))]
        Star,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Hexagon_DisplayName))]
        Hexagon,

        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Pentagon_DisplayName))]
        Pentagon
    }
}