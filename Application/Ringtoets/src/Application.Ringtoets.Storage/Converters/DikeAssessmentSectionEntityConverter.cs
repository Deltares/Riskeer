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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Integration.Data;

namespace Application.Ringtoets.Storage.Converters
{
    /// <summary>
    /// Converter for <see cref="DikeAssessmentSectionEntity"/> to <see cref="DikeAssessmentSection"/> 
    /// and <see cref="DikeAssessmentSection"/> to <see cref="DikeAssessmentSectionEntity"/>.
    /// </summary>
    public class DikeAssessmentSectionEntityConverter : IEntityConverter<DikeAssessmentSection, DikeAssessmentSectionEntity>
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="DikeAssessmentSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DikeAssessmentSectionEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="DikeAssessmentSection"/>, based on the properties of <paramref name="entity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public DikeAssessmentSection ConvertEntityToModel(DikeAssessmentSectionEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var dikeAssessmentSection = new DikeAssessmentSection
            {
                StorageId = entity.DikeAssessmentSectionEntityId,
                Name = entity.Name
            };

            if (entity.Norm != null)
            {
                dikeAssessmentSection.FailureMechanismContribution.Norm = entity.Norm.Value;
            }

            return dikeAssessmentSection;
        }

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="DikeAssessmentSection"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="DikeAssessmentSectionEntity"/> to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="modelObject"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// </list></exception>
        public void ConvertModelToEntity(DikeAssessmentSection modelObject, DikeAssessmentSectionEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.DikeAssessmentSectionEntityId = modelObject.StorageId;
            entity.Name = modelObject.Name;
            entity.Norm = modelObject.FailureMechanismContribution.Norm;
        }
    }
}
