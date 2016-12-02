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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.ComponentModel;
using Core.Components.Gis.Data;
using Core.Components.Gis.Style;
using DotSpatial.Symbology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// Helper methods related to <see cref="FeatureBasedMapData"/> instances.
    /// </summary>
    public static class MapDataHelper
    {
        /// <summary>
        /// Converts <see cref="PointSymbol"/> to <see cref="PointShape"/>.
        /// </summary>
        /// <param name="symbol">The symbol to convert.</param>
        /// <returns>The converted <see cref="PointShape"/>.</returns>
        public static PointShape Convert(PointSymbol symbol)
        {
            PointShape shape;
            switch (symbol)
            {
                case PointSymbol.Circle:
                    shape = PointShape.Ellipse;
                    break;
                case PointSymbol.Square:
                    shape = PointShape.Rectangle;
                    break;
                case PointSymbol.Triangle:
                    shape = PointShape.Triangle;
                    break;
                default:
                    throw new InvalidEnumArgumentException("symbol",
                                                           (int) symbol,
                                                           typeof(PointShape));
            }
            return shape;
        }
    }
}