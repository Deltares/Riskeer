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
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.OxyPlot.Converter.Chart;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.DataSeries.Chart
{
    /// <summary>
    /// A <see cref="LineSeries"/> based on and updated according to the wrapped <see cref="ChartPointData"/>.
    /// </summary>
    public class ChartPointDataSeries : LineSeries, IChartDataSeries
    {
        private readonly ChartPointData chartPointData;
        private readonly ChartPointDataConverter converter = new ChartPointDataConverter();

        private IEnumerable<Point2D> drawnPoints;

        /// <summary>
        /// Creates a new instance of <see cref="ChartPointDataSeries"/>.
        /// </summary>
        /// <param name="chartPointData">The <see cref="ChartPointData"/> which the chart point data series is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="chartPointData"/> is <c>null</c>.</exception>
        public ChartPointDataSeries(ChartPointData chartPointData)
        {
            if (chartPointData == null)
            {
                throw new ArgumentNullException(nameof(chartPointData));
            }

            this.chartPointData = chartPointData;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(chartPointData.Points, drawnPoints))
            {
                converter.ConvertSeriesData(chartPointData, this);

                drawnPoints = chartPointData.Points;
            }

            converter.ConvertSeriesProperties(chartPointData, this);
        }
    }
}