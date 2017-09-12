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

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for an <see cref="PipingSurfaceLine"/>
    /// based on the <see cref="SurfaceLineEntity"/>.
    /// </summary>
    internal static class SurfaceLineEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="SurfaceLineEntity"/> and use the information to construct
        /// a <see cref="PipingSurfaceLine"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to create
        /// <see cref="PipingSurfaceLine"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="PipingSurfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="collector"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="SurfaceLineEntity.PointsXml"/> 
        /// of <paramref name="entity"/> is <c>null</c> or empty.</exception>
        internal static PipingSurfaceLine Read(this SurfaceLineEntity entity, ReadConversionCollector collector)
        {
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }
            if (collector.Contains(entity))
            {
                return collector.Get(entity);
            }

            var surfaceLine = new PipingSurfaceLine(entity.Name)
            {
                ReferenceLineIntersectionWorldPoint = new Point2D(
                    entity.ReferenceLineIntersectionX.ToNullAsNaN(),
                    entity.ReferenceLineIntersectionY.ToNullAsNaN())
            };
            entity.ReadSurfaceLineGeometryAndCharacteristicPoints(surfaceLine);

            collector.Read(entity, surfaceLine);

            return surfaceLine;
        }

        private static void ReadSurfaceLineGeometryAndCharacteristicPoints(this SurfaceLineEntity entity, PipingSurfaceLine surfaceLine)
        {
            Point3D[] geometryPoints = new Point3DXmlSerializer().FromXml(entity.PointsXml);
            surfaceLine.SetGeometry(geometryPoints);

            var characteristicPoints = new Dictionary<PipingCharacteristicPointType, Point3D>();
            foreach (PipingCharacteristicPointEntity pointEntity in entity.PipingCharacteristicPointEntities)
            {
                characteristicPoints[(PipingCharacteristicPointType) pointEntity.Type] = new Point3D(pointEntity.X.ToNullAsNaN(),
                                                                                               pointEntity.Y.ToNullAsNaN(),
                                                                                               pointEntity.Z.ToNullAsNaN());
            }
            foreach (KeyValuePair<PipingCharacteristicPointType, Point3D> keyValuePair in characteristicPoints)
            {
                SetCharacteristicPoint(surfaceLine, keyValuePair.Key, keyValuePair.Value);
            }
        }

        private static void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, PipingCharacteristicPointType type, Point3D geometryPoint)
        {
            switch (type)
            {
                case PipingCharacteristicPointType.DikeToeAtRiver:
                    surfaceLine.SetDikeToeAtRiverAt(geometryPoint);
                    break;
                case PipingCharacteristicPointType.DikeToeAtPolder:
                    surfaceLine.SetDikeToeAtPolderAt(geometryPoint);
                    break;
                case PipingCharacteristicPointType.DitchDikeSide:
                    surfaceLine.SetDitchDikeSideAt(geometryPoint);
                    break;
                case PipingCharacteristicPointType.BottomDitchDikeSide:
                    surfaceLine.SetBottomDitchDikeSideAt(geometryPoint);
                    break;
                case PipingCharacteristicPointType.BottomDitchPolderSide:
                    surfaceLine.SetBottomDitchPolderSideAt(geometryPoint);
                    break;
                case PipingCharacteristicPointType.DitchPolderSide:
                    surfaceLine.SetDitchPolderSideAt(geometryPoint);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(type),
                                                           (int) type,
                                                           typeof(PipingCharacteristicPointType));
            }
        }
    }
}