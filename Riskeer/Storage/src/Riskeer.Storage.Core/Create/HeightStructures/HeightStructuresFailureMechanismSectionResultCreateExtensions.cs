// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Ringtoets.HeightStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.HeightStructures
{
    /// <summary>
    /// Extension methods for <see cref="HeightStructuresFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="HeightStructuresSectionResultEntity"/>.
    /// </summary>
    internal static class HeightStructuresFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="HeightStructuresSectionResultEntity"/> based on the information of the
        /// <see cref="HeightStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="HeightStructuresSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static HeightStructuresSectionResultEntity Create(this HeightStructuresFailureMechanismSectionResult result,
                                                                   PersistenceRegistry registry)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var sectionResultEntity = new HeightStructuresSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(result.SimpleAssessmentResult),
                DetailedAssessmentResult = Convert.ToByte(result.DetailedAssessmentResult),
                TailorMadeAssessmentResult = Convert.ToByte(result.TailorMadeAssessmentResult),
                TailorMadeAssessmentProbability = result.TailorMadeAssessmentProbability.ToNaNAsNull(),
                UseManualAssembly = Convert.ToByte(result.UseManualAssembly),
                ManualAssemblyProbability = result.ManualAssemblyProbability.ToNaNAsNull()
            };
            if (result.Calculation != null)
            {
                sectionResultEntity.HeightStructuresCalculationEntity = registry.Get(result.Calculation);
            }

            return sectionResultEntity;
        }
    }
}