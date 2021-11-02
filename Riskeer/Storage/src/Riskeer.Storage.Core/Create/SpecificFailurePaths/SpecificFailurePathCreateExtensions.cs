// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Core.Common.Util.Extensions;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.Data.FailurePath;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.SpecificFailurePaths
{
    /// <summary>
    /// Extension methods for <see cref="SpecificFailurePath"/> related to creating
    /// a <see cref="SpecificFailurePathEntity"/>.
    /// </summary>
    internal static class SpecificFailurePathCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SpecificFailurePathEntity"/> based on the information of the <see cref="SpecificFailurePath"/>.
        /// </summary>
        /// <param name="specificFailurePath">The structure to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">The index at which <paramref name="specificFailurePath"/> resides within its parent.</param>
        /// <returns>A new <see cref="SpecificFailurePathEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static SpecificFailurePathEntity Create(this SpecificFailurePath specificFailurePath, PersistenceRegistry registry, int order)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(specificFailurePath))
            {
                return registry.Get(specificFailurePath);
            }

            var entity = new SpecificFailurePathEntity
            {
                Name = specificFailurePath.Name.DeepClone(),
                Order = order,
                N = specificFailurePath.Input.N,
                IsRelevant = Convert.ToByte(specificFailurePath.IsRelevant),
                InputComments = specificFailurePath.InputComments.Body.DeepClone(),
                OutputComments = specificFailurePath.OutputComments.Body.DeepClone(),
                NotRelevantComments = specificFailurePath.NotRelevantComments.Body.DeepClone(),
                FailureMechanismSectionCollectionSourcePath = specificFailurePath.FailureMechanismSectionSourcePath.DeepClone()
            };

            specificFailurePath.AddEntitiesForFailureMechanismSections(registry, entity);

            registry.Register(entity, specificFailurePath);

            return entity;
        }

        /// <summary>
        /// Creates <see cref="FailureMechanismSectionEntity"/> instances based on the information of the <see cref="FailureMechanismBase"/>.
        /// </summary>
        /// <param name="specificFailurePath">The failure mechanism to create a database failure mechanism section entities for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to which to add the created entities.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// </list>
        /// </exception>
        internal static void AddEntitiesForFailureMechanismSections(this IFailurePath specificFailurePath, PersistenceRegistry registry, SpecificFailurePathEntity entity)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            foreach (FailureMechanismSection failureMechanismSection in specificFailurePath.Sections)
            {
                entity.FailureMechanismSectionEntities.Add(failureMechanismSection.Create(registry));
            }
        }
    }
}