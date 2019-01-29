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

using System;
using System.Collections.Generic;
using System.Drawing;

namespace Core.Components.Stack.Data
{
    /// <summary>
    /// This class represents a row in the <see cref="StackChartData"/>.
    /// </summary>
    public class RowChartData
    {
        private readonly double[] values;

        /// <summary>
        /// Creates a new instance of <see cref="RowChartData"/>.
        /// </summary>
        /// <param name="name">The name of the row.</param>
        /// <param name="values">The values of the row.</param>
        /// <param name="color">The color of the row.</param>
        /// <exception cref="ArgumentNullException">Thrown when
        /// <paramref name="name"/> or <paramref name="values"/>
        /// is <c>null</c>.</exception>
        public RowChartData(string name, double[] values, Color? color)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (values == null)
            {
                throw new ArgumentNullException(nameof(values));
            }

            Name = name;
            this.values = values;
            Color = color;
        }

        /// <summary>
        /// Gets the name of the row.
        /// </summary>
        public string Name { get; }

        /// <summary>
        /// Gets the color of the row.
        /// </summary>
        public Color? Color { get; }

        /// <summary>
        /// Gets the values of the row.
        /// </summary>
        public IEnumerable<double> Values
        {
            get
            {
                return values;
            }
        }
    }
}