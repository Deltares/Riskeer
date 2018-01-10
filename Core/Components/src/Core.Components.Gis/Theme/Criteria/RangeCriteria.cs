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
    /// Criteria which can be used to specifiy ranges. 
    /// </summary>
    public class RangeCriteria : ICriteria
    {
        /// <summary>
        /// Creates a new instance of <see cref="RangeCriteria"/>.
        /// </summary>
        /// <param name="rangeCriteriaOperator"></param>
        /// <param name="lowerBound">The lower bound of the range.</param>
        /// <param name="upperBound">The upper bound of the range.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="rangeCriteriaOperator"/>
        /// contains an invalid value for <see cref="ValueCriteriaOperator"/>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="lowerBound"/> is
        /// larger or equal to the <paramref name="upperBound"/>.</exception>
        public RangeCriteria(RangeCriteriaOperator rangeCriteriaOperator,
                             double lowerBound,
                             double upperBound)
        {
            if (!Enum.IsDefined(typeof(RangeCriteriaOperator), rangeCriteriaOperator))
            {
                throw new InvalidEnumArgumentException(nameof(rangeCriteriaOperator),
                                                       (int) rangeCriteriaOperator,
                                                       typeof(RangeCriteriaOperator));
            }

            if (lowerBound >= upperBound)
            {
                throw new ArgumentException(@"Lower bound cannot be larger or equal than upper bound.", nameof(lowerBound));
            }

            RangeCriteriaOperator = rangeCriteriaOperator;
            LowerBound = lowerBound;
            UpperBound = upperBound;
        }

        /// <summary>
        /// Gets the operator for the <see cref="RangeCriteria"/>.
        /// </summary>
        public RangeCriteriaOperator RangeCriteriaOperator { get; }

        /// <summary>
        /// Gets the lower bound of the range.
        /// </summary>
        public double LowerBound { get; }

        /// <summary>
        /// Gets the upper bound of the range.
        /// </summary>
        public double UpperBound { get; }
    }
}