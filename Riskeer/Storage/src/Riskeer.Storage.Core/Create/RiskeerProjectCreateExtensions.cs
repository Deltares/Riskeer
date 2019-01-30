﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// Extension methods for <see cref="RiskeerProject"/> related to creating database entities.
    /// </summary>
    internal static class RiskeerProjectCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="ProjectEntity"/> based on the information of the <see cref="RiskeerProject"/>.
        /// </summary>
        /// <param name="project">The project to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="ProjectEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static ProjectEntity Create(this RiskeerProject project, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            var entity = new ProjectEntity
            {
                Description = project.Description.DeepClone()
            };

            AddEntitiesForAssessmentSections(project, entity, registry);

            return entity;
        }

        private static void AddEntitiesForAssessmentSections(RiskeerProject project, ProjectEntity entity, PersistenceRegistry registry)
        {
            for (var index = 0; index < project.AssessmentSections.Count; index++)
            {
                AssessmentSection assessmentSection = project.AssessmentSections[index];
                entity.AssessmentSectionEntities.Add(assessmentSection.Create(registry, index));
            }
        }
    }
}