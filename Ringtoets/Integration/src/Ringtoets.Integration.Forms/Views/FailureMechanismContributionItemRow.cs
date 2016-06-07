﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Contribution;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="FailureMechanismContributionItem"/>.
    /// </summary>
    internal class FailureMechanismContributionItemRow
    {
        private readonly FailureMechanismContributionItem contributionItem;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionItemRow"/>.
        /// </summary>
        /// <param name="contributionItem">The <see cref="FailureMechanismContributionItem"/> this row contains.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="contributionItem"/> is <c>null</c>.</exception>
        internal FailureMechanismContributionItemRow(FailureMechanismContributionItem contributionItem)
        {
            if (contributionItem == null)
            {
                throw new ArgumentNullException("contributionItem");
            }

            this.contributionItem = contributionItem;
        }

        /// <summary>
        /// Gets the name of the <see cref="FailureMechanismContributionItem"/>.
        /// </summary>
        public string Assessment
        {
            get
            {
                return contributionItem.Assessment;
            }
        }

        /// <summary>
        /// Returns the code of the <see cref="FailureMechanismContributionItem"/>
        /// </summary>
        public string Code
        {
            get
            {
                return contributionItem.AssessmentCode;
            }
        }

        /// <summary>
        /// Gets the amount of contribution of the <see cref="FailureMechanismContributionItem"/> as a percentage.
        /// </summary>
        public double Contribution
        {
            get
            {
                return contributionItem.Contribution;
            }
        }

        /// <summary>
        /// Gets or sets the norm of the <see cref="FailureMechanismContributionItem"/>.
        /// </summary>
        public int Norm
        {
            get
            {
                return contributionItem.Norm;
            }
        }

        /// <summary>
        /// Gets the probability space of the <see cref="FailureMechanismContributionItem"/>.
        /// </summary>
        public double ProbabilitySpace
        {
            get
            {
                return contributionItem.ProbabilitySpace;
            }
        }

        /// <summary>
        /// Gets or sets the isRelevant of the <see cref="FailureMechanismContributionItem"/>
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return contributionItem.IsRelevant;
            }
            set
            {
                contributionItem.IsRelevant = value;
                contributionItem.NotifyFailureMechanismObservers();
            }
        }
    }
}