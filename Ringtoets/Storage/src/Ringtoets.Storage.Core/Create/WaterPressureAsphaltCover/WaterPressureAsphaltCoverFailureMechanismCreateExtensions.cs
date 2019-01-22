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
using Ringtoets.Integration.Data.StandAlone;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.WaterPressureAsphaltCover
{
    /// <summary>
    /// Extension methods for <see cref="WaterPressureAsphaltCoverFailureMechanism"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class WaterPressureAsphaltCoverFailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="WaterPressureAsphaltCoverFailureMechanism"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this WaterPressureAsphaltCoverFailureMechanism mechanism, PersistenceRegistry registry)
        {
            FailureMechanismEntity entity = mechanism.Create(FailureMechanismType.WaterOverpressureAsphaltRevetment, registry);
            AddEntitiesForSectionResults(mechanism.SectionResults, registry);

            return entity;
        }

        private static void AddEntitiesForSectionResults(
            IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult> sectionResults,
            PersistenceRegistry registry)
        {
            foreach (WaterPressureAsphaltCoverFailureMechanismSectionResult failureMechanismSectionResult in sectionResults)
            {
                WaterPressureAsphaltCoverSectionResultEntity sectionResultEntity = failureMechanismSectionResult.Create();
                FailureMechanismSectionEntity section = registry.Get(failureMechanismSectionResult.Section);
                section.WaterPressureAsphaltCoverSectionResultEntities.Add(sectionResultEntity);
            }
        }
    }
}