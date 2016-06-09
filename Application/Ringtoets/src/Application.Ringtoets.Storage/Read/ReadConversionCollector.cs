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

using Core.Common.Base.Geometry;
using Core.Common.Utils;

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
        private readonly Dictionary<StochasticSoilModelEntity, StochasticSoilModel> stochasticSoilModels = new Dictionary<StochasticSoilModelEntity, StochasticSoilModel>(new ReferenceEqualityComparer<StochasticSoilModelEntity>());
        private readonly Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile> stochasticSoilProfiles = new Dictionary<StochasticSoilProfileEntity, StochasticSoilProfile>(new ReferenceEqualityComparer<StochasticSoilProfileEntity>());
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = new Dictionary<SoilProfileEntity, PipingSoilProfile>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine> surfaceLines = new Dictionary<SurfaceLineEntity, RingtoetsPipingSurfaceLine>(new ReferenceEqualityComparer<SurfaceLineEntity>());
        private readonly Dictionary<SurfaceLinePointEntity, Point3D> surfaceLineGeometryPoints = new Dictionary<SurfaceLinePointEntity, Point3D>(new ReferenceEqualityComparer<SurfaceLinePointEntity>());
        private readonly Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation> hydraulicBoundaryLocations = new Dictionary<HydraulicLocationEntity, HydraulicBoundaryLocation>(new ReferenceEqualityComparer<HydraulicLocationEntity>());

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

        #region SurfaceLinePointEntity: Read, Contains, Get

        /// <summary>
        /// Registers a read operation for <see cref="SurfaceLinePointEntity"/> and the
        /// <see cref="Point3D"/> (that is part of <see cref="RingtoetsPipingSurfaceLine.Points"/>)
        /// that was constructed with the information.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLinePointEntity"/> that was read.</param>
        /// <param name="model">The <see cref="Point3D"/> that was constructed.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="entity"/> is <c>null</c></item>
        /// <item><paramref name="model"/> is <c>null</c></item>
        /// </list></exception>
        internal void Read(SurfaceLinePointEntity entity, Point3D model)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            surfaceLineGeometryPoints[entity] = model;
        }

        /// <summary>
        /// Checks whether a read operation has been registered for a given <see cref="SurfaceLinePointEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLinePointEntity"/> to check for.</param>
        /// <returns><c>true</c> if the <paramref cref="entity"/> was read before, <c>false</c> otherwise.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        internal bool Contains(SurfaceLinePointEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            return surfaceLineGeometryPoints.ContainsKey(entity);
        }

        /// <summary>
        /// Obtains the <see cref="Point3D"/> that is part of <see cref="RingtoetsPipingSurfaceLine.Points"/>
        /// which was read for the given <see cref="SurfaceLinePointEntity"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLinePointEntity"/> for which a read
        /// operation has been registered.</param>
        /// <returns>The constructed <see cref="Point3D"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="entity"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when no read operation has
        /// been registered for <paramref name="entity"/>.</exception>
        /// <remarks>Use <see cref="Contains(SurfaceLinePointEntity)"/> to find out whether a
        /// read operation has been registered for <paramref name="entity"/>.</remarks>
        internal Point3D Get(SurfaceLinePointEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }
            try
            {
                return surfaceLineGeometryPoints[entity];
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
    }
}