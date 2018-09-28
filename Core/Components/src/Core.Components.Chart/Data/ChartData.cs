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
using Core.Common.Base;

namespace Core.Components.Chart.Data
{
    /// <summary>
    /// Abstract class for data with the purpose of becoming visible in charting components.
    /// </summary>
    public abstract class ChartData : Observable
    {
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="ChartData"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="ChartData"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is <c>null</c> or only whitespace.</exception>
        protected ChartData(string name)
        {
            Name = name;
            IsVisible = true;
        }

        /// <summary>
        /// Gets or sets the name of the <see cref="ChartData"/>.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when <paramref name="value"/> is <c>null</c> or only whitespace.</exception>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("A name must be set to the chart data.");
                }

                name = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="ChartData"/> is visible.
        /// </summary>
        public bool IsVisible { get; set; }

        /// <summary>
        /// Gets a value indicating whether the <see cref="ChartData"/> has data.
        /// </summary>
        public abstract bool HasData { get; }
    }
}