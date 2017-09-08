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
using System.Linq;
using Core.Common.Base.Geometry;
using Deltares.WTIStability.Data.Geo;
using Deltares.WTIStability.Data.Standard;
using Ringtoets.MacroStabilityInwards.Primitives;
using Point2D = Core.Common.Base.Geometry.Point2D;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Creators
{
    /// <summary>
    /// Creates <see cref="SurfaceLine2"/> instances which are required by the <see cref="MacroStabilityInwardsCalculator"/>.
    /// </summary>
    internal static class SurfaceLineCreator
    {
        /// <summary>
        /// Creates a <see cref="SurfaceLine2"/> based on information of <paramref name="surfaceLine"/>,
        /// which can be used in the <see cref="MacroStabilityInwardsCalculator"/>.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> from
        /// which to take the information.</param>
        /// <returns>A new <see cref="SurfaceLine2"/> with information taken from the <see cref="surfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static SurfaceLine2 Create(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            var WTISurfaceLine = new SurfaceLine2
            {
                Name = surfaceLine.Name
            };

            if (surfaceLine.Points.Any())
            {
                var geometry = new GeometryPointString();
                ((List<GeometryPoint>) geometry.Points).AddRange(surfaceLine.LocalGeometry.Select(CreateGeometryPoint));
                WTISurfaceLine.Geometry = geometry;

                foreach (CharacteristicPoint characteristicPoint in CreateCharacteristicPoints(surfaceLine).ToArray())
                {
                    WTISurfaceLine.CharacteristicPoints.Add(characteristicPoint);
                }
            }

            return WTISurfaceLine;
        }

        private static IEnumerable<CharacteristicPoint> CreateCharacteristicPoints(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            Point2D[] projectedPoints = surfaceLine.LocalGeometry.ToArray();
            var characteristicPoints = new List<CharacteristicPoint>();

            for (var i = 0; i < surfaceLine.Points.Length; i++)
            {
                IEnumerable<CharacteristicPoint> newPoints = CreateCharacteristicPoint(surfaceLine, projectedPoints, i);
                characteristicPoints.AddRange(newPoints);
            }
            return characteristicPoints;
        }

        private static IEnumerable<CharacteristicPoint> CreateCharacteristicPoint(MacroStabilityInwardsSurfaceLine surfaceLine, Point2D[] projectedPoints, int index)
        {
            Point3D surfaceLinePoint = surfaceLine.Points[index];
            Point2D projectedPoint = projectedPoints[index];

            IList<CharacteristicPoint> characteristicPoints = new List<CharacteristicPoint>();

            if (ReferenceEquals(surfaceLine.DitchPolderSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.DitchPolderSide));
            }
            if (ReferenceEquals(surfaceLine.BottomDitchPolderSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.BottomDitchPolderSide));
            }
            if (ReferenceEquals(surfaceLine.BottomDitchDikeSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.BottomDitchDikeSide));
            }
            if (ReferenceEquals(surfaceLine.DitchDikeSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.DitchDikeSide));
            }
            if (ReferenceEquals(surfaceLine.DikeToeAtPolder, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.DikeToeAtPolder));
            }
            if (ReferenceEquals(surfaceLine.DikeToeAtRiver, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.DikeToeAtRiver));
            }
            if (ReferenceEquals(surfaceLine.DikeTopAtPolder, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.DikeTopAtPolder));
            }
            if (ReferenceEquals(surfaceLine.TrafficLoadInside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.TrafficLoadInside));
            }
            if (ReferenceEquals(surfaceLine.TrafficLoadOutside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.TrafficLoadOutside));
            }
            if (ReferenceEquals(surfaceLine.ShoulderBaseInside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.ShoulderBaseInside));
            }
            if (ReferenceEquals(surfaceLine.ShoulderTopInside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.ShoulderTopInside));
            }
            if (ReferenceEquals(surfaceLine.SurfaceLevelInside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.SurfaceLevelInside));
            }
            if (ReferenceEquals(surfaceLine.SurfaceLevelOutside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(projectedPoint, CharacteristicPointType.SurfaceLevelOutside));
            }

            return characteristicPoints;
        }

        private static CharacteristicPoint CreateCharacteristicPointOfType(Point2D projectedPoint, CharacteristicPointType pointType)
        {
            return new CharacteristicPoint
            {
                CharacteristicPointType = pointType,
                GeometryPoint = CreateGeometryPoint(projectedPoint)
            };
        }

        private static GeometryPoint CreateGeometryPoint(Point2D projectedPoint)
        {
            return new GeometryPoint(projectedPoint.X, projectedPoint.Y);
        }
    }
}