// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using DotSpatial.Topology;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.Layer.BruTile.Projections
{
    /// <summary>
    /// WorldFiles complement images, giving georeferenced information for those images.
    /// The basic idea is to calculate everything based on the top left corner of the image.
    /// </summary>
    /// <remarks>
    /// Original file: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/WorldFile.cs
    /// </remarks>
    internal class WorldFile
    {
        private readonly Matrix2D matrix = new Matrix2D();
        private readonly Matrix2D inverse;

        /// <summary>
        /// Creates an instance of <see cref="WorldFile"/>.
        /// </summary>
        /// <param name="a11">X-component of the pixel width.</param>
        /// <param name="a21">Y-component of the pixel width.</param>
        /// <param name="a12">X-component of the pixel height.</param>
        /// <param name="a22">Y-component of the pixel height.</param>
        /// <param name="b1">X-ordinate of the center of the top left pixel.</param>
        /// <param name="b2">Y-ordinate of the center of the top left pixel.</param>
        public WorldFile(double a11 = 1d, double a21 = 0d, double a12 = 0d, double a22 = -1, double b1 = 0d, double b2 = 0d)
        {
            matrix.A11 = a11;
            matrix.A21 = a21;
            matrix.A12 = a12;
            matrix.A22 = a22;
            inverse = matrix.Inverse();

            B1 = b1;
            B2 = b2;
        }

        /// <summary>
        /// Gets the X-component of the pixel width.
        /// </summary>
        public double A11
        {
            get
            {
                return matrix.A11;
            }
        }

        /// <summary>
        /// Gets the Y-component of the pixel width.
        /// </summary>
        public double A21
        {
            get
            {
                return matrix.A21;
            }
        }

        /// <summary>
        /// Gets the X-component of the pixel height.
        /// </summary>
        public double A12
        {
            get
            {
                return matrix.A12;
            }
        }

        /// <summary>
        /// Gets the Y-component of the pixel height.
        /// </summary>
        /// <remarks>This value is negative most of the time.</remarks>
        public double A22
        {
            get
            {
                return matrix.A22;
            }
        }

        /// <summary>
        /// Gets the X-ordinate of the center of the top left pixel.
        /// </summary>
        public double B1 { get; }

        /// <summary>
        /// Gets the Y-ordinate of the center of the top left pixel.
        /// </summary>
        public double B2 { get; }

        /// <summary>
        /// Determines the ground coordinate for a given <paramref name="x"/>, <paramref name="y"/> pair.
        /// </summary>
        /// <param name="x">The X pixel.</param>
        /// <param name="y">The Y pixel.</param>
        /// <returns>The ground coordinate.</returns>
        public Coordinate ToGround(int x, int y)
        {
            double resX = B1 + (A11*x + A21*y);
            double resY = B2 + (A12*x + A22*y);

            return new Coordinate(resX, resY);
        }

        /// <summary>
        /// Determines the ground bounding-ordinate for a given width and height pair.
        /// </summary>
        /// <param name="width">The width pixel.</param>
        /// <param name="height">The height pixel.</param>
        /// <returns>The ground bounding-ordinate.</returns>
        public IPolygon ToGroundBounds(int width, int height)
        {
            var ringCoordinates = new List<Coordinate>(5);
            var leftTop = ToGround(0, 0);
            ringCoordinates.AddRange(new[]
            {
                leftTop,
                ToGround(0, height),
                ToGround(width, 0),
                ToGround(width, height),
                leftTop
            });

            ILinearRing ring = GeometryFactory.Default.CreateLinearRing(ringCoordinates);
            return GeometryFactory.Default.CreatePolygon(ring, null);
        }

        /// <summary>
        /// Transforms the coordinate to pixel coordinates.
        /// </summary>
        /// <param name="point">The coordinate.</param>
        /// <returns>The location in pixel coordinates.</returns>
        /// <exception cref="ArgumentNullException">Throws when <paramref name="point"/>
        /// is <c>null</c>.</exception>
        public Point ToRaster(Coordinate point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            double px = point.X - B1;
            double py = point.Y - B2;

            int x = (int) Math.Round(inverse.A11*px + inverse.A21*py,
                                     MidpointRounding.AwayFromZero);
            int y = (int) Math.Round(inverse.A12*px + inverse.A22*py,
                                     MidpointRounding.AwayFromZero);

            return new Point(x, y);
        }

        private class Matrix2D
        {
            /// <summary>
            /// Gets the X-component of the pixel width.
            /// </summary>
            public double A11 { get; set; }

            /// <summary>
            /// Gets the Y-component of the pixel width.
            /// </summary>
            public double A21 { get; set; }

            /// <summary>
            /// Gets the X-component of the pixel height.
            /// </summary>
            public double A12 { get; set; }

            /// <summary>
            /// Gets the Y-component of the pixel height (negative most of the time).
            /// </summary>
            public double A22 { get; set; }

            /// <summary>
            /// Calculates the inverse <see cref="Matrix2D"/> of this instance.
            /// </summary>
            /// <returns>The inverse matrix</returns>
            /// <exception cref="InvalidOperationException">Thrown when <see cref="IsInvertible"/>
            /// is <c>false</c>.</exception>
            /// <exception cref="Exception"/>
            public Matrix2D Inverse()
            {
                if (!IsInvertible)
                {
                    throw new InvalidOperationException("Matrix not invertible");
                }

                double det = Determinant;

                return new Matrix2D
                {
                    A11 = A22/det,
                    A21 = -A21/det,
                    A12 = -A12/det,
                    A22 = A11/det
                };
            }

            /// <summary>
            /// Gets a value indicating the determinant of this matrix.
            /// </summary>
            private double Determinant
            {
                get
                {
                    return A22*A11 - A21*A12;
                }
            }

            /// <summary>
            /// Gets a value indicating that <see cref="Inverse()"/> can be computed.
            /// </summary>
            private bool IsInvertible
            {
                get
                {
                    return Determinant != 0d;
                }
            }
        }
    }
}