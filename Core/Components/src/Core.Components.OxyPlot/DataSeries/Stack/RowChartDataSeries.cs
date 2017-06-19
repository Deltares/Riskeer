// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Components.OxyPlot.Converter.Stack;
using Core.Components.Stack.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.DataSeries.Stack
{
    /// <summary>
    /// A <see cref="ColumnSeries"/> based on and updated according to the wrapped <see cref="StackChartData"/>.
    /// </summary>
    public class RowChartDataSeries : ColumnSeries, IChartDataSeries
    {
        private readonly RowChartData rowChartData;

        private List<double> drawnValues;

        /// <summary>
        /// Creates a new instance of <see cref="RowChartDataSeries"/>.
        /// </summary>
        /// <param name="rowChartData">The <see cref="RowChartData"/> which the series is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="rowChartData"/> is <c>null</c>.</exception>
        public RowChartDataSeries(RowChartData rowChartData)
        {
            if (rowChartData == null)
            {
                throw new ArgumentNullException(nameof(rowChartData));
            }

            this.rowChartData = rowChartData;

            IsStacked = true;
            StrokeThickness = 1;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(rowChartData.Values, drawnValues))
            {
                RowChartDataConverter.ConvertSeriesData(rowChartData, this);

                drawnValues = rowChartData.Values;
            }

            RowChartDataConverter.ConvertSeriesProperties(rowChartData, this);
        }
    }
}