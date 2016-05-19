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
using System.Linq;

using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Exceptions;
using Application.Ringtoets.Storage.Properties;

using Core.Common.Base.Geometry;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Update
{
    /// <summary>
    /// Extension methods for <see cref="RingtoetsPipingSurfaceLine"/> related to updating
    /// an <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class RingtoetsPipingSurfaceLineUpdateExtensions
    {
        /// <summary>
        /// Updates a <see cref="SurfaceLineEntity"/> in the database based on the information
        /// of the <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The piping surfaceline to update the database entity for.</param>
        /// <param name="collector">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="collector"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntryPointNotFoundException">When no <see cref="SurfaceLineEntity"/>
        /// can be found in <paramref name="context"/> that matches <paramref name="surfaceLine"/>.</exception>
        internal static void Update(this RingtoetsPipingSurfaceLine surfaceLine, UpdateConversionCollector collector, IRingtoetsEntities context)
        {
            if (collector == null)
            {
                throw new ArgumentNullException("collector");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SurfaceLineEntity entity = GetCorrespondingSurfaceLineEntity(surfaceLine, context);

            entity.Name = surfaceLine.Name;
            entity.ReferenceLineIntersectionX = Convert.ToDecimal(surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            entity.ReferenceLineIntersectionY = Convert.ToDecimal(surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            UpdateGeometry(surfaceLine, entity, context, collector);
            UpdateCharacteristicPoints(surfaceLine, entity, collector);
        }

        private static void UpdateGeometry(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, IRingtoetsEntities context, UpdateConversionCollector collector)
        {
            if (HasGeometryChanges(surfaceLine, entity))
            {
                context.SurfaceLinePointEntities.RemoveRange(entity.SurfaceLinePointEntities);
                UpdateGeometryPoints(surfaceLine, entity, collector);
            }
        }

        private static bool HasGeometryChanges(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity)
        {
            if (surfaceLine.Points.Length != entity.SurfaceLinePointEntities.Count)
            {
                return true;
            }
            var existingSurfaceLinePointEntities = entity.SurfaceLinePointEntities.OrderBy(pe => pe.Order).ToArray();
            for (int i = 0; i < surfaceLine.Points.Length; i++)
            {
                // Note: Point3D is immutable, therefore checking for identity is enough.
                if (surfaceLine.Points[i].StorageId != existingSurfaceLinePointEntities[i].SurfaceLinePointEntityId)
                {
                    return true;
                }
            }
            return false;
        }

        private static void UpdateGeometryPoints(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, UpdateConversionCollector collector)
        {
            int order = 0;
            foreach (Point3D geometryPoint in surfaceLine.Points)
            {
                entity.SurfaceLinePointEntities.Add(geometryPoint.CreateSurfaceLinePoint(collector, order++));
            }
        }

        private static void UpdateCharacteristicPoints(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, UpdateConversionCollector collector)
        {
            UpdateCharacteristicPoint(surfaceLine.BottomDitchDikeSide,
                                      entity.BottomDitchDikeSidePointEntity,
                                      pointEntity => entity.BottomDitchDikeSidePointEntity = pointEntity,
                                      collector);
            UpdateCharacteristicPoint(surfaceLine.BottomDitchPolderSide,
                                      entity.BottomDitchPolderSidePointEntity,
                                      pointEntity => entity.BottomDitchPolderSidePointEntity = pointEntity,
                                      collector);
            UpdateCharacteristicPoint(surfaceLine.DikeToeAtPolder,
                                      entity.DikeToeAtPolderPointEntity,
                                      pointEntity => entity.DikeToeAtPolderPointEntity = pointEntity,
                                      collector);
            UpdateCharacteristicPoint(surfaceLine.DikeToeAtRiver,
                                      entity.DikeToeAtRiverPointEntity,
                                      pointEntity => entity.DikeToeAtRiverPointEntity = pointEntity,
                                      collector);
            UpdateCharacteristicPoint(surfaceLine.DitchDikeSide,
                                      entity.DitchDikeSidePointEntity,
                                      pointEntity => entity.DitchDikeSidePointEntity = pointEntity,
                                      collector);
            UpdateCharacteristicPoint(surfaceLine.DitchPolderSide,
                                      entity.DitchPolderSidePointEntity,
                                      pointEntity => entity.DitchPolderSidePointEntity = pointEntity,
                                      collector);
        }

        private static void UpdateCharacteristicPoint(Point3D characteristicPoint, SurfaceLinePointEntity currentCharacteristicPointEntity,
                                                      Action<SurfaceLinePointEntity> replaceCharacteristicPointEntity,
                                                      UpdateConversionCollector collector)
        {
            if (characteristicPoint == null && currentCharacteristicPointEntity != null)
            {
                replaceCharacteristicPointEntity(null);
            }
            else if (characteristicPoint != null && IsEntityDifferentFromCurrentCharacteristicPoint(characteristicPoint, currentCharacteristicPointEntity))
            {
                replaceCharacteristicPointEntity(collector.GetSurfaceLinePoint(characteristicPoint));
            }
        }

        private static bool IsEntityDifferentFromCurrentCharacteristicPoint(Point3D characteristicPoint, SurfaceLinePointEntity currentCharacteristicPointEntity)
        {
            return currentCharacteristicPointEntity == null ||
                   characteristicPoint.StorageId != currentCharacteristicPointEntity.SurfaceLinePointEntityId;
        }

        /// <summary>
        /// Gets the <see cref="SurfaceLineEntity"/> from a <see cref="IRingtoetsEntities"/>
        /// corresponding to an already saved <see cref="RingtoetsPipingSurfaceLine"/> instance.
        /// </summary>
        /// <param name="surfaceLine">The surface line.</param>
        /// <param name="context">The context.</param>
        /// <returns>The <see cref="SurfaceLineEntity"/>.</returns>
        /// <exception cref="EntryPointNotFoundException">When no <see cref="SurfaceLineEntity"/>
        /// can be found in <paramref name="context"/> that matches <paramref name="surfaceLine"/>.</exception>
        private static SurfaceLineEntity GetCorrespondingSurfaceLineEntity(RingtoetsPipingSurfaceLine surfaceLine, IRingtoetsEntities context)
        {
            try
            {
                return context.SurfaceLineEntities.Single(sle => sle.SurfaceLineEntityId == surfaceLine.StorageId);
            }
            catch (InvalidOperationException exception)
            {
                string message = string.Format(Resources.Error_Entity_Not_Found_0_1,
                                               typeof(SurfaceLineEntity).Name,
                                               surfaceLine.StorageId);
                throw new EntityNotFoundException(message, exception);
            }
        }
    }
}