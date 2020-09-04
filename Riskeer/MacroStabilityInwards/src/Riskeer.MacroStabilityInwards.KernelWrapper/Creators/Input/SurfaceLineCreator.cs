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
using Deltares.MacroStability.CSharpWrapper.Input;
using Riskeer.MacroStabilityInwards.Primitives;
using CSharpWrapperPoint2D = Deltares.MacroStability.CSharpWrapper.Point2D;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input
{
    /// <summary>
    /// Creates <see cref="SurfaceLine"/> instances which are required in a calculation.
    /// </summary>
    internal static class SurfaceLineCreator
    {
        /// <summary>
        /// Creates a <see cref="SurfaceLine"/> based on information of <paramref name="surfaceLine"/>,
        /// which can be used in a calculation.
        /// </summary>
        /// <param name="surfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/> from
        /// which to take the information.</param>
        /// <returns>A new <see cref="SurfaceLine"/> with information taken from the <see cref="surfaceLine"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="surfaceLine"/> is <c>null</c>.</exception>
        public static SurfaceLine Create(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            if (surfaceLine == null)
            {
                throw new ArgumentNullException(nameof(surfaceLine));
            }

            var cSharpWrapperSurfaceLine = new SurfaceLine();

            if (surfaceLine.Points.Any())
            {
                cSharpWrapperSurfaceLine.CharacteristicPoints = CreateCharacteristicPoints(surfaceLine).ToList();
            }

            return cSharpWrapperSurfaceLine;
        }

        private static IEnumerable<SurfaceLineCharacteristicPoint> CreateCharacteristicPoints(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            SurfaceLineCharacteristicPoint[] characteristicPoints = surfaceLine.LocalGeometry.Select(p => new SurfaceLineCharacteristicPoint
            {
                CharacteristicPoint = CharacteristicPointType.None,
                GeometryPoint = new CSharpWrapperPoint2D(p.X, p.Y)
            }).ToArray();

            for (var i = 0; i < surfaceLine.Points.Count(); i++)
            {
                SetCharacteristicPointType(surfaceLine, characteristicPoints, i);
            }

            return characteristicPoints;
        }

        private static void SetCharacteristicPointType(MacroStabilityInwardsSurfaceLine surfaceLine, IEnumerable<SurfaceLineCharacteristicPoint> characteristicPoints, int index)
        {
            Point3D surfaceLinePoint = surfaceLine.Points.ElementAt(index);
            SurfaceLineCharacteristicPoint characteristicPoint = characteristicPoints.ElementAt(index);

            if (ReferenceEquals(surfaceLine.DitchPolderSide, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.DitchPolderSide;
            }

            if (ReferenceEquals(surfaceLine.BottomDitchPolderSide, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.BottomDitchPolderSide;
            }

            if (ReferenceEquals(surfaceLine.BottomDitchDikeSide, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.BottomDitchDikeSide;
            }

            if (ReferenceEquals(surfaceLine.DitchDikeSide, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.DitchDikeSide;
            }

            if (ReferenceEquals(surfaceLine.DikeToeAtPolder, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.DikeToeAtPolder;
            }

            if (ReferenceEquals(surfaceLine.DikeToeAtRiver, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.DikeToeAtRiver;
            }

            if (ReferenceEquals(surfaceLine.DikeTopAtPolder, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.DikeTopAtPolder;
            }

            if (ReferenceEquals(surfaceLine.ShoulderBaseInside, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.ShoulderBaseInside;
            }

            if (ReferenceEquals(surfaceLine.ShoulderTopInside, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.ShoulderTopInside;
            }

            if (ReferenceEquals(surfaceLine.SurfaceLevelInside, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.SurfaceLevelInside;
            }

            if (ReferenceEquals(surfaceLine.SurfaceLevelOutside, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.SurfaceLevelOutside;
            }

            if (ReferenceEquals(surfaceLine.DikeTopAtRiver, surfaceLinePoint))
            {
                characteristicPoint.CharacteristicPoint = CharacteristicPointType.DikeTopAtRiver;
            }
        }
    }
}