// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.ClosingStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="ClosingStructuresFailureMechanismSectionResult"/> based on the
    /// <see cref="ClosingStructuresSectionResultEntity"/>.
    /// </summary>
    internal static class ClosingStructuresSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ClosingStructuresSectionResultEntity"/> and use the information to update a 
        /// <see cref="ClosingStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructuresSectionResultEntity"/> used to update 
        /// the <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this ClosingStructuresSectionResultEntity entity,
                                  ClosingStructuresFailureMechanismSectionResult sectionResult,
                                  ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            sectionResult.SimpleAssessmentResult = (SimpleAssessmentResultType) entity.SimpleAssessmentResult;
            sectionResult.DetailedAssessmentResult = (DetailedAssessmentProbabilityOnlyResultType) entity.DetailedAssessmentResult;
            sectionResult.TailorMadeAssessmentResult = (TailorMadeAssessmentProbabilityCalculationResultType) entity.TailorMadeAssessmentResult;
            sectionResult.TailorMadeAssessmentProbability = entity.TailorMadeAssessmentProbability.ToNullAsNaN();
            sectionResult.UseManualAssembly = Convert.ToBoolean(entity.UseManualAssembly);
            sectionResult.ManualAssemblyProbability = entity.ManualAssemblyProbability.ToNullAsNaN();

            if (entity.ClosingStructuresCalculationEntity != null)
            {
                sectionResult.Calculation = entity.ClosingStructuresCalculationEntity.Read(collector);
            }
        }
    }
}