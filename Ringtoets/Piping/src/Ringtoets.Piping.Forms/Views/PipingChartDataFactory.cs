﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;
using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> for data used as input in the piping failure mechanism.
    /// </summary>
    public static class PipingChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateSurfaceLineChartData()
        {
            return new ChartLineData(Resources.RingtoetsPipingSurfaceLine_DisplayName)
            {
                Style = new ChartLineStyle(Color.Sienna, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for an entry point.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateEntryPointChartData()
        {
            return new ChartPointData(Resources.PipingInput_EntryPointL_DisplayName)
            {
                Style = GetGeneralPointStyle(Color.Gold)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for an exit point.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateExitPointChartData()
        {
            return new ChartPointData(Resources.PipingInput_ExitPointL_DisplayName)
            {
                Style = GetGeneralPointStyle(Color.Tomato)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point of type ditch polder side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDitchPolderSideChartData()
        {
            return new ChartPointData(PipingDataResources.CharacteristicPoint_DitchPolderSide)
            {
                Style = GetCharacteristicPointStyle(Color.IndianRed)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point of type bottom ditch polder side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateBottomDitchPolderSideChartData()
        {
            return new ChartPointData(PipingDataResources.CharacteristicPoint_BottomDitchPolderSide)
            {
                Style = GetCharacteristicPointStyle(Color.Teal)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point of type bottom ditch dike side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateBottomDitchDikeSideChartData()
        {
            return new ChartPointData(PipingDataResources.CharacteristicPoint_BottomDitchDikeSide)
            {
                Style = GetCharacteristicPointStyle(Color.DarkSeaGreen)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point of type ditch dike side.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDitchDikeSideChartData()
        {
            return new ChartPointData(PipingDataResources.CharacteristicPoint_DitchDikeSide)
            {
                Style = GetCharacteristicPointStyle(Color.MediumPurple)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point of type dike toe at river.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDikeToeAtRiverChartData()
        {
            return new ChartPointData(PipingDataResources.CharacteristicPoint_DikeToeAtRiver)
            {
                Style = GetCharacteristicPointStyle(Color.DarkBlue)
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point of type dike toe at polder.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDikeToeAtPolderChartData()
        {
            return new ChartPointData(PipingDataResources.CharacteristicPoint_DikeToeAtPolder)
            {
                Style = GetCharacteristicPointStyle(Color.SlateGray)
            };
        }

        /// <summary>
        /// Create a <see cref="ChartDataCollection"/> for a <see cref="PipingSoilProfile"/>.
        /// </summary>
        /// <returns>The created <see cref="ChartDataCollection"/>.</returns>
        public static ChartDataCollection CreateSoilProfileChartData()
        {
            return new ChartDataCollection(Resources.StochasticSoilProfileProperties_DisplayName);
        }

        /// <summary>
        /// Create <see cref="ChartData"/> for a <see cref="PipingSoilLayer"/> based on its color.
        /// </summary>
        /// <param name="soilLayerIndex">The index of the <see cref="PipingSoilLayer"/> in <paramref name="soilProfile"/> for which to create <see cref="ChartData"/>.</param>
        /// <param name="soilProfile">The <see cref="PipingSoilProfile"/> which contains the <see cref="PipingSoilLayer"/>.</param>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        public static ChartMultipleAreaData CreateSoilLayerChartData(int soilLayerIndex, PipingSoilProfile soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException("soilProfile");
            }

            var soilLayer = soilProfile.Layers.ElementAt(soilLayerIndex);

            return new ChartMultipleAreaData(string.Format("{0} {1}", soilLayerIndex + 1, soilLayer.MaterialName))
            {
                Style = new ChartAreaStyle(soilLayer.Color, Color.Black, 1)
            };
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="surfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="CreateSurfaceLineChartData"/>) when <paramref name="surfaceLine"/> is <c>null</c>.</remarks>
        public static void UpdateSurfaceLineChartDataName(ChartLineData chartData, RingtoetsPipingSurfaceLine surfaceLine)
        {
            chartData.Name = surfaceLine != null
                                 ? surfaceLine.Name
                                 : Resources.RingtoetsPipingSurfaceLine_DisplayName;
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="stochasticSoilProfile"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartDataCollection"/> to update the name for.</param>
        /// <param name="stochasticSoilProfile">The <see cref="StochasticSoilProfile"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="CreateSoilProfileChartData"/>) when:
        /// <list type="bullet">
        /// <item><paramref name="stochasticSoilProfile"/> is <c>null</c>;</item>
        /// <item>the <see cref="PipingSoilProfile"/> in <paramref name="stochasticSoilProfile"/> is <c>null</c>.</item>
        /// </list>
        /// </remarks>
        public static void UpdateSoilProfileChartDataName(ChartDataCollection chartData, StochasticSoilProfile stochasticSoilProfile)
        {
            chartData.Name = stochasticSoilProfile != null && stochasticSoilProfile.SoilProfile != null
                                 ? stochasticSoilProfile.SoilProfile.Name
                                 : Resources.StochasticSoilProfileProperties_DisplayName;
        }

        private static ChartPointStyle GetGeneralPointStyle(Color color)
        {
            return new ChartPointStyle(color, 8, Color.Transparent, 0, ChartPointSymbol.Triangle);
        }

        private static ChartPointStyle GetCharacteristicPointStyle(Color indianRed)
        {
            return new ChartPointStyle(indianRed, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }
    }
}