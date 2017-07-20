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
using Ringtoets.Common.Data;
using Ringtoets.MacroStabilityInwards.Primitives.Exceptions;
using Ringtoets.MacroStabilityInwards.Primitives.Properties;

using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Definition of a surfaceline for macro stability inwards.
    /// </summary>
    public class RingtoetsMacroStabilityInwardsSurfaceLine : Observable, IMechanismSurfaceLine
    {
        private const int numberOfDecimalPlaces = 2;
        private Point2D[] localGeometry;

        /// <summary>
        /// Initializes a new instance of the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> class.
        /// </summary>
        public RingtoetsMacroStabilityInwardsSurfaceLine()
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
        /// Gets the first 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D StartingWorldPoint { get; private set; }

        /// <summary>
        /// Gets the last 3D geometry point defining the surfaceline in world coordinates.
        /// </summary>
        public Point3D EndingWorldPoint { get; private set; }

        /// <summary>
        /// Gets the location which generalizes the height of the surface
        /// on the oustide of the polder.
        /// </summary>
        public Point3D SurfaceLevelOutside { get; private set; }

        /// <summary>
        /// Gets the location of dike toe when approaching from outside 
        /// the polder.
        /// </summary>
        public Point3D DikeToeAtRiver { get; private set; }

        /// <summary>
        /// Gets the location of the start of traffic load when approaching 
        /// from outside the polder.
        /// </summary>
        public Point3D TrafficLoadOutside { get; private set; }

        /// <summary>
        /// Gets the location of the start of traffic load when approaching 
        /// from inside the polder.
        /// </summary>
        public Point3D TrafficLoadInside { get; private set; }

        /// <summary>
        /// Gets the location of the top of the dike when approaching from 
        /// inside the polder.
        /// </summary>
        public Point3D DikeTopAtPolder { get; private set; }

        /// <summary>
        /// Gets the location where the shoulder on the side of the polder
        /// connects with the dike.
        /// </summary>
        public Point3D ShoulderBaseInside { get; private set; }

        /// <summary>
        /// Gets the location where the shoulder on the side of the polder
        /// declines towards the location of the dike toe when approaching from inside 
        /// the polder.
        /// </summary>
        public Point3D ShoulderTopInside { get; private set; }

        /// <summary>
        /// Gets the location of dike toe when approaching from inside
        /// the polder.
        /// </summary>
        public Point3D DikeToeAtPolder { get; private set; }

        /// <summary>
        /// Gets the location of the start of the ditch when approaching
        /// from the dike.
        /// </summary>
        public Point3D DitchDikeSide { get; private set; }

        /// <summary>
        /// Gets the location of the bottom of the ditch when approaching
        /// from the dike.
        /// </summary>
        public Point3D BottomDitchDikeSide { get; private set; }

        /// <summary>
        /// Gets the location of the bottom of the ditch when approaching 
        /// from inside the polder.
        /// </summary>
        public Point3D BottomDitchPolderSide { get; private set; }

        /// <summary>
        /// Gets the location of the start of the ditch when approaching from
        /// inside the polder.
        /// </summary>
        public Point3D DitchPolderSide { get; private set; }

        /// <summary>
        /// Gets the location which generalizes the surface level on the
        /// inside of the polder
        /// </summary>
        public Point3D SurfaceLevelInside { get; private set; }

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
                throw new ArgumentNullException(nameof(points), Resources.RingtoetsMacroStabilityInwardsSurfaceLine_Collection_of_points_for_geometry_is_null);
            }
            if (points.Any(p => p == null))
            {
                throw new ArgumentException(Resources.RingtoetsMacroStabilityInwardsSurfaceLine_A_point_in_the_collection_was_null);
            }
            Points = points.Select(p => new Point3D(p)).ToArray();

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
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_DitchPolderSide);
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
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_BottomDitchPolderSide);
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
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_BottomDitchDikeSide);
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
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_DitchDikeSide);
            }
            DitchDikeSide = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="DikeTopAtPolder"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DikeTopAtPolder"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetDikeTopAtPolderAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtPolder);
            }
            DikeTopAtPolder = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="ShoulderBaseInside"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="ShoulderBaseInside"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetShoulderBaseInsideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_ShoulderBaseInside);
            }
            ShoulderBaseInside = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="ShoulderTopInside"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="ShoulderTopInside"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetShoulderTopInsideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_ShoulderTopInside);
            }
            ShoulderTopInside = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="TrafficLoadInside"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="TrafficLoadInside"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetTrafficLoadInsideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_TrafficLoadInside);
            }
            TrafficLoadInside = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="TrafficLoadOutside"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="TrafficLoadOutside"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetTrafficLoadOutsideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_TrafficLoadOutside);
            }
            TrafficLoadOutside = geometryPoint;
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
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtRiver);
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
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_DikeToeAtPolder);
            }
            DikeToeAtPolder = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="SurfaceLevelInside"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="SurfaceLevelInside"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetSurfaceLevelInsideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelInside);
            }
            SurfaceLevelInside = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="SurfaceLevelOutside"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="SurfaceLevelOutside"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="point"/> is <c>null</c>.</exception>
        public void SetSurfaceLevelOutsideAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_SurfaceLevelOutside);
            }
            SurfaceLevelOutside = geometryPoint;
        }

        /// <summary>
        /// Gets the height of the projected <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> at a L=<paramref name="l"/>.
        /// </summary>
        /// <param name="l">The L coordinate from where to take the height of the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.</param>
        /// <returns>The height of the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/> at L=<paramref name="l"/>.</returns>
        /// <exception cref="RingtoetsMacroStabilityInwardsSurfaceLineException">Thrown when the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>
        /// intersection point at <paramref name="l"/> have a significant difference in their y coordinate.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="l"/> is not in range of the LZ-projected <see cref="Points"/>.</exception>
        /// <exception cref="InvalidOperationException"><see cref="Points"/> is empty.</exception>
        public double GetZAtL(RoundedDouble l)
        {
            ValidateHasPoints();

            if (!ValidateInRange(l))
            {
                var localRangeL = new Range<double>(LocalGeometry.First().X, LocalGeometry.Last().X);
                string outOfRangeMessage = string.Format(Resources.RingtoetsMacroStabilityInwardsSurfaceLine_0_L_needs_to_be_in_Range_1_,
                                                         Resources.RingtoetsMacroStabilityInwardsSurfaceLine_GetZAtL_Cannot_determine_height,
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

            string message = string.Format(Resources.RingtoetsMacroStabilityInwardsSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l, l);
            throw new RingtoetsMacroStabilityInwardsSurfaceLineException(message);
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
        /// the <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <param name="fromSurfaceLine">The <see cref="RingtoetsMacroStabilityInwardsSurfaceLine"/>
        /// to get the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromSurfaceLine"/>
        /// is <c>null</c>.</exception>
        public void CopyProperties(RingtoetsMacroStabilityInwardsSurfaceLine fromSurfaceLine)
        {
            if (fromSurfaceLine == null)
            {
                throw new ArgumentNullException(nameof(fromSurfaceLine));
            }

            Name = fromSurfaceLine.Name;
            ReferenceLineIntersectionWorldPoint = fromSurfaceLine.ReferenceLineIntersectionWorldPoint != null
                                                      ? new Point2D(fromSurfaceLine.ReferenceLineIntersectionWorldPoint)
                                                      : null;
            SetGeometry(fromSurfaceLine.Points);
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
            return Equals((RingtoetsMacroStabilityInwardsSurfaceLine) obj);
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

                return hashCode;
            }
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
                throw new ArgumentNullException(nameof(point), @"Cannot find a point in geometry using a null point.");
            }
            return Points.FirstOrDefault(p => p.Equals(point));
        }

        private static ArgumentException CreatePointNotInGeometryException(Point3D point, string characteristicPointDescription)
        {
            string message = string.Format(Resources.MacroStabilityInwardsSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                           point,
                                           characteristicPointDescription);
            return new ArgumentException(message);
        }

        private bool Equals(RingtoetsMacroStabilityInwardsSurfaceLine other)
        {
            return string.Equals(Name, other.Name)
                   && Equals(ReferenceLineIntersectionWorldPoint, other.ReferenceLineIntersectionWorldPoint)
                   && EqualGeometricPoints(other.Points);
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
        /// Checks whether the current <see cref="Points"/> collection is not empty.
        /// </summary>
        /// <exception cref="InvalidOperationException"><see cref="Points"/> is empty.</exception>
        private void ValidateHasPoints()
        {
            if (!Points.Any())
            {
                throw new InvalidOperationException(Resources.RingtoetsMacroStabilityInwardsSurfaceLine_SurfaceLine_has_no_Geometry);
            }
        }
    }
}