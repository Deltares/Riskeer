﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Components.Charting.Styles;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// This class represents data in 2D space which forms a closed area.
    /// </summary>
    public class ChartAreaData : PointBasedChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ChartAreaData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartAreaData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        public ChartAreaData(string name) : base(name) {}

        /// <summary>
        /// Gets or sets the style of the chart area.
        /// </summary>
        public ChartAreaStyle Style { get; set; }
    }
}