// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Geometry;
using Core.Common.Util.Extensions;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.Piping.Primitives;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Serializers;

namespace Riskeer.Storage.Core.Read
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
        /// <exception cref="InvalidEnumArgumentException">Thrown when the <paramref name="entity"/> 
        /// contains an invalid type of characteristic point.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="entity"/> contains a 
        /// characteristic point that is not supported.</exception>
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
        /// <exception cref="InvalidEnumArgumentException">Thrown when the <paramref name="entity"/> 
        /// contains an invalid type of characteristic point.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="entity"/> contains a 
        /// characteristic point that is not supported.</exception>
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

        /// <summary>
        /// Reads the characteristic points from the <paramref name="entity"/> and sets these 
        /// to the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="entity">The entity to read.</param>
        /// <param name="surfaceLine">The surface line to set the characteristic point on.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when the <paramref name="entity"/> 
        /// contains an invalid type of characteristic point.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="entity"/> contains a 
        /// characteristic point that is not supported.</exception>
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

        /// <summary>
        /// Reads the characteristic points from the <paramref name="entity"/> and sets these 
        /// to the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="entity">The entity to read.</param>
        /// <param name="surfaceLine">The surface line to set the characteristic point on.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when the <paramref name="entity"/> 
        /// contains an invalid type of characteristic point.</exception>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="entity"/> contains a 
        /// characteristic point that is not supported.</exception>
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

        /// <summary>
        /// Sets the characteristic point and its coordinate to the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to set the characteristic point on.</param>
        /// <param name="type">The type of characteristic point.</param>
        /// <param name="geometryPoint">The point associated with the characteristic point.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="type"/> is not 
        /// a valid <see cref="PipingCharacteristicPointType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="type"/> is not supported.</exception>
        private static void SetCharacteristicPoint(PipingSurfaceLine surfaceLine,
                                                   PipingCharacteristicPointType type,
                                                   Point3D geometryPoint)
        {
            if (!Enum.IsDefined(typeof(PipingCharacteristicPointType), type))
            {
                throw new InvalidEnumArgumentException(nameof(type),
                                                       (int) type,
                                                       typeof(PipingCharacteristicPointType));
            }

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
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the characteristic point and its coordinate to the <paramref name="surfaceLine"/>.
        /// </summary>
        /// <param name="surfaceLine">The surface line to set the characteristic point on.</param>
        /// <param name="type">The type of characteristic point.</param>
        /// <param name="geometryPoint">The point associated with the characteristic point.</param>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="type"/> is not 
        /// a valid <see cref="MacroStabilityInwardsCharacteristicPointType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="type"/> is not supported.</exception>
        private static void SetCharacteristicPoint(MacroStabilityInwardsSurfaceLine surfaceLine,
                                                   MacroStabilityInwardsCharacteristicPointType type,
                                                   Point3D geometryPoint)
        {
            if (!Enum.IsDefined(typeof(MacroStabilityInwardsCharacteristicPointType), type))
            {
                throw new InvalidEnumArgumentException(nameof(type),
                                                       (int) type,
                                                       typeof(MacroStabilityInwardsCharacteristicPointType));
            }

            switch (type)
            {
                case MacroStabilityInwardsCharacteristicPointType.SurfaceLevelOutside:
                    surfaceLine.SetSurfaceLevelOutsideAt(geometryPoint);
                    break;
                case MacroStabilityInwardsCharacteristicPointType.SurfaceLevelInside:
                    surfaceLine.SetSurfaceLevelInsideAt(geometryPoint);
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
                    throw new NotSupportedException();
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
            return new Point3DCollectionXmlSerializer().FromXml(xml);
        }
    }
}