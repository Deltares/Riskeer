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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;

using Ringtoets.Common.Data.FailureMechanism;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="FailureMechanismSection"/> related to updating a
    /// <see cref="FailureMechanismSectionEntity"/>.
    /// </summary>
    internal static class FailureMechanismSectionUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="AssessmentSectionEntity"/> in the database based on the information of the 
        /// <see cref="FailureMechanismSection"/>.
        /// </summary>
        /// <param name="section">The section to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="section"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this FailureMechanismSection section, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            FailureMechanismSectionEntity entity = section.GetCorrespondingEntity(
                context.FailureMechanismSectionEntities, 
                o => o.FailureMechanismSectionEntityId);
            entity.Name = section.Name;

            UpdateGeometry(section, entity);

            registry.Register(entity, section);
        }

        private static void UpdateGeometry(FailureMechanismSection section, FailureMechanismSectionEntity entity)
        {
            byte[] newBinaryData = new Point2DBinaryConverter().ToBytes(section.Points);
            if(!BinaryDataEqualityHelper.AreEqual(entity.FailureMechanismSectionPointData, newBinaryData))
            {
                entity.FailureMechanismSectionPointData = newBinaryData;
            }
        }
    }
}