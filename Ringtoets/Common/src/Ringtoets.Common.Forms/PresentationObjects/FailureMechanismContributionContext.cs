// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Controls.PresentationObjects;

using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;

namespace Ringtoets.Common.Forms.PresentationObjects
{
    /// <summary>
    /// This class is a presentation object for a <see cref="FailureMechanismContribution"/> instance.
    /// </summary>
    public sealed class FailureMechanismContributionContext : WrappedObjectContextBase<FailureMechanismContribution>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FailureMechanismContributionContext"/> class.
        /// </summary>
        /// <param name="contribution">The contribution.</param>
        /// <param name="contributionOwner">The parent of <paramref name="contribution"/>.</param>
        /// <exception cref="ArgumentNullException">When any input argument is null.</exception>
        public FailureMechanismContributionContext(FailureMechanismContribution contribution, IAssessmentSection contributionOwner)
            : base(contribution)
        {
            AssertInputsAreNotNull(contributionOwner);

            Parent = contributionOwner;
        }

        /// <summary>
        /// Gets the assessment section to which <see cref="WrappedObjectContextBase{T}.WrappedData"/> belongs to.
        /// </summary>
        public IAssessmentSection Parent { get; private set; }

        private void AssertInputsAreNotNull(IAssessmentSection contributionOwner)
        {
            if (contributionOwner == null)
            {
                throw new ArgumentNullException("contributionOwner", "Assessment section cannot be null.");
            }
        }
    }
}