// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Common.Data.Probability
{
    /// <summary>
    /// This class contains the results of a probabilistic assessment calculation.
    /// </summary>
    public class ProbabilityAssessmentOutput
    {
        private double probability;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilityAssessmentOutput"/>.
        /// </summary>
        /// <param name="probability">The probability of failure.</param>
        /// <param name="reliability">The reliability of the failure mechanism.</param>
        public ProbabilityAssessmentOutput(double probability, double reliability)
        {
            Probability = probability;
            Reliability = new RoundedDouble(5, reliability);
        }

        /// <summary>
        /// Gets the probability of failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range and isn't <see cref="double.NaN"/>.</exception>
        public double Probability
        {
            get
            {
                return probability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                probability = value;
            }
        }

        /// <summary>
        /// Gets the reliability of the failure mechanism.
        /// </summary>
        public RoundedDouble Reliability { get; }
    }
}