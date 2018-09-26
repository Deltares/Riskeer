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
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Primitives
{
    /// <summary>
    /// Definition of a surface line for macro stability inwards.
    /// </summary>
    public class MacroStabilityInwardsSurfaceLine : MechanismSurfaceLineBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <param name="name">The name of the surface line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> 
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsSurfaceLine(string name) : base(name) {}

        /// <summary>
        /// Gets the location which generalizes the height of the surface
        /// on the outside of the polder.
        /// </summary>
        public Point3D SurfaceLevelOutside { get; private set; }

        /// <summary>
        /// Gets the location of dike toe when approaching from outside 
        /// the polder.
        /// </summary>
        public Point3D DikeToeAtRiver { get; private set; }

        /// <summary>
        /// Gets the location of the top of the dike when approaching from 
        /// inside the polder.
        /// </summary>
        public Point3D DikeTopAtPolder { get; private set; }

        /// <summary>
        /// Gets the location of the top of the dike when approaching from 
        /// outside the polder.
        /// </summary>
        public Point3D DikeTopAtRiver { get; private set; }

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
        /// inside of the polder.
        /// </summary>
        public Point3D SurfaceLevelInside { get; private set; }

        /// <summary>
        /// Sets the <see cref="DitchPolderSide"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DitchPolderSide"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// Sets the <see cref="DikeTopAtRiver"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DikeTopAtRiver"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
        public void SetDikeTopAtRiverAt(Point3D point)
        {
            Point3D geometryPoint = GetPointFromGeometry(point);
            if (geometryPoint == null)
            {
                throw CreatePointNotInGeometryException(point, RingtoetsCommonDataResources.CharacteristicPoint_DikeTopAtRiver);
            }

            DikeTopAtRiver = geometryPoint;
        }

        /// <summary>
        /// Sets the <see cref="ShoulderBaseInside"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="ShoulderBaseInside"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// Sets the <see cref="DikeToeAtRiver"/> at the given point.
        /// </summary>
        /// <param name="point">The location as a <see cref="Point3D"/> which to set as the <see cref="DikeToeAtRiver"/>.</param>
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// <exception cref="ArgumentException">Thrown when <see cref="MechanismSurfaceLineBase.Points"/> doesn't contain a <see cref="Point3D"/> at 
        /// <paramref name="point"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="point"/> is <c>null</c>.</exception>
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
        /// Copies the property values of the <paramref name="fromSurfaceLine"/> to 
        /// the <see cref="MacroStabilityInwardsSurfaceLine"/>.
        /// </summary>
        /// <param name="fromSurfaceLine">The <see cref="MacroStabilityInwardsSurfaceLine"/>
        /// to get the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromSurfaceLine"/>
        /// is <c>null</c>.</exception>
        public void CopyProperties(MacroStabilityInwardsSurfaceLine fromSurfaceLine)
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

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((MacroStabilityInwardsSurfaceLine) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        private void SetCharacteristicPoints(MacroStabilityInwardsSurfaceLine fromSurfaceLine)
        {
            SurfaceLevelOutside = PointFromGeometryOrNull(fromSurfaceLine.SurfaceLevelOutside);
            DikeTopAtPolder = PointFromGeometryOrNull(fromSurfaceLine.DikeTopAtPolder);
            DikeTopAtRiver = PointFromGeometryOrNull(fromSurfaceLine.DikeTopAtRiver);
            ShoulderBaseInside = PointFromGeometryOrNull(fromSurfaceLine.ShoulderBaseInside);
            ShoulderTopInside = PointFromGeometryOrNull(fromSurfaceLine.ShoulderTopInside);
            BottomDitchDikeSide = PointFromGeometryOrNull(fromSurfaceLine.BottomDitchDikeSide);
            BottomDitchPolderSide = PointFromGeometryOrNull(fromSurfaceLine.BottomDitchPolderSide);
            DikeToeAtPolder = PointFromGeometryOrNull(fromSurfaceLine.DikeToeAtPolder);
            DikeToeAtRiver = PointFromGeometryOrNull(fromSurfaceLine.DikeToeAtRiver);
            DitchDikeSide = PointFromGeometryOrNull(fromSurfaceLine.DitchDikeSide);
            DitchPolderSide = PointFromGeometryOrNull(fromSurfaceLine.DitchPolderSide);
            SurfaceLevelInside = PointFromGeometryOrNull(fromSurfaceLine.SurfaceLevelInside);
        }

        private Point3D PointFromGeometryOrNull(Point3D point3D)
        {
            return point3D != null ? GetPointFromGeometry(point3D) : null;
        }

        private bool Equals(MacroStabilityInwardsSurfaceLine other)
        {
            return string.Equals(Name, other.Name)
                   && Equals(ReferenceLineIntersectionWorldPoint, other.ReferenceLineIntersectionWorldPoint)
                   && EqualGeometricPoints(other.Points)
                   && EqualCharacteristicPoints(other);
        }

        private bool EqualCharacteristicPoints(MacroStabilityInwardsSurfaceLine other)
        {
            return Equals(SurfaceLevelInside, other.SurfaceLevelInside)
                   && Equals(SurfaceLevelOutside, other.SurfaceLevelOutside)
                   && Equals(DikeTopAtPolder, other.DikeTopAtPolder)
                   && Equals(DikeTopAtRiver, other.DikeTopAtRiver)
                   && Equals(ShoulderBaseInside, other.ShoulderBaseInside)
                   && Equals(ShoulderTopInside, other.ShoulderTopInside)
                   && Equals(DikeToeAtPolder, other.DikeToeAtPolder)
                   && Equals(DikeToeAtRiver, other.DikeToeAtRiver)
                   && Equals(DitchDikeSide, other.DitchDikeSide)
                   && Equals(DitchPolderSide, other.DitchPolderSide)
                   && Equals(BottomDitchDikeSide, other.BottomDitchDikeSide)
                   && Equals(BottomDitchPolderSide, other.BottomDitchPolderSide);
        }

        private bool EqualGeometricPoints(IEnumerable<Point3D> otherPoints)
        {
            int nrOfOtherPoints = otherPoints.Count();
            int nrOfPoints = Points.Count();
            if (nrOfPoints != nrOfOtherPoints)
            {
                return false;
            }

            for (var index = 0; index < nrOfPoints; index++)
            {
                if (!Points.ElementAt(index).Equals(otherPoints.ElementAt(index)))
                {
                    return false;
                }
            }

            return true;
        }
    }
}