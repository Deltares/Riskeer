// Copyright (C) Stichting Deltares 2018. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Extension methods for <see cref="Segment2D"/> objects.
    /// </summary>
    public static class Segment2DExtensions
    {
        /// <summary>
        /// Interpolate the segment on <paramref name="x"/>.
        /// </summary>
        /// <param name="segment">The segment to interpolate on.</param>
        /// <param name="x">The x value to use for interpolation.</param>
        /// <returns>The interpolated y value on the segment or <see cref="double.NaN"/>
        /// when no interpolation was found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="segment"/> is <c>null</c>.</exception>
        public static double Interpolate(this Segment2D segment, double x)
        {
            if (segment == null)
            {
                throw new ArgumentNullException(nameof(segment));
            }

            double differenceInX = segment.SecondPoint.X - segment.FirstPoint.X;
            if (Math.Abs(differenceInX) < 1e-6)
            {
                return double.NaN;
            }

            double m = (segment.SecondPoint.Y - segment.FirstPoint.Y) / differenceInX;
            double b = segment.FirstPoint.Y - (m * segment.FirstPoint.X);

            return m * x + b;
        }
    }
}