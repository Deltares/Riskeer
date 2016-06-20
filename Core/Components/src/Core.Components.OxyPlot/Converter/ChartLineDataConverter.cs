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
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="ChartLineData"/> into <see cref="LineSeries"/>.
    /// </summary>
    public class ChartLineDataConverter : ChartDataConverter<ChartLineData>
    {
        protected override IList<Series> Convert(ChartLineData data)
        {
            var series = new LineSeries
            {
                ItemsSource = data.Points.ToArray(),
                Mapping = Point2DToDataPoint,
                IsVisible = data.IsVisible,
                Tag = data
            };

            CreateStyle(series, data.Style);

            return new List<Series>{ series };
        }

        private static void CreateStyle(LineSeries series, ChartLineStyle style)
        {
            if (style != null)
            {
                series.Color = ChartDataHelper.Convert(style.Color);
                series.StrokeThickness = style.Width;
                series.LineStyle = ChartDataHelper.Convert(style.Style);
            }
        }
    }
}