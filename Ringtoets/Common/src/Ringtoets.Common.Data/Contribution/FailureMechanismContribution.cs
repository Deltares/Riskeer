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
using Core.Common.Utils.Extensions;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Contribution
{
    /// <summary>
    /// This class represents the distribution of all failure mechanism contributions.
    /// </summary>
    public class FailureMechanismContribution : Observable
    {
        private const double defaultNorm = 1.0 / 30000;
        private static readonly Range<double> normValidityRange = new Range<double>(1.0 / 1000000, 1.0 / 10);

        private readonly ICollection<FailureMechanismContributionItem> distribution = new List<FailureMechanismContributionItem>();
        private readonly OtherFailureMechanism otherFailureMechanism = new OtherFailureMechanism();
        private double norm;
        private double lowerLimitNorm;
        private double signalingNorm;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContribution"/>. Values are taken from the 
        /// <paramref name="failureMechanisms"/> and one item is added with a value of <paramref name="otherContribution"/>
        /// which represents the contribution of any other failure mechanisms.
        /// </summary>
        /// <param name="failureMechanisms">The <see cref="IEnumerable{T}"/> of <see cref="IFailureMechanism"/> 
        /// on which to base the <see cref="FailureMechanismContribution"/>.</param>
        /// <param name="otherContribution">The collective contribution for other failure mechanisms.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanisms"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>any of the <paramref name="failureMechanisms"/> has a value for <see cref="IFailureMechanism.Contribution"/> 
        /// not in the interval [0, 100].</item>
        /// <item>the value of <paramref name="otherContribution"/> is not in the interval [0, 100]</item>
        /// </list>
        /// </exception>
        public FailureMechanismContribution(IEnumerable<IFailureMechanism> failureMechanisms, double otherContribution)
        {
            Norm = defaultNorm;
            signalingNorm = defaultNorm;
            lowerLimitNorm = defaultNorm;
            NormativeNorm = NormType.LowerLimit;

            UpdateContributions(failureMechanisms, otherContribution);
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
                ValidateNorm(value);

                if (value > lowerLimitNorm)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.FailureMechanismContribution_SignalingNorm_should_be_same_or_smaller_than_LowerLimitNorm);
                }

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
                    throw new ArgumentOutOfRangeException(nameof(value), Resources.FailureMechanismContribution_SignalingNorm_should_be_same_or_smaller_than_LowerLimitNorm);
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
            set
            {
                ValidateNorm(value);

                norm = value;
                distribution.ForEachElementDo(d => d.Norm = norm);
            }
        }

        /// <summary>
        /// Gets or sets the norm type which has been defined on the assessment section.
        /// </summary>
        public NormType NormativeNorm { get; set; }

        /// <summary>
        /// Gets the distribution of failure mechanism contributions.
        /// </summary>
        public IEnumerable<FailureMechanismContributionItem> Distribution
        {
            get
            {
                return distribution;
            }
        }

        /// <summary>
        /// Fully updates the contents of <see cref="Distribution"/> for a new set of failure
        /// mechanisms and the remainder contribution.
        /// </summary>
        /// <param name="newFailureMechanisms">The new failure mechanisms.</param>
        /// <param name="otherContribution">The collective contribution for other failure mechanisms.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newFailureMechanisms"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>any of the <paramref name="newFailureMechanisms"/> has a value for 
        /// <see cref="IFailureMechanism.Contribution"/> not in the interval [0, 100].</item>
        /// <item>the value of <paramref name="otherContribution"/> is not in the interval [0, 100]</item>
        /// </list>
        /// </exception>
        public void UpdateContributions(IEnumerable<IFailureMechanism> newFailureMechanisms, double otherContribution)
        {
            if (newFailureMechanisms == null)
            {
                throw new ArgumentNullException(nameof(newFailureMechanisms),
                                                Resources.FailureMechanismContribution_UpdateContributions_Can_not_create_FailureMechanismContribution_without_FailureMechanism_collection);
            }

            distribution.Clear();
            newFailureMechanisms.ForEachElementDo(AddContributionItem);
            AddOtherContributionItem(otherContribution);
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
                throw new ArgumentOutOfRangeException(nameof(value), message);
            }
        }

        /// <summary>
        /// Adds a <see cref="FailureMechanismContributionItem"/> based on <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> to add a <see cref="FailureMechanismContributionItem"/> for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        private void AddContributionItem(IFailureMechanism failureMechanism)
        {
            distribution.Add(new FailureMechanismContributionItem(failureMechanism, norm));
        }

        /// <summary>
        /// Adds a <see cref="FailureMechanismContributionItem"/> representing all other failure mechanisms not in the failure mechanism
        /// list supported within Ringtoets.
        /// </summary>
        /// <param name="otherContribution">The contribution to set for other failure mechanisms.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="otherContribution"/> is not in the interval [0, 100]</exception>
        private void AddOtherContributionItem(double otherContribution)
        {
            otherFailureMechanism.Contribution = otherContribution;
            var otherContributionItem = new FailureMechanismContributionItem(otherFailureMechanism, norm, true);
            distribution.Add(otherContributionItem);
        }
    }
}