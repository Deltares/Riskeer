// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Factory for creating <see cref="ChartData"/> based on information used as input in views.
    /// </summary>
    public static class ChartDataFactory
    {
        /// <summary>
        /// Create a <see cref="ChartLineData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartLineData"/>.</param>
        /// <returns>An empty <see cref="ChartLineData"/> object.</returns>
        public static ChartLineData CreateEmptyLineData(string name)
        {
            return new ChartLineData(Enumerable.Empty<Point2D>(), name);
        }

        /// <summary>
        /// Create a <see cref="ChartPointData"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartPointData"/>.</param>
        /// <returns>An empty <see cref="ChartPointData"/> object.</returns>
        public static ChartPointData CreateEmptyPointData(string name)
        {
            return new ChartPointData(Enumerable.Empty<Point2D>(), name);
        }

        /// <summary>
        /// Create a <see cref="ChartDataCollection"/> instance with a name, but without data.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartDataCollection"/>.</param>
        /// <returns>An empty <see cref="ChartDataCollection"/> object.</returns>
        public static ChartDataCollection CreateEmptyChartDataCollection(string name)
        {
            return new ChartDataCollection(new List<ChartData>(), name);
        }
    }
}