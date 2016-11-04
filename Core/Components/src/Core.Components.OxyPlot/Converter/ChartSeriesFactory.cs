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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class creates new <see cref="Series"/> objects from <see cref="ChartData"/>.
    /// </summary>
    public class ChartSeriesFactory
    {
        /// <summary>
        /// Collection of converters that the <see cref="ChartSeriesFactory"/> can use to transform <see cref="ChartData"/>.
        /// </summary>
        private readonly IEnumerable<IChartDataConverter> converters = new Collection<IChartDataConverter>
        {
            new ChartMultipleAreaDataConverter(),
            new ChartAreaDataConverter(),
            new ChartLineDataConverter(),
            new ChartPointDataConverter(),
            new ChartDataCollectionConverter()
        };

        /// <summary>
        /// Creates one or more new <see cref="Series"/> from the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to base the creation of the <see cref="Series"/> upon.</param>
        /// <returns>A new <see cref="IList{T}"/> of <see cref="Series"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the given <paramref name="data"/> type is not supported.</exception>
        public IList<Series> Create(ChartData data)
        {
            foreach (var converter in converters)
            {
                if (converter.CanConvertSeries(data))
                {
                    return converter.Convert(data);
                }
            }
            throw new NotSupportedException(string.Format(CultureInfo.CurrentCulture,
                                                          "ChartData of type {0} is not supported.",
                                                          data.GetType().Name));
        }
    }
}