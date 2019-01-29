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

namespace Riskeer.Common.Data.DikeProfiles
{
    /// <summary>
    /// Definition for a dike profile for a failure mechanism.
    /// </summary>
    public class DikeProfile : Observable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DikeProfile"/> class.
        /// </summary>
        /// <param name="worldCoordinate">The value for <see cref="WorldReferencePoint"/>.</param>
        /// <param name="dikeGeometry">The geometry of the dike.</param>
        /// <param name="foreshoreGeometry">The geometry of the dike foreshore.</param>
        /// <param name="breakWater">The break water definition (can be null).</param>
        /// <param name="properties">The property values required to create an instance of <see cref="DikeProfile"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when either <paramref name="dikeGeometry"/>,
        /// <paramref name="foreshoreGeometry"/>, <paramref name="worldCoordinate"/> or 
        /// <paramref name="properties"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>any element of <paramref name="dikeGeometry"/>
        /// or <paramref name="foreshoreGeometry"/> is <c>null</c>.</item>
        /// <item><paramref name="properties.Id"/> is <c>null</c>, is empty
        /// or whitespaces.</item>
        /// </list> </exception>
        public DikeProfile(Point2D worldCoordinate, IEnumerable<RoughnessPoint> dikeGeometry, IEnumerable<Point2D> foreshoreGeometry,
                           BreakWater breakWater, ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            ForeshoreProfile = new ForeshoreProfile(worldCoordinate, foreshoreGeometry, breakWater,
                                                    new ForeshoreProfile.ConstructionProperties
                                                    {
                                                        Id = properties.Id,
                                                        Name = properties.Name,
                                                        Orientation = properties.Orientation,
                                                        X0 = properties.X0
                                                    });

            SetGeometry(dikeGeometry);
            DikeHeight = new RoundedDouble(2, properties.DikeHeight);
        }

        /// <summary>
        /// Gets the foreshore profile.
        /// </summary>
        public ForeshoreProfile ForeshoreProfile { get; }

        /// <summary>
        /// Gets the ID of the dike profile.
        /// </summary>
        public string Id
        {
            get
            {
                return ForeshoreProfile.Id;
            }
        }

        /// <summary>
        /// Gets the name of the dike profile.
        /// </summary>
        public string Name
        {
            get
            {
                return ForeshoreProfile.Name;
            }
        }

        /// <summary>
        /// Gets the reference point in world coordinates corresponding to the local coordinate <see cref="X0"/>.
        /// </summary>
        public Point2D WorldReferencePoint
        {
            get
            {
                return ForeshoreProfile.WorldReferencePoint;
            }
        }

        /// <summary>
        /// Gets the local x-coordinate corresponding to the world reference point <see cref="WorldReferencePoint"/>.
        /// </summary>
        public double X0
        {
            get
            {
                return ForeshoreProfile.X0;
            }
        }

        /// <summary>
        /// Gets or the orientation of the dike profile geometry with respect to North
        /// in degrees. A positive value equals a clockwise rotation.
        /// </summary>
        public RoundedDouble Orientation
        {
            get
            {
                return ForeshoreProfile.Orientation;
            }
        }

        /// <summary>
        /// Indicates if there is a break water object available for this instance or not.
        /// </summary>
        public bool HasBreakWater
        {
            get
            {
                return BreakWater != null;
            }
        }

        /// <summary>
        /// Gets the break water object of the dike profile, if any.
        /// </summary>
        public BreakWater BreakWater
        {
            get
            {
                return ForeshoreProfile.BreakWater;
            }
        }

        /// <summary>
        /// Gets the geometry of the foreshore.
        /// </summary>
        public RoundedPoint2DCollection ForeshoreGeometry
        {
            get
            {
                return ForeshoreProfile.Geometry;
            }
        }

