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
using Core.Common.Util.Extensions;
using Ringtoets.Common.Data.FailureMechanism;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="FailureMechanismBase"/> related to creating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class FailureMechanismCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="FailureMechanismEntity"/> based on the information of the <see cref="FailureMechanismBase"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database entity for.</param>
        /// <param name="type">The type of the failure mechanism that is being created.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="FailureMechanismEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static FailureMechanismEntity Create(this IFailureMechanism mechanism, FailureMechanismType type, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new FailureMechanismEntity
            {
                FailureMechanismType = (short) type,
                IsRelevant = Convert.ToByte(mechanism.IsRelevant),
                InputComments = mechanism.InputComments.Body.DeepClone(),
                OutputComments = mechanism.OutputComments.Body.DeepClone(),
                NotRelevantComments = mechanism.NotRelevantComments.Body.DeepClone(),
                FailureMechanismSectionCollectionSourcePath = mechanism.FailureMechanismSectionSourcePath.DeepClone()
            };

            mechanism.AddEntitiesForFailureMechanismSections(registry, entity);

            return entity;
        }

        /// <summary>
        /// Creates <see cref="FailureMechanismSectionEntity"/> instances based on the information of the <see cref="FailureMechanismBase"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to create a database failure mechanism section entities for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> to which to add the created entities.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// </list>
        /// </exception>
        internal static void AddEntitiesForFailureMechanismSections(this IFailureMechanism mechanism, PersistenceRegistry registry, FailureMechanismEntity entity)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            foreach (FailureMechanismSection failureMechanismSection in mechanism.Sections)
            {
                entity.FailureMechanismSectionEntities.Add(failureMechanismSection.Create(registry));
            }
        }
    }
}