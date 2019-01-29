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
using Core.Components.OxyPlot.CustomSeries;

namespace Core.Components.OxyPlot.DataSeries.Chart
{
    /// <summary>
    /// A <see cref="MultipleLineSeries"/> based on and updated according to the wrapped <see cref="ChartMultipleLineData"/>.
    /// </summary>
    public class ChartMultipleLineDataSeries : MultipleLineSeries, IChartDataSeries
    {
        private readonly ChartMultipleLineData chartMultipleLineData;
        private readonly ChartMultipleLineDataConverter converter = new ChartMultipleLineDataConverter();

        private IEnumerable<IEnumerable<Point2D>> drawnLines;

        /// <summary>
        /// Creates a new instance of <see cref="ChartMultipleLineDataSeries"/>.
        /// </summary>
        /// <param name="chartMultipleLineData">The <see cref="ChartMultipleLineData"/>
        /// which the chart multiple line data series is based upon.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="chartMultipleLineData"/>
        /// is <c>null</c>.</exception>
        public ChartMultipleLineDataSeries(ChartMultipleLineData chartMultipleLineData)
        {
            if (chartMultipleLineData == null)
            {
                throw new ArgumentNullException(nameof(chartMultipleLineData));
            }

            this.chartMultipleLineData = chartMultipleLineData;

            Update();
        }

        public void Update()
        {
            if (!ReferenceEquals(chartMultipleLineData.Lines, drawnLines))
            {
                converter.ConvertSeriesData(chartMultipleLineData, this);

                drawnLines = chartMultipleLineData.Lines;
            }

            converter.ConvertSeriesProperties(chartMultipleLineData, this);
        }
    }
}