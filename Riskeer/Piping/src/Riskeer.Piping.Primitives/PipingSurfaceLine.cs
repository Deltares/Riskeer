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
using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Riskeer.Piping.Primitives
{
    /// <summary>
    /// Definition of a surface line for piping.
    /// </summary>
    public class PipingSurfaceLine : MechanismSurfaceLineBase
    {
        /// <summary>
        /// Creates a new instance of <see cref="PipingSurfaceLine"/>.
        /// </summary>
        /// <param name="name">The name of the surface line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is 
        /// <c>null</c>.</exception>
        public PipingSurfaceLine(string name) : base(name) {}

        /// <summary>
        /// Gets the location of dike toe when approaching from outside 
        /// the polder.
        /// </summary>
        public Point3D DikeToeAtRiver { get; private set; }

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
        /// Copies the property values of the <paramref name="fromSurfaceLine"/> to 
        /// the <see cref="PipingSurfaceLine"/>.
        /// </summary>
        /// <param name="fromSurfaceLine">The <see cref="PipingSurfaceLine"/>
        /// to get the property values from.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="fromSurfaceLine"/>
        /// is <c>null</c>.</exception>
        public void CopyProperties(PipingSurfaceLine fromSurfaceLine)
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

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((PipingSurfaceLine) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        private void SetCharacteristicPoints(PipingSurfaceLine fromSurfaceLine)
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

        private bool Equals(PipingSurfaceLine other)
        {
            return string.Equals(Name, other.Name)
                   && Equals(ReferenceLineIntersectionWorldPoint, other.ReferenceLineIntersectionWorldPoint)
                   && EqualGeometricPoints(other.Points)
                   && EqualCharacteristicPoints(other);
        }

        private bool EqualCharacteristicPoints(PipingSurfaceLine other)
        {
            return Equals(DikeToeAtPolder, other.DikeToeAtPolder)
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