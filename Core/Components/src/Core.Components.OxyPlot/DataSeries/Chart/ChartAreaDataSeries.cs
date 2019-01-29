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
    /// A <see cref="AreaSeries"/> based on and updated according to the wrapped <see cref="ChartAreaData"/>.
    /// </summary>
    public class ChartAreaDataSeries : AreaSeries, IChartDataSeries
    {
        private readonly ChartAreaData chartAreaData;
        private readonly ChartAreaDataConverter converter = new ChartAreaDataConverter();

        private IEnumerable<Point2D> drawnPoints;

        /// <summary>
        /// Creates a new instance of <see cref="ChartAreaDataSeries"/>.
        /// </summary>
        /// <param name="chartAreaData">The <see cref="ChartAreaData"/> which the chart area data series is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="chartAreaData"/> is <c>null</c>.</exception>
        public ChartAreaDataSeries(ChartAreaData chartAreaData)
        {
            if (chartAreaData == null)
            {
                throw new ArgumentNullException(nameof(chartAreaData));
            }

            this.chartAreaData = chartAreaData;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(chartAreaData.Points, drawnPoints))
            {
                converter.ConvertSeriesData(chartAreaData, this);

                drawnPoints = chartAreaData.Points;
            }

            converter.ConvertSeriesProperties(chartAreaData, this);
        }
    }
}