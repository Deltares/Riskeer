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
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.DikeProfiles
{
    /// <summary>
    /// Definition for a foreshore profile for a failure mechanism.
    /// </summary>
    public class ForeshoreProfile : Observable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ForeshoreProfile"/> class.
        /// </summary>
        /// <param name="worldCoordinate">The value for <see cref="WorldReferencePoint"/>.</param>
        /// <param name="geometry">The geometry of the foreshore.</param>
        /// <param name="breakWater">The break water definition (can be null).</param>
        /// <param name="properties">The property values required to create an instance of <see cref="ForeshoreProfile"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="geometry"/>, 
        /// <paramref name="worldCoordinate"/> or <paramref name="properties"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>any element of <paramref name="geometry"/> is <c>null</c></item>
        /// <item><paramref name="properties.Id"/> is <c>null</c>, empty or whitespaces</item>
        /// </list>
        /// </exception>
        public ForeshoreProfile(Point2D worldCoordinate, IEnumerable<Point2D> geometry,
                                BreakWater breakWater, ConstructionProperties properties)
        {
            if (worldCoordinate == null)
            {
                throw new ArgumentNullException(nameof(worldCoordinate));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (string.IsNullOrWhiteSpace(properties.Id))
            {
                throw new ArgumentException(@"Id is null, empty or consists of whitespace.", nameof(properties));
            }

            SetGeometry(geometry);

            Orientation = new RoundedDouble(2, properties.Orientation);

            BreakWater = breakWater;
            Id = properties.Id;
            Name = string.IsNullOrWhiteSpace(properties.Name) ? properties.Id : properties.Name;
            WorldReferencePoint = worldCoordinate;
            X0 = properties.X0;
        }

        /// <summary>
        /// Gets the ID of the foreshore profile.
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// Gets the name of the foreshore profile.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the reference point in world coordinates corresponding to the local coordinate <see cref="X0"/>.
        /// </summary>
        public Point2D WorldReferencePoint { get; private set; }

        /// <summary>
        /// Gets the local x-coordinate corresponding to the world reference point <see cref="WorldReferencePoint"/>.
        /// </summary>
        public double X0 { get; private set; }

        /// <summary>
        /// Gets the orientation of the foreshore profile geometry with respect to North
        /// in degrees. A positive value equals a clockwise rotation.
        /// </summary>
        public RoundedDouble Orientation { get; private set; }

        /// <summary>
        /// Gets a value indicating if there is a break water object available.
        /// </summary>
        public bool HasBreakWater
        {
            get
            {
                return BreakWater != null;
            }
        }

        /// <summary>
        /// Gets the break water object of the foreshore profile, if any.
        /// </summary>
        public BreakWater BreakWater { get; private set; }

        /// <summary>
        /// Gets the geometry of the foreshore profile.
        /// </summary>
        public RoundedPoint2DCollection Geometry { get; private set; }

        /// <summary>
        /// Copies all the properties of <paramref name="fromForeshoreProfile"/>
        /// to the current instance.
        /// </summary>
        /// <param name="fromForeshoreProfile">The foreshore profile to copy the
        /// properties from.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="fromForeshoreProfile"/> is <c>null</c>.</exception>
        public void CopyProperties(ForeshoreProfile fromForeshoreProfile)
        {
            if (fromForeshoreProfile == null)
            {
                throw new ArgumentNullException(nameof(fromForeshoreProfile));
            }

            Id = fromForeshoreProfile.Id;
            Name = fromForeshoreProfile.Name;
            X0 = fromForeshoreProfile.X0;
            Orientation = fromForeshoreProfile.Orientation;

            BreakWater = fromForeshoreProfile.BreakWater != null
                             ? new BreakWater(fromForeshoreProfile.BreakWater.Type, fromForeshoreProfile.BreakWater.Height)
                             : null;
            WorldReferencePoint = new Point2D(fromForeshoreProfile.WorldReferencePoint);
            SetGeometry(fromForeshoreProfile.Geometry);
        }

        public override string ToString()
        {
            return Name;
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

            return Equals((ForeshoreProfile) obj);
        }

        public override int GetHashCode()
        {
            return 0;
        }

        private bool Equals(ForeshoreProfile other)
        {
            return Id.Equals(other.Id)
                   && string.Equals(Name, other.Name)
                   && WorldReferencePoint.Equals(other.WorldReferencePoint)
                   && X0.Equals(other.X0)
                   && Orientation.Equals(other.Orientation)
                   && Equals(BreakWater, other.BreakWater)
                   && EqualGeometry(other.Geometry.ToArray());
        }

        /// <summary>
        /// Sets the geometry of the foreshore profile.
        /// </summary>
        /// <param name="points">The points corresponding to the geometry of 
        /// the foreshore profile.</param>
        /// <exception cref="ArgumentException">Thrown when an element in 
        /// <paramref name="points"/> is <c>null</c>.</exception>
        private void SetGeometry(IEnumerable<Point2D> points)
        {
            if (points.Any(p => p == null))
            {
                throw new ArgumentException(Resources.ForeshoreProfile_SetGeometry_A_point_in_the_collection_is_null);
            }

            Geometry = new RoundedPoint2DCollection(2, points.Select(p => new Point2D(p)).ToArray());
        }

        private bool EqualGeometry(IEnumerable<Point2D> otherGeometry)
        {
            int nrOfPoints = Geometry.Count();
            if (otherGeometry.Count() != nrOfPoints)
            {
                return false;
            }

            for (var i = 0; i < nrOfPoints; i++)
            {
                if (!Geometry.ElementAt(i).Equals(otherGeometry.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ForeshoreProfile"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="ForeshoreProfile.Id"/>.
            /// </summary>
            public string Id { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ForeshoreProfile.Name"/>.
            /// </summary>
            public string Name { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ForeshoreProfile.X0"/>.
            /// </summary>
            public double X0 { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ForeshoreProfile.Orientation"/>.
            /// </summary>
            /// <remarks><paramref name="value"/> will be rounded to the <see cref="RoundedDouble.NumberOfDecimalPlaces"/> 
            /// of <see cref="ForeshoreProfile.Orientation"/>.</remarks>
            public double Orientation { internal get; set; }
        }
    }
}