﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Ringtoets.GrassCoverErosionInwards.Data;

namespace Application.Ringtoets.Storage.Create.GrassCoverErosionInwards
{
    /// <summary>
    /// Extension methods for <see cref="GrassCoverErosionInwardsFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionInwardsFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="GrassCoverErosionInwardsFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this GrassCoverErosionInwardsFailureMechanism mechanism, PersistenceRegistry registry)
        {
            var entity = mechanism.Create(FailureMechanismType.GrassRevetmentTopErosionAndInwards, registry);
            AddEntitiesForGeneralInput(mechanism, registry, entity);
            AddEntitiesForDikeProfiles(mechanism, registry, entity);
            entity.CalculationGroupEntity = mechanism.CalculationsGroup.Create(registry, 0);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);

            registry.Register(entity, mechanism);
            return entity;
        }

        private static void AddEntitiesForGeneralInput(GrassCoverErosionInwardsFailureMechanism mechanism, PersistenceRegistry registry, FailureMechanismEntity entity)
        {
            entity.GrassCoverErosionInwardsFailureMechanismMetaEntities.Add(mechanism.GeneralInput.Create(registry));
        }

        private static void AddEntitiesForDikeProfiles(GrassCoverErosionInwardsFailureMechanism mechanism, PersistenceRegistry registry, FailureMechanismEntity entity)
        {
            for (int i = 0; i < mechanism.DikeProfiles.Count; i++)
            {
                DikeProfile dikeProfile = mechanism.DikeProfiles[i];
                entity.DikeProfileEntities.Add(dikeProfile.Create(registry, i));
            }
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<GrassCoverErosionInwardsFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (var failureMechanismSectionResult in sectionResults)
            {
                var sectionResultEntity = failureMechanismSectionResult.Create(registry);
                var section = registry.Get(failureMechanismSectionResult.Section);
                section.GrassCoverErosionInwardsSectionResultEntities.Add(sectionResultEntity);
            }
        }
    }
}