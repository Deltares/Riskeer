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
using System.Globalization;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// Provides an abstract base class for transforming <see cref="ChartData"/> in specific
    /// <see cref="Series"/> instances.
    /// </summary>
    public abstract class ChartDataConverter<T> : IChartDataConverter where T : ChartData
    {
        public bool CanConvertSeries(ChartData data)
        {
            return data.GetType() == typeof(T);
        }

        public IList<Series> Convert(ChartData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", @"Null data cannot be converted into series.");
            }
            if (!CanConvertSeries(data))
            {
                var message = string.Format(CultureInfo.CurrentCulture,
                                            "The data of type {0} cannot be converted by this converter.",
                                            data.GetType());
                throw new ArgumentException(message);
            }
            return Convert((T) data);
        }

        /// <summary>
        /// Transforms a given object into a <see cref="DataPoint"/>. Can be used as a 
        /// <see cref="DataPointSeries.Mapping"/>.
        /// </summary>
        /// <param name="point2D">The object to convert into a <see cref="DataPoint"/>.</param>
        /// <returns>A new <see cref="DataPoint"/> based on <paramref name="point2D"/>.</returns>
        /// <exception cref="InvalidCastException">Thrown when <paramref name="point2D"/> is not
        /// of type <see cref="Point2D"/>.</exception>
        protected static DataPoint Point2DToDataPoint(object point2D)
        {
            Point2D point = (Point2D) point2D;
            return new DataPoint(point.X, point.Y);
        }

        /// <summary>
        /// Creates one or more <see cref="Series"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into one or more <see cref="Series"/>.</param>
        /// <returns>A new <see cref="IList{T}"/> of <see cref="Series"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        protected abstract IList<Series> Convert(T data);
    }
}