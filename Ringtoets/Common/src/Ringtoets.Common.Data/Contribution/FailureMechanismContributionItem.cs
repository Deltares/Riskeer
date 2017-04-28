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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.Contribution
{
    /// <summary>
    /// This class represents an amount for which a <see cref="IFailureMechanism"/> will 
    /// contribute to the overall verdict of an assessment section.
    /// </summary>
    public class FailureMechanismContributionItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionItem"/>. With
        /// <see cref="Assessment"/>, <see cref="Contribution"/> and <see cref="Norm"/> set.
        /// </summary>
        /// <param name="failureMechanism">The <see cref="IFailureMechanism"/> for which the contribution is defined.</param>
        /// <param name="norm">The norm used to calculate the probability space.</param>
        /// <param name="isFailureMechanismAlwaysRelevant">Gets a value indicating whether
        /// the corresponding failure mechanism is always relevant. When <c>true</c>, then
        /// <see cref="IsRelevant"/> cannot be set to <c>false</c>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/> is <c>null</c>.</exception>
        public FailureMechanismContributionItem(IFailureMechanism failureMechanism, double norm, bool isFailureMechanismAlwaysRelevant = false)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism), Resources.FailureMechanismContributionItem_Can_not_create_contribution_item_without_failure_mechanism);
            }
            FailureMechanism = failureMechanism;

            Norm = norm;
            IsAlwaysRelevant = isFailureMechanismAlwaysRelevant;
        }

        /// <summary>
        /// Gets the name of the assessment for which to configure the <see cref="FailureMechanismContribution"/>.
        /// </summary>
        public string Assessment
        {
            get
            {
                return FailureMechanism.Name;
            }
        }

        /// <summary>
        /// Returns the code of the assessment for which to configure the <see cref="FailureMechanismContribution"/>
        /// </summary>
        public string AssessmentCode
        {
            get
            {
                return FailureMechanism.Code;
            }
        }

        /// <summary>
        /// Gets the amount of contribution as a percentage.
        /// </summary>
        public double Contribution
        {
            get
            {
                return FailureMechanism.Contribution;
            }
        }

        /// <summary>
        /// Gets or sets the norm of the complete assessment section.
        /// </summary>
        public double Norm { get; internal set; }

        /// <summary>
        /// Gets the probability space per year for the <see cref="FailureMechanismContribution"/>.
        /// </summary>
        public double ProbabilitySpace
        {
            get
            {
                return 100.0 / (Norm * Contribution);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the corresponding failure mechanism is
        /// relevant or not.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return IsAlwaysRelevant || FailureMechanism.IsRelevant;
            }
            set
            {
                if (!IsAlwaysRelevant)
                {
                    FailureMechanism.IsRelevant = value;
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the corresponding <see cref="IFailureMechanism"/>
        /// is always relevant. When <c>true</c>, then <see cref="IsRelevant"/> cannot be set to <c>false</c>.
        /// </summary>
        public bool IsAlwaysRelevant { get; }

        /// <summary>
        /// Gets the failure mechanism.
        /// </summary>
        public IFailureMechanism FailureMechanism { get; }

        /// <summary>
        /// Notifies the observers for the wrapped <see cref="IFailureMechanism"/>.
        /// </summary>
        public void NotifyFailureMechanismObservers()
        {
            FailureMechanism.NotifyObservers();
        }
    }
}