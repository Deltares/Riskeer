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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Integration.Data.StandAlone.SectionResults;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/> based on the
    /// <see cref="MacrostabilityOutwardsSectionResultEntity"/>.
    /// </summary>
    internal static class MacrostabilityOutwardsSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="MacrostabilityOutwardsSectionResultEntity"/> and use the information to update a 
        /// <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacrostabilityOutwardsSectionResultEntity"/> to create <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/> for.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <returns>A new <see cref="MacrostabilityOutwardsFailureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        internal static void Read(this MacrostabilityOutwardsSectionResultEntity entity, MacrostabilityOutwardsFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            sectionResult.AssessmentLayerOne = (AssessmentLayerOneState) entity.LayerOne;
            sectionResult.AssessmentLayerTwoA = (RoundedDouble) entity.LayerTwoA.ToNullAsNaN();
            sectionResult.AssessmentLayerThree = (RoundedDouble) entity.LayerThree.ToNullAsNaN();
        }
    }
}