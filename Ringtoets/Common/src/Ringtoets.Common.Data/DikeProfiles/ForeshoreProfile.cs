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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.Properties;

namespace Ringtoets.Common.Data.DikeProfiles
{
    /// <summary>
    /// Definition for a foreshore profile for a failure mechanism.
    /// </summary>
    public class ForeshoreProfile
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
        /// <exception cref="ArgumentException">Thrown when any element of 
        /// <paramref name="geometry"/> is <c>null</c>.</exception>
        public ForeshoreProfile(Point2D worldCoordinate, IEnumerable<Point2D> geometry,
                                BreakWater breakWater, ConstructionProperties properties)
        {
            if (worldCoordinate == null)
            {
                throw new ArgumentNullException("worldCoordinate");
            }
            if (geometry == null)
            {
                throw new ArgumentNullException("geometry");
            }
            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            SetGeometry(geometry);

            Orientation = new RoundedDouble(2, properties.Orientation);

            BreakWater = breakWater;
            Name = properties.Name;
            WorldReferencePoint = worldCoordinate;
            X0 = properties.X0;
        }

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

        public override string ToString()
        {
            return Name;
        }

        private void SetGeometry(IEnumerable<Point2D> points)
        {
            var foreshorePoints = points.ToArray();
            if (foreshorePoints.Any(p => p == null))
            {
                throw new ArgumentException(Resources.ForeshoreProfile_SetGeometry_A_point_in_the_collection_is_null);
            }

            Geometry = new RoundedPoint2DCollection(2, foreshorePoints);
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ForeshoreProfile"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="ForeshoreProfile.Name"/>.
            /// </summary>
            public string Name { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ForeshoreProfile.X0"/>.
            /// </summary>
            public double X0 { get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ForeshoreProfile.Orientation"/>.
            /// </summary>
            /// <remark><paramref name="value"/> will be rounded to the <see cref="RoundedDouble.NumberOfDecimalPlaces"/> 
            /// of <see cref="ForeshoreProfile.Orientation"/>.</remark>
            public double Orientation { get; set; }
        }
    }
}