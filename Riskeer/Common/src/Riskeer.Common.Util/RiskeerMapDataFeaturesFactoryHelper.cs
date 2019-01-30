// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;

namespace Riskeer.Common.Util
{
    /// <summary>
    /// Helper class for map data features factory.
    /// </summary>
    public static class RiskeerMapDataFeaturesFactoryHelper
    {
        /// <summary>
        /// Creates a map feature with one single point.
        /// </summary>
        /// <param name="point">The point of the map feature.</param>
        /// <returns>The map feature with the <paramref name="point"/> as geometry.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        public static MapFeature CreateSinglePointMapFeature(Point2D point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            return new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    new[]
                    {
                        point
                    }
                })
            });
        }
    }
}