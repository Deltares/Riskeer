﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Drawing;
using System.Linq;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> for data used as input in the macro stability inwards failure mechanism.
    /// </summary>
    internal static class MacroStabilityInwardsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type shoulder base inside.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateShoulderBaseInsideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_ShoulderBaseInside,
                                      GetCharacteristicPointStyle(Color.BlueViolet, Color.SeaGreen, ChartPointSymbol.Triangle));
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
        /// of type dike top at polder.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDikeTopAtPolderChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtPolder,
                                      GetCharacteristicPointStyle(Color.LightSkyBlue,
                                                                  Color.SeaGreen,
                                                                  ChartPointSymbol.Triangle));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type shoulder top inside.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateShoulderTopInsideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_ShoulderTopInside,
                                      GetCharacteristicPointStyle(Color.DeepSkyBlue,
                                                                  Color.SeaGreen,
                                                                  ChartPointSymbol.Triangle));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type surface level inside.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateSurfaceLevelInsideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelInside,
                                      GetCharacteristicPointStyle(Color.ForestGreen,
                                                                  Color.Black,
                                                                  ChartPointSymbol.Square));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type surface level outside.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateSurfaceLevelOutsideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelOutside,
                                      GetCharacteristicPointStyle(Color.LightSeaGreen,
                                                                  Color.Black,
                                                                  ChartPointSymbol.Square));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type traffic load inside.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateTrafficLoadInsideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_TrafficLoadInside,
                                      GetCharacteristicPointStyle(Color.BurlyWood,
                                                                  Color.Transparent,
                                                                  ChartPointSymbol.Diamond));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type traffic load outside.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateTrafficLoadOutsideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_TrafficLoadOutside,
                                      GetCharacteristicPointStyle(Color.SlateGray,
                                                                  Color.Transparent,
                                                                  ChartPointSymbol.Diamond));
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

        /// <summary>
        /// Create <see cref="ChartData"/> for a <see cref="MacroStabilityInwardsSoilLayer"/> based on its color.
        /// </summary>
        /// <param name="soilLayerIndex">The index of the <see cref="MacroStabilityInwardsSoilLayer"/> in <paramref name="soilProfile"/> for which to create <see cref="ChartData"/>.</param>
        /// <param name="soilProfile">The <see cref="MacroStabilityInwardsSoilProfile"/> which contains the <see cref="MacroStabilityInwardsSoilLayer"/>.</param>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="soilLayerIndex"/> is outside the allowable range of values ([0, number_of_soil_layers>).</exception>
        public static ChartMultipleAreaData CreateSoilLayerChartData(int soilLayerIndex, MacroStabilityInwardsSoilProfile soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            if (soilLayerIndex < 0 || soilLayerIndex >= soilProfile.Layers.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(soilLayerIndex));
            }

            MacroStabilityInwardsSoilLayer soilLayer = soilProfile.Layers.ElementAt(soilLayerIndex);

            return new ChartMultipleAreaData($"{soilLayerIndex + 1} {soilLayer.MaterialName}",
                                             new ChartAreaStyle
                                             {
                                                 FillColor = soilLayer.Color,
                                                 StrokeColor = Color.Black,
                                                 StrokeThickness = 1
                                             });
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="CreateSurfaceLineChartData"/>) when <paramref name="surfaceLine"/> is <c>null</c>.</remarks>
        public static void UpdateSurfaceLineChartDataName(ChartLineData chartData, RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
        {
            chartData.Name = surfaceLine != null
                                 ? surfaceLine.Name
                                 : Resources.RingtoetsMacroStabilityInwardsSurfaceLine_DisplayName;
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartDataCollection"/> to update the name for.</param>
        /// <param name="soilProfile">The <see cref="MacroStabilityInwardsSoilProfile"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="CreateSoilProfileChartData"/>) when
        /// <paramref name="soilProfile"/> is <c>null</c>.</remarks>
        public static void UpdateSoilProfileChartDataName(ChartDataCollection chartData, MacroStabilityInwardsSoilProfile soilProfile)
        {
            chartData.Name = soilProfile != null
                                 ? soilProfile.Name
                                 : Resources.StochasticSoilProfileProperties_DisplayName;
        }

        private static ChartPointStyle GetCharacteristicPointStyle(Color fillColor, Color strokeColor, ChartPointSymbol symbol)
        {
            return new ChartPointStyle
            {
                Color = fillColor,
                StrokeColor = strokeColor,
                Size = 8,
                StrokeThickness = strokeColor == Color.Transparent ? 0 : 1,
                Symbol = symbol
            };
        }
    }
}