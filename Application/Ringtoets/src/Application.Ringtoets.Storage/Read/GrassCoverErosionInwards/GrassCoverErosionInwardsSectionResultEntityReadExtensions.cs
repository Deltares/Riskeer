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
using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Read.GrassCoverErosionInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> based on the
    /// <see cref="GrassCoverErosionInwardsSectionResultEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionInwardsSectionResultEntity"/> and use the information to construct a 
        /// <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsSectionResultEntity"/> to create <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/> for.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="GrassCoverErosionInwardsFailureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this GrassCoverErosionInwardsSectionResultEntity entity, GrassCoverErosionInwardsFailureMechanismSectionResult sectionResult, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            sectionResult.AssessmentLayerOne = (AssessmentLayerOneState) entity.LayerOne;
            sectionResult.AssessmentLayerThree = (RoundedDouble) entity.LayerThree.ToNullAsNaN();
            if (entity.GrassCoverErosionInwardsCalculationEntity != null)
            {
                sectionResult.Calculation = entity.GrassCoverErosionInwardsCalculationEntity.Read(collector);
            }
        }
    }
}