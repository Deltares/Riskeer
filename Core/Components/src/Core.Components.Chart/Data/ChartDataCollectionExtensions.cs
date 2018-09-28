// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;

namespace Core.Components.Chart.Data
{
    /// <summary>
    /// Extension methods for <see cref="ChartDataCollection"/>.
    /// </summary>
    public static class ChartDataCollectionExtensions
    {
        /// <summary>
        /// Gets all the <see cref="ChartData"/> recursively in the given <paramref name="chartDataCollection"/>.
        /// </summary>
        /// <param name="chartDataCollection">The collection to get all <see cref="ChartData"/> from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="ChartData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="chartDataCollection"/> is <c>null</c>.</exception>
        public static IEnumerable<ChartData> GetChartDataRecursively(this ChartDataCollection chartDataCollection)
        {
            if (chartDataCollection == null)
            {
                throw new ArgumentNullException(nameof(chartDataCollection));
            }

            var chartDataList = new List<ChartData>();

            foreach (ChartData chartData in chartDataCollection.Collection)
            {
                var nestedChartDataCollection = chartData as ChartDataCollection;
                if (nestedChartDataCollection != null)
                {
                    chartDataList.AddRange(GetChartDataRecursively(nestedChartDataCollection));
                    continue;
                }

                chartDataList.Add(chartData);
            }

            return chartDataList;
        }
    }
}