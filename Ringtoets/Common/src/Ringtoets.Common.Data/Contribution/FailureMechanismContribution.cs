// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Contribution
{
    /// <summary>
    /// This class represents the distribution of all failure mechanism contributions.
    /// </summary>
    public class FailureMechanismContribution : Observable
    {
        private static readonly Range<double> normValidityRange = new Range<double>(1.0 / 1000000, 1.0 / 10);

        private double lowerLimitNorm;
        private double signalingNorm;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContribution"/>
        /// </summary>
        /// <param name="lowerLimitNorm">The lower limit norm.</param>
        /// <param name="signalingNorm">The signaling norm.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNorm"/> is larger than <paramref name="lowerLimitNorm"/>.</item>
        /// </list>
        /// </exception>
        public FailureMechanismContribution(double lowerLimitNorm, double signalingNorm)
        {
            ValidateNorms(signalingNorm, lowerLimitNorm);

            this.lowerLimitNorm = lowerLimitNorm;
            this.signalingNorm = signalingNorm;
            NormativeNorm = NormType.LowerLimit;
        }

        /// <summary>
        /// Gets the signaling norm which has been defined on the assessment section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <list type="bullet">
        /// <item>The new value is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The new value is larger than the <see cref="LowerLimitNorm"/>.</item>
        /// </list></exception>
        public double SignalingNorm
        {
            get
            {
                return signalingNorm;
            }
            set
            {
                ValidateNorms(value, lowerLimitNorm);

                signalingNorm = value;
            }
        }

        /// <summary>
        /// Gets the lower limit norm which has been defined on the assessment section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when
        /// <list type="bullet">
        /// <item>The new value is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The new value is smaller than the <see cref="SignalingNorm"/>.</item>
        /// </list></exception>
        public double LowerLimitNorm
        {
            get
            {
                return lowerLimitNorm;
            }
            set
            {
                ValidateNorm(value);

                if (value < signalingNorm)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          value,
                                                          Resources.FailureMechanismContribution_LowerLimitNorm_should_be_same_or_greater_than_SignalingNorm);
                }

                lowerLimitNorm = value;
            }
        }

        /// <summary>
        /// Gets the norm which has been defined on the assessment section.
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the new value is not in 
        /// the interval [0.000001, 0.1] or is <see cref="double.NaN"/>.</exception>
        public double Norm
        {
            get
            {
                return NormativeNorm == NormType.LowerLimit
                           ? LowerLimitNorm
                           : SignalingNorm;
            }
        }

        /// <summary>
        /// Gets or sets the norm type which has been defined on the assessment section.
        /// </summary>
        public NormType NormativeNorm { get; set; }

        /// <summary>
        /// Validates the norm value;
        /// </summary>
        /// <param name="value">The value to validate.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when the new value is not in 
        /// the interval [0.000001, 0.1] or is <see cref="double.NaN"/>.</exception>
        private static void ValidateNorm(double value)
        {
            if (!normValidityRange.InRange(value))
            {
                string message = string.Format(Resources.Norm_should_be_in_Range_0_,
                                               normValidityRange.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(nameof(value), value, message);
            }
        }

        /// <summary>
        /// Validates the norm values.
        /// </summary>
        /// <param name="signalingNormValue">The signaling norm to validate.</param>
        /// <param name="lowerLimitNormValue">The lower limit norm to validate against.</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNormValue"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNormValue"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNormValue"/> is larger than <paramref name="lowerLimitNormValue"/>.</item>
        /// </list>
        /// </exception>
        private static void ValidateNorms(double signalingNormValue,
                                          double lowerLimitNormValue)
        {
            ValidateNorm(signalingNormValue);
            ValidateNorm(lowerLimitNormValue);

            if (signalingNormValue > lowerLimitNormValue)
            {
                throw new ArgumentOutOfRangeException(nameof(signalingNormValue),
                                                      signalingNormValue,
                                                      Resources.FailureMechanismContribution_SignalingNorm_should_be_same_or_smaller_than_LowerLimitNorm);
            }
        }
    }
}