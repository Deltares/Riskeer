// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using System.Collections.Generic;

namespace Riskeer.Common.Service.ValidationRules
{
    /// <summary>
    /// Base implementation of a validation rule.
    /// </summary>
    public abstract class ValidationRule
    {
        /// <summary>
        /// Validates the subject.
        /// </summary>
        /// <returns>A <see cref="IEnumerable{T}"/> containing validation messages. 
        /// Empty if no validation errors are found.</returns>
        public abstract IEnumerable<string> Validate();

        /// <summary>
        /// Checks if a value is <c>NaN</c> or <c>Infinity</c>.
        /// </summary>
        /// <param name="value">The value which needs to be checked.</param>
        /// <returns><c>true</c>if <paramref name="value"/>is not a concrete number, <c>false</c> if otherwise.</returns>
        protected static bool IsNotConcreteNumber(double value)
        {
            return double.IsNaN(value) || double.IsInfinity(value);
        }
    }
}