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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Piping.Primitives.Exceptions;
using Ringtoets.Piping.Primitives.Properties;

namespace Ringtoets.Piping.Primitives
{
    /// <summary>
    /// Definition of a surfaceline for piping.
    /// </summary>
    public class RingtoetsPipingSurfaceLine : Observable
    {
        private const int numberOfDecimalPlaces = 2;
        private Point2D[] localGeometry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsPipingSurfaceLine"/> class.
        /// </summary>
        public RingtoetsPipingSurfaceLine()
        {
            Name = string.Empty;
            Points = new Point3D[0];
            localGeometry = new Point2D[0];
        }

        /// <summary>
        /// Gets or sets the name of the surfaceline.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets the 3D points describing its geometry.
        /// </summary>
        public Point3D[] Points { get; private set; }

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
        /// Gets or sets the reference line intersection point in world coordinates.
        /// </summary>
        public Point2D ReferenceLineIntersectionWorldPoint { get; set; }

        /// <summary>
        /// Gets the 2D points describing the local geometry of the surface line.
        /// </summary>
        public IEnumerable<Point2D> LocalGeometry
        {
            get
            {
                return localGeometry;
            }
        }

        /// <summary>
        /// Sets the geometry of the surfaceline.
        /// </summary>
        /// <param name="points">The collection of points defining the surfaceline geometry.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any element of <paramref name="points"/> is <c>null</c>.</exception>
        public void SetGeometry(IEnumerable<Point3D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points), Resources.RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null);
            }
            if (points.Any(p => p == null))
            {
                throw new ArgumentException(Resources.RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null);
            }
            Points = points.Select(p => new Point3D(p.X, p.Y, p.Z)).ToArray();

            if (Points.Length > 0)
            {
                StartingWorldPoint = Points[0];
                EndingWorldPoint = Points[Points.Length - 1];
            }

            localGeometry = ProjectGeometryToLZ().ToArray();
        }

        /// <summary>
        /// Sets the <see cref="DitchPolderSide"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DitchPolderSide"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetDitchPolderSideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
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
        public void SetBottomDitchPolderSideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
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
        public void SetBottomDitchDikeSideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
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
        public void SetDitchDikeSideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
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
        public void SetDikeToeAtRiverAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
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
        public void SetDikeToeAtPolderAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
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
        public double GetZAtL(RoundedDouble l)
        {
            ValidateHasPoints();

            if (!ValidateInRange(l))
            {
                var localRangeL = new Range<double>(LocalGeometry.First().X, LocalGeometry.Last().X);
                string outOfRangeMessage = string.Format(Resources.RingtoetsPipingSurfaceLine_0_L_needs_to_be_in_Range_1_,
                                                         Resources.RingtoetsPipingSurfaceLine_GetZAtL_Cannot_determine_height,
                                                         localRangeL.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(null, outOfRangeMessage);
            }

            var segments = new Collection<Segment2D>();
            for (var i = 1; i < localGeometry.Length; i++)
            {
                segments.Add(new Segment2D(localGeometry[i - 1], localGeometry[i]));
            }

            IEnumerable<Point2D> intersectionPoints = Math2D.SegmentsIntersectionWithVerticalLine(segments, l).OrderBy(p => p.Y).ToArray();

            const double intersectionTolerance = 1e-2;
            bool equalIntersections = Math.Abs(intersectionPoints.First().Y - intersectionPoints.Last().Y) < intersectionTolerance;

            if (equalIntersections)
            {
                return intersectionPoints.First().Y;
            }

            string message = string.Format(Resources.RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l, l);
            throw new RingtoetsPipingSurfaceLineException(message);
        }

        /// <summary>
        /// Projects the points in <see cref="Points"/> to localized coordinate (LZ-plane) system.
        /// Z-values are retained, and the first point is put a L=0.
        /// </summary>
        /// <returns>Collection of 2D points in the LZ-plane.</returns>
        public RoundedPoint2DCollection ProjectGeometryToLZ()
        {
            int count = Points.Length;
            if (count == 0)
            {
                return new RoundedPoint2DCollection(numberOfDecimalPlaces, Enumerable.Empty<Point2D>());
            }

            Point3D first = Points.First();
            if (count == 1)
            {
                return new RoundedPoint2DCollection(numberOfDecimalPlaces, new[]
                {
                    new Point2D(0.0, first.Z)
                });
            }

            Point3D last = Points.Last();
            var firstPoint = new Point2D(first.X, first.Y);
            var lastPoint = new Point2D(last.X, last.Y);
            return new RoundedPoint2DCollection(numberOfDecimalPlaces, Points.Select(p => p.ProjectIntoLocalCoordinates(firstPoint, lastPoint)));
        }

        /// <summary>
        /// Checks whether <paramref name="localCoordinateL"/> is in range of the geometry projected in local coordinate system 
        /// where the points are ordered on the L-coordinate being monotonically non-decreasing.
        /// </summary>
        /// <param name="localCoordinateL">The local L-coordinate value to check for.</param>
        /// <returns><c>true</c> when local L-coordinate is in range of the local geometry. <c>false</c> otherwise.</returns>
        public bool ValidateInRange(double localCoordinateL)
        {
            Point2D firstLocalPoint = LocalGeometry.First();
            Point2D lastLocalPoint = LocalGeometry.Last();
            var roundedLocalCoordinateL = new RoundedDouble(numberOfDecimalPlaces, localCoordinateL);
            return !(firstLocalPoint.X > roundedLocalCoordinateL) && !(lastLocalPoint.X < roundedLocalCoordinateL);
        }

        /// <summary>
        /// Gets the local coordinate with rounded values based on the geometry of the surface line and the given world coordinate.
        /// </summary>
        /// <param name="worldCoordinate">The world coordinate to get the local coordinate for.</param>
        /// <returns>The local coordinate.</returns>
        public Point2D GetLocalPointFromGeometry(Point3D worldCoordinate)
        {
            int count = Points.Length;
            if (count <= 1)
            {
                return new Point2D(double.NaN, double.NaN);
            }

            Point3D first = Points.First();
            Point3D last = Points.Last();
            var firstPoint = new Point2D(first.X, first.Y);
            var lastPoint = new Point2D(last.X, last.Y);

            Point2D localCoordinate = worldCoordinate.ProjectIntoLocalCoordinates(firstPoint, lastPoint);
            return new Point2D(new RoundedDouble(numberOfDecimalPlaces, localCoordinate.X),
                               new RoundedDouble(numberOfDecimalPlaces, localCoordinate.Y));
        }

        /// <summary>
        /// Copies the property values of the <paramref name="fromSurfaceLine"/> to 
        /// the <see cref="RingtoetsPipingSurfaceLine"/>.
        /// </summary>
        /// <param name="fromSurfaceLine">The <see cref="RingtoetsPipingSurfaceLine"/>
        /// to get the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromSurfaceLine"/>
        /// is <c>null</c>.</exception>
        public void CopyProperties(RingtoetsPipingSurfaceLine fromSurfaceLine)
        {
            if (fromSurfaceLine == null)
            {
                throw new ArgumentNullException(nameof(fromSurfaceLine));
            }

            Name = fromSurfaceLine.Name;
            ReferenceLineIntersectionWorldPoint = fromSurfaceLine.ReferenceLineIntersectionWorldPoint != null
                                                      ? new Point2D(fromSurfaceLine.ReferenceLineIntersectionWorldPoint.X,
                                                                    fromSurfaceLine.ReferenceLineIntersectionWorldPoint.Y)
                                                      : null;
            SetGeometry(fromSurfaceLine.Points);
            ClearCharacteristicPoints();
            SetCharacteristicPoints(fromSurfaceLine);
        }

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
            return Equals((RingtoetsPipingSurfaceLine) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = Name.GetHashCode();
                foreach (Point3D point in Points)
                {
                    hashCode = (hashCode * 397) ^ point.GetHashCode();
                }

                hashCode = (hashCode * 397) ^ (DikeToeAtPolder?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (DikeToeAtRiver?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (DitchDikeSide?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (DitchPolderSide?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (BottomDitchDikeSide?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (BottomDitchPolderSide?.GetHashCode() ?? 0);

                return hashCode;
            }
        }

        public override string ToString()
        {
            return Name;
        }

        private void SetCharacteristicPoints(RingtoetsPipingSurfaceLine fromSurfaceLine)
        {
            if (fromSurfaceLine.BottomDitchDikeSide != null)
            {
                SetBottomDitchDikeSideAt(fromSurfaceLine.BottomDitchDikeSide);
            }
            if (fromSurfaceLine.BottomDitchPolderSide != null)
            {
                SetBottomDitchPolderSideAt(fromSurfaceLine.BottomDitchPolderSide);
            }
            if (fromSurfaceLine.DikeToeAtPolder != null)
            {
                SetDikeToeAtPolderAt(fromSurfaceLine.DikeToeAtPolder);
            }
            if (fromSurfaceLine.DikeToeAtRiver != null)
            {
                SetDikeToeAtRiverAt(fromSurfaceLine.DikeToeAtRiver);
            }
            if (fromSurfaceLine.DitchDikeSide != null)
            {
                SetDitchDikeSideAt(fromSurfaceLine.DitchDikeSide);
            }
            if (fromSurfaceLine.DitchPolderSide != null)
            {
                SetDitchPolderSideAt(fromSurfaceLine.DitchPolderSide);
            }
        }

        private void ClearCharacteristicPoints()
        {
            BottomDitchDikeSide = null;
            BottomDitchPolderSide = null;
            DikeToeAtPolder = null;
            DikeToeAtRiver = null;
            DitchDikeSide = null;
            DitchPolderSide = null;
        }

        private bool Equals(RingtoetsPipingSurfaceLine other)
        {
            return string.Equals(Name, other.Name)
                   && Equals(ReferenceLineIntersectionWorldPoint, other.ReferenceLineIntersectionWorldPoint)
                   && EqualGeometricPoints(other.Points)
                   && EqualCharacteristicPoints(other);
        }

        private bool EqualCharacteristicPoints(RingtoetsPipingSurfaceLine other)
        {
            return Equals(DikeToeAtPolder, other.DikeToeAtPolder)
                   && Equals(DikeToeAtRiver, other.DikeToeAtRiver)
                   && Equals(DitchDikeSide, other.DitchDikeSide)
                   && Equals(DitchPolderSide, other.DitchPolderSide)
                   && Equals(BottomDitchDikeSide, other.BottomDitchDikeSide)
                   && Equals(BottomDitchPolderSide, other.BottomDitchPolderSide);
        }

        private bool EqualGeometricPoints(Point3D[] otherPoints)
        {
            int nrOfOtherPoints = otherPoints.Length;
            if (Points.Length != nrOfOtherPoints)
            {
                return false;
            }

            for (var index = 0; index < Points.Length; index++)
            {
                if (!Points[index].Equals(otherPoints[index]))
                {
                    return false;
                }
            }
            return true;
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
                throw new ArgumentNullException(nameof(point), @"Cannot find a point in geometry using a null point.");
            }
            return Points.FirstOrDefault(p => p.Equals(point));
        }

        private static ArgumentException CreatePointNotInGeometryException(Point3D point, string characteristicPointDescription)
        {
            string message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                           point,
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
    }
}