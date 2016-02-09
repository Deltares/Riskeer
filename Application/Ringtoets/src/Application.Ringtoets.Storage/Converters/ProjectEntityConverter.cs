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
using Core.Common.Base.Data;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Converter for <see cref="ProjectEntity"/> to <see cref="Project"/> 
    /// and <see cref="Project"/> to <see cref="ProjectEntity"/>.
    /// </summary>
    public class ProjectEntityConverter : IEntityConverter<Project, ProjectEntity>
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="Project"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="Project"/>, based on the properties of <paramref name="entity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public Project ConvertEntityToModel(ProjectEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var project = new Project
            {
                StorageId = entity.ProjectEntityId,
                Description = entity.Description
            };

            return project;
        }

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="Project"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="ProjectEntity"/> to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="modelObject"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// </list></exception>
        public void ConvertModelToEntity(Project modelObject, ProjectEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.ProjectEntityId = modelObject.StorageId;
            entity.Description = modelObject.Description;
            entity.LastUpdated = new DateTime().Ticks;
        }
    }
}