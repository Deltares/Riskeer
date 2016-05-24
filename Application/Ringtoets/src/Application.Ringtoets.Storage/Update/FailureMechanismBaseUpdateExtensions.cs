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
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;
using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="FailureMechanismBase"/> related to updating a <see cref="FailureMechanismEntity"/>.
    /// </summary>
    internal static class FailureMechanismBaseUpdateExtensions
    {
        /// <summary>
        /// Updates <see cref="FailureMechanismSectionEntity"/> instances of a <see cref="FailureMechanismEntity"/>
        /// based on the sections defined on the <see cref="FailureMechanismBase"/>.
        /// </summary>
        /// <param name="mechanism">The failure mechanism to update the database failure mechanism section entities for.</param>
        /// <param name="collector">The object keeping track of update operations.</param>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> for which to update the assessment section entities.</param>
        /// <param name="context">The context to obtain the existing entities from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="collector"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list>
        /// </exception>
        internal static void UpdateFailureMechanismSections(this IFailureMechanism mechanism, PersistenceRegistry collector, FailureMechanismEntity entity, IRingtoetsEntities context)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            foreach (var failureMechanismSection in mechanism.Sections)
            {
                if (failureMechanismSection.IsNew())
                {
                    entity.FailureMechanismSectionEntities.Add(failureMechanismSection.Create(collector));
                }
                else
                {
                    failureMechanismSection.Update(collector, context);
                }
            }
        }

        internal static FailureMechanismEntity GetSingleFailureMechanism(this IFailureMechanism mechanism, IRingtoetsEntities context)
        {
            try
            {
                return context.FailureMechanismEntities.Single(fme => fme.FailureMechanismEntityId == mechanism.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                throw new EntityNotFoundException(string.Format(Resources.Error_Entity_Not_Found_0_1, typeof(FailureMechanismEntity).Name, mechanism.StorageId), exception);
            }
        }
    }
}