// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Riskeer.Common.Primitives;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.FailureMechanismSectionResults
{
    /// <summary>
    /// This class defines extension methods for read operations for instances of <see cref="FailureMechanismSectionResult"/>
    /// based on a failure mechanism section result entity.
    /// </summary>
    internal static class FailureMechanismSectionResultEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="AdoptableFailureMechanismSectionResultEntity"/> and use the information
        /// to update an <see cref="AdoptableFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="AdoptableFailureMechanismSectionResultEntity"/> used to update 
        /// the <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this AdoptableFailureMechanismSectionResultEntity entity,
                                  AdoptableFailureMechanismSectionResult sectionResult)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            sectionResult.InitialFailureMechanismResultType = (AdoptableInitialFailureMechanismResultType) entity.InitialFailureMechanismResultType;
            sectionResult.SetCommonFailureMechanismSectionResultProperties(entity);
        }

        /// <summary>
        /// Reads the <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> and use the information
        /// to update an <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> used to update 
        /// the <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal static void Read(this AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity entity,
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

            sectionResult.InitialFailureMechanismResultType = (AdoptableInitialFailureMechanismResultType) entity.InitialFailureMechanismResultType;
            sectionResult.ManualInitialFailureMechanismResultProfileProbability = entity.ManualInitialFailureMechanismResultProfileProbability.ToNullAsNaN();
            sectionResult.ProbabilityRefinementType = (ProbabilityRefinementType) entity.ProbabilityRefinementType;
            sectionResult.RefinedProfileProbability = entity.RefinedProfileProbability.ToNullAsNaN();
            sectionResult.SetCommonFailureMechanismSectionResultProperties(entity);
        }

        /// <summary>
        /// Reads the <see cref="NonAdoptableFailureMechanismSectionResultEntity"/> and use the information
        /// to update a <see cref="NonAdoptableFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="entity">The <see cref="NonAdoptableFailureMechanismSectionResultEntity"/> used to update 
        /// the <paramref name="sectionResult"/>.</param>
        /// <param name="sectionResult">The target of the read operation.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void Read(this NonAdoptableFailureMechanismSectionResultEntity entity,
                                  NonAdoptableFailureMechanismSectionResult sectionResult)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            sectionResult.InitialFailureMechanismResultType = (NonAdoptableInitialFailureMechanismResultType) entity.InitialFailureMechanismResultType;
            sectionResult.SetCommonFailureMechanismSectionResultProperties(entity);
        }

        private static void SetCommonFailureMechanismSectionResultProperties(this FailureMechanismSectionResult result,
                                                                             IFailureMechanismSectionResultEntity entity)
        {
            result.IsRelevant = Convert.ToBoolean(entity.IsRelevant);
            result.ManualInitialFailureMechanismResultSectionProbability = entity.ManualInitialFailureMechanismResultSectionProbability.ToNullAsNaN();
            result.FurtherAnalysisType = (FailureMechanismSectionResultFurtherAnalysisType) entity.FurtherAnalysisType;
            result.RefinedSectionProbability = entity.RefinedSectionProbability.ToNullAsNaN();
        }
    }
}