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

namespace Riskeer.Storage.Core.Create.FailureMechanismSectionResults
{
    /// <summary>
    /// Extension methods for instances of <see cref="FailureMechanismSectionResult"/> related to creating
    /// instances of failure mechanism section result entities.
    /// </summary>
    internal static class FailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates an instance of <see cref="AdoptableFailureMechanismSectionResultEntity"/> 
        /// based on the information of the <see cref="AdoptableFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>An <see cref="AdoptableFailureMechanismSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static AdoptableFailureMechanismSectionResultEntity Create(this AdoptableFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionResultEntity = new AdoptableFailureMechanismSectionResultEntity
            {
                InitialFailureMechanismResultType = Convert.ToByte(result.InitialFailureMechanismResultType)
            };
            sectionResultEntity.SetCommonFailureMechanismSectionResultProperties(result);
            return sectionResultEntity;
        }

        /// <summary>
        /// Creates an instance of <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/> 
        /// based on the information of the <see cref="AdoptableFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>An <see cref="AdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity Create(
            this AdoptableWithProfileProbabilityFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionResultEntity = new AdoptableWithProfileProbabilityFailureMechanismSectionResultEntity
            {
                InitialFailureMechanismResultType = Convert.ToByte(result.InitialFailureMechanismResultType),
                ManualInitialFailureMechanismResultProfileProbability = result.ManualInitialFailureMechanismResultProfileProbability.ToNaNAsNull(),
                ProbabilityRefinementType = Convert.ToByte(result.ProbabilityRefinementType),
                RefinedProfileProbability = result.RefinedProfileProbability.ToNaNAsNull()
            };
            sectionResultEntity.SetCommonFailureMechanismSectionResultProperties(result);
            return sectionResultEntity;
        }

        /// <summary>
        /// Creates an instance of <see cref="NonAdoptableFailureMechanismSectionResultEntity"/> 
        /// based on the information of the <see cref="NonAdoptableFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A <see cref="NonAdoptableFailureMechanismSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static NonAdoptableFailureMechanismSectionResultEntity Create(this NonAdoptableFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionResultEntity = new NonAdoptableFailureMechanismSectionResultEntity
            {
                InitialFailureMechanismResultType = Convert.ToByte(result.InitialFailureMechanismResultType)
            };
            sectionResultEntity.SetCommonFailureMechanismSectionResultProperties(result);
            return sectionResultEntity;
        }

        /// <summary>
        /// Creates an instance of <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/> 
        /// based on the information of the <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity Create(
            this NonAdoptableWithProfileProbabilityFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var sectionResultEntity = new NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity
            {
                InitialFailureMechanismResultType = Convert.ToByte(result.InitialFailureMechanismResultType),
                ManualInitialFailureMechanismResultProfileProbability = result.ManualInitialFailureMechanismResultProfileProbability.ToNaNAsNull(),
                RefinedProfileProbability = result.RefinedProfileProbability.ToNaNAsNull()
            };
            sectionResultEntity.SetCommonFailureMechanismSectionResultProperties(result);
            return sectionResultEntity;
        }

        private static void SetCommonFailureMechanismSectionResultProperties(this IFailureMechanismSectionResultEntity entity,
                                                                             FailureMechanismSectionResult result)
        {
            entity.IsRelevant = Convert.ToByte(result.IsRelevant);
            entity.ManualInitialFailureMechanismResultSectionProbability = result.ManualInitialFailureMechanismResultSectionProbability.ToNaNAsNull();
            entity.FurtherAnalysisType = Convert.ToByte(result.FurtherAnalysisType);
            entity.RefinedSectionProbability = result.RefinedSectionProbability.ToNaNAsNull();
        }
    }
}