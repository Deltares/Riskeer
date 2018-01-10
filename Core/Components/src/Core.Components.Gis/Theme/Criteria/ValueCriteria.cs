// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Core.Components.Gis.Theme.Criteria
{
    /// <summary>
    /// Criteria to be used for equal or unequal values.
    /// </summary>
    public class ValueCriteria : IMapCriteria
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValueCriteria"/>.
        /// </summary>
        /// <param name="valueOperator">The <see cref="ValueCriteriaOperator"/> belonging to this criteria.</param>
        /// <param name="value">The value to apply when using this criteria.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="valueOperator"/>
        /// contains an invalid value for <see cref="ValueCriteriaOperator"/>.</exception>
        public ValueCriteria(ValueCriteriaOperator valueOperator, double value)
        {
            if (!Enum.IsDefined(typeof(ValueCriteriaOperator), valueOperator))
            {
                throw new InvalidEnumArgumentException(nameof(valueOperator),
                                                       (int) valueOperator,
                                                       typeof(ValueCriteriaOperator));
            }

            ValueOperator = valueOperator;
            Value = value;
        }

        /// <summary>
        /// Gets the operator for the criteria.
        /// </summary>
        public ValueCriteriaOperator ValueOperator { get; }

        /// <summary>
        /// Gets the value that belongs to the criteria.
        /// </summary>
        public double Value { get; }
    }
}