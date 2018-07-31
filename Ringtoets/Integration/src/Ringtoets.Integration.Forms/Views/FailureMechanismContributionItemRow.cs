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
using Core.Common.Controls.Views;
using Core.Common.Gui.Commands;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Integration.Forms.Views
{
    /// <summary>
    /// This class represents a row of <see cref="FailureMechanismContributionItem"/>.
    /// </summary>
    internal class FailureMechanismContributionItemRow
    {
        private readonly FailureMechanismContributionItem contributionItem;
        private readonly IViewCommands viewCommands;
        private IFailureMechanism failureMechanism;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionItemRow"/>.
        /// </summary>
        /// <param name="contributionItem">The <see cref="FailureMechanismContributionItem"/> this row contains.</param>
        /// <param name="viewCommands">Class responsible for exposing high level <see cref="IView"/>
        /// related commands.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input arguments is <c>null</c>.</exception>
        internal FailureMechanismContributionItemRow(FailureMechanismContributionItem contributionItem, IViewCommands viewCommands)
        {
            if (contributionItem == null)
            {
                throw new ArgumentNullException(nameof(contributionItem));
            }
            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.contributionItem = contributionItem;
            this.viewCommands = viewCommands;
        }

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismContributionItemRow"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism this row contains.</param>
        /// <param name="viewCommands">>Class responsible for exposing high level <see cref="IView"/>
        /// related commands.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal FailureMechanismContributionItemRow(IFailureMechanism failureMechanism, IViewCommands viewCommands)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (viewCommands == null)
            {
                throw new ArgumentNullException(nameof(viewCommands));
            }

            this.failureMechanism = failureMechanism;
            this.viewCommands = viewCommands;
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.Assessment"/>.
        /// </summary>
        public string Assessment
        {
            get
            {
                return contributionItem.Assessment;
            }
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.AssessmentCode"/>.
        /// </summary>
        public string Code
        {
            get
            {
                return contributionItem.AssessmentCode;
            }
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.Contribution"/>.
        /// </summary>
        public double Contribution
        {
            get
            {
                return contributionItem.Contribution;
            }
        }

        /// <summary>
        /// Gets <see cref="FailureMechanismContributionItem.ProbabilitySpace"/>.
        /// </summary>
        public double ProbabilitySpace
        {
            get
            {
                return contributionItem.ProbabilitySpace;
            }
        }

        /// <summary>
        /// Gets or sets <see cref="FailureMechanismContributionItem.IsRelevant"/>.
        /// </summary>
        public bool IsRelevant
        {
            get
            {
                return contributionItem.IsRelevant;
            }
            set
            {
                if (!value)
                {
                    viewCommands.RemoveAllViewsForItem(contributionItem.FailureMechanism);
                }

                contributionItem.IsRelevant = value;
                contributionItem.NotifyFailureMechanismObservers();
            }
        }
    }
}