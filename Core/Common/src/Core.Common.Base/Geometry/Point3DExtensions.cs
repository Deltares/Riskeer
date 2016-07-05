﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using MathNet.Numerics.LinearAlgebra;

namespace Core.Common.Base.Geometry
{
    /// <summary>
    /// Extension methods for <see cref="Point3D"/>.
    /// </summary>
    public static class Point3DExtensions
    {
        /// <summary>
        /// This method defines the 'spanning line' as the 2D vector going from <paramref name="startWorldCoordinate"/> 
        /// to <paramref name="endWorldCoordinate"/>. Then the <paramref name="worldCoordinate"/> is projected onto
        /// this vector. Then the local coordinate is determined by taking the length of each vector along the 'spanning line'.
        /// </summary>
        /// <param name="worldCoordinate">The point to project.</param>
        /// <param name="startWorldCoordinate">The start of the 'spanning line'.</param>
        /// <param name="endWorldCoordinate">The end point of the 'spanning line'.</param>
        /// <exception cref="ArgumentNullException">Thrown when either:
        /// <list type="bullet">
        /// <item><paramref name="worldCoordinate"/> is <c>null</c>.</item>
        /// <item><paramref name="startWorldCoordinate"/> is <c>null</c>.</item>
        /// <item><paramref name="endWorldCoordinate"/> is <c>null</c>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="ArgumentException">Thrown when the <paramref name="startWorldCoordinate"/> 
        /// and the <paramref name="endWorldCoordinate"/> are the same.</exception>
        public static Point2D ProjectIntoLocalCoordinates(this Point3D worldCoordinate, Point2D startWorldCoordinate, Point2D endWorldCoordinate)
        {
            if (worldCoordinate == null)
            {
                throw new ArgumentNullException("worldCoordinate");
            }
            if (startWorldCoordinate == null)
            {
                throw new ArgumentNullException("startWorldCoordinate");
            }
            if (endWorldCoordinate == null)
            {
                throw new ArgumentNullException("endWorldCoordinate");
            }

            Point2D worldCoordinate2D = new Point2D(worldCoordinate.X, worldCoordinate.Y);
            Vector<double> vectorToPoint = (worldCoordinate2D - startWorldCoordinate);

            // Determine the 'spanning line' vector:
            Vector<double> spanningVector = endWorldCoordinate - startWorldCoordinate;
            double spanningVectorDotProduct = spanningVector.DotProduct(spanningVector);
            double length = Math.Sqrt(spanningVectorDotProduct);

            if (Math.Abs(length) < 1e-6)
            {
                throw new ArgumentException("The startWorldCoordinate and endWorldCoordinate can't be the same.");
            }

            // Project vector onto the 'spanning vector' to determine its X-coordinate in local coordinates:
            double projectOnSpanningVectorFactor = (vectorToPoint.DotProduct(spanningVector)) / spanningVectorDotProduct;
            double localCoordinateX = projectOnSpanningVectorFactor * length;

            return new Point2D(localCoordinateX, worldCoordinate.Z);
        }
    }
}