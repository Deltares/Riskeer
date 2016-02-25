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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using MathNet.Numerics.LinearAlgebra.Double;
using Ringtoets.Piping.Data.Calculation;
using Ringtoets.Piping.Data.Exceptions;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data
{
    /// <summary>
    /// Definition of a surfaceline for piping.
    /// </summary>
    public class RingtoetsPipingSurfaceLine
    {
        private Point3D[] geometryPoints;

        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsPipingSurfaceLine"/> class.
        /// </summary>
        public RingtoetsPipingSurfaceLine()
        {
            Name = string.Empty;
            geometryPoints = new Point3D[0];
        }

        /// <summary>
        /// Gets or sets the name of the surfaceline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the 3D points describing its geometry.
        /// </summary>
        public Point3D[] Points
        {
            get
            {
                return geometryPoints;
            }
        }

        /// <summary>
        /// Gets or sets the first 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D StartingWorldPoint { get; private set; }

        /// <summary>
        /// Gets or sets the last 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D EndingWorldPoint { get; private set; }

        /// <summary>
        /// Gets the point which characterizes the ditch at polder side.
        /// </summary>
        public Point3D DitchPolderSide { get; private set; }

        /// <summary>
        /// Gets the point which characterizes the bottom of the ditch at polder side.
        /// </summary>
        public Point3D BottomDitchPolderSide { get; private set; }

        /// <summary>
        /// Gets the point which characterizes the bottom of the ditch at dike side.
        /// </summary>
        public Point3D BottomDitchDikeSide { get; private set; }

        /// <summary>
        /// Gets the point which characterizes the ditch at dike side.
        /// </summary>
        public Point3D DitchDikeSide { get; private set; }

        /// <summary>
        /// Gets the point which characterizes the dike toe at river side.
        /// </summary>
        public Point3D DikeToeAtRiver { get; private set; }

        /// <summary>
        /// Gets the point which characterizes the dike toe at polder side.
        /// </summary>
        public Point3D DikeToeAtPolder { get; private set; }

        /// <summary>
        /// Sets the geometry of the surfaceline.
        /// </summary>
        /// <param name="points">The collection of points defining the surfaceline geometry.</param>
        /// <exception cref="ArgumentException">Thrown when any element of <paramref name="points"/> is <c>null</c>.</exception>
        public void SetGeometry(IEnumerable<Point3D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException("points", Resources.RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null);
            }
            if (points.Any(p => p == null))
            {
                throw new ArgumentException(Resources.RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null);
            }
            geometryPoints = points.ToArray();

            if (geometryPoints.Length > 0)
            {
                StartingWorldPoint = geometryPoints[0];
                EndingWorldPoint = geometryPoints[geometryPoints.Length - 1];
            }
        }

        /// <summary>
        /// Sets the <see cref="DitchPolderSide"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DitchPolderSide"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        public void SetDitchPolderSideAt(Point3D point)
        {
            var geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, Resources.CharacteristicPoint_DitchPolderSide);
            }
            DitchPolderSide = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="BottomDitchPolderSide"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="BottomDitchPolderSide"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        public void SetBottomDitchPolderSideAt(Point3D point)
        {
            var geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, Resources.CharacteristicPoint_BottomDitchPolderSide);
            }
            BottomDitchPolderSide = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="BottomDitchDikeSide"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="BottomDitchDikeSide"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        public void SetBottomDitchDikeSideAt(Point3D point)
        {
            var geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, Resources.CharacteristicPoint_BottomDitchDikeSide);
            }
            BottomDitchDikeSide = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="DitchDikeSide"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DitchDikeSide"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        public void SetDitchDikeSideAt(Point3D point)
        {
            var geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, Resources.CharacteristicPoint_DitchDikeSide);
            }
            DitchDikeSide = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="DikeToeAtRiver"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DikeToeAtRiver"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        public void SetDikeToeAtRiverAt(Point3D point)
        {
            var geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, Resources.CharacteristicPoint_DikeToeAtRiver);
            }
            DikeToeAtRiver = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="DikeToeAtPolder"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DikeToeAtPolder"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        public void SetDikeToeAtPolderAt(Point3D point)
        {
            var geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, Resources.CharacteristicPoint_DikeToeAtPolder);
            }
            DikeToeAtPolder = geometryPoint;
        }

        /// <summary>
        /// Gets the height of the projected <see cref="RingtoetsPipingSurfaceLine"/> at a L=<paramref name="l"/>.
        /// </summary>
        /// <param name="l">The L coordinate from where to take the height of the <see cref="RingtoetsPipingSurfaceLine"/>.</param>
        /// <returns>The height of the <see cref="RingtoetsPipingSurfaceLine"/> at L=<paramref name="l"/>.</returns>
        /// <exception cref="RingtoetsPipingSurfaceLineException">Thrown when the <see cref="RingtoetsPipingSurfaceLine"/>
        /// intersection point at <paramref name="l"/> have a significant difference in their y coordinate.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="l"/> is not in range of the LZ-projected <see cref="Points"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Points"/> is empty.</exception>
        public double GetZAtL(double l)
        {
            ValidateHasPoints();

            Point2D[] pointsInLocalCoordinates = ProjectGeometryToLZ().ToArray();

            ValidateInRange(l, pointsInLocalCoordinates);

            var segments = new Collection<Segment2D>();
            for (int i = 1; i < pointsInLocalCoordinates.Length; i++)
            {
                segments.Add(new Segment2D(pointsInLocalCoordinates[i - 1], pointsInLocalCoordinates[i]));
            }

            IEnumerable<Point2D> intersectionPoints = Math2D.SegmentsIntersectionWithVerticalLine(segments, l).OrderBy(p => p.Y).ToArray();

            const double intersectionTolerance = 1e-8;
            bool equalIntersections = Math.Abs(intersectionPoints.First().Y - intersectionPoints.Last().Y) < intersectionTolerance;

            if (equalIntersections)
            {
                return intersectionPoints.First().Y;
            }

            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l, l);
            throw new RingtoetsPipingSurfaceLineException(message);
        }

        /// <summary>
        /// Projects the points in <see cref="Points"/> to localized coordinate (LZ-plane) system.
        /// Z-values are retained, and the first point is put a L=0.
        /// </summary>
        /// <returns>Collection of 2D points in the LZ-plane.</returns>
        public IEnumerable<Point2D> ProjectGeometryToLZ()
        {
            var count = geometryPoints.Length;
            if (count == 0)
            {
                return Enumerable.Empty<Point2D>();
            }

            var localCoordinatesX = new double[count];
            localCoordinatesX[0] = 0.0;
            if (count > 1)
            {
                ProjectPointsAfterFirstOntoSpanningLine(localCoordinatesX);
            }

            var result = new Point2D[count];
            for (int i = 0; i < count; i++)
            {
                var x = localCoordinatesX[i];
                var y = geometryPoints[i].Z;
                result[i] = new Point2D(x, y);
            }
            return result;
        }

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Finds a point from <see cref="Points"/> which is at the same position as <paramref name="point"/>.
        /// </summary>
        /// <param name="point">The location of a point from <see cref="Points"/>.</param>
        /// <returns>The <see cref="Point3D"/> from <see cref="Points"/> at the same location as <paramref name="point"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        private Point3D GetPointFromGeometry(Point3D point)
        {
            if (point == null)
            {
                throw new ArgumentNullException("point", "Cannot find a point in geometry using a null point.");
            }
            return Points.FirstOrDefault(p => p.Equals(point));
        }

        private static ArgumentException CreatePointNotInGeometryException(Point3D point, string characteristicPointDescription)
        {
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point_3_,
                                        point.X,
                                        point.Y,
                                        point.Z,
                                        characteristicPointDescription);
            return new ArgumentException(message);
        }

        /// <summary>
        /// Checks whether the current <see cref="Points"/> collection is not empty.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Points"/> is empty.</exception>
        private void ValidateHasPoints()
        {
            if (!Points.Any())
            {
                throw new InvalidOperationException(Resources.RingtoetsPipingSurfaceLine_SurfaceLine_has_no_Geometry);
            }
        }

        /// <summary>
        /// Checks whether <paramref name="localCoordinateL"/> is in range of the <paramref name="geometryInLocalCoordinates"/>.
        /// </summary>
        /// <param name="localCoordinateL">The value to check for.</param>
        /// <param name="geometryInLocalCoordinates">Geometry projected in local coordinate system where the points are ordered on the
        /// L-coordinate being monotonically non-decreasing</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="localCoordinateL"/> falls outside the L-coordiante span
        /// defined by <paramref name="geometryInLocalCoordinates"/>.</exception>
        private static void ValidateInRange(double localCoordinateL, Point2D[] geometryInLocalCoordinates)
        {
            if (geometryInLocalCoordinates.First().X > localCoordinateL || geometryInLocalCoordinates.Last().X < localCoordinateL)
            {
                var outOfRangeMessage = string.Format(Resources.RingtoetsPipingSurfaceLine_0_L_needs_to_be_in_1_2_range,
                                                      Resources.RingtoetsPipingSurfaceLine_GetZAtL_Cannot_determine_height,
                                                      geometryInLocalCoordinates.First().X,
                                                      geometryInLocalCoordinates.Last().X);
                throw new ArgumentOutOfRangeException("localCoordinateL", outOfRangeMessage);
            }
        }

        /// <summary>
        /// This method defines the 'spanning line' as the 2D vector going from start to end
        /// of the surface line points. Then all except the first point is projected onto
        /// this vector. Then the local coordinates are determined by taking the length of
        /// each vector along the 'spanning line'.
        /// </summary>
        /// <param name="localCoordinatesX">The array into which the projected X-coordinate 
        /// values should be stored. Its <see cref="Array.Length"/> should be the same as
        /// the collection-size of <see cref="geometryPoints"/>.</param>
        private void ProjectPointsAfterFirstOntoSpanningLine(double[] localCoordinatesX)
        {
            // Determine the vectors from the first coordinate to each other coordinate point 
            // in the XY world coordinate plane:
            Point2D[] worldCoordinates = Points.Select(p => new Point2D(p.X, p.Y)).ToArray();
            var worldCoordinateVectors = new Vector[worldCoordinates.Length - 1];
            for (int i = 1; i < worldCoordinates.Length; i++)
            {
                worldCoordinateVectors[i - 1] = worldCoordinates[i] - worldCoordinates[0];
            }

            // Determine the 'spanning line' vector:
            Vector spanningVector = worldCoordinateVectors[worldCoordinateVectors.Length - 1];
            double spanningVectorDotProduct = spanningVector.DotProduct(spanningVector);
            double length = Math.Sqrt(spanningVectorDotProduct);

            // Project each vector onto the 'spanning vector' to determine it's X coordinate in local coordinates:
            for (int i = 0; i < worldCoordinateVectors.Length - 1; i++)
            {
                double projectOnSpanningVectorFactor = (worldCoordinateVectors[i].DotProduct(spanningVector))/
                                                       (spanningVectorDotProduct);
                localCoordinatesX[i + 1] = projectOnSpanningVectorFactor*length;
            }
            localCoordinatesX[localCoordinatesX.Length - 1] = length;
        }
    }
}