// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Components.DotSpatial.Properties;
using DotSpatial.Topology;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.Projections
{
    /// <summary>
    /// WorldFiles complement images, giving georeferenced information for those images.
    /// The basic idea is to calculate everything based on the top left corner of the image.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Original file: https://github.com/FObermaier/DotSpatial.Plugins/blob/master/DotSpatial.Plugins.BruTileLayer/Reprojection/WorldFile.cs
    /// Original license: http://www.apache.org/licenses/LICENSE-2.0.html
    /// </para>
    /// <para>
    /// Parameters in this object correspond to those of an affine transformation expressed
    /// in homogeneous coordinates. For mathematical background, see: https://en.wikipedia.org/wiki/Transformation_matrix
    /// and https://en.wikipedia.org/wiki/Affine_transformation
    /// </para>
    /// </remarks>
    internal class WorldFile
    {
        private readonly Matrix2D matrix = new Matrix2D();
        private readonly Matrix2D inverse;

        /// <summary>
        /// Creates an instance of <see cref="WorldFile"/>, capturing the variables for the
        /// affine transformation of screen coordinate into world coordinates.
        /// </summary>
        /// <param name="a11">X-component of the pixel width (Sum of scaling along the X-axis
        /// and rotation components).</param>
        /// <param name="a21">Y-component of the pixel width (Sum of rotation and shear components).</param>
        /// <param name="a12">X-component of the pixel height (Sum of rotation and shear components).</param>
        /// <param name="a22">Y-component of the pixel height (Sum of scaling along the Y-axis
        /// and rotation components).</param>
        /// <param name="b1">X-ordinate of the center of the top left pixel (Translation
        /// along the X-axis).</param>
        /// <param name="b2">Y-ordinate of the center of the top left pixel. (Translation
        /// along the Y-axis)</param>
        /// <exception cref="ArgumentException">Thrown when the 'a' input arguments do not
        /// define an invertible matrix.</exception>
        public WorldFile(double a11, double a21, double a12, double a22, double b1, double b2)
        {
            matrix.A11 = a11;
            matrix.A21 = a21;
            matrix.A12 = a12;
            matrix.A22 = a22;

            if (!matrix.IsInvertible)
            {
                throw new ArgumentException(Resources.WorldFile_Not_invertable_transformation_arguments_error);
            }

            inverse = matrix.Inverse();

            B1 = b1;
            B2 = b2;
        }

        /// <summary>
        /// Gets the X-component of the pixel width.
        /// </summary>
        /// <remarks>This is often the sum of the following components:
        /// <list type="bullet">
        /// <item>Scaling along the X-axis.</item>
        /// <item>Rotation about the origin.</item>
        /// </list></remarks>
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
        /// <remarks>This is often the sum of the following components:
        /// <list type="bullet">
        /// <item>Shear in the direction of the X-axis.</item>
        /// <item>Rotation about the origin.</item>
        /// </list></remarks>
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
        /// <remarks>This is often the sum of the following components:
        /// <list type="bullet">
        /// <item>Shear in the direction of the Y-axis.</item>
        /// <item>Rotation about the origin.</item>
        /// </list></remarks>
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
        /// <remarks>Typically this value is negative due to the orientation of the Y-axis
        /// in screen-coordinates being inverted compared to that of the map.
        /// This is often the sum of the following components:
        /// <list type="bullet">
        /// <item>Scaling along the Y-axis.</item>
        /// <item>Rotation about the origin.</item>
        /// </list></remarks>
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
        /// <remarks>This value is often the translation in the direction of the X-axis.</remarks>
        public double B1 { get; }

        /// <summary>
        /// Gets the Y-ordinate of the center of the top left pixel.
        /// </summary>
        /// <remarks>This value is often the translation in the direction of the Y-axis.</remarks>
        public double B2 { get; }

        /// <summary>
        /// Determines the ground coordinate for a given <paramref name="x"/>, <paramref name="y"/> pair.
        /// </summary>
        /// <param name="x">The X pixel.</param>
        /// <param name="y">The Y pixel.</param>
        /// <returns>The ground coordinate.</returns>
        public Coordinate ToWorldCoordinates(int x, int y)
        {
            double resX = B1 + (A11 * x + A21 * y);
            double resY = B2 + (A12 * x + A22 * y);

            return new Coordinate(resX, resY);
        }

        /// <summary>
        /// Determines the ground bounding-ordinate for a given width and height pair.
        /// </summary>
        /// <param name="width">The width pixel.</param>
        /// <param name="height">The height pixel.</param>
        /// <returns>The ground bounding-ordinate.</returns>
        public IPolygon BoundingOrdinatesToWorldCoordinates(int width, int height)
        {
            var ringCoordinates = new List<Coordinate>(5);
            Coordinate leftTop = ToWorldCoordinates(0, 0);
            ringCoordinates.AddRange(new[]
            {
                leftTop,
                ToWorldCoordinates(0, height),
                ToWorldCoordinates(width, 0),
                ToWorldCoordinates(width, height),
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
        public Point ToScreenCoordinates(Coordinate point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point));
            }

            double px = point.X - B1;
            double py = point.Y - B2;

            var x = (int) Math.Round(inverse.A11 * px + inverse.A21 * py,
                                     MidpointRounding.AwayFromZero);
            var y = (int) Math.Round(inverse.A12 * px + inverse.A22 * py,
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
            /// Gets a value indicating that <see cref="Inverse()"/> can be computed.
            /// </summary>
            public bool IsInvertible
            {
                get
                {
                    return Math.Abs(Determinant) > double.Epsilon;
                }
            }

            /// <summary>
            /// Calculates the inverse <see cref="Matrix2D"/> of this instance.
            /// </summary>
            /// <returns>The inverse matrix</returns>
            /// <exception cref="InvalidOperationException">Thrown when <see cref="IsInvertible"/>
            /// is <c>false</c>.</exception>
            public Matrix2D Inverse()
            {
                if (!IsInvertible)
                {
                    throw new InvalidOperationException("Matrix not invertable");
                }

                double det = Determinant;

                return new Matrix2D
                {
                    A11 = A22 / det,
                    A21 = -A21 / det,
                    A12 = -A12 / det,
                    A22 = A11 / det
                };
            }

            /// <summary>
            /// Gets a value indicating the determinant of this matrix.
            /// </summary>
            private double Determinant
            {
                get
                {
                    return A22 * A11 - A21 * A12;
                }
            }
        }
    }
}