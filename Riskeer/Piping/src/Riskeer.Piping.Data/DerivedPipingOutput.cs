// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Probability;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// This class contains the results of a semi-probabilistic assessment of the piping
    /// failure mechanism.
    /// </summary>
    public class DerivedPipingOutput
    {
        private double requiredProbability;
        private double pipingProbability;
        private double upliftProbability;
        private double heaveProbability;
        private double sellmeijerProbability;

        /// <summary>
        /// Creates a new instance of <see cref="DerivedPipingOutput"/>.
        /// </summary>
        /// <param name="upliftFactorOfSafety">The factor of safety for the uplift sub mechanism.</param>
        /// <param name="upliftReliability">The reliability of uplift the sub mechanism.</param>
        /// <param name="upliftProbability">The probability of failure due to the uplift sub mechanism.</param>
        /// <param name="heaveFactorOfSafety">The factor of safety for the heave sub mechanism.</param>
        /// <param name="heaveReliability">The reliability of the heave sub mechanism.</param>
        /// <param name="heaveProbability">The probability of failure due to the heave sub mechanism.</param>
        /// <param name="sellmeijerFactorOfSafety">The factor of safety for the Sellmeijer sub mechanism.</param>
        /// <param name="sellmeijerReliability">The reliability of the Sellmeijer sub mechanism.</param>
        /// <param name="sellmeijerProbability">The probability of failure due to the Sellmeijer sub mechanism.</param>
        /// <param name="requiredProbability">The required (maximum allowed) probability of failure due to piping.</param>
        /// <param name="requiredReliability">The required (maximum allowed) reliability of the piping failure mechanism</param>
        /// <param name="pipingProbability">The calculated probability of failing due to piping.</param>
        /// <param name="pipingReliability">The calculated reliability of the piping failure mechanism.</param>
        /// <param name="pipingFactorOfSafety">The factor of safety for the piping failure mechanism.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a probability that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public DerivedPipingOutput(double upliftFactorOfSafety, double upliftReliability, double upliftProbability,
                                   double heaveFactorOfSafety, double heaveReliability, double heaveProbability,
                                   double sellmeijerFactorOfSafety, double sellmeijerReliability, double sellmeijerProbability,
                                   double requiredProbability, double requiredReliability,
                                   double pipingProbability, double pipingReliability, double pipingFactorOfSafety)
        {
            UpliftFactorOfSafety = new RoundedDouble(3, upliftFactorOfSafety);
            UpliftReliability = new RoundedDouble(5, upliftReliability);
            UpliftProbability = upliftProbability;
            HeaveFactorOfSafety = new RoundedDouble(3, heaveFactorOfSafety);
            HeaveReliability = new RoundedDouble(5, heaveReliability);
            HeaveProbability = heaveProbability;
            SellmeijerFactorOfSafety = new RoundedDouble(3, sellmeijerFactorOfSafety);
            SellmeijerReliability = new RoundedDouble(5, sellmeijerReliability);
            SellmeijerProbability = sellmeijerProbability;

            RequiredProbability = requiredProbability;
            RequiredReliability = new RoundedDouble(5, requiredReliability);
            PipingProbability = pipingProbability;
            PipingReliability = new RoundedDouble(5, pipingReliability);
            PipingFactorOfSafety = new RoundedDouble(3, pipingFactorOfSafety);
        }

        /// <summary>
        /// Gets the required probability of the piping failure mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double RequiredProbability
        {
            get
            {
                return requiredProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                requiredProbability = value;
            }
        }

        /// <summary>
        /// Get the required reliability of the piping failure mechanism.
        /// </summary>
        public RoundedDouble RequiredReliability { get; }

        /// <summary>
        /// Gets the factor of safety of the piping failure mechanism.
        /// </summary>
        public RoundedDouble PipingFactorOfSafety { get; }

        /// <summary>
        /// Gets the reliability of the piping failure mechanism.
        /// </summary>
        public RoundedDouble PipingReliability { get; }

        /// <summary>
        /// Gets the probability of failing due to the piping failure mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double PipingProbability
        {
            get
            {
                return pipingProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                pipingProbability = value;
            }
        }

        /// <summary>
        /// Gets the factor of safety for the uplift sub mechanism.
        /// </summary>
        public RoundedDouble UpliftFactorOfSafety { get; }

        /// <summary>
        /// Gets the reliability for the uplift sub mechanism.
        /// </summary>
        public RoundedDouble UpliftReliability { get; }

        /// <summary>
        /// Gets the probability of failing due to the uplift failure sub mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double UpliftProbability
        {
            get
            {
                return upliftProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                upliftProbability = value;
            }
        }

        /// <summary>
        /// Gets the factor of safety for the heave sub mechanism.
        /// </summary>
        public RoundedDouble HeaveFactorOfSafety { get; }

        /// <summary>
        /// Gets the reliability for the heave sub mechanism.
        /// </summary>
        public RoundedDouble HeaveReliability { get; }

        /// <summary>
        /// Gets the probability of failing due to the heave failure sub mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double HeaveProbability
        {
            get
            {
                return heaveProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                heaveProbability = value;
            }
        }

        /// <summary>
        /// Gets the factor of safety for the Sellmeijer sub mechanism.
        /// </summary>
        public RoundedDouble SellmeijerFactorOfSafety { get; }

        /// <summary>
        /// Gets the reliability for the Sellmeijer sub mechanism.
        /// </summary>
        public RoundedDouble SellmeijerReliability { get; }

        /// <summary>
        /// Gets the probability of failing due to the Sellmeijer failure sub mechanism,
        /// which value in range [0,1].
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when setting a value that falls
        /// outside the [0.0, 1.0] range or isn't <see cref="double.NaN"/>.</exception>
        public double SellmeijerProbability
        {
            get
            {
                return sellmeijerProbability;
            }
            private set
            {
                ProbabilityHelper.ValidateProbability(value, nameof(value), true);
                sellmeijerProbability = value;
            }
        }
    }
}