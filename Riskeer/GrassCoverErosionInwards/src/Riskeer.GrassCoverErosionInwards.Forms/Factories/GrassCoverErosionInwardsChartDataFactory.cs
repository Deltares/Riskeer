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
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Forms.Factories;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.GrassCoverErosionInwards.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.GrassCoverErosionInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input
    /// in the grass cover erosion inwards failure mechanism.
    /// </summary>
    internal static class GrassCoverErosionInwardsChartDataFactory
    {
        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a dike geometry.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateDikeGeometryChartData()
        {
            return new ChartLineData(Resources.DikeProfile_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.SaddleBrown,
                                         Width = 2,
                                         DashStyle = ChartLineDashStyle.Solid,
                                         IsEditable = true
                                     });
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for a dike height.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateDikeHeightChartData()
        {
            return new ChartLineData(Resources.DikeHeight_ChartName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.MediumSeaGreen,
                                         Width = 2,
                                         DashStyle = ChartLineDashStyle.Dash,
                                         IsEditable = true
                                     });
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
                                 ? string.Format(RingtoetsCommonFormsResources.ChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                 dikeProfile.Name,
                                                 Resources.DikeProfile_DisplayName)
                                 : Resources.DikeProfile_DisplayName;
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="input"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="input">The <see cref="GrassCoverErosionInwardsInput"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="RingtoetsChartDataFactory.CreateForeshoreGeometryChartData"/>) when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the dike profile in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore should not be used.</item>
        /// </list>
        /// </remarks>
        public static void UpdateForeshoreGeometryChartDataName(ChartLineData chartData, GrassCoverErosionInwardsInput input)
        {
            chartData.Name = input?.DikeProfile != null && input.UseForeshore
                                 ? string.Format(RingtoetsCommonFormsResources.ChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                 input.DikeProfile.Name,
                                                 RingtoetsCommonFormsResources.Foreshore_DisplayName)
                                 : RingtoetsCommonFormsResources.Foreshore_DisplayName;
        }
    }
}