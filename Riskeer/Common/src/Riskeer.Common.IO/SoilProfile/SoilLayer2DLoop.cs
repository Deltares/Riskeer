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
using Riskeer.Common.IO.Properties;

namespace Riskeer.Common.IO.SoilProfile
{
    /// <summary>
    /// Class containing the (outer) geometry of a 2D soil layer.
    /// </summary>
    public class SoilLayer2DLoop
    {
        /// <summary>
        /// Creates a new instance of <see cref="SoilLayer2DLoop"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="segments"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="segments"/> do not form a loop.</exception>
        public SoilLayer2DLoop(Segment2D[] segments)
        {
            if (segments == null)
            {
                throw new ArgumentNullException(nameof(segments));
            }

            CheckValidLoop(segments);

            Segments = segments;
        }

        /// <summary>
        /// Gets the segments of the loop.
        /// </summary>
        /// <remarks>The points in the segments are ordered.</remarks>
        public Segment2D[] Segments { get; }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((SoilLayer2DLoop) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = 397;

                foreach (Segment2D segment in Segments)
                {
                    hashCode = (hashCode * 397) ^ segment.GetHashCode();
                }

                return hashCode;
            }
        }

        private bool Equals(SoilLayer2DLoop other)
        {
            return Segments.SequenceEqual(other.Segments);
        }

        /// <summary>
        /// Validates that <paramref name="segments"/> form a loop.
        /// </summary>
        /// <param name="segments">The segments to validate.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="segments"/> do not form a loop.</exception>
        /// <remarks>The points in <paramref name="segments"/> must be ordered.</remarks>
        private static void CheckValidLoop(Segment2D[] segments)
        {
            if (segments.Length == 1 || !IsValidLoop(segments))
            {
                throw new ArgumentException(Resources.Loop_contains_disconnected_segments);
            }
        }

        private static bool IsValidLoop(Segment2D[] segments)
        {
            int segmentCount = segments.Length;
            if (segmentCount == 2)
            {
                return segments[0].Equals(segments[1]);
            }

            for (var i = 0; i < segmentCount; i++)
            {
                Segment2D segmentA = segments[i];
                Segment2D segmentB = segments[(i + 1) % segmentCount];

                if (!segmentA.SecondPoint.Equals(segmentB.FirstPoint))
                {
                    return false;
                }
            }

            return true;
        }
    }
}