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
using System.ComponentModel;

namespace Core.Components.Gis.Theme
{
    /// <summary>
    /// Criterion to be used for equal or unequal values.
    /// </summary>
    public class ValueCriterion
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValueCriterion"/>.
        /// </summary>
        /// <param name="valueOperator">The <see cref="ValueCriterionOperator"/> belonging to this criterion.</param>
        /// <param name="value">The value to apply when using this criteria.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="valueOperator"/>
        /// contains an invalid value for <see cref="ValueCriterionOperator"/>.</exception>
        public ValueCriterion(ValueCriterionOperator valueOperator, string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (!Enum.IsDefined(typeof(ValueCriterionOperator), valueOperator))
            {
                throw new InvalidEnumArgumentException(nameof(valueOperator),
                                                       (int) valueOperator,
                                                       typeof(ValueCriterionOperator));
            }

            ValueOperator = valueOperator;
            Value = value;
        }

        /// <summary>
        /// Gets the operator for the criteria.
        /// </summary>
        public ValueCriterionOperator ValueOperator { get; }

        /// <summary>
        /// Gets the value that belongs to the criteria.
        /// </summary>
        public string Value { get; }
    }
}