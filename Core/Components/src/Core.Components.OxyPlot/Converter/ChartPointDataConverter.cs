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

using System.Collections.Generic;
using System.Linq;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="ChartPointData"/> into <see cref="LineSeries"/> with point styling.
    /// </summary>
    public class ChartPointDataConverter : ChartDataConverter<ChartPointData>
    {
        protected override IList<Series> Convert(ChartPointData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                IsVisible = data.IsVisible,
                Mapping = TupleToDataPoint,
                LineStyle = LineStyle.None,
                MarkerType = MarkerType.Circle,
                Tag = data
            };

            CreateStyle(series, data.Style);

            return new List<Series> { series };
        }

        private void CreateStyle(LineSeries series, ChartPointStyle style)
        {
            if (style != null)
            {
                series.MarkerFill = ChartDataHelper.Convert(style.Color);
                series.MarkerSize = style.Size;
                series.MarkerType = ChartDataHelper.Convert(style.Symbol);
                series.MarkerStroke = ChartDataHelper.Convert(style.StrokeColor);
                series.MarkerStrokeThickness = style.StrokeThickness;
            }
        }
    }
}