        /// <summary>
        /// Gets the geometry of the dike with roughness data.
        /// </summary>
        /// <remarks>
        /// The roughness of a <see cref="RoughnessPoint"/> in the list represents
        /// the roughness of the section between this <see cref="RoughnessPoint"/>
        /// and the succeeding <see cref="RoughnessPoint"/>. The roughness of the last
        /// point is irrelevant.
        /// </remarks>
        public IEnumerable<RoughnessPoint> DikeGeometry { get; private set; }

        /// <summary>
        /// Gets or sets the height of the dike [m+NAP].
        /// </summary>
        public RoundedDouble DikeHeight { get; private set; }

        /// <summary>
        /// Copies all the properties of the <paramref name="fromDikeProfile"/> to
        /// the current instance.
        /// </summary>
        /// <param name="fromDikeProfile">The dike profile to copy the properties
        /// from.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="fromDikeProfile"/> is <c>null</c>.</exception>
        public void CopyProperties(DikeProfile fromDikeProfile)
        {
            if (fromDikeProfile == null)
            {
                throw new ArgumentNullException(nameof(fromDikeProfile));
            }

            SetGeometry(fromDikeProfile.DikeGeometry);
            DikeHeight = fromDikeProfile.DikeHeight;
            CopyForeshoreProfileProperties(fromDikeProfile);
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

            if (GetType() != obj.GetType())
            {
                return false;
            }

            return Equals((DikeProfile) obj);
        }

        public override int GetHashCode()
        {
            return ForeshoreProfile.GetHashCode();
        }

        private void CopyForeshoreProfileProperties(DikeProfile fromDikeProfile)
        {
            ForeshoreProfile.CopyProperties(new ForeshoreProfile(fromDikeProfile.WorldReferencePoint,
                                                                 fromDikeProfile.ForeshoreGeometry,
                                                                 fromDikeProfile.BreakWater,
                                                                 new ForeshoreProfile.ConstructionProperties
                                                                 {
                                                                     Id = fromDikeProfile.Id,
                                                                     Name = fromDikeProfile.Name,
                                                                     Orientation = fromDikeProfile.Orientation,
                                                                     X0 = fromDikeProfile.X0
                                                                 }));
        }

        private bool Equals(DikeProfile other)
        {
            return Equals(ForeshoreProfile, other.ForeshoreProfile)
                   && DikeHeight.Equals(other.DikeHeight)
                   && EqualDikeGeometry(other.DikeGeometry);
        }

        private void SetGeometry(IEnumerable<RoughnessPoint> points)
        {
            if (points == null)
            {
                throw new ArgumentNullException(nameof(points), Resources.DikeProfile_SetGeometry_Collection_of_points_for_geometry_is_null);
            }

            if (points.Any(p => p == null))
            {
                throw new ArgumentException(Resources.DikeProfile_SetGeometry_A_point_in_the_collection_is_null);
            }

            DikeGeometry = points.Select(p => new RoughnessPoint(p.Point, p.Roughness)).ToArray();
        }

        private bool EqualDikeGeometry(IEnumerable<RoughnessPoint> otherPoints)
        {
            int nrOfPoints = DikeGeometry.Count();
            if (otherPoints.Count() != nrOfPoints)
            {
                return false;
            }

            for (var i = 0; i < nrOfPoints; i++)
            {
                if (!DikeGeometry.ElementAt(i).Equals(otherPoints.ElementAt(i)))
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="DikeProfile"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="DikeProfile.Id"/>.
            /// </summary>
            public string Id { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="DikeProfile.Name"/>.
            /// </summary>
            public string Name { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="DikeProfile.X0"/>.
            /// </summary>
            public double X0 { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="DikeProfile.Orientation"/>.
            /// </summary>
            public double Orientation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="DikeProfile.DikeHeight"/>.
            /// </summary>
            /// <remarks><paramref name="value"/> will be rounded to the <see cref="RoundedDouble.NumberOfDecimalPlaces"/>
            ///  of <see cref="DikeProfile.DikeHeight"/>.</remarks>
            public double DikeHeight { internal get; set; }
        }
    }
}