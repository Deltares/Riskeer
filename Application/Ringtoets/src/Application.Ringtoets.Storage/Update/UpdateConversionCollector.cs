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
using System.Collections.Generic;
using System.Linq;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Class that can be used to keep track of data model objects which were potentially created or updated during an update
    /// operation. Can be used to reuse objects when assigning an already created domain model object or to remove entities
    /// from the database which were not potentialy modified.
    /// </summary>
    public class UpdateConversionCollector : CreateConversionCollector
    {
        private readonly HashSet<ProjectEntity> projects = new HashSet<ProjectEntity>(new ReferenceEqualityComparer<ProjectEntity>());
        private readonly HashSet<AssessmentSectionEntity> assessmentSections = new HashSet<AssessmentSectionEntity>(new ReferenceEqualityComparer<AssessmentSectionEntity>());
        private readonly HashSet<FailureMechanismEntity> failureMechanisms = new HashSet<FailureMechanismEntity>(new ReferenceEqualityComparer<FailureMechanismEntity>());
        private readonly HashSet<FailureMechanismSectionEntity> failureMechanismSections = new HashSet<FailureMechanismSectionEntity>(new ReferenceEqualityComparer<FailureMechanismSectionEntity>());
        private readonly HashSet<HydraulicLocationEntity> hydraulicLocations = new HashSet<HydraulicLocationEntity>(new ReferenceEqualityComparer<HydraulicLocationEntity>());
        private readonly HashSet<StochasticSoilModelEntity> stochasticSoilModels = new HashSet<StochasticSoilModelEntity>(new ReferenceEqualityComparer<StochasticSoilModelEntity>());
        private readonly HashSet<StochasticSoilProfileEntity> stochasticSoilProfiles = new HashSet<StochasticSoilProfileEntity>(new ReferenceEqualityComparer<StochasticSoilProfileEntity>());
        private readonly HashSet<SoilProfileEntity> soilProfiles = new HashSet<SoilProfileEntity>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly HashSet<SoilLayerEntity> soilLayers = new HashSet<SoilLayerEntity>(new ReferenceEqualityComparer<SoilLayerEntity>());

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(ProjectEntity entity)
        {
            Update(entity, projects);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(AssessmentSectionEntity entity)
        {
            Update(entity, assessmentSections);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(FailureMechanismEntity entity)
        {
            Update(entity, failureMechanisms);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        public void Update(FailureMechanismSectionEntity entity)
        {
            Update(entity, failureMechanismSections);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(HydraulicLocationEntity entity)
        {
            Update(entity, hydraulicLocations);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(StochasticSoilModelEntity entity)
        {
            Update(entity, stochasticSoilModels);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(StochasticSoilProfileEntity entity)
        {
            Update(entity, stochasticSoilProfiles);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(SoilProfileEntity entity)
        {
            Update(entity, soilProfiles);
        }

        /// <summary>
        /// Registers an update operation for <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SoilLayerEntity"/> that was updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c></exception>
        internal void Update(SoilLayerEntity entity)
        {
            Update(entity, soilLayers);
        }

        /// <summary>
        /// Removes all the entities for which no update operation was registered from the <paramref name="dbContext"/>.
        /// </summary>
        /// <param name="dbContext">The <see cref="IRingtoetsEntities"/> from which to remove the entities.</param>
        internal void RemoveUntouched(IRingtoetsEntities dbContext)
        {
            var projectEntities = dbContext.ProjectEntities;
            var projectEntitiesToRemove = projectEntities
                .Local
                .Where(entity => entity.ProjectEntityId > 0)
                .Except(projects);
            projectEntities.RemoveRange(projectEntitiesToRemove);

            var assessmentSectionEntities = dbContext.AssessmentSectionEntities;
            var assessmentSectionEntitiesToRemove = assessmentSectionEntities
                .Local
                .Where(entity => entity.AssessmentSectionEntityId > 0)
                .Except(assessmentSections);
            assessmentSectionEntities.RemoveRange(assessmentSectionEntitiesToRemove);

            var failureMechanismEntities = dbContext.FailureMechanismEntities;
            var failureMechanismEntitiesToRemove = failureMechanismEntities
                .Local
                .Where(entity => entity.FailureMechanismEntityId > 0)
                .Except(failureMechanisms);
            failureMechanismEntities.RemoveRange(failureMechanismEntitiesToRemove);

            var failureMechanismSectionEntities = dbContext.FailureMechanismSectionEntities;
            var failureMechanismSectionEntitiesToRemove = failureMechanismSectionEntities
                .Local
                .Where(entity => entity.FailureMechanismSectionEntityId > 0)
                .Except(failureMechanismSections);
            failureMechanismSectionEntities.RemoveRange(failureMechanismSectionEntitiesToRemove);

            var hydraulicLocationEntities = dbContext.HydraulicLocationEntities;
            var hydraulicLocationEntitiesToRemove = hydraulicLocationEntities
                .Local
                .Where(entity => entity.HydraulicLocationEntityId > 0)
                .Except(hydraulicLocations);
            hydraulicLocationEntities.RemoveRange(hydraulicLocationEntitiesToRemove);

            var stochasticSoilModelEntities = dbContext.StochasticSoilModelEntities;
            var stochasticSoilModelEntitiesToRemove = stochasticSoilModelEntities
                .Local
                .Where(entity => entity.StochasticSoilModelEntityId > 0)
                .Except(stochasticSoilModels);
            stochasticSoilModelEntities.RemoveRange(stochasticSoilModelEntitiesToRemove);

            var stochasticSoilProfileEntities = dbContext.StochasticSoilProfileEntities;
            var stochasticSoilProfileEntitiesToRemove = stochasticSoilProfileEntities
                .Local
                .Where(entity => entity.StochasticSoilProfileEntityId > 0)
                .Except(stochasticSoilProfiles);
            stochasticSoilProfileEntities.RemoveRange(stochasticSoilProfileEntitiesToRemove);

            var soilProfileEntities = dbContext.SoilProfileEntities;
            var soilProfileEntitiesToRemove = soilProfileEntities
                .Local
                .Where(entity => entity.SoilProfileEntityId > 0)
                .Except(soilProfiles);
            soilProfileEntities.RemoveRange(soilProfileEntitiesToRemove);

            var soilLayerEntities = dbContext.SoilLayerEntities;
            var soilLayerEntitiesToRemove = soilLayerEntities
                .Local
                .Where(entity => entity.SoilLayerEntityId > 0)
                .Except(soilLayers);
            soilLayerEntities.RemoveRange(soilLayerEntitiesToRemove);
        }

        private void Update<T>(T entity, HashSet<T> collection)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            collection.Add(entity);
        }
    }
}