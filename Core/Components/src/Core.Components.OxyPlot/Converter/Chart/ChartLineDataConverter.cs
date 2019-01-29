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

using System.Linq;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter.Chart
{
    /// <summary>
    /// The converter that converts <see cref="ChartLineData"/> data into <see cref="LineSeries"/> data.
    /// </summary>
    public class ChartLineDataConverter : ChartDataConverter<ChartLineData, LineSeries>
    {
        protected override void SetSeriesData(ChartLineData data, LineSeries series)
        {
            series.ItemsSource = data.Points.Select(p => new DataPoint(p.X, p.Y)).ToArray();
        }

        protected override void SetSeriesStyle(ChartLineData data, LineSeries series)
        {
            ChartLineStyle style = data.Style;
            series.Color = ChartDataHelper.Convert(style.Color);
            series.StrokeThickness = style.Width;
            series.LineStyle = ChartDataHelper.Convert(style.DashStyle);
        }
    }
}