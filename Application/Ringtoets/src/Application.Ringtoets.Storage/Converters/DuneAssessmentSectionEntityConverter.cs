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
    /// Converter for <see cref="DuneAssessmentSectionEntity"/> to <see cref="DuneAssessmentSection"/> 
    /// and <see cref="DuneAssessmentSection"/> to <see cref="DuneAssessmentSectionEntity"/>.
    /// </summary>
    public class DuneAssessmentSectionEntityConverter : IEntityConverter<DuneAssessmentSection, DuneAssessmentSectionEntity>
    {
        /// <summary>
        /// Converts <paramref name="entity"/> to <see cref="DuneAssessmentSection"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneAssessmentSectionEntity"/> to convert.</param>
        /// <returns>A new instance of <see cref="DuneAssessmentSection"/>, based on the properties of <paramref name="entity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        public DuneAssessmentSection ConvertEntityToModel(DuneAssessmentSectionEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            var duneAssessmentSection = new DuneAssessmentSection
            {
                StorageId = entity.DuneAssessmentSectionEntityId,
                Name = entity.Name ?? string.Empty,
                FailureMechanismContribution =
                {
                    Norm = entity.Norm
                }
            };

            return duneAssessmentSection;
        }

        /// <summary>
        /// Converts <paramref name="modelObject"/> to <paramref name="entity"/>.
        /// </summary>
        /// <param name="modelObject">The <see cref="DuneAssessmentSection"/> to convert.</param>
        /// <param name="entity">A reference to the <see cref="DuneAssessmentSectionEntity"/> to be saved.</param>
        /// <exception cref="ArgumentNullException">Thrown when: <list type="bullet">
        /// <item><paramref name="modelObject"/> is <c>null</c></item>
        /// <item><paramref name="entity"/> is <c>null</c>.</item>
        /// </list></exception>
        public void ConvertModelToEntity(DuneAssessmentSection modelObject, DuneAssessmentSectionEntity entity)
        {
            if (modelObject == null)
            {
                throw new ArgumentNullException("modelObject");
            }
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            entity.DuneAssessmentSectionEntityId = modelObject.StorageId;
            entity.Name = modelObject.Name;
            entity.Norm = modelObject.FailureMechanismContribution.Norm;
        }
    }
}