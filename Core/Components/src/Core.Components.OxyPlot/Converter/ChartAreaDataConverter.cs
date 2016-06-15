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
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// This class converts <see cref="ChartAreaData"/> into <see cref="AreaSeries"/>.
    /// </summary>
    public class ChartAreaDataConverter : ChartDataConverter<ChartAreaData>
    {
        protected override IList<Series> Convert(ChartAreaData data)
        {
            var series = new AreaSeries
            {
                IsVisible = data.IsVisible,
                Tag = data
            };
            foreach (var p in data.Points)
            {
                series.Points.Add(TupleToDataPoint(p));
            }
            if (series.Points.Count > 0)
            {
                series.Points2.Add(series.Points[0]);
            }
            return new List<Series> { series };
        }
    }
}