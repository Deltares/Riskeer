// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Create.GrassCoverErosionOutwards
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
            AddEntitiesForFailureMechanismMeta(mechanism, entity, registry);
            AddEntitiesForForeshoreProfiles(mechanism.ForeshoreProfiles, entity, registry);
            entity.CalculationGroupEntity = mechanism.WaveConditionsCalculationGroup.Create(registry, 0);

            return entity;
        }

        private static void AddEntitiesForFailureMechanismMeta(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                               FailureMechanismEntity entity, PersistenceRegistry registry)
        {
            var metaEntity = new GrassCoverErosionOutwardsFailureMechanismMetaEntity
            {
                ForeshoreProfileCollectionSourcePath = failureMechanism.ForeshoreProfiles.SourcePath.DeepClone(),
                N = failureMechanism.GeneralInput.N,
                HydraulicLocationCalculationCollectionEntity5 = failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm
                                                                                .Create(registry),
                HydraulicLocationCalculationCollectionEntity4 = failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm
                                                                                .Create(registry),
                HydraulicLocationCalculationCollectionEntity3 = failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm
                                                                                .Create(registry),
                HydraulicLocationCalculationCollectionEntity2 = failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm
                                                                                .Create(registry),
                HydraulicLocationCalculationCollectionEntity1 = failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm
                                                                                .Create(registry),
                HydraulicLocationCalculationCollectionEntity = failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm
                                                                               .Create(registry)
            };

            entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Add(metaEntity);
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (GrassCoverErosionOutwardsFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                GrassCoverErosionOutwardsSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.GrassCoverErosionOutwardsSectionResultEntities.Add(sectionResultEntity);
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