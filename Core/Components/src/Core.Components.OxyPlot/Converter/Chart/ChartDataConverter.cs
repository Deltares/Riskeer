// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Components.Chart.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter.Chart
{
    /// <summary>
    /// Abstract base class for transforming <see cref="ChartData"/> data into <see cref="Series"/> data.
    /// </summary>
    /// <typeparam name="TChartData">The type of chart data to convert.</typeparam>
    /// <typeparam name="TSeries">The type of series to set the converted data to.</typeparam>
    public abstract class ChartDataConverter<TChartData, TSeries>
        where TChartData : ChartData
        where TSeries : Series
    {
        /// <summary>
        /// Converts all chart data from <paramref name="data"/> to <paramref name="series"/>.
        /// </summary>
        /// <param name="data">The chart data to create the series from.</param>
        /// <param name="series">The series to set the converted data to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> or <paramref name="series"/> is <c>null</c>.</exception>
        public void ConvertSeriesData(TChartData data, TSeries series)
        {
            ValidateParameters(data, series);

            SetSeriesData(data, series);
        }

        /// <summary>
        /// Converts all general properties (like <see cref="ChartData.Name"/> and <see cref="ChartData.IsVisible"/>) 
        /// from <paramref name="data"/> to <paramref name="series"/>.
        /// </summary>
        /// <param name="data">The chart data to convert the general properties from.</param>
        /// <param name="series">The series to convert the general properties to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> or <paramref name="series"/> is <c>null</c>.</exception>
        public void ConvertSeriesProperties(TChartData data, TSeries series)
        {
            ValidateParameters(data, series);

            series.Title = data.Name;
            series.IsVisible = data.IsVisible;

            SetSeriesStyle(data, series);
        }

        /// <summary>
        /// Sets data to <paramref name="series"/> based on <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The chart data to create the series from.</param>
        /// <param name="series">The series to set the converted data to.</param>
        protected abstract void SetSeriesData(TChartData data, TSeries series);

        /// <summary>
        /// Set a style to <paramref name="series"/> based on <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The chart data to create the style from.</param>
        /// <param name="series">The series to set the style to.</param>
        protected abstract void SetSeriesStyle(TChartData data, TSeries series);

        private static void ValidateParameters(TChartData data, TSeries series)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), @"Null data cannot be converted into series data.");
            }

            if (series == null)
            {
                throw new ArgumentNullException(nameof(series), @"Null data cannot be used as conversion target.");
            }
        }
    }
}