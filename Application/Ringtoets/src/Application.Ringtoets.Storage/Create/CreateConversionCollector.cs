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
using Core.Common.Utils;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// This class can be used to keep track of create operations on a database. This information can used to reuse previously created
    /// objects. When all operations have been performed, then the collected information can be used to transfer the ids assigned to 
    /// the created database instances back to the data model.
    /// </summary>
    public class CreateConversionCollector
    {
        private readonly Dictionary<ProjectEntity, Project> projects = new Dictionary<ProjectEntity, Project>(new ReferenceEqualityComparer<ProjectEntity>());
        private readonly Dictionary<AssessmentSectionEntity, AssessmentSection> assessmentSections = new Dictionary<AssessmentSectionEntity, AssessmentSection>(new ReferenceEqualityComparer<AssessmentSectionEntity>());
        private readonly Dictionary<FailureMechanismEntity, FailureMechanismBase> failureMechanisms = new Dictionary<FailureMechanismEntity, FailureMechanismBase>(new ReferenceEqualityComparer<FailureMechanismEntity>());
        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicLocations = new Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>(new ReferenceEqualityComparer<HydraulicLocationEntity>());
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> stochasticSoilModels = new Dictionary<StochasticSoilModelEntity, StochasticSoilModel>(new ReferenceEqualityComparer<StochasticSoilModelEntity>());
        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> stochasticSoilProfiles = new Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile>(new ReferenceEqualityComparer<StochasticSoilProfileEntity>());
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = new Dictionary<SoilProfileEntity, PipingSoilProfile>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly Dictionary<SoilLayerEntity, PipingSoilLayer> soilLayers = new Dictionary<SoilLayerEntity, PipingSoilLayer>(new ReferenceEqualityComparer<SoilLayerEntity>());

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ProjectEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="Project"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(ProjectEntity entity, Project model)
        {
            Create(projects, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="AssessmentSectionEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="AssessmentSection"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(AssessmentSectionEntity entity, AssessmentSection model)
        {
            Create(assessmentSections, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="HydraulicBoundaryLocation"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(HydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            Create(hydraulicLocations, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="FailureMechanismBase"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(FailureMechanismEntity entity, FailureMechanismBase model)
        {
            Create(failureMechanisms, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="StochasticSoilModel"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(StochasticSoilModelEntity entity, StochasticSoilModel model)
        {
            Create(stochasticSoilModels, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="StochasticSoilProfile"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(StochasticSoilProfileEntity entity, StochasticSoilProfile model)
        {
            Create(stochasticSoilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="PipingSoilProfile"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(SoilProfileEntity entity, PipingSoilProfile model)
        {
            Create(soilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilLayerEntity"/> that was constructed.</param>
        /// <param name="model">The new <see cref="PipingSoilLayer"/> which needed to be created.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Create(SoilLayerEntity entity, PipingSoilLayer model)
        {
            Create(soilLayers, entity, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="SoilProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was created before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingSoilProfile model)
        {
            return ContainsValue(soilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SoilProfileEntity"/> which was created for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="SoilProfileEntity"/> for which a read operation has been registerd.</param>
        /// <returns>The constructed <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation has been registered for 
        /// <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains"/> to find out whether a create operation has been registered for
        /// <paramref name="model"/>.</remarks>
        internal SoilProfileEntity Get(PipingSoilProfile model)
        {
            return Get(soilProfiles, model);
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

            foreach (var entity in assessmentSections.Keys)
            {
                assessmentSections[entity].StorageId = entity.AssessmentSectionEntityId;
            }

            foreach (var entity in hydraulicLocations.Keys)
            {
                hydraulicLocations[entity].StorageId = entity.HydraulicLocationEntityId;
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
        }

        private bool ContainsValue<T,U>(Dictionary<T, U> collection, U model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.ContainsValue(model);
        }

        private void Create<T, U>(Dictionary<T, U> collection, T entity, U model)
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