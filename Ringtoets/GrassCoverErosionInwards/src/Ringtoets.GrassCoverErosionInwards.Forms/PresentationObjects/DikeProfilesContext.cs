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
using Core.Common.Base;
using Core.Common.Controls.PresentationObjects;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects
{
    /// <summary>
    /// This is a presentation object for <see cref="ObservableList{T}"/> for elements
    /// of type <see cref="DikeProfile"/>.
    /// </summary>
    public class DikeProfilesContext : ObservableWrappedObjectContextBase<ObservableList<DikeProfile>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DikeProfilesContext"/> class.
        /// </summary>
        /// <param name="dikeProfilesList">The observable list of dike profiles.</param>
        /// <param name="parentFailureMechanism">The parent failure mechanism.</param>
        /// <param name="parentAssessmentSection">The parent assessment section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is null.</exception>
        public DikeProfilesContext(ObservableList<DikeProfile> dikeProfilesList, GrassCoverErosionInwardsFailureMechanism parentFailureMechanism, IAssessmentSection parentAssessmentSection) : base(dikeProfilesList)
        {
            if (parentAssessmentSection == null)
            {
                throw new ArgumentNullException(nameof(parentAssessmentSection));
            }
            if (parentFailureMechanism == null)
            {
                throw new ArgumentNullException(nameof(parentFailureMechanism));
            }
            ParentAssessmentSection = parentAssessmentSection;
            ParentFailureMechanism = parentFailureMechanism;
        }

        /// <summary>
        /// Gets the parent assessment section.
        /// </summary>
        public IAssessmentSection ParentAssessmentSection { get; private set; }

        /// <summary>
        /// Gets the parent failure mechanism.
        /// </summary>
        public GrassCoverErosionInwardsFailureMechanism ParentFailureMechanism { get; private set; }
    }
}