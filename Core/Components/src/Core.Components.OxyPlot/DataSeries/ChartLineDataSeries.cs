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
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Converter;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.DataSeries
{
    /// <summary>
    /// A <see cref="LineSeries"/> based on and updated according to the wrapped <see cref="ChartLineData"/>.
    /// </summary>
    public class ChartLineDataSeries : LineSeries, IChartDataSeries
    {
        private readonly ChartLineData chartLineData;
        private readonly ChartLineDataConverter converter = new ChartLineDataConverter();

        private Point2D[] drawnPoints;

        /// <summary>
        /// Creates a new instance of <see cref="ChartLineDataSeries"/>.
        /// </summary>
        /// <param name="chartLineData">The <see cref="ChartLineData"/> which the chart line data series is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="chartLineData"/> is <c>null</c>.</exception>
        public ChartLineDataSeries(ChartLineData chartLineData)
        {
            if (chartLineData == null)
            {
                throw new ArgumentNullException(nameof(chartLineData));
            }

            this.chartLineData = chartLineData;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(chartLineData.Points, drawnPoints))
            {
                converter.ConvertSeriesItems(chartLineData, this);

                drawnPoints = chartLineData.Points;
            }

            converter.ConvertSeriesProperties(chartLineData, this);
        }
    }
}