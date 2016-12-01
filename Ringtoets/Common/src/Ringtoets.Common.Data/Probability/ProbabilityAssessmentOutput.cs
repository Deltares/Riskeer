// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Calculation;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.Common.Data.Probability
{
    /// <summary>
    /// This class contains the results of a probabilistic assessment calculation.
    /// </summary>
    public class ProbabilityAssessmentOutput : Observable, ICalculationOutput
    {
        private double requiredProbability;
        private double probability;

        /// <summary>
        /// Creates a new instance of <see cref="ProbabilityAssessmentOutput"/>.
        /// </summary>
        /// <param name="requiredProbability">The required (maximum allowed) probability of failure.</param>
        /// <param name="requiredReliability">The required (maximum allowed) reliability of the failure mechanism.</param>
        /// <param name="probability">The probability of failure.</param>
        /// <param name="reliability">The reliability of the failure mechanism.</param>
        /// <param name="factorOfSafety">The factor of safety of the failure mechanism.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a probability that falls
        /// outside the [0.0, 1.0] range and isn't <see cref="double.NaN"/>.</exception>
        public ProbabilityAssessmentOutput(double requiredProbability, double requiredReliability, double probability, double reliability, double factorOfSafety)
        {
            RequiredProbability = requiredProbability;
            RequiredReliability = new RoundedDouble(5, requiredReliability);
            Probability = probability;
            Reliability = new RoundedDouble(5, reliability);
            FactorOfSafety = new RoundedDouble(3, factorOfSafety);
        }

        /// <summary>
        /// Gets the required (maximum allowed) probability of failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range and isn't <see cref="double.NaN"/>.</exception>
        public double RequiredProbability
        {
            get
            {
                return requiredProbability;
            }
            private set
            {
                if (double.IsNaN(value) || (0.0 <= value && value <= 1.0))
                {
                    requiredProbability = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          RingtoetsCommonDataResources.Probability_Must_be_in_range_zero_to_one);
                }
            }
        }

        /// <summary>
        /// Get the required (maximum allowed) reliability of the failure mechanism.
        /// </summary>
        public RoundedDouble RequiredReliability { get; private set; }

        /// <summary>
        /// Gets the probability of failure.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double Probability
        {
            get
            {
                return probability;
            }
            private set
            {
                if (double.IsNaN(value) || (0.0 <= value && value <= 1.0))
                {
                    probability = value;
                }
                else
                {
                    throw new ArgumentOutOfRangeException("value",
                                                          RingtoetsCommonDataResources.Probability_Must_be_in_range_zero_to_one);
                }
            }
        }

        /// <summary>
        /// Gets the reliability of the failure mechanism.
        /// </summary>
        public RoundedDouble Reliability { get; private set; }

        /// <summary>
        /// Gets the factor of safety of the failure mechanism.
        /// </summary>
        public RoundedDouble FactorOfSafety { get; private set; }
    }
}