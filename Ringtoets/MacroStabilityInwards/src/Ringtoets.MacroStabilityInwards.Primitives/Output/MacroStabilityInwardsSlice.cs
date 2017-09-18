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
using Core.Common.Base.Geometry;

namespace Ringtoets.MacroStabilityInwards.Primitives.Output
{
    /// <summary>
    /// The slice result of a macro stability calculation.
    /// </summary>
    public class MacroStabilityInwardsSlice
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSlice"/>.
        /// </summary>
        /// <param name="topLeftPoint">The top left point of the slice.</param>
        /// <param name="topRightPoint">The top right point of the slice.</param>
        /// <param name="bottomLeftPoint">The bottom left point of the slice.</param>
        /// <param name="bottomRightPoint">The bottom right point of the slice.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public MacroStabilityInwardsSlice(Point2D topLeftPoint, Point2D topRightPoint,
                                          Point2D bottomLeftPoint, Point2D bottomRightPoint)
        {
            if (topLeftPoint == null)
            {
                throw new ArgumentNullException(nameof(topLeftPoint));
            }
            if (topRightPoint == null)
            {
                throw new ArgumentNullException(nameof(topRightPoint));
            }
            if (bottomLeftPoint == null)
            {
                throw new ArgumentNullException(nameof(bottomLeftPoint));
            }
            if (bottomRightPoint == null)
            {
                throw new ArgumentNullException(nameof(bottomRightPoint));
            }

            TopLeftPoint = topLeftPoint;
            TopRightPoint = topRightPoint;
            BottomLeftPoint = bottomLeftPoint;
            BottomRightPoint = bottomRightPoint;
        }

        /// <summary>
        /// Gets the top left point of the slice.
        /// </summary>
        public Point2D TopLeftPoint { get; }

        /// <summary>
        /// Gets the top right point of the slice.
        /// </summary>
        public Point2D TopRightPoint { get; }

        /// <summary>
        /// Gets the bottom left point of the slice.
        /// </summary>
        public Point2D BottomLeftPoint { get; }

        /// <summary>
        /// Gets the bottom right point of the slice.
        /// </summary>
        public Point2D BottomRightPoint { get; }
    }
}