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
using System.Globalization;
using Core.Components.Chart.Data;

namespace Core.Components.OxyPlot.DataSeries.Chart
{
    /// <summary>
    /// A factory to create <see cref="IChartDataSeries"/> based on <see cref="ChartData"/>.
    /// </summary>
    public static class ChartDataSeriesFactory
    {
        /// <summary>
        /// Creates a <see cref="IChartDataSeries"/> based on <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to create a <see cref="IChartDataSeries"/> from.</param>
        /// <returns>A <see cref="IChartDataSeries"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="data"/> type is not supported.</exception>
        public static IChartDataSeries Create(ChartData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            var chartPointData = data as ChartPointData;
            if (chartPointData != null)
            {
                return new ChartPointDataSeries(chartPointData);
            }

            var chartLineData = data as ChartLineData;
            if (chartLineData != null)
            {
                return new ChartLineDataSeries(chartLineData);
            }

            var chartAreaData = data as ChartAreaData;
            if (chartAreaData != null)
            {
                return new ChartAreaDataSeries(chartAreaData);
            }

            var chartMultipleAreaData = data as ChartMultipleAreaData;
            if (chartMultipleAreaData != null)
            {
                return new ChartMultipleAreaDataSeries(chartMultipleAreaData);
            }

            var chartMultipleLineData = data as ChartMultipleLineData;
            if (chartMultipleLineData != null)
            {
                return new ChartMultipleLineDataSeries(chartMultipleLineData);
            }

            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "ChartData of type {0} is not supported.", data.GetType().Name));
        }
    }
}