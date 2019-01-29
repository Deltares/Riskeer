// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Util;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.DuneErosion.Data;
using Riskeer.GrassCoverErosionInwards.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create
{
    /// <summary>
    /// This class can be used to keep track of create and update operations on a database.
    /// This information can be used to reuse objects. When all operations have been performed,
    /// then the collected information can be used to transfer the ids assigned to the created
    /// database instances back to the data model or to clean up orphans.
    /// </summary>
    internal class PersistenceRegistry
    {
        private readonly Dictionary<FailureMechanismSectionEntity, FailureMechanismSection> failureMechanismSections =
            CreateDictionary<FailureMechanismSectionEntity, FailureMechanismSection>();

        private readonly Dictionary<DikeProfileEntity, DikeProfile> dikeProfiles =
            CreateDictionary<DikeProfileEntity, DikeProfile>();

        private readonly Dictionary<ForeshoreProfileEntity, ForeshoreProfile> foreshoreProfiles =
            CreateDictionary<ForeshoreProfileEntity, ForeshoreProfile>();

        private readonly Dictionary<GrassCoverErosionInwardsCalculationEntity, GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations =
            CreateDictionary<GrassCoverErosionInwardsCalculationEntity, GrassCoverErosionInwardsCalculation>();

        private readonly Dictionary<StochasticSoilModelEntity, PipingStochasticSoilModel> pipingStochasticSoilModels =
            CreateDictionary<StochasticSoilModelEntity, PipingStochasticSoilModel>();

        private readonly Dictionary<PipingStochasticSoilProfileEntity, PipingStochasticSoilProfile> pipingStochasticSoilProfiles =
            CreateDictionary<PipingStochasticSoilProfileEntity, PipingStochasticSoilProfile>();

        private readonly Dictionary<PipingSoilProfileEntity, PipingSoilProfile> pipingSoilProfiles =
            CreateDictionary<PipingSoilProfileEntity, PipingSoilProfile>();

        private readonly Dictionary<SurfaceLineEntity, PipingSurfaceLine> pipingSurfaceLines =
            CreateDictionary<SurfaceLineEntity, PipingSurfaceLine>();

        private readonly Dictionary<StochasticSoilModelEntity, MacroStabilityInwardsStochasticSoilModel> macroStabilityInwardsStochasticSoilModels =
            CreateDictionary<StochasticSoilModelEntity, MacroStabilityInwardsStochasticSoilModel>();

        private readonly Dictionary<MacroStabilityInwardsStochasticSoilProfileEntity, MacroStabilityInwardsStochasticSoilProfile> macroStabilityInwardsStochasticSoilProfiles =
            CreateDictionary<MacroStabilityInwardsStochasticSoilProfileEntity, MacroStabilityInwardsStochasticSoilProfile>();

        private readonly Dictionary<MacroStabilityInwardsSoilProfileOneDEntity, MacroStabilityInwardsSoilProfile1D> macroStabilityInwardsSoil1DProfiles =
            CreateDictionary<MacroStabilityInwardsSoilProfileOneDEntity, MacroStabilityInwardsSoilProfile1D>();

        private readonly Dictionary<MacroStabilityInwardsSoilProfileTwoDEntity, MacroStabilityInwardsSoilProfile2D> macroStabilityInwardsSoil2DProfiles =
            CreateDictionary<MacroStabilityInwardsSoilProfileTwoDEntity, MacroStabilityInwardsSoilProfile2D>();

        private readonly Dictionary<SurfaceLineEntity, MacroStabilityInwardsSurfaceLine> macroStabilityInwardsSurfaceLines =
            CreateDictionary<SurfaceLineEntity, MacroStabilityInwardsSurfaceLine>();

        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicLocations =
            CreateDictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>();

        private readonly Dictionary<DuneLocationEntity, DuneLocation> duneLocations =
            CreateDictionary<DuneLocationEntity, DuneLocation>();

        private readonly Dictionary<HeightStructureEntity, HeightStructure> heightStructures =
            CreateDictionary<HeightStructureEntity, HeightStructure>();

        private readonly Dictionary<ClosingStructureEntity, ClosingStructure> closingStructures =
            CreateDictionary<ClosingStructureEntity, ClosingStructure>();

        private readonly Dictionary<StabilityPointStructureEntity, StabilityPointStructure> stabilityPointStructures =
            CreateDictionary<StabilityPointStructureEntity, StabilityPointStructure>();

        private readonly Dictionary<HeightStructuresCalculationEntity, StructuresCalculation<HeightStructuresInput>> heightStructuresCalculations =
            CreateDictionary<HeightStructuresCalculationEntity, StructuresCalculation<HeightStructuresInput>>();

        private readonly Dictionary<ClosingStructuresCalculationEntity, StructuresCalculation<ClosingStructuresInput>> closingStructuresCalculations =
            CreateDictionary<ClosingStructuresCalculationEntity, StructuresCalculation<ClosingStructuresInput>>();

        private readonly Dictionary<StabilityPointStructuresCalculationEntity, StructuresCalculation<StabilityPointStructuresInput>> stabilityPointStructuresCalculations =
            CreateDictionary<StabilityPointStructuresCalculationEntity, StructuresCalculation<StabilityPointStructuresInput>>();

        private static Dictionary<TEntity, TModel> CreateDictionary<TEntity, TModel>()
        {
            return new Dictionary<TEntity, TModel>(new ReferenceEqualityComparer<TEntity>());
        }

        private bool ContainsValue<TEntity, TModel>(Dictionary<TEntity, TModel> collection, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return collection.Values.Contains(model, new ReferenceEqualityComparer<TModel>());
        }

        private void Register<TEntity, TModel>(Dictionary<TEntity, TModel> collection, TEntity entity, TModel model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            collection[entity] = model;
        }

        private TEntity Get<TEntity, TModel>(Dictionary<TEntity, TModel> collection, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
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
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(HydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            Register(hydraulicLocations, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="DuneLocation"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(DuneLocationEntity entity, DuneLocation model)
        {
            Register(duneLocations, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingStochasticSoilModel"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(StochasticSoilModelEntity entity, PipingStochasticSoilModel model)
        {
            Register(pipingStochasticSoilModels, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingStochasticSoilProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingStochasticSoilProfile"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(PipingStochasticSoilProfileEntity entity, PipingStochasticSoilProfile model)
        {
            Register(pipingStochasticSoilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSoilProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingSoilProfile"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(PipingSoilProfileEntity entity, PipingSoilProfile model)
        {
            Register(pipingSoilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="PipingSurfaceLine"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(SurfaceLineEntity entity, PipingSurfaceLine model)
        {
            Register(pipingSurfaceLines, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilModel"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(StochasticSoilModelEntity entity, MacroStabilityInwardsStochasticSoilModel model)
        {
            Register(macroStabilityInwardsStochasticSoilModels, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilProfile"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(MacroStabilityInwardsStochasticSoilProfileEntity entity, MacroStabilityInwardsStochasticSoilProfile model)
        {
            Register(macroStabilityInwardsStochasticSoilProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfile1D"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(MacroStabilityInwardsSoilProfileOneDEntity entity, MacroStabilityInwardsSoilProfile1D model)
        {
            Register(macroStabilityInwardsSoil1DProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfile2D"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(MacroStabilityInwardsSoilProfileTwoDEntity entity, MacroStabilityInwardsSoilProfile2D model)
        {
            Register(macroStabilityInwardsSoil2DProfiles, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsSurfaceLine"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(SurfaceLineEntity entity, MacroStabilityInwardsSurfaceLine model)
        {
            Register(macroStabilityInwardsSurfaceLines, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructureEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="HeightStructure"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(HeightStructureEntity entity, HeightStructure model)
        {
            Register(heightStructures, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="ClosingStructure"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(ClosingStructureEntity entity, ClosingStructure model)
        {
            Register(closingStructures, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructureEntity"/> to be registered.</param>
        /// <param name="model">The <see cref="StabilityPointStructure"/> to be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Register(StabilityPointStructureEntity entity, StabilityPointStructure model)
        {
            Register(stabilityPointStructures, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructuresCalculationEntity"/>
        /// to be registered.</param>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> to
        /// be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal void Register(HeightStructuresCalculationEntity entity, StructuresCalculation<HeightStructuresInput> model)
        {
            Register(heightStructuresCalculations, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructuresCalculationEntity"/>
        /// to be registered.</param>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> to
        /// be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal void Register(ClosingStructuresCalculationEntity entity, StructuresCalculation<ClosingStructuresInput> model)
        {
            Register(closingStructuresCalculations, entity, model);
        }

        /// <summary>
        /// Registers a create operation for <paramref name="model"/> and the <paramref name="entity"/>
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructuresCalculationEntity"/>
        /// to be registered.</param>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> to
        /// be registered.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal void Register(StabilityPointStructuresCalculationEntity entity, StructuresCalculation<StabilityPointStructuresInput> model)
        {
            Register(stabilityPointStructuresCalculations, entity, model);
        }

        #endregion

        #region Contains Methods

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingStochasticSoilModel"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingStochasticSoilModel model)
        {
            return ContainsValue(pipingStochasticSoilModels, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingStochasticSoilProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingStochasticSoilProfile model)
        {
            return ContainsValue(pipingStochasticSoilProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingSoilProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingSoilProfile model)
        {
            return ContainsValue(pipingSoilProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingSurfaceLine"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(PipingSurfaceLine model)
        {
            return ContainsValue(pipingSurfaceLines, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilModel"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsStochasticSoilModel model)
        {
            return ContainsValue(macroStabilityInwardsStochasticSoilModels, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilProfile"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsStochasticSoilProfile model)
        {
            return ContainsValue(macroStabilityInwardsStochasticSoilProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfile1D"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsSoilProfile1D model)
        {
            return ContainsValue(macroStabilityInwardsSoil1DProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfile2D"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsSoilProfile2D model)
        {
            return ContainsValue(macroStabilityInwardsSoil2DProfiles, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsSurfaceLine"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsSurfaceLine model)
        {
            return ContainsValue(macroStabilityInwardsSurfaceLines, model);
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
        /// <param name="model">The <see cref="DuneLocation"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(DuneLocation model)
        {
            return ContainsValue(duneLocations, model);
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

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="ClosingStructure"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(ClosingStructure model)
        {
            return ContainsValue(closingStructures, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StabilityPointStructure"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StabilityPointStructure model)
        {
            return ContainsValue(stabilityPointStructures, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StructuresCalculation<HeightStructuresInput> model)
        {
            return ContainsValue(heightStructuresCalculations, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StructuresCalculation<ClosingStructuresInput> model)
        {
            return ContainsValue(closingStructuresCalculations, model);
        }

        /// <summary>
        /// Checks whether a create operations has been registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="model"/> was registered before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        internal bool Contains(StructuresCalculation<StabilityPointStructuresInput> model)
        {
            return ContainsValue(stabilityPointStructuresCalculations, model);
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
        /// <param name="model">The <see cref="PipingStochasticSoilModel"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The created <see cref="StochasticSoilModelEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingStochasticSoilModel)"/> to find out whether
        /// a create operation has been registered for <paramref name="model"/>.</remarks>
        internal StochasticSoilModelEntity Get(PipingStochasticSoilModel model)
        {
            return Get(pipingStochasticSoilModels, model);
        }

        /// <summary>
        /// Obtains the <see cref="PipingStochasticSoilProfileEntity"/> which was registered for
        /// the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingStochasticSoilProfile"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The created <see cref="PipingStochasticSoilProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingStochasticSoilProfile)"/> to find out whether
        /// a create/create operation has been registered for <paramref name="model"/>.</remarks>
        internal PipingStochasticSoilProfileEntity Get(PipingStochasticSoilProfile model)
        {
            return Get(pipingStochasticSoilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="PipingSoilProfileEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingSoilProfileEntity"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingSoilProfile)"/> to find out whether a
        /// create operation has been registered for <paramref name="model"/>.</remarks>
        internal PipingSoilProfileEntity Get(PipingSoilProfile model)
        {
            return Get(pipingSoilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SurfaceLineEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="PipingSurfaceLine"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="SurfaceLineEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingSurfaceLine)"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal SurfaceLineEntity Get(PipingSurfaceLine model)
        {
            return Get(pipingSurfaceLines, model);
        }

        /// <summary>
        /// Obtains the <see cref="StochasticSoilModelEntity"/> which was registered for
        /// the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilModel"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The created <see cref="StochasticSoilModelEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsStochasticSoilModel)"/> to find out whether
        /// a create operation has been registered for <paramref name="model"/>.</remarks>
        internal StochasticSoilModelEntity Get(MacroStabilityInwardsStochasticSoilModel model)
        {
            return Get(macroStabilityInwardsStochasticSoilModels, model);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> which was registered for
        /// the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilProfile"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The created <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsStochasticSoilProfile)"/> to find out whether
        /// a create/create operation has been registered for <paramref name="model"/>.</remarks>
        internal MacroStabilityInwardsStochasticSoilProfileEntity Get(MacroStabilityInwardsStochasticSoilProfile model)
        {
            return Get(macroStabilityInwardsStochasticSoilProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="MacroStabilityInwardsSoilProfile1D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsSoilProfile1D)"/> to find out whether a
        /// create operation has been registered for <paramref name="model"/>.</remarks>
        internal MacroStabilityInwardsSoilProfileOneDEntity Get(MacroStabilityInwardsSoilProfile1D model)
        {
            return Get(macroStabilityInwardsSoil1DProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> for which a create
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="MacroStabilityInwardsSoilProfile2D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsSoilProfile2D)"/> to find out whether a
        /// create operation has been registered for <paramref name="model"/>.</remarks>
        internal MacroStabilityInwardsSoilProfileTwoDEntity Get(MacroStabilityInwardsSoilProfile2D model)
        {
            return Get(macroStabilityInwardsSoil2DProfiles, model);
        }

        /// <summary>
        /// Obtains the <see cref="SurfaceLineEntity"/> which was registered for the given
        /// <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="MacroStabilityInwardsSurfaceLine"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="SurfaceLineEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsSurfaceLine)"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal SurfaceLineEntity Get(MacroStabilityInwardsSurfaceLine model)
        {
            return Get(macroStabilityInwardsSurfaceLines, model);
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
        internal HydraulicLocationEntity Get(HydraulicBoundaryLocation model)
        {
            return Get(hydraulicLocations, model);
        }

        /// <summary>
        /// Obtains the <see cref="DuneLocationEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="DuneLocation"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="DuneLocationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(DuneLocation)"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal DuneLocationEntity Get(DuneLocation model)
        {
            return Get(duneLocations, model);
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

        /// <summary>
        /// Obtains the <see cref="ClosingStructureEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="ClosingStructure"/> for which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="ClosingStructureEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(ClosingStructure)"/> to find out whether a
        /// create operation has been registered for <paramref name="model"/>.</remarks>
        internal ClosingStructureEntity Get(ClosingStructure model)
        {
            return Get(closingStructures, model);
        }

        /// <summary>
        /// Obtains the <see cref="StabilityPointStructureEntity"/> which was registered for the
        /// given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StabilityPointStructure"/> for which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="StabilityPointStructureEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StabilityPointStructure)"/> to find out whether a
        /// create operation has been registered for <paramref name="model"/>.</remarks>
        internal StabilityPointStructureEntity Get(StabilityPointStructure model)
        {
            return Get(stabilityPointStructures, model);
        }

        /// <summary>
        /// Obtains the <see cref="HeightStructuresCalculationEntity"/> which was
        /// registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> for
        /// which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="HeightStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StructuresCalculation{HeightStructuresInput})"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal HeightStructuresCalculationEntity Get(StructuresCalculation<HeightStructuresInput> model)
        {
            return Get(heightStructuresCalculations, model);
        }

        /// <summary>
        /// Obtains the <see cref="ClosingStructuresCalculationEntity"/> which was
        /// registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> for
        /// which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="ClosingStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StructuresCalculation{ClosingStructuresInput})"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal ClosingStructuresCalculationEntity Get(StructuresCalculation<ClosingStructuresInput> model)
        {
            return Get(closingStructuresCalculations, model);
        }

        /// <summary>
        /// Obtains the <see cref="StabilityPointStructuresCalculationEntity"/> which was
        /// registered for the given <paramref name="model"/>.
        /// </summary>
        /// <param name="model">The <see cref="StructuresCalculation{T}"/> for
        /// which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="StabilityPointStructuresCalculationEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="model"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no create operation 
        /// has been registered for <paramref name="model"/>.</exception>
        /// <remarks>Use <see cref="Contains(StructuresCalculation{StabilityPointStructuresInput})"/> to find out
        /// whether a create operation has been registered for <paramref name="model"/>.</remarks>
        internal StabilityPointStructuresCalculationEntity Get(StructuresCalculation<StabilityPointStructuresInput> model)
        {
            return Get(stabilityPointStructuresCalculations, model);
        }

        #endregion
    }
}