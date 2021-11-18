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
using System.Linq;
using Riskeer.Integration.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Exceptions;
using Riskeer.Storage.Core.Properties;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="RiskeerProject"/> based on the
    /// <see cref="ProjectEntity"/>.
    /// </summary>
    internal static class ProjectEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="ProjectEntity"/> and use the information to construct a <see cref="RiskeerProject"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> to create <see cref="RiskeerProject"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="RiskeerProject"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        /// <exception cref="EntityReadException">Thrown when <paramref name="entity"/> could not be read successfully.</exception>
        internal static RiskeerProject Read(this ProjectEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            int nrOfAssessmentSectionEntities = entity.AssessmentSectionEntities.Count;
            if (nrOfAssessmentSectionEntities > 1)
            {
                throw new EntityReadException(Resources.ProjectEntityReadExtensions_Read_ProjectEntity_contains_more_than_one_AssessmentSection);
            }

            if (nrOfAssessmentSectionEntities == 0)
            {
                throw new EntityReadException(Resources.ProjectEntityReadExtensions_Read_ProjectEntity_contains_no_AssessmentSection);
            }

            AssessmentSection assessmentSection = ReadAssessmentSection(entity, collector);
            var project = new RiskeerProject(assessmentSection)
            {
                Description = entity.Description
            };
            
            return project;
        }

        private static AssessmentSection ReadAssessmentSection(ProjectEntity entity, ReadConversionCollector collector)
        {
            return entity.AssessmentSectionEntities
                         .Single()
                         .Read(collector);
        }
    }
}