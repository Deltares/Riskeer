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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionOutwardsFailureMechanismSectionResult"/> based on the
    /// <see cref="GrassCoverErosionOutwardsSectionResultEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionOutwardsSectionResultEntity"/> and use the information to update a 
        /// <see cref="GrassCoverErosionOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionOutwardsSectionResultEntity"/> to create <see cref="GrassCoverErosionOutwardsFailureMechanismSectionResult"/> for.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <returns>A new <see cref="GrassCoverErosionOutwardsFailureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        internal static void Read(this GrassCoverErosionOutwardsSectionResultEntity entity, GrassCoverErosionOutwardsFailureMechanismSectionResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException("sectionResult");
            }

            sectionResult.StorageId = entity.GrassCoverErosionOutwardsSectionResultEntityId;
            sectionResult.AssessmentLayerOne = Convert.ToBoolean(entity.LayerOne);
            sectionResult.AssessmentLayerTwoA = (AssessmentLayerTwoAResult) entity.LayerTwoA;
            sectionResult.AssessmentLayerThree = (RoundedDouble) entity.LayerThree.ToNullAsNaN();
        }
    }
}