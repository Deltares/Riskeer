// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Drawing.Drawing2D;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input
    /// in the grass cover erosion inwards failure mechanism.
    /// </summary>
    public static class GrassCoverErosionInwardsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a dike geometry.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateDikeGeometryChartData()
        {
            return new ChartLineData(Resources.DikeProfile_DisplayName)
            {
                Style = new ChartLineStyle(Color.SaddleBrown, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a foreshore geometry.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateForeshoreGeometryChartData()
        {
            return new ChartLineData(Resources.Foreshore_DisplayName)
            {
                Style = new ChartLineStyle(Color.DarkOrange, 2, DashStyle.Solid)
            };
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a dike height.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateDikeHeightChartData()
        {
            return new ChartLineData(Resources.DikeHeight_ChartName)
            {
                Style = new ChartLineStyle(Color.MediumSeaGreen, 2, DashStyle.Dash)
            };
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="dikeProfile"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="dikeProfile">The <see cref="DikeProfile"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="CreateDikeGeometryChartData"/>) when <paramref name="dikeProfile"/> is <c>null</c>.</remarks>
        public static void UpdateDikeGeometryChartDataName(ChartLineData chartData, DikeProfile dikeProfile)
        {
            chartData.Name = dikeProfile != null
                                 ? string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                 dikeProfile.Name,
                                                 Resources.DikeProfile_DisplayName)
                                 : Resources.DikeProfile_DisplayName;
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="input"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="input">The <see cref="GrassCoverErosionInwardsInput"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="CreateForeshoreGeometryChartData"/>) when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the dike profile in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore should not be used.</item>
        /// </list>
        /// </remarks>
        public static void UpdateForeshoreGeometryChartDataName(ChartLineData chartData, GrassCoverErosionInwardsInput input)
        {
            chartData.Name = input != null && input.DikeProfile != null && input.UseForeshore
                                 ? string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                 input.DikeProfile.Name,
                                                 Resources.Foreshore_DisplayName)
                                 : Resources.Foreshore_DisplayName;
        }
    }
}