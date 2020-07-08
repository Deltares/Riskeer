// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Util;
using Riskeer.ClosingStructures.Data;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Structures;
using Riskeer.DuneErosion.Data;
using Riskeer.HeightStructures.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Primitives;
using Riskeer.StabilityPointStructures.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read
{
    /// <summary>
    /// Class that can be used to keep track of data model objects which were initialized during a read operation
    /// from the database. Can be used to reuse objects when reading an already read entity.
    /// </summary>
    internal class ReadConversionCollector
    {
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

        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicBoundaryLocations =
            CreateDictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>();

        private readonly Dictionary<DuneLocationEntity, DuneLocation> duneLocations =
            CreateDictionary<DuneLocationEntity, DuneLocation>();

        private readonly Dictionary<FailureMechanismSectionEntity, FailureMechanismSection> failureMechanismSections =
            CreateDictionary<FailureMechanismSectionEntity, FailureMechanismSection>();

        private readonly Dictionary<DikeProfileEntity, DikeProfile> dikeProfiles =
            CreateDictionary<DikeProfileEntity, DikeProfile>();

        private readonly Dictionary<ForeshoreProfileEntity, ForeshoreProfile> foreshoreProfiles =
            CreateDictionary<ForeshoreProfileEntity, ForeshoreProfile>();

        private readonly Dictionary<HeightStructureEntity, HeightStructure> heightStructures =
            CreateDictionary<HeightStructureEntity, HeightStructure>();

        private readonly Dictionary<ClosingStructureEntity, ClosingStructure> closingStructures =
            CreateDictionary<ClosingStructureEntity, ClosingStructure>();

        private readonly Dictionary<StabilityPointStructureEntity, StabilityPointStructure> stabilityPointStructures =
            CreateDictionary<StabilityPointStructureEntity, StabilityPointStructure>();

        private readonly Dictionary<StabilityPointStructuresCalculationEntity, StructuresCalculationScenario<StabilityPointStructuresInput>> stabilityPointStructuresCalculations =
            CreateDictionary<StabilityPointStructuresCalculationEntity, StructuresCalculationScenario<StabilityPointStructuresInput>>();

        private static Dictionary<TEntity, TModel> CreateDictionary<TEntity, TModel>()
        {
            return new Dictionary<TEntity, TModel>(new ReferenceEqualityComparer<TEntity>());
        }

        #region StochasticSoilModelEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="StochasticSoilModelEntity"/> and the
        /// <see cref="PipingStochasticSoilModel"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> that was read.</param>
        /// <param name="model">The <see cref="PipingStochasticSoilModel"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(StochasticSoilModelEntity entity, PipingStochasticSoilModel model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            pipingStochasticSoilModels[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <see cref="StochasticSoilModelEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool ContainsPipingStochasticSoilModel(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return pipingStochasticSoilModels.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="PipingStochasticSoilModel"/> which was read for the given <see cref="StochasticSoilModelEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="PipingStochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been registered for 
        /// <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="ContainsPipingStochasticSoilModel"/> to find out 
        /// whether a read operation has been registered for <paramref name="entity"/>.</remarks>
        internal PipingStochasticSoilModel GetPipingStochasticSoilModel(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return pipingStochasticSoilModels[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Registers a read operation for <see cref="StochasticSoilModelEntity"/> and the
        /// <see cref="MacroStabilityInwardsStochasticSoilModel"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> that was read.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilModel"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(StochasticSoilModelEntity entity, MacroStabilityInwardsStochasticSoilModel model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            macroStabilityInwardsStochasticSoilModels[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <see cref="StochasticSoilModelEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool ContainsMacroStabilityInwardsStochasticSoilModel(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return macroStabilityInwardsStochasticSoilModels.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsStochasticSoilModel"/> which was read 
        /// for the given <see cref="StochasticSoilModelEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> for which a read 
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="MacroStabilityInwardsStochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been registered for 
        /// <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="ContainsMacroStabilityInwardsStochasticSoilModel"/> to find out 
        /// whether a read operation has been registered for <paramref name="entity"/>.</remarks>
        internal MacroStabilityInwardsStochasticSoilModel GetMacroStabilityInwardsStochasticSoilModel(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return macroStabilityInwardsStochasticSoilModels[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region PipingStochasticSoilProfileEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="PipingStochasticSoilProfileEntity"/> and 
        /// the <see cref="PipingStochasticSoilProfile"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingStochasticSoilProfileEntity"/> that was read.</param>
        /// <param name="model">The <see cref="PipingStochasticSoilProfile"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(PipingStochasticSoilProfileEntity entity, PipingStochasticSoilProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            pipingStochasticSoilProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <see cref="PipingStochasticSoilProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingStochasticSoilProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(PipingStochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return pipingStochasticSoilProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="PipingStochasticSoilProfile"/> which was read for the given
        /// <see cref="PipingStochasticSoilProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingStochasticSoilProfileEntity"/> for which
        /// a read operation has been registered.</param>
        /// <returns>The constructed <see cref="PipingStochasticSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been 
        /// registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingStochasticSoilProfileEntity)"/> to find out 
        /// whether a read operation has been registered for <paramref name="entity"/>.</remarks>
        internal PipingStochasticSoilProfile Get(PipingStochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return pipingStochasticSoilProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region PipingSoilProfileEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <paramref name="entity"/> and the <paramref name="model"/> 
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSoilProfileEntity"/> that was read.</param>
        /// <param name="model">The <see cref="PipingSoilProfile"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(PipingSoilProfileEntity entity, PipingSoilProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            pipingSoilProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSoilProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(PipingSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return pipingSoilProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="PipingSoilProfile"/> which was read for the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingSoilProfileEntity"/> for which a read operation 
        /// has been registered.</param>
        /// <returns>The constructed <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been 
        /// registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(PipingSoilProfileEntity)"/> to find out whether a 
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal PipingSoilProfile Get(PipingSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return pipingSoilProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region SurfaceLineEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="SurfaceLineEntity"/> and the
        /// <see cref="PipingSurfaceLine"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> that was read.</param>
        /// <param name="model">The <see cref="PipingSurfaceLine"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(SurfaceLineEntity entity, PipingSurfaceLine model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            pipingSurfaceLines[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="SurfaceLineEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool ContainsPipingSurfaceLine(SurfaceLineEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return pipingSurfaceLines.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="PipingSurfaceLine"/> which was read for the
        /// given <see cref="SurfaceLineEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> for which a read operation
        /// has been registered.</param>
        /// <returns>The constructed <see cref="PipingSurfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="ContainsPipingSurfaceLine"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal PipingSurfaceLine GetPipingSurfaceLine(SurfaceLineEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return pipingSurfaceLines[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        /// <summary>
        /// Registers a read operation for <see cref="SurfaceLineEntity"/> and the
        /// <see cref="MacroStabilityInwardsSurfaceLine"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> that was read.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsSurfaceLine"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(SurfaceLineEntity entity, MacroStabilityInwardsSurfaceLine model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            macroStabilityInwardsSurfaceLines[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="SurfaceLineEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool ContainsMacroStabilityInwardsSurfaceLine(SurfaceLineEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return macroStabilityInwardsSurfaceLines.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsSurfaceLine"/> which was read for the
        /// given <see cref="SurfaceLineEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> for which a read operation
        /// has been registered.</param>
        /// <returns>The constructed <see cref="MacroStabilityInwardsSurfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="ContainsMacroStabilityInwardsSurfaceLine"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal MacroStabilityInwardsSurfaceLine GetMacroStabilityInwardsSurfaceLine(SurfaceLineEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return macroStabilityInwardsSurfaceLines[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region MacroStabilityInwardsStochasticSoilProfileEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> and 
        /// the <see cref="MacroStabilityInwardsStochasticSoilProfile"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> that was read.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsStochasticSoilProfile"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(MacroStabilityInwardsStochasticSoilProfileEntity entity, MacroStabilityInwardsStochasticSoilProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            macroStabilityInwardsStochasticSoilProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsStochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return macroStabilityInwardsStochasticSoilProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsStochasticSoilProfile"/> which was read for the given
        /// <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsStochasticSoilProfileEntity"/> for which
        /// a read operation has been registered.</param>
        /// <returns>The constructed <see cref="MacroStabilityInwardsStochasticSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been 
        /// registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsStochasticSoilProfileEntity)"/> to find out 
        /// whether a read operation has been registered for <paramref name="entity"/>.</remarks>
        internal MacroStabilityInwardsStochasticSoilProfile Get(MacroStabilityInwardsStochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return macroStabilityInwardsStochasticSoilProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region MacroStabilityInwardsSoilProfileOneDEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> and the
        /// <see cref="MacroStabilityInwardsSoilProfile1D"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> that was read.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfile1D"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(MacroStabilityInwardsSoilProfileOneDEntity entity, MacroStabilityInwardsSoilProfile1D model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            macroStabilityInwardsSoil1DProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsSoilProfileOneDEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return macroStabilityInwardsSoil1DProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsSoilProfile1D"/> which was read for the
        /// given <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileOneDEntity"/> for which a read operation
        /// has been registered.</param>
        /// <returns>The constructed <see cref="MacroStabilityInwardsSoilProfile1D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsSoilProfileOneDEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal MacroStabilityInwardsSoilProfile1D Get(MacroStabilityInwardsSoilProfileOneDEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return macroStabilityInwardsSoil1DProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region MacroStabilityInwardsSoilProfileTwoDEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> and the
        /// <see cref="MacroStabilityInwardsSoilProfile2D"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> that was read.</param>
        /// <param name="model">The <see cref="MacroStabilityInwardsSoilProfile2D"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(MacroStabilityInwardsSoilProfileTwoDEntity entity, MacroStabilityInwardsSoilProfile2D model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            macroStabilityInwardsSoil2DProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(MacroStabilityInwardsSoilProfileTwoDEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return macroStabilityInwardsSoil2DProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="MacroStabilityInwardsSoilProfile2D"/> which was read for the
        /// given <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsSoilProfileTwoDEntity"/> for which a read operation
        /// has been registered.</param>
        /// <returns>The constructed <see cref="MacroStabilityInwardsSoilProfile2D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(MacroStabilityInwardsSoilProfileTwoDEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal MacroStabilityInwardsSoilProfile2D Get(MacroStabilityInwardsSoilProfileTwoDEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return macroStabilityInwardsSoil2DProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region HydraulicLocationEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="HydraulicLocationEntity"/> and the
        /// <see cref="HydraulicBoundaryLocation"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> that was read.</param>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(HydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            hydraulicBoundaryLocations[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="HydraulicLocationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(HydraulicLocationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return hydraulicBoundaryLocations.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="HydraulicBoundaryLocation"/> which was read for the
        /// given <see cref="HydraulicLocationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HydraulicLocationEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="HydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(HydraulicLocationEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal HydraulicBoundaryLocation Get(HydraulicLocationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return hydraulicBoundaryLocations[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region DuneLocationEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="DuneLocationEntity"/> and the
        /// <see cref="DuneLocation"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationEntity"/> that was read.</param>
        /// <param name="model">The <see cref="DuneLocation"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(DuneLocationEntity entity, DuneLocation model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            duneLocations[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="DuneLocationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(DuneLocationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return duneLocations.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="DuneLocation"/> which was read for the
        /// given <see cref="DuneLocationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="DuneLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(DuneLocationEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal DuneLocation Get(DuneLocationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return duneLocations[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region FailureMechanismSectionEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="FailureMechanismSectionEntity"/> and the
        /// <see cref="FailureMechanismSection"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> that was read.</param>
        /// <param name="model">The <see cref="FailureMechanismSection"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(FailureMechanismSectionEntity entity, FailureMechanismSection model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            failureMechanismSections[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="FailureMechanismSectionEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(FailureMechanismSectionEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return failureMechanismSections.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="FailureMechanismSection"/> which was read for the
        /// given <see cref="FailureMechanismSectionEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="FailureMechanismSection"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(FailureMechanismSectionEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal FailureMechanismSection Get(FailureMechanismSectionEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return failureMechanismSections[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region DikeProfileEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="DikeProfileEntity"/> and the
        /// <see cref="dikeProfiles"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="DikeProfileEntity"/> that was read.</param>
        /// <param name="model">The <see cref="DikeProfile"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(DikeProfileEntity entity, DikeProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            dikeProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="DikeProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DikeProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(DikeProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return dikeProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="DikeProfile"/> which was read for the
        /// given <see cref="DikeProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DikeProfileEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="DikeProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(DikeProfileEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal DikeProfile Get(DikeProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return dikeProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region ForeshoreProfileEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="ForeshoreProfileEntity"/> and the
        /// <see cref="foreshoreProfiles"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ForeshoreProfileEntity"/> that was read.</param>
        /// <param name="model">The <see cref="ForeshoreProfile"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(ForeshoreProfileEntity entity, ForeshoreProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            foreshoreProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="ForeshoreProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ForeshoreProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(ForeshoreProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return foreshoreProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="ForeshoreProfile"/> which was read for the
        /// given <see cref="ForeshoreProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ForeshoreProfileEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="ForeshoreProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(ForeshoreProfileEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal ForeshoreProfile Get(ForeshoreProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return foreshoreProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region HeightStructureEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="HeightStructureEntity"/> and the
        /// <see cref="HeightStructure"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructureEntity"/> that was read.</param>
        /// <param name="model">The <see cref="HeightStructure"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(HeightStructureEntity entity, HeightStructure model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            heightStructures[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="HeightStructureEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructureEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(HeightStructureEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return heightStructures.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="HeightStructure"/> which was read for the
        /// given <see cref="HeightStructureEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="HeightStructureEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="HeightStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(HeightStructureEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal HeightStructure Get(HeightStructureEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return heightStructures[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region ClosingStructureEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="ClosingStructureEntity"/> and the
        /// <see cref="ClosingStructure"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureEntity"/> that was read.</param>
        /// <param name="model">The <see cref="ClosingStructure"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(ClosingStructureEntity entity, ClosingStructure model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            closingStructures[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="ClosingStructureEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(ClosingStructureEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return closingStructures.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="ClosingStructure"/> which was read for the
        /// given <see cref="ClosingStructureEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="ClosingStructureEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="ClosingStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(ClosingStructureEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal ClosingStructure Get(ClosingStructureEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return closingStructures[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region StabilityPointStructureEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="StabilityPointStructureEntity"/> and the
        /// <see cref="StabilityPointStructure"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructureEntity"/> that was read.</param>
        /// <param name="model">The <see cref="StabilityPointStructure"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        internal void Read(StabilityPointStructureEntity entity, StabilityPointStructure model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            stabilityPointStructures[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="StabilityPointStructureEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructureEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(StabilityPointStructureEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return stabilityPointStructures.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="StabilityPointStructure"/> which was read for the
        /// given <see cref="StabilityPointStructureEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructureEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="StabilityPointStructure"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(StabilityPointStructureEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal StabilityPointStructure Get(StabilityPointStructureEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return stabilityPointStructures[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region StabilityPointStructuresCalculationEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="StabilityPointStructuresCalculationEntity"/>
        /// and the <see cref="StructuresCalculationScenario{T}"/> that was constructed
        /// with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructuresCalculationEntity"/>
        /// that was read.</param>
        /// <param name="model">The <see cref="StructuresCalculationScenario{T}"/> that
        /// was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal void Read(StabilityPointStructuresCalculationEntity entity, StructuresCalculationScenario<StabilityPointStructuresInput> model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            stabilityPointStructuresCalculations[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="StabilityPointStructuresCalculationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructuresCalculationEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(StabilityPointStructuresCalculationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return stabilityPointStructuresCalculations.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="StructuresCalculationScenario{T}"/> which was read
        /// for the given <see cref="StabilityPointStructuresCalculationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StabilityPointStructuresCalculationEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="StructuresCalculationScenario{T}"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(StabilityPointStructuresCalculationEntity)"/>
        /// to find out whether a read operation has been registered for <paramref name="entity"/>.</remarks>
        internal StructuresCalculationScenario<StabilityPointStructuresInput> Get(StabilityPointStructuresCalculationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            try
            {
                return stabilityPointStructuresCalculations[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion
    }
}