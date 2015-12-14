﻿using System;
using System.Collections.Generic;
using Core.Common.Base;
using Core.Common.Utils.Collections;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data.Contribution
{
    /// <summary>
    /// This class represents the distribution of all failure mechanism contributions.
    /// </summary>
    public class FailureMechanismContribution : Observable
    {
        private readonly ICollection<FailureMechanismContributionItem> distribution = new List<FailureMechanismContributionItem>();
        private int norm;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContribution"/>. Values are taken from the 
        /// <paramref name="failureMechanisms"/> and one item is added with a value of <paramref name="otherContribution"/>
        /// which represents the contribution of any other failure mechanisms.
        /// </summary>
        /// <param name="failureMechanisms">The <see cref="IEnumerable{T}"/> of <see cref="IFailureMechanism"/> on which to base
        /// the <see cref="FailureMechanismContribution"/>.</param>
        /// <param name="otherContribution">The collective contribution for other failure mechanisms.</param>
        /// <param name="norm">The norm defined on a assessment section.</param>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>any of the <paramref name="failureMechanisms"/> has a value for <see cref="IFailureMechanism.Contribution"/> not in interval [0,100].</item>
        /// <item>the value of <paramref name="otherContribution"/> is not in interval [0,100]</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanisms"/> is <c>null</c>.</exception>
        public FailureMechanismContribution(IEnumerable<IFailureMechanism> failureMechanisms, double otherContribution, int norm)
        {
            if (failureMechanisms == null)
            {
                throw new ArgumentNullException("failureMechanisms", Resources.FailureMechanismContribution_FailureMechanismContribution_Can_not_create_FailureMechanismContribution_without_FailureMechanism_collection);
            }
            Norm = norm;
            failureMechanisms.ForEachElementDo(AddContributionItem);
            AddOtherContributionItem(otherContribution);
        }

        /// <summary>
        /// Gets or sets the norm which has been defined on the assessment section.
        /// </summary>
        public int Norm
        {
            get
            {
                return norm;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value", Resources.FailureMechanismContributionItem_Norm_must_be_larger_than_zero);
                }
                norm = value;
                distribution.ForEachElementDo(d => d.Norm = norm);
            }
        }

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
        /// <exception cref="ArgumentException">Thrown when <paramref name="otherContribution"/> is not in interval [0,100]</exception>
        private void AddOtherContributionItem(double otherContribution)
        {
            var otherFailureMechanism = new OtherFailureMechanism
            {
                Contribution = otherContribution
            };
            var otherContributionItem = new FailureMechanismContributionItem(otherFailureMechanism, norm);
            distribution.Add(otherContributionItem);
        }
    }
}