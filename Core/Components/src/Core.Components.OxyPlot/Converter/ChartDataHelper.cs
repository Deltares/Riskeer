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
using System.Drawing;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using OxyPlot;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// Helper methods related to <see cref="ChartData"/> instances.
    /// </summary>
    public static class ChartDataHelper
    {
        /// <summary>
        /// Converts <see cref="ChartLineDashStyle"/> to <see cref="LineStyle"/>.
        /// </summary>
        /// <param name="dashStyle">The <see cref="ChartLineDashStyle"/> to convert.</param>
        /// <returns>The converted <see cref="LineStyle"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="dashStyle"/> 
        /// cannot be converted.</exception>
        public static LineStyle Convert(ChartLineDashStyle dashStyle)
        {
            var lineStyle = LineStyle.Solid;
            switch (dashStyle)
            {
                case ChartLineDashStyle.Solid:
                    break;
                case ChartLineDashStyle.Dash:
                    lineStyle = LineStyle.Dash;
                    break;
                case ChartLineDashStyle.Dot:
                    lineStyle = LineStyle.Dot;
                    break;
                case ChartLineDashStyle.DashDot:
                    lineStyle = LineStyle.DashDot;
                    break;
                case ChartLineDashStyle.DashDotDot:
                    lineStyle = LineStyle.DashDotDot;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(dashStyle),
                                                           (int) dashStyle,
                                                           typeof(ChartLineDashStyle));
            }
            return lineStyle;
        }

        /// <summary>
        /// Converts <see cref="ChartPointSymbol"/> to <see cref="MarkerType"/>.
        /// </summary>
        /// <param name="symbol">The <see cref="ChartPointSymbol"/> to convert.</param>
        /// <returns>The converted <see cref="MarkerType"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="symbol"/> 
        /// cannot be converted.</exception>
        public static MarkerType Convert(ChartPointSymbol symbol)
        {
            MarkerType markerType;
            switch (symbol)
            {
                case ChartPointSymbol.Circle:
                    markerType = MarkerType.Circle;
                    break;
                case ChartPointSymbol.Square:
                    markerType = MarkerType.Square;
                    break;
                case ChartPointSymbol.Diamond:
                    markerType = MarkerType.Diamond;
                    break;
                case ChartPointSymbol.Triangle:
                    markerType = MarkerType.Triangle;
                    break;
                case ChartPointSymbol.Star:
                    markerType = MarkerType.Star;
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(symbol),
                                                           (int) symbol,
                                                           typeof(ChartPointSymbol));
            }
            return markerType;
        }

        /// <summary>
        /// Converts <see cref="Color"/> to <see cref="OxyColor"/>.
        /// </summary>
        /// <param name="color">The <see cref="Color"/> to convert.</param>
        /// <returns>The converted <see cref="OxyColor"/>.</returns>
        public static OxyColor Convert(Color color)
        {
            return OxyColor.FromArgb(color.A, color.R, color.G, color.B);
        }
    }
}