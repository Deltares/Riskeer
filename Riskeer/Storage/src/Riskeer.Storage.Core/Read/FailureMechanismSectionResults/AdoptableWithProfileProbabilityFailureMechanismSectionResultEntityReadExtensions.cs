﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

namespace Riskeer.Storage.Core.Read.FailureMechanismSectionResults
{
    /// <summary>
    /// This class defines extension methods for read operations for an
    /// <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>
    /// based on the <see cref="IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>.
    /// </summary>
    internal static class AdoptableWithProfileProbabilityFailureMechanismSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> and use the information
        /// to update an <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSectionResultEntity"/> used to update 
        /// the <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this IAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity entity,
                                  AdoptableWithProfileProbabilityFailureMechanismSectionResult sectionResult)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            ((IAdoptableFailureMechanismSectionResultEntity) entity).Read(sectionResult);
            sectionResult.ManualInitialFailureMechanismResultProfileProbability = entity.ManualInitialFailureMechanismResultProfileProbability.ToNullAsNaN();
            sectionResult.ProbabilityRefinementType = (ProbabilityRefinementType) entity.ProbabilityRefinementType;
            sectionResult.RefinedProfileProbability = entity.RefinedProfileProbability.ToNullAsNaN();
        }
    }
}