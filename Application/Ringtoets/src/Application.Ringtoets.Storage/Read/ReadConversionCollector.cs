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
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Utils;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// Class that can be used to keep track of data model objects which were initialized during a read operation
    /// from the database. Can be used to reuse objects when reading an already read entity.
    /// </summary>
    internal class ReadConversionCollector
    {
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> stochasticSoilModels = CreateDictionary<StochasticSoilModelEntity, StochasticSoilModel>();
        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> stochasticSoilProfiles = CreateDictionary<StochasticSoilProfileEntity, StochasticSoilProfile>();
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = CreateDictionary<SoilProfileEntity, PipingSoilProfile>();
        private readonly Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine> surfaceLines = CreateDictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine>();
        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicBoundaryLocations = CreateDictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>();
        private readonly Dictionary<GrassCoverErosionOutwardsHydraulicLocationEntity, HydraulicBoundaryLocation> grassCoverErosionOutwardsHydraulicBoundaryLocations = CreateDictionary<GrassCoverErosionOutwardsHydraulicLocationEntity, HydraulicBoundaryLocation>();
        private readonly Dictionary<FailureMechanismSectionEntity, FailureMechanismSection> failureMechanismSections = CreateDictionary<FailureMechanismSectionEntity, FailureMechanismSection>();
        private readonly Dictionary<DikeProfileEntity, DikeProfile> dikeProfiles = CreateDictionary<DikeProfileEntity, DikeProfile>();
        private readonly Dictionary<ForeshoreProfileEntity, ForeshoreProfile> foreshoreProfiles = CreateDictionary<ForeshoreProfileEntity, ForeshoreProfile>();
        private readonly Dictionary<GrassCoverErosionInwardsCalculationEntity, GrassCoverErosionInwardsCalculation> grassCoverErosionInwardsCalculations = CreateDictionary<GrassCoverErosionInwardsCalculationEntity, GrassCoverErosionInwardsCalculation>();
        private readonly Dictionary<HeightStructureEntity, HeightStructure> heightStructures = CreateDictionary<HeightStructureEntity, HeightStructure>();
        private readonly Dictionary<ClosingStructureEntity, ClosingStructure> closingStructures = CreateDictionary<ClosingStructureEntity, ClosingStructure>();

        private static Dictionary<TEntity, TModel> CreateDictionary<TEntity, TModel>()
        {
            return new Dictionary<TEntity, TModel>(new ReferenceEqualityComparer<TEntity>());
        }

        #region StochasticSoilModelEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="StochasticSoilModelEntity"/> and the
        /// <see cref="StochasticSoilModel"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> that was read.</param>
        /// <param name="model">The <see cref="StochasticSoilModel"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(StochasticSoilModelEntity entity, StochasticSoilModel model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            stochasticSoilModels[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <see cref="StochasticSoilModelEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return stochasticSoilModels.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="StochasticSoilModel"/> which was read for the given <see cref="StochasticSoilModelEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilModelEntity"/> for which a
        /// read operation has been registered.</param>
        /// <returns>The constructed <see cref="StochasticSoilModel"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been registered for 
        /// <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(StochasticSoilModelEntity)"/> to find out 
        /// whether a read operation has been registered for <paramref name="entity"/>.</remarks>
        internal StochasticSoilModel Get(StochasticSoilModelEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                return stochasticSoilModels[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region StochasticSoilProfileEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="StochasticSoilProfileEntity"/> and 
        /// the <see cref="StochasticSoilProfile"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> that was read.</param>
        /// <param name="model">The <see cref="StochasticSoilProfile"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(StochasticSoilProfileEntity entity, StochasticSoilProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            stochasticSoilProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <see cref="StochasticSoilProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(StochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return stochasticSoilProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="StochasticSoilProfile"/> which was read for the given
        /// <see name="StochasticSoilProfileEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="StochasticSoilProfileEntity"/> for which
        /// a read operation has been registered.</param>
        /// <returns>The constructed <see cref="StochasticSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been registered for 
        /// <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(StochasticSoilProfileEntity)"/> to find out whether a read operation has been registered for
        /// <paramref name="entity"/>.</remarks>
        internal StochasticSoilProfile Get(StochasticSoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                return stochasticSoilProfiles[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region SoilProfileEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <paramref name="entity"/> and the <paramref name="model"/> that
        /// was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> that was read.</param>
        /// <param name="model">The <see cref="PipingSoilProfile"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(SoilProfileEntity entity, PipingSoilProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            soilProfiles[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operations has been registered for the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <see cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(SoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return soilProfiles.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="PipingSoilProfile"/> which was read for the given <paramref name="entity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SoilProfileEntity"/> for which a read operation has been registered.</param>
        /// <returns>The constructed <see cref="PipingSoilProfile"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has been registered for 
        /// <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(SoilProfileEntity)"/> to find out whether a read operation has been registered for
        /// <paramref name="entity"/>.</remarks>
        internal PipingSoilProfile Get(SoilProfileEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                return soilProfiles[entity];
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
        /// <see cref="RingtoetsPipingSurfaceLine"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> that was read.</param>
        /// <param name="model">The <see cref="RingtoetsPipingSurfaceLine"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(SurfaceLineEntity entity, RingtoetsPipingSurfaceLine model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            surfaceLines[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="SurfaceLineEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(SurfaceLineEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return surfaceLines.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="RingtoetsPipingSurfaceLine"/> which was read for the
        /// given <see cref="SurfaceLineEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> for which a read operation
        /// has been registered.</param>
        /// <returns>The constructed <see cref="RingtoetsPipingSurfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(SurfaceLineEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal RingtoetsPipingSurfaceLine Get(SurfaceLineEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                return surfaceLines[entity];
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
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(HydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
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
                throw new ArgumentNullException("entity");
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
                throw new ArgumentNullException("entity");
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

        #region FailureMechanismSectionEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="FailureMechanismSectionEntity"/> and the
        /// <see cref="FailureMechanismSection"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="FailureMechanismSectionEntity"/> that was read.</param>
        /// <param name="model">The <see cref="FailureMechanismSection"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(FailureMechanismSectionEntity entity, FailureMechanismSection model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
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
                throw new ArgumentNullException("entity");
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
                throw new ArgumentNullException("entity");
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
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(DikeProfileEntity entity, DikeProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
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
                throw new ArgumentNullException("entity");
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
                throw new ArgumentNullException("entity");
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
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(ForeshoreProfileEntity entity, ForeshoreProfile model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
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
                throw new ArgumentNullException("entity");
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
                throw new ArgumentNullException("entity");
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

        #region GrassCoverErosionInwardsCalculationEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="GrassCoverErosionInwardsCalculationEntity"/>
        /// and the <see cref="GrassCoverErosionInwardsCalculation"/> that was constructed
        /// with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/>
        /// that was read.</param>
        /// <param name="model">The <see cref="GrassCoverErosionInwardsCalculation"/> that
        /// was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(GrassCoverErosionInwardsCalculationEntity entity, GrassCoverErosionInwardsCalculation model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            grassCoverErosionInwardsCalculations[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="GrassCoverErosionInwardsCalculationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(GrassCoverErosionInwardsCalculationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return grassCoverErosionInwardsCalculations.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="GrassCoverErosionInwardsCalculation"/> which was read
        /// for the given <see cref="GrassCoverErosionInwardsCalculationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionInwardsCalculationEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="GrassCoverErosionInwardsCalculation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(GrassCoverErosionInwardsCalculationEntity)"/>
        /// to find out whether a read operation has been registered for <paramref name="entity"/>.</remarks>
        internal GrassCoverErosionInwardsCalculation Get(GrassCoverErosionInwardsCalculationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                return grassCoverErosionInwardsCalculations[entity];
            }
            catch (KeyNotFoundException e)
            {
                throw new InvalidOperationException(e.Message, e);
            }
        }

        #endregion

        #region GrassCoverErosionOutwardsHydraulicLocationEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/> and the
        /// <see cref="HydraulicBoundaryLocation"/> that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/> that was read.</param>
        /// <param name="model">The <see cref="HydraulicBoundaryLocation"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(GrassCoverErosionOutwardsHydraulicLocationEntity entity, HydraulicBoundaryLocation model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            grassCoverErosionOutwardsHydraulicBoundaryLocations[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(GrassCoverErosionOutwardsHydraulicLocationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return grassCoverErosionOutwardsHydraulicBoundaryLocations.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="HydraulicBoundaryLocation"/> which was read for the
        /// given <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionOutwardsHydraulicLocationEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="HydraulicBoundaryLocation"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(GrassCoverErosionOutwardsHydraulicLocationEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal HydraulicBoundaryLocation Get(GrassCoverErosionOutwardsHydraulicLocationEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                return grassCoverErosionOutwardsHydraulicBoundaryLocations[entity];
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
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(HeightStructureEntity entity, HeightStructure model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
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
                throw new ArgumentNullException("entity");
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
                throw new ArgumentNullException("entity");
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
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(ClosingStructureEntity entity, ClosingStructure model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
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
                throw new ArgumentNullException("entity");
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
                throw new ArgumentNullException("entity");
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
    }
}