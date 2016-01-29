// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System;
using Ringtoets.Common.Data;
using Ringtoets.Integration.Data.Properties;

using CommonResources = Ringtoets.Common.Data.Properties.Resources;

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
        /// Gets or sets the norm of the complete dike section.
        /// </summary>
        public int Norm { get; internal set; }

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
    }
}