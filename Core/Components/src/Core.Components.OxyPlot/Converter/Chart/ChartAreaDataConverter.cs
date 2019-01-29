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
    /// The converter that converts <see cref="ChartAreaData"/> data into <see cref="AreaSeries"/> data.
    /// </summary>
    public class ChartAreaDataConverter : ChartDataConverter<ChartAreaData, AreaSeries>
    {
        protected override void SetSeriesData(ChartAreaData data, AreaSeries series)
        {
            series.Points.Clear();
            series.Points2.Clear();

            series.Points.AddRange(data.Points.Select(p => new DataPoint(p.X, p.Y)));

            if (series.Points.Count > 0)
            {
                series.Points2.Add(series.Points[0]);
            }
        }

        protected override void SetSeriesStyle(ChartAreaData data, AreaSeries series)
        {
            ChartAreaStyle style = data.Style;
            series.Fill = ChartDataHelper.Convert(style.FillColor);
            series.Color = ChartDataHelper.Convert(style.StrokeColor);
            series.StrokeThickness = style.StrokeThickness;
        }
    }
}