// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.HeightStructures
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="FailureMechanismSectionResult"/> 
    /// based on the <see cref="HeightStructuresSectionResultEntity"/>.
    /// </summary>
    internal static class HeightStructuresSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="HeightStructuresSectionResultEntity"/> and use the information 
        /// to update a <see cref="FailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionResult"/> used to 
        /// update the <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <returns>A new <see cref="FailureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this HeightStructuresSectionResultEntity entity,
                                  FailureMechanismSectionResult sectionResult)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }


            sectionResult.IsRelevant = Convert.ToBoolean(entity.IsRelevant);
            sectionResult.InitialFailureMechanismResult = (InitialFailureMechanismResultType) entity.InitialFailureMechanismResultType;
            sectionResult.ManualInitialFailureMechanismResultSectionProbability = entity.ManualInitialFailureMechanismResultSectionProbability.ToNullAsNaN();
            sectionResult.FurtherAnalysisNeeded = Convert.ToBoolean(entity.FurtherAnalysisNeeded);
            sectionResult.RefinedSectionProbability = entity.RefinedSectionProbability.ToNullAsNaN();
        }
    }
}