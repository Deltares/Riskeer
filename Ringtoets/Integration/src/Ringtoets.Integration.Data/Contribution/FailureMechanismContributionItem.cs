using System;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Properties;

namespace Ringtoets.Integration.Data.Contribution
{
    /// <summary>
    /// This class represents an amount for which a failure mechanism will contribute to the 
    /// overall verdict of a dike section.
    /// </summary>
    public class FailureMechanismContributionItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionItem"/>. With
        /// <see cref="Assessment"/>, <see cref="Contribution"/> and <see cref="Norm"/> set.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> for which the contribution is defined.</param>
        /// <param name="norm">The norm used to calculate the probability space.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public FailureMechanismContributionItem(IFailureMechanism failureMechanism, int norm)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException("failureMechanism", Resources.FailureMechanismContributionItem_Can_not_create_contribution_item_without_failure_mechanism);
            }
            Assessment = failureMechanism.Name;
            Contribution = failureMechanism.Contribution;
            Norm = norm;
        }

        /// <summary>
        /// Gets the name of the assessment for which to configure the <see cref="FailureMechanismContribution"/>.
        /// </summary>
        public string Assessment { get; private set; }

        /// <summary>
        /// Gets the amount of contribution as a percentage.
        /// </summary>
        public double Contribution { get; private set; }

        /// <summary>
        /// Gets the probability space per year for the <see cref="FailureMechanismContribution"/>.
        /// </summary>
        public double ProbabilitySpace
        {
            get
            {
                return (Norm / Contribution) * 100;
            }
        }

        /// <summary>
        /// Gets or sets the norm of the complete dike section.
        /// </summary>
        internal double Norm { private get; set; }
    }
}