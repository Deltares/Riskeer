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
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="RingtoetsProject"/> related to updating a <see cref="ProjectEntity"/>.
    /// </summary>
    internal static class RingtoetsProjectUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="ProjectEntity"/> in the database based on the information of the 
        /// <see cref="RingtoetsProject"/>.
        /// </summary>
        /// <param name="project">The project to update the database entity for.</param>
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntityNotFoundException">When <paramref name="project"/>
        /// does not have a corresponding entity in the database.</exception>
        internal static void Update(this RingtoetsProject project, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }

            ProjectEntity entity = project.GetCorrespondingEntity(
                context.ProjectEntities,
                o => o.ProjectEntityId);

            entity.Description = project.Description;

            foreach (var result in project.AssessmentSections)
            {
                if (result.IsNew())
                {
                    entity.AssessmentSectionEntities.Add(result.Create(registry));
                }
                else
                {
                    result.Update(registry, context);
                }
            }

            registry.Register(entity, project);
        }
    }
}