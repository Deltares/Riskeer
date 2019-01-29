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
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;

namespace Riskeer.MacroStabilityInwards.Forms.Factories
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> for slice data used in the macro stability inwards failure mechanism.
    /// </summary>
    internal static class MacroStabilityInwardsSliceChartDataFactory
    {
        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the slices in a slip plane.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateSlicesChartData()
        {
            return new ChartMultipleAreaData(Resources.Slices_DisplayName,
                                             new ChartAreaStyle
                                             {
                                                 FillColor = Color.Empty,
                                                 StrokeColor = Color.DarkGreen,
                                                 StrokeThickness = 2,
                                                 IsEditable = true
                                             });
        }

        /// <summary>
        /// Create a <see cref="ChartDataCollection"/> for the slice output parameters.
        /// </summary>
        public static ChartDataCollection CreateSliceParametersChartDataCollection()
        {
            return new ChartDataCollection(Resources.SliceParameters_DisplayName);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.Cohesion"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateCohesionChartData()
        {
            return CreateSliceParameterChartData(Resources.Cohesion_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.EffectiveStress"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateEffectiveStressChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_EffectiveStress_DisplayName, true);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.EffectiveStressDaily"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateEffectiveStressDailyChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_EffectiveStressDaily_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.TotalPorePressure"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateTotalPorePressureChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_TotalPorePressure_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.Weight"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateWeightChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_Weight_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.PiezometricPorePressure"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreatePiezometricPorePressureChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_PiezometricPorePressure_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.PorePressure"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreatePorePressureChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_PorePressure_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.VerticalPorePressure"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateVerticalPorePressureChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_VerticalPorePressure_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.HorizontalPorePressure"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateHorizontalPorePressureChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_HorizontalPorePressure_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.OverConsolidationRatio"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateOverConsolidationRatioChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_OverConsolidationRatio_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.Pop"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreatePopChartData()
        {
            return CreateSliceParameterChartData(Resources.Pop_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.NormalStress"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateNormalStressChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_NormalStress_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.ShearStress"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateShearStressChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_ShearStress_DisplayName, false);
        }

        /// <summary>
        /// Create a <see cref="ChartMultipleAreaData"/> for the representation
        /// of <see cref="MacroStabilityInwardsSlice.LoadStress"/> values in a
        /// sliding curve.
        /// </summary>
        /// <returns>The created <see cref="ChartMultipleAreaData"/>.</returns>
        public static ChartMultipleAreaData CreateLoadStressChartData()
        {
            return CreateSliceParameterChartData(Resources.MacroStabilityInwardsSlice_LoadStress_DisplayName, false);
        }

        private static ChartMultipleAreaData CreateSliceParameterChartData(string name, bool isVisible)
        {
            return new ChartMultipleAreaData(name, new ChartAreaStyle
            {
                StrokeThickness = 1,
                StrokeColor = Color.Black,
                FillColor = Color.FromArgb(150, Color.Red),
                IsEditable = true
            })
            {
                IsVisible = isVisible
            };
        }
    }
}