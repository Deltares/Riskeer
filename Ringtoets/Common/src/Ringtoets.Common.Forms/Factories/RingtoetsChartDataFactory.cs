// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System.Drawing;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Ringtoets.Common.Forms.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input.
    /// </summary>
    public static class RingtoetsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a foreshore geometry.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateForeshoreGeometryChartData()
        {
            return new ChartLineData(Resources.Foreshore_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.DarkOrange,
                                         Width = 2,
                                         DashStyle = ChartLineDashStyle.Solid,
                                         IsEditable = true
                                     });
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a surface line.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateSurfaceLineChartData()
        {
            return new ChartLineData(Resources.SurfaceLine_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.Sienna,
                                         Width = 2,
                                         DashStyle = ChartLineDashStyle.Solid,
                                         IsEditable = true
                                     });
        }

        /// <summary>
        /// Create a <see cref="ChartDataCollection"/> for a soil profile.
        /// </summary>
        /// <returns>The created <see cref="ChartDataCollection"/>.</returns>
        public static ChartDataCollection CreateSoilProfileChartData()
        {
            return new ChartDataCollection(Resources.StochasticSoilProfileProperties_DisplayName);
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type ditch polder side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDitchPolderSideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_DitchPolderSide,
                                      GetCharacteristicPointStyle(Color.IndianRed,
                                                                  Color.Transparent,
                                                                  ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type bottom ditch polder side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateBottomDitchPolderSideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_BottomDitchPolderSide,
                                      GetCharacteristicPointStyle(Color.Teal,
                                                                  Color.Transparent,
                                                                  ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type bottom ditch dike side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateBottomDitchDikeSideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_BottomDitchDikeSide,
                                      GetCharacteristicPointStyle(Color.DarkSeaGreen,
                                                                  Color.Transparent,
                                                                  ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type ditch dike side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDitchDikeSideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_DitchDikeSide,
                                      GetCharacteristicPointStyle(Color.MediumPurple,
                                                                  Color.Transparent,
                                                                  ChartPointSymbol.Circle));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type dike toe at polder.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDikeToeAtPolderChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtPolder,
                                      GetCharacteristicPointStyle(Color.LightGray,
                                                                  Color.Black,
                                                                  ChartPointSymbol.Square));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type dike toe at river.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDikeToeAtRiverChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtRiver,
                                      GetCharacteristicPointStyle(Color.DarkGray,
                                                                  Color.Black,
                                                                  ChartPointSymbol.Square));
        }

        private static ChartPointStyle GetCharacteristicPointStyle(Color fillColor, Color strokeColor, ChartPointSymbol symbol)
        {
            return new ChartPointStyle
            {
                Color = fillColor,
                StrokeColor = strokeColor,
                Size = 8,
                StrokeThickness = strokeColor == Color.Transparent ? 0 : 1,
                Symbol = symbol,
                IsEditable = true
            };
        }
    }
}