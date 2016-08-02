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

using Application.Ringtoets.Storage.BinaryConverters;
using Application.Ringtoets.Storage.DbContext;

using Core.Common.Base.Geometry;

using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Create.Piping
{
    /// <summary>
    /// Extensions methods for <see cref="RingtoetsPipingSurfaceLine"/> related to creating
    /// a <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class RingtoetsPipingSurfaceLineCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SurfaceLineEntity"/> based on the information of the <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>a new <see cref="AssessmentSectionEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static SurfaceLineEntity Create(this RingtoetsPipingSurfaceLine surfaceLine, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            if (registry.Contains(surfaceLine))
            {
                return registry.Get(surfaceLine);
            }

            var entity = new SurfaceLineEntity
            {
                Name = surfaceLine.Name,
                ReferenceLineIntersectionX = surfaceLine.ReferenceLineIntersectionWorldPoint.X.ToNaNAsNull(),
                ReferenceLineIntersectionY = surfaceLine.ReferenceLineIntersectionWorldPoint.Y.ToNaNAsNull(),
                PointsData = new Point3DBinaryConverter().ToBytes(surfaceLine.Points)
            };
            CreateCharacteristicPointEntities(surfaceLine, registry, entity);

            registry.Register(entity, surfaceLine);

            return entity;
        }

        private static void CreateCharacteristicPointEntities(RingtoetsPipingSurfaceLine surfaceLine, PersistenceRegistry registry, SurfaceLineEntity entity)
        {
            var characteristicPointAssociations = new[]
            {
                Tuple.Create(surfaceLine.BottomDitchPolderSide, CharacteristicPointType.BottomDitchPolderSide),
                Tuple.Create(surfaceLine.BottomDitchDikeSide, CharacteristicPointType.BottomDitchDikeSide),
                Tuple.Create(surfaceLine.DikeToeAtPolder, CharacteristicPointType.DikeToeAtPolder),
                Tuple.Create(surfaceLine.DikeToeAtRiver, CharacteristicPointType.DikeToeAtRiver),
                Tuple.Create(surfaceLine.DitchDikeSide, CharacteristicPointType.DitchDikeSide),
                Tuple.Create(surfaceLine.DitchPolderSide, CharacteristicPointType.DitchPolderSide)
            };
            foreach (Tuple<Point3D, CharacteristicPointType> characteristicPointToSave in characteristicPointAssociations.Where(t => t.Item1 != null))
            {
                var characteristicPointEntity = CreateCharacteristicPointEntity(characteristicPointToSave.Item1,
                                                                                characteristicPointToSave.Item2,
                                                                                registry);
                entity.CharacteristicPointEntities.Add(characteristicPointEntity);
            }
        }

        private static CharacteristicPointEntity CreateCharacteristicPointEntity(Point3D point, CharacteristicPointType type, PersistenceRegistry registry)
        {
            var entity = new CharacteristicPointEntity
            {
                Type = (short)type,
                X = point.X.ToNaNAsNull(),
                Y = point.Y.ToNaNAsNull(),
                Z = point.Z.ToNaNAsNull()
            };
            registry.Register(entity, point);
            return entity;
        }
    }
}