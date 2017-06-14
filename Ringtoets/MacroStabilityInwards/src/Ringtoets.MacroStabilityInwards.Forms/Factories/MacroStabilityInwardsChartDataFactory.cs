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
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> for data used as input in the macro stability inwards failure mechanism.
    /// </summary>
    internal static class MacroStabilityInwardsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateSurfaceLineChartData()
        {
            return new ChartLineData(Resources.RingtoetsMacroStabilityInwardsSurfaceLine_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.Sienna,
                                         Width = 2,
                                         DashStyle = ChartLineDashStyle.Solid
                                     });
        }

        /// <summary>
        /// Create a <see cref="ChartDataCollection"/> for a <see cref="MacroStabilityInwardsSoilProfile"/>.
        /// </summary>
        /// <returns>The created <see cref="ChartDataCollection"/>.</returns>
        public static ChartDataCollection CreateSoilProfileChartData()
        {
            return new ChartDataCollection(Resources.StochasticSoilProfileProperties_DisplayName);
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
    }
}