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
using Core.Common.Utils;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
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
        private readonly Dictionary<FailureMechanismSectionEntity, FailureMechanismSection> failureMechanismSections = CreateDictionary<FailureMechanismSectionEntity, FailureMechanismSection>();
        private readonly Dictionary<DikeProfileEntity, DikeProfile> dikeProfiles = CreateDictionary<DikeProfileEntity, DikeProfile>();
        private readonly Dictionary<ForeshoreProfileEntity, ForeshoreProfile> foreshoreProfiles = CreateDictionary<ForeshoreProfileEntity, ForeshoreProfile>();
        private readonly Dictionary<GrassCoverErosionInwardsCalculationEntity, GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations = CreateDictionary<GrassCoverErosionInwardsCalculationEntity, GrassCoverErosionInwardsCalculation>();
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> stochasticSoilModels = CreateDictionary<StochasticSoilModelEntity, StochasticSoilModel>();
        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> stochasticSoilProfiles = CreateDictionary<StochasticSoilProfileEntity, StochasticSoilProfile>();
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = CreateDictionary<SoilProfileEntity, PipingSoilProfile>();
        private readonly Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine> surfaceLines = CreateDictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine>();
        private readonly Dictionary<object, HydraulicBoundaryLocation> hydraulicLocations = CreateDictionary<object, HydraulicBoundaryLocation>();
        private readonly Dictionary<HeightStructureEntity, HeightStructure> heightStructures = CreateDictionary<HeightStructureEntity, HeightStructure>();

        private static Dictionary<TEntity, TModel> CreateDictionary<TEntity, TModel>()
        {
            return new Dictionary<TEntity, TModel>(new ReferenceEqualityComparer<TEntity>());
        }

        private bool ContainsValue<TEntity, TModel>(Dictionary<TEntity, TModel> collection, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.ContainsValue(model);
        }

        private void Register<TEntity, TModel>(Dictionary<TEntity, TModel> collection, TEntity entity, TModel model)
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

        private TEntity Get<TEntity, TModel>(Dictionary<TEntity, TModel> collection, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            return collection.Keys.Single(k => ReferenceEquals(collection[k], model));
        }

        #region Register Methods

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/>
        /// to be registered.</param>
        /// <param name="model">The <see cref="GrassCoverErosionInwardsCalculation"/> to
        /// be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(GrassCoverErosionInwardsCalculationEntity entity, GrassCoverErosionInwardsCalculation model)
        {
            Register(grassCoverErosionInwardsCalculations, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="FailureMechanismSection"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(FailureMechanismSectionEntity entity, FailureMechanismSection model)
        {
            Register(failureMechanismSections, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="DikeProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="DikeProfile"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(DikeProfileEntity entity, DikeProfile model)
        {
            Register(dikeProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ForeshoreProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="ForeshoreProfile"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(ForeshoreProfileEntity entity, ForeshoreProfile model)
        {
            Register(foreshoreProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> to be registered.</param>
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
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(GrassCoverErosionOutwardsHydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            Register(hydraulicLocations, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="StochasticSoilModel"/> to be registered.</param>
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
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> to be registered.</param>
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
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingSoilProfile"/> to be registered.</param>
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
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> to be registered.</param>
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
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructureEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="HeightStructure"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Register(HeightStructureEntity entity, HeightStructure model)
        {
            Register(heightStructures, entity, model);
        }

        #endregion

        #region Contains Methods

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilModel"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StochasticSoilModel model)
        {
            return ContainsValue(stochasticSoilModels, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StochasticSoilProfile model)
        {
            return ContainsValue(stochasticSoilProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingSoilProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingSoilProfile model)
        {
            return ContainsValue(soilProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(HydraulicBoundaryLocation model)
        {
            return ContainsValue(hydraulicLocations, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(RingtoetsPipingSurfaceLine model)
        {
            return ContainsValue(surfaceLines, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="FailureMechanismSection"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(FailureMechanismSection model)
        {
            return ContainsValue(failureMechanismSections, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="DikeProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(DikeProfile model)
        {
            return ContainsValue(dikeProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="ForeshoreProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(ForeshoreProfile model)
        {
            return ContainsValue(foreshoreProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="HeightStructure"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(HeightStructure model)
        {
            return ContainsValue(heightStructures, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="GrassCoverErosionInwardsCalculation"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(GrassCoverErosionInwardsCalculation model)
        {
            return ContainsValue(grassCoverErosionInwardsCalculations, model);
        }

        #endregion

        #region Get Methods

        /// <summary>
        /// Obtains the <see cref="ForeshoreProfileEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="ForeshoreProfile"/> for which a read operation
        /// has been registered.</param>
        /// <returns>The constructed <see cref="ForeshoreProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(DikeProfile)"/> to find out whether a create
        /// operation has been registered for <paramref name="model"/>.</remarks>
        public ForeshoreProfileEntity Get(ForeshoreProfile model)
        {
            return Get(foreshoreProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="StochasticSoilModelEntity"/> which was registered for
        /// the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilModel"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The created <see cref="StochasticSoilModelEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StochasticSoilModel)"/> to find out whether
        /// a create operation has been registered for <paramref name="model"/>.</remarks>
        internal StochasticSoilModelEntity Get(StochasticSoilModel model)
        {
            return Get(stochasticSoilModels, model);
        }

        /// <summary>
        /// Obtains the <see cref="StochasticSoilProfileEntity"/> which was registered for
        /// the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The created <see cref="StochasticSoilProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StochasticSoilProfile)"/> to find out whether
        /// a create/create operation has been registered for <paramref name="model"/>.</remarks>
        internal StochasticSoilProfileEntity Get(StochasticSoilProfile model)
        {
            return Get(stochasticSoilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SoilProfileEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="SoilProfileEntity"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingSoilProfile)"/> to find out whether a
        /// create operation has been registered for <paramref name="model"/>.</remarks>
        internal SoilProfileEntity Get(PipingSoilProfile model)
        {
            return Get(soilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SurfaceLineEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="SurfaceLineEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(RingtoetsPipingSurfaceLine)"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal SurfaceLineEntity Get(RingtoetsPipingSurfaceLine model)
        {
            return Get(surfaceLines, model);
        }

        /// <summary>
        /// Obtains the <see cref="HydraulicLocationEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="HydraulicLocationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(HydraulicBoundaryLocation)"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal T Get<T>(HydraulicBoundaryLocation model) where T : class
        {
            return Get(hydraulicLocations, model) as T;
        }

        /// <summary>
        /// Obtains the <see cref="FailureMechanismSection"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="FailureMechanismSection"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="FailureMechanismSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(FailureMechanismSection)"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal FailureMechanismSectionEntity Get(FailureMechanismSection model)
        {
            return Get(failureMechanismSections, model);
        }

        /// <summary>
        /// Obtains the <see cref="DikeProfileEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="DikeProfile"/> for which a read operation
        /// has been registered.</param>
        /// <returns>The constructed <see cref="DikeProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(DikeProfile)"/> to find out whether a create
        /// operation has been registered for <paramref name="model"/>.</remarks>
        internal DikeProfileEntity Get(DikeProfile model)
        {
            return Get(dikeProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="GrassCoverErosionInwardsCalculationEntity"/> which was
        /// registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="GrassCoverErosionInwardsCalculation"/> for
        /// which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="GrassCoverErosionInwardsCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(GrassCoverErosionInwardsCalculation)"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal GrassCoverErosionInwardsCalculationEntity Get(GrassCoverErosionInwardsCalculation model)
        {
            return Get(grassCoverErosionInwardsCalculations, model);
        }

        /// <summary>
        /// Obtains the <see cref="HeightStructureEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="HeightStructure"/> for which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="HeightStructureEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(HeightStructure)"/> to find out whether a
        /// create operation has been registered for <paramref name="model"/>.</remarks>
        internal HeightStructureEntity Get(HeightStructure model)
        {
            return Get(heightStructures, model);
        }

        #endregion
    }
}