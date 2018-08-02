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
using System.Collections.Generic;
using System.Globalization;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Contribution
{
    /// <summary>
    /// This class represents the distribution of all failure mechanism contributions.
    /// </summary>
    public class FailureMechanismContribution : Observable
    {
        private static readonly Range<double> normValidityRange = new Range<double>(1.0 / 1000000, 1.0 / 10);

        private readonly List<FailureMechanismContributionItem> distribution = new List<FailureMechanismContributionItem>();
        private double lowerLimitNorm;
        private double signalingNorm;
        private NormType normativeNorm;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContribution"/>. Values are taken from the 
        /// <paramref name="failureMechanisms"/> and one item is added with a value of <paramref name="otherContribution"/>
        /// which represents the contribution of any other failure mechanisms.
        /// </summary>
        /// <param name="failureMechanisms">The <see cref="IEnumerable{T}"/> of <see cref="IFailureMechanism"/> 
        /// on which to base the <see cref="FailureMechanismContribution"/>.</param>
        /// <param name="otherContribution">The collective contribution for other failure mechanisms.</param>
        /// <param name="lowerLimitNorm">The lower limit norm.</param>
        /// <param name="signalingNorm">The signaling norm.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanisms"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>any of the <paramref name="failureMechanisms"/> has a value for <see cref="IFailureMechanism.Contribution"/> 
        /// not in the interval [0, 100].</item>
        /// <item>the value of <paramref name="otherContribution"/> is not in the interval [0, 100]</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="lowerLimitNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item><paramref name="signalingNorm"/> is not in the interval [0.000001, 0.1] or is <see cref="double.NaN"/>;</item>
        /// <item>The <paramref name="signalingNorm"/> is larger than <paramref name="lowerLimitNorm"/>.</item>
        /// </list>
        /// </exception>
        public FailureMechanismContribution(IEnumerable<IFailureMechanism> failureMechanisms,
                                            double otherContribution,
                                            double lowerLimitNorm,
                                            double signalingNorm)
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
                SetDistribution();
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
                SetDistribution();
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
                return normativeNorm == NormType.LowerLimit
                           ? LowerLimitNorm
                           : SignalingNorm;
            }
        }

        /// <summary>
        /// Gets or sets the norm type which has been defined on the assessment section.
        /// </summary>
        public NormType NormativeNorm
        {
            get
            {
                return normativeNorm;
            }
            set
            {
                normativeNorm = value;
                SetDistribution();
            }
        }

        private void SetDistribution()
        {
            distribution.ForEachElementDo(d => d.Norm = Norm);
        }

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