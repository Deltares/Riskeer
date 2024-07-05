﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System;
using Core.Common.Base.Data;

namespace Riskeer.Common.Data.IllustrationPoints
{
    /// <summary>
    /// An output variable for an illustration point.
    /// </summary>
    public class IllustrationPointResult : ICloneable
    {
        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointResult"/>.
        /// </summary>
        /// <param name="description">The description.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="value">The value.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="description"/>
        /// or <paramref name="unit"/> is <c>null</c>.</exception>
        public IllustrationPointResult(string description, string unit, double value)
        {
            if (description == null)
            {
                throw new ArgumentNullException(nameof(description));
            }

            if (unit == null)
            {
                throw new ArgumentNullException(nameof(unit));
            }

            Description = description;
            Unit = unit;
            Value = new RoundedDouble(5, value);
        }

        /// <summary>
        /// Gets the description of the illustration point result.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// Gets the unit of the illustration point result.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets the value of the illustration point result.
        /// </summary>
        public RoundedDouble Value { get; }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}