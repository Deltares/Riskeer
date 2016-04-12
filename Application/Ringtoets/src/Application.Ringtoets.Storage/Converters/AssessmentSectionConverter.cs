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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Converter for <see cref="AssessmentSectionEntity"/> to <see cref="AssessmentSection"/> 
    /// and <see cref="AssessmentSection"/> to <see cref="AssessmentSectionEntity"/>.
    /// </summary>
    public class AssessmentSectionConverter : IEntityConverter<AssessmentSection, AssessmentSectionEntity>
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="AssessmentSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="AssessmentSection"/>, based on the properties of <paramref name="entity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public AssessmentSection ConvertEntityToModel(AssessmentSectionEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            var assessmentSection = new AssessmentSection(GetAssessmentSectionComposition(entity.Composition));
            assessmentSection.StorageId = entity.AssessmentSectionEntityId;
            assessmentSection.Name = entity.Name ?? string.Empty;
            assessmentSection.FailureMechanismContribution.Norm = entity.Norm;

            if (entity.HydraulicDatabaseLocation != null && entity.HydraulicDatabaseVersion != null)
            {
                assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    FilePath = entity.HydraulicDatabaseLocation,
                    Version = entity.HydraulicDatabaseVersion
                };
            }

            return assessmentSection;
        }

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="AssessmentSection"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="AssessmentSectionEntity"/> to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="modelObject"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// </list></exception>
        public void ConvertModelToEntity(AssessmentSection modelObject, AssessmentSectionEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.AssessmentSectionEntityId = modelObject.StorageId;
            entity.Name = modelObject.Name;
            entity.Composition = (short) modelObject.Composition;
            entity.Norm = modelObject.FailureMechanismContribution.Norm;

            if (modelObject.HydraulicBoundaryDatabase != null)
            {
                entity.HydraulicDatabaseLocation = modelObject.HydraulicBoundaryDatabase.FilePath;
                entity.HydraulicDatabaseVersion = modelObject.HydraulicBoundaryDatabase.Version;
            }
        }

        private AssessmentSectionComposition GetAssessmentSectionComposition(short composition)
        {
            return (AssessmentSectionComposition) composition;
        }
    }
}