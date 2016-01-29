// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using System.Collections.Generic;
using Core.Components.Charting.Data;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Converter
{
    /// <summary>
    /// The interface for a converter which converts <see cref="ChartData"/> into <see cref="Series"/>.
    /// </summary>
    public interface IChartDataConverter {

        /// <summary>
        /// Checks whether the <see cref="IChartDataConverter"/> can convert the <paramref name="data"/>.
        /// </summary>
        /// <param name="data">The <see cref="ChartData"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref name="data"/> can be converted by the
        /// <see cref="IChartDataConverter"/>, <c>false</c> otherwise.</returns>
        bool CanConvertSeries(ChartData data);

        /// <summary>
        /// Creates one or more <see cref="Series"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into a <see cref="Series"/>.</param>
        /// <returns>A new <see cref="IList{T}"/> of <see cref="Series"/>.</returns>
        /// <exception cref="ArgumentException">Thrown when <see cref="CanConvertSeries"/>
        /// returns <c>false</c>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/>
        /// is <c>null</c>.</exception>
        IList<Series> Convert(ChartData data);
    }
}