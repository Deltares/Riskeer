// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Deltares.MacroStability.Geometry;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="SurfaceLine2"/> instances which are required in a calculation.
    /// </summary>
    internal static class SurfaceLineCreator
    {
        /// <summary>
        /// Creates a <see cref="SurfaceLine2"/> based on information of <paramref name="surfaceLine"/>,
        /// which can be used in a calculation.
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

            var wtiSurfaceLine = new SurfaceLine2
            {
                Name = surfaceLine.Name
            };

            if (surfaceLine.Points.Any())
            {
                GeometryPoint[] geometryPoints = surfaceLine.LocalGeometry.Select(projectedPoint => new GeometryPoint(projectedPoint.X, projectedPoint.Y)).ToArray();

                var geometry = new GeometryPointString();
                ((List<GeometryPoint>) geometry.Points).AddRange(geometryPoints);
                wtiSurfaceLine.Geometry = geometry;

                foreach (CharacteristicPoint characteristicPoint in CreateCharacteristicPoints(surfaceLine, geometryPoints).ToArray())
                {
                    wtiSurfaceLine.CharacteristicPoints.Add(characteristicPoint);
                }
            }

            wtiSurfaceLine.Geometry.SyncCalcPoints();

            return wtiSurfaceLine;
        }

        private static IEnumerable<CharacteristicPoint> CreateCharacteristicPoints(MacroStabilityInwardsSurfaceLine surfaceLine, GeometryPoint[] geometryPoints)
        {
            var characteristicPoints = new List<CharacteristicPoint>();

            for (var i = 0; i < surfaceLine.Points.Count(); i++)
            {
                characteristicPoints.AddRange(CreateCharacteristicPoint(surfaceLine, geometryPoints, i));
            }

            return characteristicPoints;
        }

        private static IEnumerable<CharacteristicPoint> CreateCharacteristicPoint(MacroStabilityInwardsSurfaceLine surfaceLine, GeometryPoint[] geometryPoints, int index)
        {
            Point3D surfaceLinePoint = surfaceLine.Points.ElementAt(index);
            GeometryPoint geometryPoint = geometryPoints[index];

            var characteristicPoints = new List<CharacteristicPoint>();

            if (ReferenceEquals(surfaceLine.DitchPolderSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.DitchPolderSide));
            }

            if (ReferenceEquals(surfaceLine.BottomDitchPolderSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.BottomDitchPolderSide));
            }

            if (ReferenceEquals(surfaceLine.BottomDitchDikeSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.BottomDitchDikeSide));
            }

            if (ReferenceEquals(surfaceLine.DitchDikeSide, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.DitchDikeSide));
            }

            if (ReferenceEquals(surfaceLine.DikeToeAtPolder, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.DikeToeAtPolder));
            }

            if (ReferenceEquals(surfaceLine.DikeToeAtRiver, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.DikeToeAtRiver));
            }

            if (ReferenceEquals(surfaceLine.DikeTopAtPolder, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.DikeTopAtPolder));
            }

            if (ReferenceEquals(surfaceLine.ShoulderBaseInside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.ShoulderBaseInside));
            }

            if (ReferenceEquals(surfaceLine.ShoulderTopInside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.ShoulderTopInside));
            }

            if (ReferenceEquals(surfaceLine.SurfaceLevelInside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.SurfaceLevelInside));
            }

            if (ReferenceEquals(surfaceLine.SurfaceLevelOutside, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.SurfaceLevelOutside));
            }

            if (ReferenceEquals(surfaceLine.DikeTopAtRiver, surfaceLinePoint))
            {
                characteristicPoints.Add(CreateCharacteristicPointOfType(geometryPoint, CharacteristicPointType.DikeTopAtRiver));
            }

            return characteristicPoints;
        }

        private static CharacteristicPoint CreateCharacteristicPointOfType(GeometryPoint geometryPoints, CharacteristicPointType pointType)
        {
            return new CharacteristicPoint
            {
                CharacteristicPointType = pointType,
                GeometryPoint = geometryPoints
            };
        }
    }
}