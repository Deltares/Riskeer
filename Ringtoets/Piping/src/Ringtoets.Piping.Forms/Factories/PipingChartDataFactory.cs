// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Primitives;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> for data used as input in the piping failure mechanism.
    /// </summary>
    internal static class PipingChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for an entry point.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateEntryPointChartData()
        {
            return new ChartPointData(Resources.PipingInput_EntryPointL_DisplayName,
                                      GetGeneralPointStyle(Color.Gold));
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for an exit point.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateExitPointChartData()
        {
            return new ChartPointData(Resources.PipingInput_ExitPointL_DisplayName,
                                      GetGeneralPointStyle(Color.Tomato));
        }

        /// <summary>
        /// Create <see cref="ChartData"/> for a <see cref="PipingSoilLayer"/> based on its color.
        /// </summary>
        /// <param name="soilLayerIndex">The index of the <see cref="PipingSoilLayer"/> in <paramref name="soilProfile"/> for which to create <see cref="ChartData"/>.</param>
        /// <param name="soilProfile">The <see cref="PipingSoilProfile"/> which contains the <see cref="PipingSoilLayer"/>.</param>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="soilProfile"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="soilLayerIndex"/> is outside the allowable range of values ([0, number_of_soil_layers>).</exception>
        public static ChartMultipleAreaData CreateSoilLayerChartData(int soilLayerIndex, PipingSoilProfile soilProfile)
        {
            if (soilProfile == null)
            {
                throw new ArgumentNullException(nameof(soilProfile));
            }

            if (soilLayerIndex < 0 || soilLayerIndex >= soilProfile.Layers.Count())
            {
                throw new ArgumentOutOfRangeException(nameof(soilLayerIndex));
            }

            PipingSoilLayer soilLayer = soilProfile.Layers.ElementAt(soilLayerIndex);

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
        /// <param name="surfaceLine">The <see cref="PipingSurfaceLine"/> used for obtaining the name.</param>
        /// <remarks>A default name is set when <paramref name="surfaceLine"/> is <c>null</c>.</remarks>
        public static void UpdateSurfaceLineChartDataName(ChartLineData chartData, PipingSurfaceLine surfaceLine)
        {
            chartData.Name = surfaceLine != null
                                 ? surfaceLine.Name
                                 : RingtoetsCommonFormsResources.SurfaceLine_DisplayName;
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartDataCollection"/> to update the name for.</param>
        /// <param name="soilProfile">The <see cref="PipingSoilProfile"/> used for obtaining the name.</param>
        /// <remarks>A default name is set when <paramref name="soilProfile"/> is <c>null</c>.</remarks>
        public static void UpdateSoilProfileChartDataName(ChartDataCollection chartData, PipingSoilProfile soilProfile)
        {
            chartData.Name = soilProfile != null
                                 ? soilProfile.Name
                                 : RingtoetsCommonFormsResources.StochasticSoilProfileProperties_DisplayName;
        }

        private static ChartPointStyle GetGeneralPointStyle(Color color)
        {
            return new ChartPointStyle
            {
                Color = color,
                StrokeColor = Color.Transparent,
                Size = 8,
                StrokeThickness = 0,
                Symbol = ChartPointSymbol.Triangle
            };
        }
    }
}