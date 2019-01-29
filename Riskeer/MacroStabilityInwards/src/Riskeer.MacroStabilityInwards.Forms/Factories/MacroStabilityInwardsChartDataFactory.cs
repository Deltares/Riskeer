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

using System;
using System.Drawing;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Riskeer.Common.Data.Helpers;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> for data used in the macro stability inwards failure mechanism.
    /// </summary>
    internal static class MacroStabilityInwardsChartDataFactory
    {
        /// <summary>
        /// Create a <see cref="ChartDataCollection"/> for waternet zones
        /// under extreme circumstances.
        /// </summary>
        /// <returns>The created <see cref="ChartDataCollection"/>.</returns>
        public static ChartDataCollection CreateWaternetZonesExtremeChartDataCollection()
        {
            return new ChartDataCollection(Resources.WaternetLinesExtreme_DisplayName);
        }

        /// <summary>
        /// Create a <see cref="ChartDataCollection"/> for waternet zones
        /// under daily circumstances.
        /// </summary>
        /// <returns>The created <see cref="ChartDataCollection"/>.</returns>
        public static ChartDataCollection CreateWaternetZonesDailyChartDataCollection()
        {
            return new ChartDataCollection(Resources.WaternetLinesDaily_DisplayName);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for a waternet zone.
        /// </summary>
        /// <param name="name">The name of the zone.</param>
        /// <param name="isVisible">The default visibility of the zone.</param>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public static ChartMultipleAreaData CreateWaternetZoneChartData(string name, bool isVisible)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new ChartMultipleAreaData(name, new ChartAreaStyle
            {
                StrokeThickness = 0,
                FillColor = Color.FromArgb(60, Color.DeepSkyBlue),
                IsEditable = true
            })
            {
                IsVisible = isVisible
            };
        }

        /// <summary>
        /// Create a <see cref="ChartLineData"/> for a phreatic line.
        /// </summary>
        /// <param name="name">The name of the line.</param>
        /// <param name="isVisible">The default visibility of the line.</param>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/>
        /// is <c>null</c>.</exception>
        public static ChartLineData CreatePhreaticLineChartData(string name, bool isVisible)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            return new ChartLineData(name, new ChartLineStyle
            {
                Color = Color.Blue,
                DashStyle = ChartLineDashStyle.Solid,
                Width = 2,
                IsEditable = true
            })
            {
                IsVisible = isVisible
            };
        }

        /// <summary>
        /// Create <see cref="ChartPointData"/> with default styling for a characteristic point 
        /// of type shoulder base inside.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateShoulderBaseInsideChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_ShoulderBaseInside,
                                      GetCharacteristicPointStyle(Color.BlueViolet,
                                                                  Color.SeaGreen,
                                                                  ChartPointSymbol.Triangle));
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
        /// of type dike top at river.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateDikeTopAtRiverChartData()
        {
            return new ChartPointData(RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtRiver,
                                      GetCharacteristicPointStyle(Color.LightSteelBlue,
                                                                  Color.SeaGreen,
                                                                  ChartPointSymbol.Triangle));
        }

        /// <summary>
        /// Creates <see cref="ChartPointData"/> with default styling for grids for the right grid.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateRightGridChartData()
        {
            return new ChartPointData(Resources.RightGrid_DisplayName,
                                      GetGridStyle(Color.Black, ChartPointSymbol.Plus));
        }

        /// <summary>
        /// Creates <see cref="ChartPointData"/> with default styling for grids for the left grid.
        /// </summary>
        /// <returns>The created <see cref="ChartPointData"/>.</returns>
        public static ChartPointData CreateLeftGridChartData()
        {
            return new ChartPointData(Resources.LeftGrid_DisplayName,
                                      GetGridStyle(Color.Black, ChartPointSymbol.Plus));
        }

        /// <summary>
        /// Creates <see cref="ChartLineData"/> with default styling for the slip plane.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateSlipPlaneChartData()
        {
            return new ChartLineData(Resources.SlipPlane_DisplayName,
                                     new ChartLineStyle
                                     {
                                         Color = Color.SaddleBrown,
                                         DashStyle = ChartLineDashStyle.Solid,
                                         Width = 3,
                                         IsEditable = true
                                     });
        }

        /// <summary>
        /// Creates <see cref="ChartLineData"/> with default styling for the radius of
        /// the active circle.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreateActiveCircleRadiusChartData()
        {
            return new ChartLineData(Resources.ActiveCircleRadius_DisplayName,
                                     GetCircleRadiusStyle());
        }

        /// <summary>
        /// Creates <see cref="ChartLineData"/> with default styling for the radius of
        /// the passive circle.
        /// </summary>
        /// <returns>The created <see cref="ChartLineData"/>.</returns>
        public static ChartLineData CreatePassiveCircleRadiusChartData()
        {
            return new ChartLineData(Resources.PassiveCircleRadius_DisplayName,
                                     GetCircleRadiusStyle());
        }

        /// <summary>
        /// Create <see cref="ChartMultipleAreaData"/> for a <see cref="MacroStabilityInwardsSoilLayer2D"/>.
        /// </summary>
        /// <param name="layer">The layer to create the <see cref="ChartMultipleAreaData"/> for.</param>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="layer"/> is <c>null</c>.</exception>
        public static ChartMultipleAreaData CreateSoilLayerChartData(MacroStabilityInwardsSoilLayer2D layer)
        {
            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer));
            }

            MacroStabilityInwardsSoilLayerData data = layer.Data;
            return new ChartMultipleAreaData(SoilLayerDataHelper.GetValidName(data.MaterialName),
                                             new ChartAreaStyle
                                             {
                                                 FillColor = SoilLayerDataHelper.GetValidColor(data.Color),
                                                 StrokeColor = Color.Black,
                                                 StrokeThickness = 1
                                             });
        }

        /// <summary>
        /// Creates <see cref="ChartMultipleLineData"/> with default styling for the tangent lines.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleLineData"/>.</returns>
        public static ChartMultipleLineData CreateTangentLinesChartData()
        {
            return new ChartMultipleLineData(Resources.TangentLines_DisplayName,
                                             new ChartLineStyle
                                             {
                                                 Color = Color.Green,
                                                 DashStyle = ChartLineDashStyle.Dash,
                                                 IsEditable = true,
                                                 Width = 1
                                             });
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartLineData"/> to update the name for.</param>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> used for obtaining the name.</param>
        /// <remarks>A default name is set when <paramref name="surfaceLine"/> is <c>null</c>.</remarks>
        public static void UpdateSurfaceLineChartDataName(ChartLineData chartData, MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            chartData.Name = surfaceLine != null
                                 ? surfaceLine.Name
                                 : RingtoetsCommonFormsResources.SurfaceLine_DisplayName;
        }

        /// <summary>
        /// Updates the name of <paramref name="chartData"/> based on <paramref name="soilProfile"/>.
        /// </summary>
        /// <param name="chartData">The <see cref="ChartDataCollection"/> to update the name for.</param>
        /// <param name="soilProfile">The <see cref="IMacroStabilityInwardsSoilProfile{T}"/> used for obtaining the name.</param>
        /// <remarks>A default name is set when <paramref name="soilProfile"/> is <c>null</c>.</remarks>
        public static void UpdateSoilProfileChartDataName(ChartDataCollection chartData,
                                                          IMacroStabilityInwardsSoilProfile<IMacroStabilityInwardsSoilLayer> soilProfile)
        {
            chartData.Name = soilProfile != null
                                 ? soilProfile.Name
                                 : RingtoetsCommonFormsResources.StochasticSoilProfileProperties_DisplayName;
        }

        private static ChartLineStyle GetCircleRadiusStyle()
        {
            return new ChartLineStyle
            {
                Color = Color.Gray,
                DashStyle = ChartLineDashStyle.Dash,
                Width = 1,
                IsEditable = true
            };
        }

        private static ChartPointStyle GetCharacteristicPointStyle(Color fillColor, Color strokeColor, ChartPointSymbol symbol)
        {
            return new ChartPointStyle
            {
                Color = fillColor,
                StrokeColor = strokeColor,
                Size = 8,
                StrokeThickness = strokeColor == Color.Transparent
                                      ? 0
                                      : 1,
                Symbol = symbol,
                IsEditable = true
            };
        }

        private static ChartPointStyle GetGridStyle(Color color, ChartPointSymbol symbol)
        {
            return new ChartPointStyle
            {
                Color = color,
                StrokeColor = color,
                Size = 6,
                StrokeThickness = color == Color.Transparent
                                      ? 0
                                      : 2,
                Symbol = symbol,
                IsEditable = true
            };
        }
    }
}