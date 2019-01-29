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
using System.Collections.Generic;
using Core.Common.Util.Extensions;
using Riskeer.Common.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.StabilityPointStructures
{
    /// <summary>
    /// Extension methods for <see cref="StabilityPointStructuresFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class StabilityPointStructuresFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="StabilityPointStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this StabilityPointStructuresFailureMechanism mechanism, PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.StabilityPointStructures, registry);
            AddEntitiesForForeshoreProfiles(mechanism.ForeshoreProfiles, entity, registry);
            AddEntitiesForStabilityPointStructures(mechanism.StabilityPointStructures, entity, registry);
            AddEntitiesForFailureMechanismMeta(mechanism, entity);
            entity.CalculationGroupEntity = mechanism.CalculationsGroup.Create(registry, 0);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);

            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (StabilityPointStructuresFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                StabilityPointStructuresSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create(registry);
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.StabilityPointStructuresSectionResultEntities.Add(sectionResultEntity);
            }
        }

        private static void AddEntitiesForForeshoreProfiles(
            ForeshoreProfileCollection foreshoreProfiles,
            FailureMechanismEntity entity,
            PersistenceRegistry registry)
        {
            for (var i = 0; i < foreshoreProfiles.Count; i++)
            {
                ForeshoreProfileEntity foreshoreProfileEntity = foreshoreProfiles[i].Create(registry, i);
                entity.ForeshoreProfileEntities.Add(foreshoreProfileEntity);
            }
        }

        private static void AddEntitiesForStabilityPointStructures(
            StructureCollection<StabilityPointStructure> structures,
            FailureMechanismEntity entity,
            PersistenceRegistry registry)
        {
            for (var i = 0; i < structures.Count; i++)
            {
                StabilityPointStructureEntity structureEntity = structures[i].Create(registry, i);
                entity.StabilityPointStructureEntities.Add(structureEntity);
            }
        }

        private static void AddEntitiesForFailureMechanismMeta(
            StabilityPointStructuresFailureMechanism failureMechanism,
            FailureMechanismEntity entity)
        {
            var metaEntity = new StabilityPointStructuresFailureMechanismMetaEntity
            {
                ForeshoreProfileCollectionSourcePath = failureMechanism.ForeshoreProfiles.SourcePath.DeepClone(),
                StabilityPointStructureCollectionSourcePath = failureMechanism.StabilityPointStructures.SourcePath.DeepClone(),
                N = failureMechanism.GeneralInput.N
            };
            entity.StabilityPointStructuresFailureMechanismMetaEntities.Add(metaEntity);
        }
    }
}