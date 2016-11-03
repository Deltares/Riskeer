// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Create.GrassCoverErosionOutwards
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
            var entity = mechanism.Create(FailureMechanismType.GrassRevetmentErosionOutwards, registry);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);
            AddEntitiesForFailureMechanismMeta(mechanism.GeneralInput, entity);
            AddEntitiesForForeshoreProfiles(mechanism.ForeshoreProfiles, entity, registry);
            AddEntitiesForHydraulicBoundaryLocations(mechanism.HydraulicBoundaryLocations, entity, registry);
            entity.CalculationGroupEntity = mechanism.WaveConditionsCalculationGroup.Create(registry, 0);

            return entity;
        }

        private static void AddEntitiesForHydraulicBoundaryLocations(ObservableList<HydraulicBoundaryLocation> hydraulicBoundaryLocations, FailureMechanismEntity entity, PersistenceRegistry registry)
        {
            int i = 0;
            foreach (var location in hydraulicBoundaryLocations)
            {
                entity.GrassCoverErosionOutwardsHydraulicLocationEntities.Add(location.CreateGrassCoverErosionOutwardsHydraulicBoundaryLocation(registry, i++));
            }
        }

        private static void AddEntitiesForFailureMechanismMeta(GeneralGrassCoverErosionOutwardsInput generalInput, FailureMechanismEntity entity)
        {
            entity.GrassCoverErosionOutwardsFailureMechanismMetaEntities.Add(generalInput.Create());
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (var failureMechanismSectionResult in sectionResults)
            {
                var sectionResultEntity = failureMechanismSectionResult.Create();
                var section = registry.Get(failureMechanismSectionResult.Section);
                section.GrassCoverErosionOutwardsSectionResultEntities.Add(sectionResultEntity);
            }
        }

        private static void AddEntitiesForForeshoreProfiles(
            IEnumerable<ForeshoreProfile> foreshoreProfiles,
            FailureMechanismEntity entity,
            PersistenceRegistry registry)
        {
            int i = 0;

            foreach (var foreshoreProfile in foreshoreProfiles)
            {
                var foreshoreProfileEntity = foreshoreProfile.Create(registry, i++);
                entity.ForeshoreProfileEntities.Add(foreshoreProfileEntity);
            }
        }
    }
}