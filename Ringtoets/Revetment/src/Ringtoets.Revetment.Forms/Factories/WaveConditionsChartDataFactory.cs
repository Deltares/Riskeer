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

using System.Drawing;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Ringtoets.Common.Forms.Factories;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as
    /// wave conditions input.
    /// </summary>
    internal static class WaveConditionsChartDataFactory
    {
        private const int revetmentThickness = 8;
        private const int levelThickness = 3;

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for lower boundary revetment.
        /// </summary>
        /// <param name="lineColor">The color of the <see cref="ChartLineData"/>.</param>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateLowerRevetmentBoundaryChartData(Color lineColor)
        {
            return new ChartLineData(Resources.WaveConditionsChartDataFactory_LowerBoundaryRevetment_DisplayName,
                                     GetRevetmentBoundaryStyle(lineColor));
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for upper boundary revetment.
        /// </summary>
        /// <param name="lineColor">The color of the <see cref="ChartLineData"/>.</param>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateUpperRevetmentBoundaryChartData(Color lineColor)
        {
            return new ChartLineData(Resources.WaveConditionsChartDataFactory_UpperBoundaryRevetment_DisplayName,
                                     GetRevetmentBoundaryStyle(lineColor));
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for revetment.
        /// </summary>
        /// <param name="lineColor">The color of the <see cref="ChartLineData"/>.</param>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateRevetmentChartData(Color lineColor)
        {
            return new ChartLineData(Resources.WaveConditionsChartDataFactory_Revetment_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = lineColor,
                                         Width = revetmentThickness,
                                         DashStyle = ChartLineDashStyle.Solid,
                                         IsEditable = true
                                     });
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for revetment base.
        /// </summary>
        /// <param name="lineColor">The color of the <see cref="ChartLineData"/>.</param>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateRevetmentBaseChartData(Color lineColor)
        {
            return new ChartLineData(Resources.WaveConditionsChartDataFactory_RevetmentBase_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.FromArgb(120, lineColor),
                                         Width = revetmentThickness,
                                         DashStyle = ChartLineDashStyle.Dash,
                                         IsEditable = true
                                     });
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for lower boundary water levels.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateLowerWaterLevelsBoundaryChartData()
        {
            return new ChartLineData(Resources.WaveConditionsChartDataFactory_LowerBoundaryWaterLevels_DisplayName,
                                     GetWaterLevelsBoundaryStyle());
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for upper boundary water levels.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateUpperWaterLevelsBoundaryChartData()
        {
            return new ChartLineData(Resources.WaveConditionsChartDataFactory_UpperBoundaryWaterLevels_DisplayName,
                                     GetWaterLevelsBoundaryStyle());
        }

        /// <summary>
        /// Create <see cref="ChartLineData"/> with default styling for the assessment level.
        /// </summary>
        public static ChartLineData CreateAssessmentLevelChartData()
        {
            return new ChartLineData(Resources.WaveConditionsInput_AssessmentLevel_DisplayName_without_unit,
                                     new ChartLineStyle
                                     {
                                         Color = Color.LightCoral,
                                         Width = levelThickness,
                                         DashStyle = ChartLineDashStyle.Solid,
                                         IsEditable = true
                                     });
        }

        /// <summary>
        /// Create <see cref="ChartMultipleLineData"/> with default styling for water levels.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleLineData"/>.</returns>
        public static ChartMultipleLineData CreateWaterLevelsChartData()
        {
            return new ChartMultipleLineData(Resources.WaveConditionsChartDataFactory_WaterLevels_DisplayName,
                                             new ChartLineStyle
                                             {
                                                 Color = Color.DarkTurquoise,
                                                 Width = levelThickness,
                                                 DashStyle = ChartLineDashStyle.DashDotDot,
                                                 IsEditable = true
                                             });
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="input"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="input">The <see cref="WaveConditionsInput"/> used for obtaining the name.</param>
        /// <remarks>A default name is set (the same as in <see cref="RingtoetsChartDataFactory.CreateForeshoreGeometryChartData"/>) when:
        /// <list type="bullet">
        /// <item><paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore profile in <paramref name="input"/> is <c>null</c>;</item>
        /// <item>the foreshore profile should not be used.</item>
        /// </list>
        /// </remarks>
        public static void UpdateForeshoreGeometryChartDataName(ChartLineData chartData, WaveConditionsInput input)
        {
            chartData.Name = input?.ForeshoreProfile != null && input.UseForeshore
                                 ? string.Format(RingtoetsCommonFormsResources.ChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                                 input.ForeshoreProfile.Name,
                                                 RingtoetsCommonFormsResources.Foreshore_DisplayName)
                                 : RingtoetsCommonFormsResources.Foreshore_DisplayName;
        }

        private static ChartLineStyle GetRevetmentBoundaryStyle(Color lineColor)
        {
            return new ChartLineStyle
            {
                Color = lineColor,
                Width = levelThickness,
                DashStyle = ChartLineDashStyle.Solid,
                IsEditable = true
            };
        }

        private static ChartLineStyle GetWaterLevelsBoundaryStyle()
        {
            return new ChartLineStyle
            {
                Color = Color.MediumBlue,
                Width = levelThickness,
                DashStyle = ChartLineDashStyle.Solid,
                IsEditable = true
            };
        }
    }
}