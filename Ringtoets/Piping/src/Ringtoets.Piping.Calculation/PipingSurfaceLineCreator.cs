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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Deltares.WTIPiping;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.KernelWrapper
{
    /// <summary>
    /// Creates <see cref="Deltares.WTIPiping.PipingSurfaceLine"/> instances which are required by the <see cref="PipingCalculator"/>.
    /// </summary>
    internal static class PipingSurfaceLineCreator
    {
        /// <summary>
        /// Creates a <see cref="Deltares.WTIPiping.PipingSurfaceLine"/> for the kernel
        /// given different surface line.
        /// </summary>
        /// <param name="line">The surface line configured in the Ringtoets application.</param>
        /// <returns>The surface line to be consumed by the kernel.</returns>
        public static PipingSurfaceLine Create(RingtoetsPipingSurfaceLine line)
        {
            var surfaceLine = new PipingSurfaceLine
            {
                Name = line.Name
            };
            if (line.Points.Any())
            {
                surfaceLine.Points.AddRange(CreatePoints(line));
            }
            
            return surfaceLine;
        }

        private static IEnumerable<PipingPoint> CreatePoints(RingtoetsPipingSurfaceLine line)
        {
            var projectedPoints = line.ProjectGeometryToLZ().ToArray();
            var pipingPoints = new List<PipingPoint>();
            for (int i = 0; i < line.Points.Length; i++)
            {
                var pipingPoint = CreatePoint(line, projectedPoints, i);
                pipingPoints.Add(pipingPoint);
            }
            return pipingPoints;
        }

        private static PipingPoint CreatePoint(RingtoetsPipingSurfaceLine line, Point2D[] projectedPoints, int index)
        {
            var surfaceLinePoint = line.Points[index];
            var projectedPoint = projectedPoints[index];
            var pipingPoint = new PipingPoint(projectedPoint.X, 0.0, projectedPoint.Y);

            if (ReferenceEquals(line.DitchPolderSide, surfaceLinePoint))
            {
                pipingPoint.Type = PipingCharacteristicPointType.DitchPolderSide;
            }
            if (ReferenceEquals(line.BottomDitchPolderSide, surfaceLinePoint))
            {
                pipingPoint.Type = PipingCharacteristicPointType.BottomDitchPolderSide;
            }
            if (ReferenceEquals(line.BottomDitchDikeSide, surfaceLinePoint))
            {
                pipingPoint.Type = PipingCharacteristicPointType.BottomDitchDikeSide;
            }
            if (ReferenceEquals(line.DitchDikeSide, surfaceLinePoint))
            {
                pipingPoint.Type = PipingCharacteristicPointType.DitchDikeSide;
            }
            if (ReferenceEquals(line.DikeToeAtPolder, surfaceLinePoint))
            {
                pipingPoint.Type = PipingCharacteristicPointType.DikeToeAtPolder;
            }

            return pipingPoint;
        }
    }
}