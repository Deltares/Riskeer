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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Exceptions;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data
{
    /// <summary>
    /// Base class for mechanism specific surface lines.
    /// </summary>
    public abstract class MechanismSurfaceLineBase : Observable, IMechanismSurfaceLine
    {
        private const int numberOfDecimalPlaces = 2;

        /// <summary>
        /// Initializes a new instance of the <see cref="MechanismSurfaceLineBase"/> class.
        /// </summary>
        /// <param name="name">The name of the surface line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        protected MechanismSurfaceLineBase(string name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
            Points = new Point3D[0];
            LocalGeometry = new RoundedPoint2DCollection(numberOfDecimalPlaces, new Point2D[0]);
        }

        /// <summary>
        /// Gets the name of the surface line.
        /// </summary>
        public string Name { get; protected set; }

        /// <summary>
        /// Gets the 3D points describing the geometry of the surface line.
        /// </summary>
        public IEnumerable<Point3D> Points { get; private set; }

        /// <summary>
        /// Gets the first 3D geometry point defining the surface line in world coordinates.
        /// </summary>
        public Point3D StartingWorldPoint { get; private set; }

        /// <summary>
        /// Gets the last 3D geometry point defining the surface line in world coordinates.
        /// </summary>
        public Point3D EndingWorldPoint { get; private set; }

        /// <summary>
        /// Gets or sets the reference line intersection point in world coordinates.
        /// </summary>
        public Point2D ReferenceLineIntersectionWorldPoint { get; set; }

        /// <summary>
        /// Gets the 2D points describing the local geometry of the surface line.
        /// </summary>
        public RoundedPoint2DCollection LocalGeometry { get; private set; }

        /// <summary>
        /// Sets the geometry of the surface line.
        /// </summary>
        /// <param name="points">The collection of points defining the surface line geometry.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="points"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when any element of <paramref name="points"/> is <c>null</c>.</exception>
        public void SetGeometry(IEnumerable<Point3D> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points), Resources.MechanismSurfaceLineBase_Collection_of_points_for_geometry_is_null);
            }

            if (points.Any(p => p == null))
            {
                throw new ArgumentException(Resources.MechanismSurfaceLineBase_A_point_in_the_collection_was_null);
            }

            Points = points.Select(p => new Point3D(p)).ToArray();

            if (Points.Any())
            {
                StartingWorldPoint = Points.First();
                EndingWorldPoint = Points.Last();
            }

            LocalGeometry = new RoundedPoint2DCollection(numberOfDecimalPlaces, Points.ProjectToLZ());
        }

        /// <summary>
        /// Gets the local coordinate with rounded values based on the geometry of the surface line and the given world coordinate.
        /// </summary>
        /// <param name="worldCoordinate">The world coordinate to get the local coordinate for.</param>
        /// <returns>The local coordinate.</returns>
        public Point2D GetLocalPointFromGeometry(Point3D worldCoordinate)
        {
            if (Points.Count() <= 1)
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
        /// Gets the height of the projected <see cref="MechanismSurfaceLineBase"/> at a L=<paramref name="l"/>.
        /// </summary>
        /// <param name="l">The L coordinate from where to take the height of the <see cref="MechanismSurfaceLineBase"/>.</param>
        /// <returns>The height of the <see cref="MechanismSurfaceLineBase"/> at L=<paramref name="l"/>.</returns>
        /// <exception cref="MechanismSurfaceLineException">Thrown when the <see cref="MechanismSurfaceLineBase"/>
        /// intersection point at <paramref name="l"/> have a significant difference in their y coordinate.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="l"/> is not in range of the LZ-projected <see cref="Points"/>.</exception>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Points"/> is empty.</exception>
        public double GetZAtL(RoundedDouble l)
        {
            ValidateHasPoints();

            if (!ValidateInRange(l))
            {
                var localRangeL = new Range<double>(LocalGeometry.First().X, LocalGeometry.Last().X);
                string outOfRangeMessage = string.Format(Resources.MechanismSurfaceLineBase_0_L_needs_to_be_in_Range_1_,
                                                         Resources.MechanismSurfaceLineBase_GetZAtL_Cannot_determine_height,
                                                         localRangeL.ToString(FormattableConstants.ShowAtLeastOneDecimal, CultureInfo.CurrentCulture));
                throw new ArgumentOutOfRangeException(null, outOfRangeMessage);
            }

            var segments = new Collection<Segment2D>();
            for (var i = 1; i < LocalGeometry.Count(); i++)
            {
                segments.Add(new Segment2D(LocalGeometry.ElementAt(i - 1), LocalGeometry.ElementAt(i)));
            }

            IEnumerable<Point2D> intersectionPoints = Math2D.SegmentsIntersectionWithVerticalLine(segments, l).OrderBy(p => p.Y).ToArray();

            const double intersectionTolerance = 1e-2;
            bool equalIntersections = Math.Abs(intersectionPoints.First().Y - intersectionPoints.Last().Y) < intersectionTolerance;

            if (equalIntersections)
            {
                return intersectionPoints.First().Y;
            }

            string message = string.Format(Resources.MechanismSurfaceLineBase_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l, l);
            throw new MechanismSurfaceLineException(message);
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

        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Finds a point from <see cref="Points"/> which is at the same position as <paramref name="point"/>.
        /// </summary>
        /// <param name="point">The location of a point from <see cref="Points"/>.</param>
        /// <returns>The <see cref="Point3D"/> from <see cref="Points"/> at the same location as <paramref name="point"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        protected Point3D GetPointFromGeometry(Point3D point)
        {
            if (point == null)
            {
                throw new ArgumentNullException(nameof(point), @"Cannot find a point in geometry using a null point.");
            }

            return Points.FirstOrDefault(p => p.Equals(point));
        }

        /// <summary>
        /// Creates a configured <see cref="ArgumentException"/> for the case that a characteristic point
        /// is not in the geometry of the surface line.
        /// </summary>
        /// <param name="point">The point that is not in the geometry.</param>
        /// <param name="characteristicPointDescription">The description of the characteristic point.</param>
        /// <returns>Returns a configured <see cref="ArgumentException"/>.</returns>
        protected static ArgumentException CreatePointNotInGeometryException(Point3D point, string characteristicPointDescription)
        {
            string message = string.Format(Resources.MechanismSurfaceLineBase_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                           point,
                                           characteristicPointDescription);
            return new ArgumentException(message);
        }

        /// <summary>
        /// Checks whether the current <see cref="Points"/> collection is not empty.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when <see cref="Points"/> is empty.</exception>
        private void ValidateHasPoints()
        {
            if (!Points.Any())
            {
                throw new InvalidOperationException(Resources.MechanismSurfaceLineBase_SurfaceLine_has_no_Geometry);
            }
        }
    }
}