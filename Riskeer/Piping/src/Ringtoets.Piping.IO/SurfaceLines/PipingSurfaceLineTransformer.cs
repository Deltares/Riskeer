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
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.SurfaceLines
{
    /// <summary>
    /// Transforms generic <see cref="SurfaceLine"/> into piping specific <see cref="PipingSurfaceLine"/>.
    /// </summary>
    public class PipingSurfaceLineTransformer : ISurfaceLineTransformer<PipingSurfaceLine>
    {
        private readonly ReferenceLine referenceLine;

        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLineTransformer"/>.
        /// </summary>
        /// <param name="referenceLine">The reference line to determine locations for the surface
        /// lines for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="referenceLine"/> is <c>null</c>.</exception>
        public PipingSurfaceLineTransformer(ReferenceLine referenceLine)
        {
            if (referenceLine == null)
            {
                throw new ArgumentNullException(nameof(referenceLine));
            }

            this.referenceLine = referenceLine;
        }

        public PipingSurfaceLine Transform(SurfaceLine surfaceLine, CharacteristicPoints characteristicPoints)
        {
            Point2D intersectionPoint = surfaceLine.GetSingleReferenceLineIntersection(referenceLine);

            var pipingSurfaceLine = new PipingSurfaceLine(surfaceLine.Name ?? string.Empty);
            pipingSurfaceLine.SetGeometry(surfaceLine.Points);

            pipingSurfaceLine.SetCharacteristicPoints(characteristicPoints);

            pipingSurfaceLine.ReferenceLineIntersectionWorldPoint = intersectionPoint;

            return pipingSurfaceLine;
        }
    }
}