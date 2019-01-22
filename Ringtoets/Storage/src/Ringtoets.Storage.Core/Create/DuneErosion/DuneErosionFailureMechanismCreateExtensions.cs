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
using System.Collections.Generic;
using Ringtoets.DuneErosion.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.DuneErosion
{
    /// <summary>
    /// Extension methods for <see cref="DuneErosionFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class DuneErosionFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="DuneErosionFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this DuneErosionFailureMechanism mechanism, PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.DuneErosion, registry);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);
            AddEntitiesForDuneLocations(mechanism.DuneLocations, entity, registry);
            AddEntitiesForFailureMechanismMeta(mechanism, entity, registry);
            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<DuneErosionFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (DuneErosionFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                DuneErosionSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.DuneErosionSectionResultEntities.Add(sectionResultEntity);
            }
        }

        private static void AddEntitiesForFailureMechanismMeta(DuneErosionFailureMechanism failureMechanism,
                                                               FailureMechanismEntity entity,
                                                               PersistenceRegistry registry)
        {
            var metaEntity = new DuneErosionFailureMechanismMetaEntity
            {
                N = failureMechanism.GeneralInput.N,
                DuneLocationCalculationCollectionEntity4 = failureMechanism.CalculationsForMechanismSpecificFactorizedSignalingNorm.Create(registry),
                DuneLocationCalculationCollectionEntity3 = failureMechanism.CalculationsForMechanismSpecificSignalingNorm.Create(registry),
                DuneLocationCalculationCollectionEntity2 = failureMechanism.CalculationsForMechanismSpecificLowerLimitNorm.Create(registry),
                DuneLocationCalculationCollectionEntity1 = failureMechanism.CalculationsForLowerLimitNorm.Create(registry),
                DuneLocationCalculationCollectionEntity = failureMechanism.CalculationsForFactorizedLowerLimitNorm.Create(registry)
            };

            entity.DuneErosionFailureMechanismMetaEntities.Add(metaEntity);
        }

        private static void AddEntitiesForDuneLocations(IEnumerable<DuneLocation> locations, FailureMechanismEntity entity, PersistenceRegistry registry)
        {
            var i = 0;
            foreach (DuneLocation duneLocation in locations)
            {
                entity.DuneLocationEntities.Add(duneLocation.Create(registry, i++));
            }
        }
    }
}