// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Components.Charting.Data;

namespace Core.Components.OxyPlot.DataSeries
{
    /// <summary>
    /// A factory to create <see cref="IItemBasedChartDataSeries"/> based on <see cref="ItemBasedChartData"/>.
    /// </summary>
    public static class ItemBasedChartDataSeriesFactory
    {
        /// <summary>
        /// Creates a <see cref="IItemBasedChartDataSeries"/> based on <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ItemBasedChartData"/> to create a <see cref="IItemBasedChartDataSeries"/> from.</param>
        /// <returns>A <see cref="IItemBasedChartDataSeries"/> instance.</returns>
        /// <exception cref="ArgumentNullException">Thrown when the given <paramref name="data"/> is <c>null</c>.</exception>
        /// <exception cref="NotSupportedException">Thrown when the given <paramref name="data"/> type is not supported.</exception>
        public static IItemBasedChartDataSeries Create(ItemBasedChartData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
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

            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture, "ItemBasedChartData of type {0} is not supported.", data.GetType().Name));
        }
    }
}