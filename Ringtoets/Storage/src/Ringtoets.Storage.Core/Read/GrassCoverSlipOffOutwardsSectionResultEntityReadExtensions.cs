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
using Ringtoets.Common.Primitives;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverSlipOffOutwardsFailureMechanismSectionResult"/> based on the
    /// <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/>.
    /// </summary>
    internal static class GrassCoverSlipOffOutwardsSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/> and use the information to update a 
        /// <see cref="GrassCoverSlipOffOutwardsFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverSlipOffOutwardsSectionResultEntity"/> used to update <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this GrassCoverSlipOffOutwardsSectionResultEntity entity, GrassCoverSlipOffOutwardsFailureMechanismSectionResult sectionResult)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            sectionResult.SimpleAssessmentResult = (SimpleAssessmentResultType) entity.SimpleAssessmentResult;
            sectionResult.DetailedAssessmentResult = (DetailedAssessmentResultType) entity.DetailedAssessmentResult;
            sectionResult.TailorMadeAssessmentResult = (TailorMadeAssessmentResultType) entity.TailorMadeAssessmentResult;
            sectionResult.UseManualAssemblyCategoryGroup = Convert.ToBoolean(entity.UseManualAssemblyCategoryGroup);
            sectionResult.ManualAssemblyCategoryGroup = (ManualFailureMechanismSectionAssemblyCategoryGroup) entity.ManualAssemblyCategoryGroup;
        }
    }
}