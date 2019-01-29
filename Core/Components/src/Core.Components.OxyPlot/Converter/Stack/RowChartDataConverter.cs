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
using System.Linq;
using Core.Components.Stack.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter.Stack
{
    /// <summary>
    /// The converter that converts <see cref="RowChartData"/> data into <see cref="ColumnSeries"/> data.
    /// </summary>
    public static class RowChartDataConverter
    {
        /// <summary>
        /// Sets data to <paramref name="series"/> based on <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The row chart data to create the series from.</param>
        /// <param name="series">The series to set the converted data to.</param>
        public static void ConvertSeriesData(RowChartData data, ColumnSeries series)
        {
            ValidateParameters(data, series);

            series.Items.AddRange(data.Values.Select(i => new ColumnItem(i)));
        }

        /// <summary>
        /// Converts all general properties of <see cref="RowChartData"/>
        /// from <paramref name="data"/> to <paramref name="series"/>.
        /// </summary>
        /// <param name="data">The row chart data to convert the general properties from.</param>
        /// <param name="series">The series to convert the general properties to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> 
        /// or <paramref name="series"/> is <c>null</c>.</exception>
        public static void ConvertSeriesProperties(RowChartData data, ColumnSeries series)
        {
            ValidateParameters(data, series);

            series.Title = data.Name;

            if (data.Color.HasValue)
            {
                series.FillColor = ChartDataHelper.Convert(data.Color.Value);
            }
        }

        private static void ValidateParameters(RowChartData data, ColumnSeries series)
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