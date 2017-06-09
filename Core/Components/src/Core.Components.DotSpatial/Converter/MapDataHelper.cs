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

using System;
using System.ComponentModel;
using System.Drawing.Drawing2D;
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
                case PointSymbol.Diamond:
                    shape = PointShape.Diamond;
                    break;
                case PointSymbol.Star:
                    shape = PointShape.Star;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(symbol),
                                                           (int) symbol,
                                                           typeof(PointShape));
            }
            return shape;
        }

        /// <summary>
        /// Converts <see cref="LineDashStyle"/> to <see cref="DashStyle"/>.
        /// </summary>
        /// <param name="dashStyle">The <see cref="LineDashStyle"/> to convert.</param>
        /// <returns>The converted <see cref="DashStyle"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dashStyle"/> 
        /// cannot be converted.</exception>
        public static DashStyle Convert(LineDashStyle dashStyle)
        {
            var lineStyle = DashStyle.Solid;
            switch (dashStyle)
            {
                case LineDashStyle.Solid:
                    break;
                case LineDashStyle.Dash:
                    lineStyle = DashStyle.Dash;
                    break;
                case LineDashStyle.Dot:
                    lineStyle = DashStyle.Dot;
                    break;
                case LineDashStyle.DashDot:
                    lineStyle = DashStyle.DashDot;
                    break;
                case LineDashStyle.DashDotDot:
                    lineStyle = DashStyle.DashDotDot;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(dashStyle),
                                                           (int)dashStyle,
                                                           typeof(LineDashStyle));
            }
            return lineStyle;
        }
    }
}