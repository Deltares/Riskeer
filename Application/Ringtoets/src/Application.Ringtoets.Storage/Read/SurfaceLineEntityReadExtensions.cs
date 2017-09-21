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
using Core.Common.Utils.Extensions;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.Piping.Primitives;

namespace Application.Ringtoets.Storage.Read
{
    /// <summary>
    /// This class defines extension methods for read operations for a surface line
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
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="SurfaceLineEntity.PointsXml"/> 
        /// of <paramref name="entity"/> is empty.</exception>
        public static PipingSurfaceLine ReadAsPipingSurfaceLine(this SurfaceLineEntity entity,
                                                                ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.ContainsPipingSurfaceLine(entity))
            {
                return collector.GetPipingSurfaceLine(entity);
            }

            var surfaceLine = new PipingSurfaceLine(entity.Name)
            {
                ReferenceLineIntersectionWorldPoint = GetReferenceLineIntersectionWorldPoint(entity)
            };

            surfaceLine.SetGeometry(ReadGeometryPoints(entity.PointsXml));
            entity.ReadCharacteristicPoints(surfaceLine);

            collector.Read(entity, surfaceLine);

            return surfaceLine;
        }

        /// <summary>
        /// Read the <see cref="SurfaceLineEntity"/> and use the information to construct
        /// a <see cref="MacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <param name="entity">The <see cref="SurfaceLineEntity"/> to create
        /// <see cref="MacroStabilityInwardsSurfaceLine"/> for.</param>
        /// <param name="collector">The object keeping track of read operations.</param>
        /// <returns>A new <see cref="MacroStabilityInwardsSurfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="SurfaceLineEntity.PointsXml"/> 
        /// of <paramref name="entity"/> is empty.</exception>
        public static MacroStabilityInwardsSurfaceLine ReadAsMacroStabilityInwardsSurfaceLine(
            this SurfaceLineEntity entity,
            ReadConversionCollector collector)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            if (collector == null)
            {
                throw new ArgumentNullException(nameof(collector));
            }

            if (collector.ContainsMacroStabilityInwardsSurfaceLine(entity))
            {
                return collector.GetMacroStabilityInwardsSurfaceLine(entity);
            }

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(entity.Name)
            {
                ReferenceLineIntersectionWorldPoint = GetReferenceLineIntersectionWorldPoint(entity)
            };

            surfaceLine.SetGeometry(ReadGeometryPoints(entity.PointsXml));
            entity.ReadCharacteristicPoints(surfaceLine);

            collector.Read(entity, surfaceLine);

            return surfaceLine;
        }

        private static Point2D GetReferenceLineIntersectionWorldPoint(SurfaceLineEntity entity)
        {
            return new Point2D(entity.ReferenceLineIntersectionX.ToNullAsNaN(),
                               entity.ReferenceLineIntersectionY.ToNullAsNaN());
        }

        private static void ReadCharacteristicPoints(this SurfaceLineEntity entity, PipingSurfaceLine surfaceLine)
        {
            var characteristicPoints = new Dictionary<PipingCharacteristicPointType, Point3D>();
            foreach (PipingCharacteristicPointEntity pointEntity in entity.PipingCharacteristicPointEntities)
            {
                characteristicPoints[(PipingCharacteristicPointType) pointEntity.Type] = new Point3D(pointEntity.X.ToNullAsNaN(),
                                                                                                     pointEntity.Y.ToNullAsNaN(),
                                                                                                     pointEntity.Z.ToNullAsNaN());
            }

            characteristicPoints.ForEachElementDo(cp => SetCharacteristicPoint(surfaceLine, cp.Key, cp.Value));
        }

        private static void ReadCharacteristicPoints(this SurfaceLineEntity entity, MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            var characteristicPoints = new Dictionary<MacroStabilityInwardsCharacteristicPointType, Point3D>();
            foreach (MacroStabilityInwardsCharacteristicPointEntity pointEntity in entity.MacroStabilityInwardsCharacteristicPointEntities)
            {
                characteristicPoints[(MacroStabilityInwardsCharacteristicPointType) pointEntity.Type] = new Point3D(pointEntity.X.ToNullAsNaN(),
                                                                                                                    pointEntity.Y.ToNullAsNaN(),
                                                                                                                    pointEntity.Z.ToNullAsNaN());
            }

            characteristicPoints.ForEachElementDo(cp => SetCharacteristicPoint(surfaceLine, cp.Key, cp.Value));
        }

        private static void SetCharacteristicPoint(PipingSurfaceLine surfaceLine,
                                                   PipingCharacteristicPointType type,
                                                   Point3D geometryPoint)
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

        private static void SetCharacteristicPoint(MacroStabilityInwardsSurfaceLine surfaceLine,
                                                   MacroStabilityInwardsCharacteristicPointType type,
                                                   Point3D geometryPoint)
        {
            switch (type)
            {
                case MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside:
                    surfaceLine.SetSurfaceLevelOutsideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside:
                    surfaceLine.SetSurfaceLevelInsideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.TrafficLoadOutside:
                    surfaceLine.SetTrafficLoadOutsideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.TrafficLoadInside:
                    surfaceLine.SetTrafficLoadInsideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.ShoulderBaseInside:
                    surfaceLine.SetShoulderBaseInsideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.ShoulderTopInside:
                    surfaceLine.SetShoulderTopInsideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.DikeToeAtRiver:
                    surfaceLine.SetDikeToeAtRiverAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.DikeToeAtPolder:
                    surfaceLine.SetDikeToeAtPolderAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.BottomDitchDikeSide:
                    surfaceLine.SetBottomDitchDikeSideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.BottomDitchPolderSide:
                    surfaceLine.SetBottomDitchPolderSideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.DitchDikeSide:
                    surfaceLine.SetDitchDikeSideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.DikeTopAtRiver:
                    surfaceLine.SetDikeTopAtRiverAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.DikeTopAtPolder:
                    surfaceLine.SetDikeTopAtPolderAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.DitchPolderSide:
                    surfaceLine.SetDitchPolderSideAt(geometryPoint);
                    break;
                default:
                    throw new InvalidEnumArgumentException(nameof(type),
                                                           (int) type,
                                                           typeof(MacroStabilityInwardsCharacteristicPointType));
            }
        }

        /// <summary>
        /// Reads the geometry points.
        /// </summary>
        /// <param name="xml">The xml containing a collection of <see cref="Point3D"/>.</param>
        /// <returns>The read geometry points.</returns>
        /// <exception cref="ArgumentException">Thrown when <paramref name="xml"/> is empty.</exception>
        private static IEnumerable<Point3D> ReadGeometryPoints(string xml)
        {
            return new Point3DXmlSerializer().FromXml(xml);
        }
    }
}