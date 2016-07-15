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

using Core.Common.Base.Geometry;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Update.Piping
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
        /// <param name="registry">The object keeping track of update operations.</param>
        /// <param name="context">The context to obtain the existing entity from.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="registry"/> is <c>null</c></item>
        /// <item><paramref name="context"/> is <c>null</c></item>
        /// </list></exception>
        /// <exception cref="EntryPointNotFoundException">When no <see cref="SurfaceLineEntity"/>
        /// can be found in <paramref name="context"/> that matches <paramref name="surfaceLine"/>.</exception>
        internal static void Update(this RingtoetsPipingSurfaceLine surfaceLine, PersistenceRegistry registry, IRingtoetsEntities context)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            SurfaceLineEntity entity = surfaceLine.GetCorrespondingEntity(
                context.SurfaceLineEntities,
                o => o.SurfaceLineEntityId);

            entity.Name = surfaceLine.Name;
            entity.ReferenceLineIntersectionX = Convert.ToDecimal(surfaceLine.ReferenceLineIntersectionWorldPoint.X);
            entity.ReferenceLineIntersectionY = Convert.ToDecimal(surfaceLine.ReferenceLineIntersectionWorldPoint.Y);

            UpdateGeometry(surfaceLine, entity, registry);
            UpdateCharacteristicPoints(surfaceLine, entity, registry);

            registry.Register(entity, surfaceLine);
        }

        private static void UpdateGeometry(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, PersistenceRegistry registry)
        {
            if (HasGeometryChanges(surfaceLine, entity))
            {
                foreach (SurfaceLinePointEntity pointEntity in entity.SurfaceLinePointEntities.ToArray())
                {
                    entity.SurfaceLinePointEntities.Remove(pointEntity);
                }
                UpdateGeometryPoints(surfaceLine, entity, registry);
            }
            else
            {
                var orderedPointEntities = entity.SurfaceLinePointEntities.OrderBy(pe => pe.Order).ToArray();
                for (int i = 0; i < surfaceLine.Points.Length; i++)
                {
                    registry.Register(orderedPointEntities[i], surfaceLine.Points[i]);
                }
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

        private static void UpdateGeometryPoints(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, PersistenceRegistry registry)
        {
            int order = 0;
            foreach (Point3D geometryPoint in surfaceLine.Points)
            {
                entity.SurfaceLinePointEntities.Add(geometryPoint.CreateSurfaceLinePointEntity(registry, order++));
            }
        }

        private static void UpdateCharacteristicPoints(RingtoetsPipingSurfaceLine surfaceLine, SurfaceLineEntity entity, PersistenceRegistry registry)
        {
            CharacteristicPointEntity[] currentCharacteristicPointEntities = entity.SurfaceLinePointEntities
                                                                                   .SelectMany(pe => pe.CharacteristicPointEntities)
                                                                                   .ToArray();

            UpdateCharacteristicPoint(surfaceLine.DikeToeAtRiver, CharacteristicPointType.DikeToeAtRiver,
                                      currentCharacteristicPointEntities, registry);
            UpdateCharacteristicPoint(surfaceLine.DikeToeAtPolder, CharacteristicPointType.DikeToeAtPolder,
                                      currentCharacteristicPointEntities, registry);
            UpdateCharacteristicPoint(surfaceLine.DitchDikeSide, CharacteristicPointType.DitchDikeSide,
                                      currentCharacteristicPointEntities, registry);
            UpdateCharacteristicPoint(surfaceLine.BottomDitchDikeSide, CharacteristicPointType.BottomDitchDikeSide,
                                      currentCharacteristicPointEntities, registry);
            UpdateCharacteristicPoint(surfaceLine.BottomDitchPolderSide, CharacteristicPointType.BottomDitchPolderSide,
                                      currentCharacteristicPointEntities, registry);
            UpdateCharacteristicPoint(surfaceLine.DitchPolderSide, CharacteristicPointType.DitchPolderSide,
                                      currentCharacteristicPointEntities, registry);
        }

        private static void UpdateCharacteristicPoint(Point3D currentCharacteristicPoint, CharacteristicPointType type,
                                                      CharacteristicPointEntity[] currentCharacteristicPointEntities,
                                                      PersistenceRegistry registry)
        {
            short typeValue = (short)type;
            CharacteristicPointEntity characteristicPointEntity = currentCharacteristicPointEntities
                .FirstOrDefault(cpe => cpe.CharacteristicPointType == typeValue);

            if (currentCharacteristicPoint == null && characteristicPointEntity != null)
            {
                characteristicPointEntity.SurfaceLinePointEntity = null;
            }
            else if (currentCharacteristicPoint != null)
            {
                SurfaceLinePointEntity geometryPointEntity = registry.GetSurfaceLinePoint(currentCharacteristicPoint);
                if (characteristicPointEntity == null)
                {
                    characteristicPointEntity = new CharacteristicPointEntity
                    {
                        CharacteristicPointType = typeValue
                    };
                    geometryPointEntity.CharacteristicPointEntities.Add(characteristicPointEntity);
                }
                else if (characteristicPointEntity.SurfaceLinePointEntity != geometryPointEntity)
                {
                    characteristicPointEntity.SurfaceLinePointEntity = geometryPointEntity;
                }
                registry.Register(characteristicPointEntity, currentCharacteristicPoint);
            }
        }
    }
}