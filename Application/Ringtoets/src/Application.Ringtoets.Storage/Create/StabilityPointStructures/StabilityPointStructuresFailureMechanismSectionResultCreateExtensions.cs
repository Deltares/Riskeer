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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.StabilityPointStructures.Data;

namespace Application.Ringtoets.Storage.Create.StabilityPointStructures
{
    /// <summary>
    /// Extension methods for <see cref="StabilityPointStructuresFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="StabilityPointStructuresSectionResultEntity"/>.
    /// </summary>
    internal static class StabilityPointStructuresFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StabilityPointStructuresSectionResultEntity"/> based on the information of the <see cref="StabilityPointStructuresFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="StabilityPointStructuresSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StabilityPointStructuresSectionResultEntity Create(this StabilityPointStructuresFailureMechanismSectionResult result, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var sectionResultEntity = new StabilityPointStructuresSectionResultEntity
            {
                LayerTwoA = result.AssessmentLayerTwoA.ToNaNAsNull(),
                LayerThree = result.AssessmentLayerThree.Value.ToNaNAsNull()
            };

            return sectionResultEntity;
        }
    }
}