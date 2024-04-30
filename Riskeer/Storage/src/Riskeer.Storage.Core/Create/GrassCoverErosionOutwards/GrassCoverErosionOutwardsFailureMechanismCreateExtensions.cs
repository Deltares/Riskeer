﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.Storage.Core.Create.FailureMechanismSectionResults;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.GrassCoverErosionOutwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionOutwardsFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this GrassCoverErosionOutwardsFailureMechanism mechanism, PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.GrassRevetmentErosionOutwards, registry);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);
            AddEntitiesForFailureMechanismMeta(mechanism, entity);
            AddEntitiesForForeshoreProfiles(mechanism.ForeshoreProfiles, entity, registry);
            entity.CalculationGroupEntity = mechanism.CalculationsGroup.Create(registry, 0);

            return entity;
        }

        private static void AddEntitiesForFailureMechanismMeta(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                               FailureMechanismEntity entity)
        {
            var metaEntity = new GrassCoverErosionOutwardsFailureMechanismMetaEntity
            {
                ForeshoreProfileCollectionSourcePath = failureMechanism.ForeshoreProfiles.SourcePath.DeepClone()
            };

            entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Add(metaEntity);
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<NonAdoptableFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (NonAdoptableFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                NonAdoptableFailureMechanismSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.NonAdoptableFailureMechanismSectionResultEntities.Add(sectionResultEntity);
            }
        }

        private static void AddEntitiesForForeshoreProfiles(
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            FailureMechanismEntity entity,
            PersistenceRegistry registry)
        {
            var i = 0;

            foreach (ForeshoreProfile foreshoreProfile in foreshoreProfiles)
            {
                ForeshoreProfileEntity foreshoreProfileEntity = foreshoreProfile.Create(registry, i++);
                entity.ForeshoreProfileEntities.Add(foreshoreProfileEntity);
            }
        }
    }
}