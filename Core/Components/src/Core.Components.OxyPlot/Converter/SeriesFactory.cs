// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class creates new <see cref="Series"/> objects from <see cref="ChartData"/>.
    /// </summary>
    public class SeriesFactory
    {
        /// <summary>
        /// Collection of converters that the <see cref="SeriesFactory"/> can use to transform <see cref="ChartData"/>.
        /// </summary>
        private readonly IEnumerable<IChartDataConverter> converters = new Collection<IChartDataConverter>
        {
            new AreaDataConverter(),
            new LineDataConverter(),
            new PointDataConverter(),
            new ChartDataCollectionConverter()
        };

        /// <summary>
        /// Creates a new <see cref="Series"/> from the given <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to base the creation of a <see cref="Series"/> upon.</param>
        /// <returns>A new <see cref="Series"/>.</returns>
        public IList<Series> Create(ChartData data)
        {
            foreach (var converter in converters)
            {
                if (converter.CanConvertSeries(data))
                {
                    return converter.Convert(data);
                }
            }
            throw new NotSupportedException(string.Format("ChartData of type {0} is not supported.", data.GetType().Name));
        }
    }
}