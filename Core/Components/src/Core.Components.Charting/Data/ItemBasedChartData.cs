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

using System;

namespace Core.Components.Charting.Data
{
    /// <summary>
    /// Base class for <see cref="ChartData"/> that contains items like points or areas.
    /// </summary>
    public abstract class ItemBasedChartData : ChartData
    {
        /// <summary>
        /// Creates a new instance of <see cref="ItemBasedChartData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ItemBasedChartData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is 
        /// <c>null</c> or only whitespace.</exception>
        protected ItemBasedChartData(string name) : base(name) {}
    }
}