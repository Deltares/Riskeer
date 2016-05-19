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
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// Class that can be used to keep track of data model objects which were initialized during a read operation
    /// from the database. Can be used to reuse objects when reading an already read entity.
    /// </summary>
    internal class ReadConversionCollector
    {
        private readonly Dictionary<SoilProfileEntity, PipingSoilProfile> soilProfiles = new Dictionary<SoilProfileEntity, PipingSoilProfile>(new ReferenceEqualityComparer<SoilProfileEntity>());
        private readonly Dictionary<SurfaceLinePointEntity, Point3D> surfaceLineGeometryPoints = new Dictionary<SurfaceLinePointEntity, Point3D>(new ReferenceEqualityComparer<SurfaceLinePointEntity>());

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
    }
}