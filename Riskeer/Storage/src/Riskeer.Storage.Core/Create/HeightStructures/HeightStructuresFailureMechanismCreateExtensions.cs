// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.HeightStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.HeightStructures
{
    /// <summary>
    /// Extension methods for <see cref="HeightStructuresFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class HeightStructuresFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="HeightStructuresFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this HeightStructuresFailureMechanism mechanism, PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.StructureHeight, registry);
            AddEntitiesForForeshoreProfiles(mechanism.ForeshoreProfiles, entity, registry);
            AddEntitiesForHeightStructures(mechanism.HeightStructures, entity, registry);
            AddEntitiesForFailureMechanismMeta(mechanism, entity);
            entity.CalculationGroupEntity = mechanism.CalculationsGroup.Create(registry, 0);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);

            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<HeightStructuresFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (HeightStructuresFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                HeightStructuresSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.HeightStructuresSectionResultEntities.Add(sectionResultEntity);
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

        private static void AddEntitiesForHeightStructures(
            StructureCollection<HeightStructure> structures,
            FailureMechanismEntity entity,
            PersistenceRegistry registry)
        {
            for (var i = 0; i < structures.Count; i++)
            {
                HeightStructureEntity structureEntity = structures[i].Create(registry, i);
                entity.HeightStructureEntities.Add(structureEntity);
            }
        }

        private static void AddEntitiesForFailureMechanismMeta(HeightStructuresFailureMechanism mechanism, FailureMechanismEntity entity)
        {
            entity.HeightStructuresFailureMechanismMetaEntities.Add(new HeightStructuresFailureMechanismMetaEntity
            {
                N = mechanism.GeneralInput.N,
                HeightStructureCollectionSourcePath = mechanism.HeightStructures.SourcePath.DeepClone(),
                ForeshoreProfileCollectionSourcePath = mechanism.ForeshoreProfiles.SourcePath.DeepClone()
            });
        }
    }
}