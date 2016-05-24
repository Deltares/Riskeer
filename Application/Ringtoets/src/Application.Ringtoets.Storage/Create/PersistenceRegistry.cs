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

using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Utils;

using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// This class can be used to keep track of create and update operations on a database.
    /// This information can be used to reuse objects. When all operations have been performed,
    /// then the collected information can be used to transfer the ids assigned to the created
    /// database instances back to the data model or to clean up orphans.
    /// </summary>
    internal class PersistenceRegistry
    {
        private readonly Dictionary<ProjectEntity, Project> projects = new Dictionary<ProjectEntity, Project>(new ReferenceEqualityComparer<ProjectEntity>());
        private readonly Dictionary<AssessmentSectionEntity, AssessmentSection> assessmentSections = new Dictionary<AssessmentSectionEntity, AssessmentSection>(new ReferenceEqualityComparer<AssessmentSectionEntity>());
        private readonly Dictionary<FailureMechanismEntity, IFailureMechanism> failureMechanisms = new Dictionary<FailureMechanismEntity, IFailureMechanism>(new ReferenceEqualityComparer<FailureMechanismEntity>());
        private readonly Dictionary<FailureMechanismSectionEntity, FailureMechanismSection> failureMechanismSections = new Dictionary<FailureMechanismSectionEntity, FailureMechanismSection>();
        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicLocations = new Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>(new ReferenceEqualityComparer<HydraulicLocationEntity>());
        private readonly Dictionary<CalculationGroupEntity, CalculationGroup> calculationGroups = new Dictionary<CalculationGroupEntity, CalculationGroup>(new ReferenceEqualityComparer<CalculationGroupEntity>());
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> stochasticSoilModels = new Dictionary<StochasticSoilModelEntity, StochasticSoilModel>(new ReferenceEqualityComparer<StochasticSoilModelEntity>());
        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> stochasticSoilProfiles = new Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile>(new ReferenceEqualityComparer<StochasticSoilProfileEntity>());
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = new Dictionary<SoilProfileEntity, PipingSoilProfile>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly Dictionary<SoilLayerEntity, PipingSoilLayer> soilLayers = new Dictionary<SoilLayerEntity, PipingSoilLayer>(new ReferenceEqualityComparer<SoilLayerEntity>());
        private readonly Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine> surfaceLines = new Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine>(new ReferenceEqualityComparer<SurfaceLineEntity>());
        private readonly Dictionary<SurfaceLinePointEntity, Point3D> surfaceLinePoints = new Dictionary<SurfaceLinePointEntity, Point3D>(new ReferenceEqualityComparer<SurfaceLinePointEntity>());
        private readonly Dictionary<CharacteristicPointEntity, Point3D> characteristicPoints = new Dictionary<CharacteristicPointEntity, Point3D>(new ReferenceEqualityComparer<CharacteristicPointEntity>());

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="FailureMechanismSection"/> which needed to registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(FailureMechanismSectionEntity entity, FailureMechanismSection model)
        {
            Register(failureMechanismSections, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="CalculationGroupEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="CalculationGroup"/> which needed to registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        public void Register(CalculationGroupEntity entity, CalculationGroup model)
        {
            Register(calculationGroups, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="Project"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(ProjectEntity entity, Project model)
        {
            Register(projects, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="AssessmentSection"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(AssessmentSectionEntity entity, AssessmentSection model)
        {
            Register(assessmentSections, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(HydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            Register(hydraulicLocations, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="IFailureMechanism"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(FailureMechanismEntity entity, IFailureMechanism model)
        {
            Register(failureMechanisms, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="StochasticSoilModel"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(StochasticSoilModelEntity entity, StochasticSoilModel model)
        {
            Register(stochasticSoilModels, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(StochasticSoilProfileEntity entity, StochasticSoilProfile model)
        {
            Register(stochasticSoilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="PipingSoilProfile"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SoilProfileEntity entity, PipingSoilProfile model)
        {
            Register(soilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilLayerEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="PipingSoilLayer"/> which needed to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SoilLayerEntity entity, PipingSoilLayer model)
        {
            Register(soilLayers, entity, model);
        }

        /// <summary>
        /// Checks whether a create or update operations has been registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="SoilProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was created before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingSoilProfile model)
        {
            return ContainsValue(soilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SoilProfileEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="SoilProfileEntity"/> for which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains"/> to find out whether a create operation has
        /// been registered for <paramref name="model"/>.</remarks>
        internal SoilProfileEntity Get(PipingSoilProfile model)
        {
            return Get(soilProfiles, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> that was registered.</param>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> which needed 
        /// to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SurfaceLineEntity entity, RingtoetsPipingSurfaceLine model)
        {
            Register(surfaceLines, entity, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLinePointEntity"/> that was registered.</param>
        /// <param name="model">The surfaceline geometry <see cref="Point3D"/> corresponding
        /// the registered database entity.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(SurfaceLinePointEntity entity, Point3D model)
        {
            Register(surfaceLinePoints, entity, model);
        }

        /// <summary>
        /// Obtains the <see cref="SurfaceLinePointEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The surfaceline geometry <see cref="Point3D"/> for which
        /// a create or update operation has been registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains"/> to find out whether a create or update
        /// operation has been registered for <paramref name="model"/>.</remarks>
        internal SurfaceLinePointEntity GetSurfaceLinePoint(Point3D model)
        {
            return Get(surfaceLinePoints, model);
        }

        /// <summary>
        /// Registers a create or update operation for <paramref name="model"/> and the
        /// <paramref name="entity"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="CharacteristicPointEntity"/> that was registered.</param>
        /// <param name="model">The surfaceline geometry <see cref="Point3D"/> corresponding
        /// to the characteristic point data being registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(CharacteristicPointEntity entity, Point3D model)
        {
            Register(characteristicPoints, entity, model);
        }

        /// <summary>
        /// Transfer ids from the created entities to the domain model objects' property.
        /// </summary>
        internal void TransferIds()
        {
            foreach (var entity in projects.Keys)
            {
                projects[entity].StorageId = entity.ProjectEntityId;
            }

            foreach (var entity in failureMechanisms.Keys)
            {
                failureMechanisms[entity].StorageId = entity.FailureMechanismEntityId;
            }

            foreach (var entity in failureMechanismSections.Keys)
            {
                failureMechanismSections[entity].StorageId = entity.FailureMechanismSectionEntityId;
            }

            foreach (var entity in assessmentSections.Keys)
            {
                assessmentSections[entity].StorageId = entity.AssessmentSectionEntityId;
            }

            foreach (var entity in hydraulicLocations.Keys)
            {
                hydraulicLocations[entity].StorageId = entity.HydraulicLocationEntityId;
            }

            foreach (var entity in calculationGroups.Keys)
            {
                calculationGroups[entity].StorageId = entity.CalculationGroupEntityId;
            }

            foreach (var entity in stochasticSoilModels.Keys)
            {
                stochasticSoilModels[entity].StorageId = entity.StochasticSoilModelEntityId;
            }

            foreach (var entity in stochasticSoilProfiles.Keys)
            {
                stochasticSoilProfiles[entity].StorageId = entity.StochasticSoilProfileEntityId;
            }

            foreach (var entity in soilProfiles.Keys)
            {
                soilProfiles[entity].StorageId = entity.SoilProfileEntityId;
            }

            foreach (var entity in soilLayers.Keys)
            {
                soilLayers[entity].StorageId = entity.SoilLayerEntityId;
            }

            foreach (var entity in surfaceLines.Keys)
            {
                surfaceLines[entity].StorageId = entity.SurfaceLineEntityId;
            }

            foreach (var entity in surfaceLinePoints.Keys)
            {
                surfaceLinePoints[entity].StorageId = entity.SurfaceLinePointEntityId;
            }

            // CharacteristicPoints do not really have a 'identity' within the object-model.
            // As such, no need to copy StorageId. This is already covered by surfaceLinePoints.
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
                .Except(projects.Keys);
            projectEntities.RemoveRange(projectEntitiesToRemove);

            var assessmentSectionEntities = dbContext.AssessmentSectionEntities;
            var assessmentSectionEntitiesToRemove = assessmentSectionEntities
                .Local
                .Where(entity => entity.AssessmentSectionEntityId > 0)
                .Except(assessmentSections.Keys);
            assessmentSectionEntities.RemoveRange(assessmentSectionEntitiesToRemove);

            var failureMechanismEntities = dbContext.FailureMechanismEntities;
            var failureMechanismEntitiesToRemove = failureMechanismEntities
                .Local
                .Where(entity => entity.FailureMechanismEntityId > 0)
                .Except(failureMechanisms.Keys);
            failureMechanismEntities.RemoveRange(failureMechanismEntitiesToRemove);

            var failureMechanismSectionEntities = dbContext.FailureMechanismSectionEntities;
            var failureMechanismSectionEntitiesToRemove = failureMechanismSectionEntities
                .Local
                .Where(entity => entity.FailureMechanismSectionEntityId > 0)
                .Except(failureMechanismSections.Keys);
            failureMechanismSectionEntities.RemoveRange(failureMechanismSectionEntitiesToRemove);

            var hydraulicLocationEntities = dbContext.HydraulicLocationEntities;
            var hydraulicLocationEntitiesToRemove = hydraulicLocationEntities
                .Local
                .Where(entity => entity.HydraulicLocationEntityId > 0)
                .Except(hydraulicLocations.Keys);
            hydraulicLocationEntities.RemoveRange(hydraulicLocationEntitiesToRemove);

            var calculationGroupEntities = dbContext.CalculationGroupEntities;
            var calculationGroupEntitiesToRemove = calculationGroupEntities
                .Local
                .Where(entity => entity.CalculationGroupEntityId > 0)
                .Except(calculationGroups.Keys);
            calculationGroupEntities.RemoveRange(calculationGroupEntitiesToRemove);

            var stochasticSoilModelEntities = dbContext.StochasticSoilModelEntities;
            var stochasticSoilModelEntitiesToRemove = stochasticSoilModelEntities
                .Local
                .Where(entity => entity.StochasticSoilModelEntityId > 0)
                .Except(stochasticSoilModels.Keys);
            stochasticSoilModelEntities.RemoveRange(stochasticSoilModelEntitiesToRemove);

            var stochasticSoilProfileEntities = dbContext.StochasticSoilProfileEntities;
            var stochasticSoilProfileEntitiesToRemove = stochasticSoilProfileEntities
                .Local
                .Where(entity => entity.StochasticSoilProfileEntityId > 0)
                .Except(stochasticSoilProfiles.Keys);
            stochasticSoilProfileEntities.RemoveRange(stochasticSoilProfileEntitiesToRemove);

            var soilProfileEntities = dbContext.SoilProfileEntities;
            var soilProfileEntitiesToRemove = soilProfileEntities
                .Local
                .Where(entity => entity.SoilProfileEntityId > 0)
                .Except(soilProfiles.Keys);
            soilProfileEntities.RemoveRange(soilProfileEntitiesToRemove);

            var soilLayerEntities = dbContext.SoilLayerEntities;
            var soilLayerEntitiesToRemove = soilLayerEntities
                .Local
                .Where(entity => entity.SoilLayerEntityId > 0)
                .Except(soilLayers.Keys);
            soilLayerEntities.RemoveRange(soilLayerEntitiesToRemove);

            var surfaceLineEntities = dbContext.SurfaceLineEntities;
            var surfaceLineEntitiesToRemove = surfaceLineEntities
                .Local
                .Where(entity => entity.SurfaceLineEntityId > 0)
                .Except(surfaceLines.Keys);
            surfaceLineEntities.RemoveRange(surfaceLineEntitiesToRemove);

            var surfaceLinePointEntities = dbContext.SurfaceLinePointEntities;
            var surfaceLinePointEntitiesToRemove = surfaceLinePointEntities
                .Local
                .Where(entity => entity.SurfaceLinePointEntityId > 0)
                .Except(surfaceLinePoints.Keys);
            surfaceLinePointEntities.RemoveRange(surfaceLinePointEntitiesToRemove);

            var characteristicPointEntities = dbContext.CharacteristicPointEntities;
            var characteristicPointEntitiesToRemove = characteristicPointEntities
                .Local
                .Where(entity => entity.CharacteristicPointEntityId > 0)
                .Except(characteristicPoints.Keys);
            characteristicPointEntities.RemoveRange(characteristicPointEntitiesToRemove);
        }

        private bool ContainsValue<T, U>(Dictionary<T, U> collection, U model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.ContainsValue(model);
        }

        private void Register<T, U>(Dictionary<T, U> collection, T entity, U model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            collection[entity] = model;
        }

        private T Get<T, U>(Dictionary<T, U> collection, U model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.Keys.Single(k => ReferenceEquals(collection[k], model));
        }
    }
}