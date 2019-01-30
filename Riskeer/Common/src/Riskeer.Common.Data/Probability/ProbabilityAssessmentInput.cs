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

using System;
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Riskeer.Common.Data.Properties;

namespace Riskeer.Common.Data.Probability
{
    /// <summary>
    /// Base class for an object that represents a probability assessment input.
    /// </summary>
    public abstract class ProbabilityAssessmentInput
    {
        private static readonly Range<double> validityRangeA = new Range<double>(0, 1);
        private double a;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilityAssessmentInput"/>.
        /// </summary>
        /// <param name="a">The default value for the parameter 'a' to be used to factor in the 'length effect'
        /// when determining the maximum tolerated probability of failure.</param>
        /// <param name="b">The default value for the parameter 'b' to be used to factor in the 'length effect'
        /// when determining the maximum tolerated probability of failure.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="a"/> is not in the range [0, 1].</exception>
        protected ProbabilityAssessmentInput(double a, double b)
        {
            A = a;
            B = b;
        }

        /// <summary>
        /// Gets or sets 'a' parameter used to factor in the 'length effect' when determining the
        /// maximum tolerated probability of failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when value is not in the range [0, 1].</exception>
        public double A
        {
            get
            {
                return a;
            }
            set
            {
                if (!validityRangeA.InRange(value))
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          string.Format(Resources.ProbabilityAssessmentInput_A_Value_must_be_in_Range_0_,
                                                                        validityRangeA.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture)));
                }

                a = value;
            }
        }

        /// <summary>
        /// Gets the 'b' parameter used to factor in the 'length effect' when determining
        /// the maximum tolerated probability of failure.
        /// </summary>
        public double B { get; }
    }
}