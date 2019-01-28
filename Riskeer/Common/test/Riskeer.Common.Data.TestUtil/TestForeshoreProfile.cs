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
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Simple foreshore profile that can be used for testing.
    /// </summary>
    public class TestForeshoreProfile : ForeshoreProfile
    {
        /// <summary>
        /// Creates a new instance of the <see cref="TestForeshoreProfile"/> at a specified
        /// <see cref="Point2D"/>.
        /// </summary>
        /// <param name="worldReferencePoint">Location of the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="worldReferencePoint"/> is <c>null</c>.</exception>
        public TestForeshoreProfile(Point2D worldReferencePoint)
            : this("id", "name", worldReferencePoint, null, Enumerable.Empty<Point2D>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestForeshoreProfile"/>.
        /// </summary>
        /// <param name="useBreakWater">If <c>true</c>, create the ForeshoreProfile with a default <see cref="BreakWater"/>.</param>
        public TestForeshoreProfile(bool useBreakWater = false)
            : this("id", "name", new Point2D(0, 0), useBreakWater ? new BreakWater(BreakWaterType.Dam, 10) : null, Enumerable.Empty<Point2D>()) {}

        /// <summary>
        /// Creates a new instance of the <see cref="TestForeshoreProfile"/> with a given
        /// name and no <see cref="BreakWater"/>.
        /// </summary>
        /// <param name="profileId">The id of the profile.</param>
        /// <exception cref="ArgumentException">Thrown when 
        /// <paramref name="profileId"/> is null, empty or whitespaces.</exception>
        public TestForeshoreProfile(string profileId)
            : this(profileId, "name", new Point2D(0, 0), null, Enumerable.Empty<Point2D>()) {}

        /// <summary>
        /// Creates a new instance of the <see cref="TestForeshoreProfile"/> with a given
        /// name and id and no <see cref="BreakWater"/>.
        /// </summary>
        /// <param name="profileName">The name of the profile.</param>
        /// <param name="profileId">The id of the profile.</param>
        /// <exception cref="ArgumentException">Thrown when 
        /// <paramref name="profileId"/> is null, empty or whitespaces.</exception>
        public TestForeshoreProfile(string profileName, string profileId)
            : this(profileId, profileName, new Point2D(0, 0), null, Enumerable.Empty<Point2D>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestForeshoreProfile"/> with a specified <see cref="BreakWater"/>.
        /// </summary>
        /// <param name="breakWater">The <see cref="BreakWater"/> which needs to be set on the <see cref="ForeshoreProfile"/>.</param>
        public TestForeshoreProfile(BreakWater breakWater)
            : this("id", "name", new Point2D(0, 0), breakWater, Enumerable.Empty<Point2D>()) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestForeshoreProfile"/> with a specified geometry.
        /// </summary>
        /// <param name="geometry">The geometry of the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when 
        /// any element of <paramref name="geometry"/> is <c>null</c>.</exception>
        public TestForeshoreProfile(IEnumerable<Point2D> geometry)
            : this("id", "name", new Point2D(0, 0), null, geometry) {}

        /// <summary>
        /// Creates a new instance of the <see cref="TestForeshoreProfile"/> with a given
        /// name and geometry.
        /// </summary>
        /// <param name="profileId">The id of the profile.</param>
        /// <param name="geometry">The geometry of the profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when 
        /// <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>Any element of <paramref name="geometry"/> is <c>null</c>.</item>
        /// <item><paramref name="profileId"/> is null, empty or whitespaces.</item>
        /// </list></exception>
        public TestForeshoreProfile(string profileId, IEnumerable<Point2D> geometry)
            : this(profileId, "name", new Point2D(0, 0), null, geometry) {}

        /// <summary>
        /// Creates a new instance of <see cref="TestForeshoreProfile"/> with given properties.
        /// </summary>
        /// <param name="profileId">The id of the foreshore profile.</param>
        /// <param name="profileName">The name of the foreshore profile.</param>
        /// <param name="worldCoordinate">The location of the foreshore profile.</param>
        /// <param name="breakWater">The breakwater of the foreshore profile.</param>
        /// <param name="geometry">The geometry of the foreshore profile.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/>
        /// or <paramref name="worldCoordinate"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>Any element of <paramref name="geometry"/> is <c>null</c>.</item>
        /// <item><paramref name="profileId"/> is null, empty or whitespaces.</item>
        /// </list></exception>
        private TestForeshoreProfile(string profileId, string profileName, Point2D worldCoordinate, BreakWater breakWater, IEnumerable<Point2D> geometry)
            : base(worldCoordinate,
                   geometry,
                   breakWater,
                   new ConstructionProperties
                   {
                       Id = profileId,
                       Name = profileName
                   }) {}

        /// <summary>
        /// Modifies <see cref="BreakWater"/> properties of the current instance 
        /// of the foreshore profile to different values.
        /// </summary>
        /// <param name="foreshoreProfile">The current instance of which the properties 
        /// need to be modified.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="foreshoreProfile"/>
        /// is <c>null</c>.</exception>
        public static void ChangeBreakWaterProperties(TestForeshoreProfile foreshoreProfile)
        {
            if (foreshoreProfile == null)
            {
                throw new ArgumentNullException(nameof(foreshoreProfile));
            }

            BreakWater differentBreakWater = null;
            if (!foreshoreProfile.HasBreakWater)
            {
                differentBreakWater = new BreakWater(BreakWaterType.Caisson, 12.34);
            }

            var foreshoreProfileToUpdateFrom = new TestForeshoreProfile(differentBreakWater);

            foreshoreProfile.CopyProperties(foreshoreProfileToUpdateFrom);
        }
    }
}