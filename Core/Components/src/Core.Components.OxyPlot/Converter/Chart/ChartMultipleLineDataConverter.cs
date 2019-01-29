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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using Core.Components.OxyPlot.CustomSeries;
using OxyPlot;

namespace Core.Components.OxyPlot.Converter.Chart
{
    /// <summary>
    /// The converter that converts <see cref="ChartMultipleLineData"/> data into <see cref="MultipleLineSeries"/> data.
    /// </summary>
    public class ChartMultipleLineDataConverter : ChartDataConverter<ChartMultipleLineData, MultipleLineSeries>
    {
        protected override void SetSeriesData(ChartMultipleLineData data, MultipleLineSeries series)
        {
            series.Lines.Clear();

            foreach (IEnumerable<Point2D> line in data.Lines)
            {
                series.Lines.Add(line.Select(p => new DataPoint(p.X, p.Y)).ToArray());
            }
        }

        protected override void SetSeriesStyle(ChartMultipleLineData data, MultipleLineSeries series)
        {
            ChartLineStyle lineStyle = data.Style;

            series.Color = ChartDataHelper.Convert(lineStyle.Color);
            series.StrokeThickness = lineStyle.Width;
            series.LineStyle = ChartDataHelper.Convert(lineStyle.DashStyle);
        }
    }
}