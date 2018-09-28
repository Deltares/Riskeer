// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Base.Geometry;
using Core.Common.Util.Extensions;
using Ringtoets.Piping.Primitives;
using Ringtoets.Storage.Core.DbContext;
using Ringtoets.Storage.Core.Serializers;

namespace Ringtoets.Storage.Core.Create.Piping
{
    /// <summary>
    /// Extensions methods for <see cref="PipingSurfaceLine"/> related to creating
    /// a <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class PipingSurfaceLineCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="SurfaceLineEntity"/> based on the information of the <see cref="PipingSurfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <param name="order">Index at which this instance resides inside its parent container.</param>
        /// <returns>A new <see cref="SurfaceLineEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static SurfaceLineEntity Create(this PipingSurfaceLine surfaceLine, PersistenceRegistry registry, int order)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (registry.Contains(surfaceLine))
            {
                return registry.Get(surfaceLine);
            }

            var entity = new SurfaceLineEntity
            {
                Name = surfaceLine.Name.DeepClone(),
                ReferenceLineIntersectionX = surfaceLine.ReferenceLineIntersectionWorldPoint?.X.ToNaNAsNull(),
                ReferenceLineIntersectionY = surfaceLine.ReferenceLineIntersectionWorldPoint?.Y.ToNaNAsNull(),
                PointsXml = new Point3DCollectionXmlSerializer().ToXml(surfaceLine.Points),
                Order = order
            };
            CreateCharacteristicPointEntities(surfaceLine, entity);

            registry.Register(entity, surfaceLine);

            return entity;
        }

        private static void CreateCharacteristicPointEntities(PipingSurfaceLine surfaceLine, SurfaceLineEntity entity)
        {
            Tuple<Point3D, PipingCharacteristicPointType>[] characteristicPointAssociations =
            {
                Tuple.Create(surfaceLine.BottomDitchPolderSide, PipingCharacteristicPointType.BottomDitchPolderSide),
                Tuple.Create(surfaceLine.BottomDitchDikeSide, PipingCharacteristicPointType.BottomDitchDikeSide),
                Tuple.Create(surfaceLine.DikeToeAtPolder, PipingCharacteristicPointType.DikeToeAtPolder),
                Tuple.Create(surfaceLine.DikeToeAtRiver, PipingCharacteristicPointType.DikeToeAtRiver),
                Tuple.Create(surfaceLine.DitchDikeSide, PipingCharacteristicPointType.DitchDikeSide),
                Tuple.Create(surfaceLine.DitchPolderSide, PipingCharacteristicPointType.DitchPolderSide)
            };
            foreach (Tuple<Point3D, PipingCharacteristicPointType> characteristicPointToSave in characteristicPointAssociations.Where(t => t.Item1 != null))
            {
                PipingCharacteristicPointEntity characteristicPointEntity = CreateCharacteristicPointEntity(characteristicPointToSave.Item1,
                                                                                                            characteristicPointToSave.Item2);
                entity.PipingCharacteristicPointEntities.Add(characteristicPointEntity);
            }
        }

        private static PipingCharacteristicPointEntity CreateCharacteristicPointEntity(Point3D point, PipingCharacteristicPointType type)
        {
            var entity = new PipingCharacteristicPointEntity
            {
                Type = Convert.ToByte(type),
                X = point.X.ToNaNAsNull(),
                Y = point.Y.ToNaNAsNull(),
                Z = point.Z.ToNaNAsNull()
            };
            return entity;
        }
    }
}