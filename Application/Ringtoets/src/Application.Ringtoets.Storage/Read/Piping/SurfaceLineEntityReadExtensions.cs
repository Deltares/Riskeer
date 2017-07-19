// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.ComponentModel;
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Serializers;
using Core.Common.Base.Geometry;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="RingtoetsPipingSurfaceLine"/>
    /// based on the <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class SurfaceLineEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="SurfaceLineEntity"/> and use the information to construct
        /// a <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to create
        /// <see cref="RingtoetsPipingSurfaceLine"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="RingtoetsPipingSurfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="SurfaceLineEntity.PointsXml"/> 
        /// of <paramref name="entity"/> is <c>null</c> or empty.</exception>
        internal static RingtoetsPipingSurfaceLine Read(this SurfaceLineEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }
            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = entity.Name,
                ReferenceLineIntersectionWorldPoint = new Point2D(
                    entity.ReferenceLineIntersectionX.ToNullAsNaN(),
                    entity.ReferenceLineIntersectionY.ToNullAsNaN())
            };
            entity.ReadSurfaceLineGeometryAndCharacteristicPoints(surfaceLine);

            collector.Read(entity, surfaceLine);

            return surfaceLine;
        }

        private static void ReadSurfaceLineGeometryAndCharacteristicPoints(this SurfaceLineEntity entity, RingtoetsPipingSurfaceLine surfaceLine)
        {
            Point3D[] geometryPoints = new Point3DXmlSerializer().FromXml(entity.PointsXml);
            surfaceLine.SetGeometry(geometryPoints);

            var characteristicPoints = new Dictionary<CharacteristicPointType, Point3D>();
            foreach (CharacteristicPointEntity pointEntity in entity.CharacteristicPointEntities)
            {
                characteristicPoints[(CharacteristicPointType) pointEntity.Type] = new Point3D(pointEntity.X.ToNullAsNaN(),
                                                                                               pointEntity.Y.ToNullAsNaN(),
                                                                                               pointEntity.Z.ToNullAsNaN());
            }
            foreach (KeyValuePair<CharacteristicPointType, Point3D> keyValuePair in characteristicPoints)
            {
                SetCharacteristicPoint(surfaceLine, keyValuePair.Key, keyValuePair.Value);
            }
        }

        private static void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, CharacteristicPointType type, Point3D geometryPoint)
        {
            switch (type)
            {
                case CharacteristicPointType.DikeToeAtRiver:
                    surfaceLine.SetDikeToeAtRiverAt(geometryPoint);
                    break;
                case CharacteristicPointType.DikeToeAtPolder:
                    surfaceLine.SetDikeToeAtPolderAt(geometryPoint);
                    break;
                case CharacteristicPointType.DitchDikeSide:
                    surfaceLine.SetDitchDikeSideAt(geometryPoint);
                    break;
                case CharacteristicPointType.BottomDitchDikeSide:
                    surfaceLine.SetBottomDitchDikeSideAt(geometryPoint);
                    break;
                case CharacteristicPointType.BottomDitchPolderSide:
                    surfaceLine.SetBottomDitchPolderSideAt(geometryPoint);
                    break;
                case CharacteristicPointType.DitchPolderSide:
                    surfaceLine.SetDitchPolderSideAt(geometryPoint);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(type),
                                                           (int) type,
                                                           typeof(CharacteristicPointType));
            }
        }
    }
}