// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

namespace Riskeer.Common.Data.Contribution
{
    /// <summary>
    /// This class represents the distribution of all failure mechanism contributions.
    /// </summary>
    public class FailureMechanismContribution : Observable
    {
        private static readonly Range<double> floodingProbabilityValidityRange = new Range<double>(1.0 / 1000000, 1.0 / 10);

        private double maximumAllowableFloodingProbability;
        private double signalFloodingProbability;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContribution"/>
        /// </summary>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability.</param>
        /// <param name="signalFloodingProbability">The signal flooding probability.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="maximumAllowableFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalFloodingProbability"/> is larger than <paramref name="maximumAllowableFloodingProbability"/>.</item>
        /// </list>
        /// </exception>
        public FailureMechanismContribution(double maximumAllowableFloodingProbability, double signalFloodingProbability)
        {
            ValidateProbabilities(signalFloodingProbability, maximumAllowableFloodingProbability);

            this.maximumAllowableFloodingProbability = maximumAllowableFloodingProbability;
            this.signalFloodingProbability = signalFloodingProbability;
            NormativeProbabilityType = NormativeProbabilityType.MaximumAllowableFloodingProbability;
        }

        /// <summary>
        /// Gets the signal flooding probability which has been defined on the assessment section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <list type="bullet">
        /// <item>The new value is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The new value is larger than the <see cref="MaximumAllowableFloodingProbability"/>.</item>
        /// </list>
        /// </exception>
        public double SignalFloodingProbability
        {
            get => signalFloodingProbability;
            set
            {
                ValidateProbabilities(value, maximumAllowableFloodingProbability);

                signalFloodingProbability = value;
            }
        }

        /// <summary>
        /// Gets the maximum allowable flooding probability which has been defined on the assessment section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <list type="bullet">
        /// <item>The new value is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The new value is smaller than the <see cref="SignalFloodingProbability"/>.</item>
        /// </list>
        /// </exception>
        public double MaximumAllowableFloodingProbability
        {
            get => maximumAllowableFloodingProbability;
            set
            {
                ValidateProbability(value);

                if (value < signalFloodingProbability)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          value,
                                                          Resources.FailureMechanismContribution_MaximumAllowableFloodingProbability_should_be_same_or_greater_than_SignalFloodingProbability);
                }

                maximumAllowableFloodingProbability = value;
            }
        }

        /// <summary>
        /// Gets the normative probability which has been defined on the assessment section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the new value is not in 
        /// the interval [0.000001, 0.1] or is <see cref="double.NaN"/>.</exception>
        public double NormativeProbability =>
            NormativeProbabilityType == NormativeProbabilityType.MaximumAllowableFloodingProbability
                ? MaximumAllowableFloodingProbability
                : SignalFloodingProbability;

        /// <summary>
        /// Gets or sets the normative probability type which has been defined on the assessment section.
        /// </summary>
        public NormativeProbabilityType NormativeProbabilityType { get; set; }

        /// <summary>
        /// Validates the probability;
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the new value is not in 
        /// the interval [0.000001, 0.1] or is <see cref="double.NaN"/>.</exception>
        private static void ValidateProbability(double value)
        {
            if (!floodingProbabilityValidityRange.InRange(value))
            {
                string message = string.Format(Resources.Norm_should_be_in_Range_0_,
                                               floodingProbabilityValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(nameof(value), value, message);
            }
        }

        /// <summary>
        /// Validates the probabilities.
        /// </summary>
        /// <param name="signalFloodingProbability">The signal flooding probability  to validate.</param>
        /// <param name="maximumAllowableFloodingProbability">The maximum allowable flooding probability to validate against.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="maximumAllowableFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalFloodingProbability"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalFloodingProbability"/> is larger than <paramref name="maximumAllowableFloodingProbability"/>.</item>
        /// </list>
        /// </exception>
        private static void ValidateProbabilities(double signalFloodingProbability,
                                                  double maximumAllowableFloodingProbability)
        {
            ValidateProbability(signalFloodingProbability);
            ValidateProbability(maximumAllowableFloodingProbability);

            if (signalFloodingProbability > maximumAllowableFloodingProbability)
            {
                throw new ArgumentOutOfRangeException(nameof(signalFloodingProbability),
                                                      signalFloodingProbability,
                                                      Resources.FailureMechanismContribution_SignalFloodingProbability_should_be_same_or_smaller_than_MaximumAllowableFloodingProbability);
            }
        }
    }
}