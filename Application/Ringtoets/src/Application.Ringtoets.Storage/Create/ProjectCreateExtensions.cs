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
using Core.Common.Base.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.DbContext
{
    /// <summary>
    /// Extension methods for <see cref="Project"/> related to creating database entities.
    /// </summary>
    public static class ProjectCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ProjectEntity"/> based on the information of the <see cref="Project"/>.
        /// </summary>
        /// <param name="project">The project to create a database entity for.</param>
        /// <param name="collector">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        public static ProjectEntity Create(this Project project, CreateConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }

            var entity = new ProjectEntity
            {
                Description = project.Description
            };

            CreateAssessmentSections(project, entity, collector);

            collector.Create(entity, project);
            return entity;
        }

        private static void CreateAssessmentSections(Project project, ProjectEntity entity, CreateConversionCollector collector)
        {
            foreach (var result in project.Items.OfType<AssessmentSection>())
            {
                entity.AssessmentSectionEntities.Add(result.Create(collector));
            }
        }
    }
}