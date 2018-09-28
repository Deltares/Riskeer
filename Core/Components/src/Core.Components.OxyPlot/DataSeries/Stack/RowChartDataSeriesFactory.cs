// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using Core.Components.Stack.Data;

namespace Core.Components.OxyPlot.DataSeries.Stack
{
    /// <summary>
    /// A factory to create <see cref="RowChartDataSeries"/> based on <see cref="StackChartData"/>.
    /// </summary>
    internal static class RowChartDataSeriesFactory
    {
        /// <summary>
        /// Creates a <see cref="RowChartDataSeries"/> based on <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="StackChartData"/> to create <see cref="RowChartDataSeries"/> from.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="RowChartDataSeries"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public static IEnumerable<RowChartDataSeries> Create(StackChartData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            foreach (RowChartData row in data.Rows)
            {
                yield return new RowChartDataSeries(row);
            }
        }
    }
}