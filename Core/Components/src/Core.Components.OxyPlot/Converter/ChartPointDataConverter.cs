﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Linq;
using Core.Components.Charting.Data;
using OxyPlot;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// The converter that converts <see cref="ChartPointData"/> data into <see cref="LineSeries"/> data.
    /// </summary>
    public class ChartPointDataConverter : ChartDataConverter<ChartPointData, LineSeries>
    {
        protected override void SetSeriesData(ChartPointData data, LineSeries series)
        {
            series.ItemsSource = data.Points.Select(p => new DataPoint(p.X, p.Y));
        }

        protected override void SetSeriesStyle(ChartPointData data, LineSeries series)
        {
            series.LineStyle = LineStyle.None;

            if (data.Style != null)
            {
                series.MarkerFill = ChartDataHelper.Convert(data.Style.Color);
                series.MarkerSize = data.Style.Size;
                series.MarkerType = ChartDataHelper.Convert(data.Style.Symbol);
                series.MarkerStroke = ChartDataHelper.Convert(data.Style.StrokeColor);
                series.MarkerStrokeThickness = data.Style.StrokeThickness;
            }
        }
    }
}