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
using System.Collections.Generic;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Storage.Core.Create.FailureMechanismSectionResults;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.GrassCoverSlipOffOutwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class GrassCoverSlipOffOutwardsFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="GrassCoverSlipOffOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this GrassCoverSlipOffOutwardsFailureMechanism mechanism, PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.GrassRevetmentSlidingOutwards, registry);
            AddEntitiesForFailureMechanismMeta(mechanism, entity);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);

            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (NonAdoptableWithProfileProbabilityFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.NonAdoptableWithProfileProbabilityFailureMechanismSectionResultEntities.Add(sectionResultEntity);
            }
        }

        private static void AddEntitiesForFailureMechanismMeta(GrassCoverSlipOffOutwardsFailureMechanism mechanism, FailureMechanismEntity entity)
        {
            entity.GrassCoverSlipOffOutwardsFailureMechanismMetaEntities.Add(mechanism.Create<GrassCoverSlipOffOutwardsFailureMechanismMetaEntity>());
        }
    }
}