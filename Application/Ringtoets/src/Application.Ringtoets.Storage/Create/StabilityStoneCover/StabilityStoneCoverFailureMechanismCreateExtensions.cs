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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.StabilityStoneCover.Data;

namespace Application.Ringtoets.Storage.Create.StabilityStoneCover
{
    /// <summary>
    /// Extension methods for <see cref="StabilityStoneCoverFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class StabilityStoneCoverFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="StabilityStoneCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this StabilityStoneCoverFailureMechanism mechanism, PersistenceRegistry registry)
        {
            var entity = mechanism.Create(FailureMechanismType.StabilityStoneRevetment, registry);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);
            AddEntitiesForForeshoreProfiles(mechanism.ForeshoreProfiles, entity, registry);

            entity.CalculationGroupEntity = mechanism.WaveConditionsCalculationGroup.Create(registry, 0);

            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<StabilityStoneCoverFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (var failureMechanismSectionResult in sectionResults)
            {
                var sectionResultEntity = failureMechanismSectionResult.Create(registry);
                var section = registry.Get(failureMechanismSectionResult.Section);
                section.StabilityStoneCoverSectionResultEntities.Add(sectionResultEntity);
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