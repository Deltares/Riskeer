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

using System.Linq;
using Core.Common.Base.Geometry;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.TestUtil
{
    /// <summary>
    /// Simple dike profile that can be used for testing.
    /// </summary>
    public class TestDikeProfile : DikeProfile
    {
        /// <summary>
        /// Initializes default <see cref="DikeProfile"/> at the world origin.
        /// </summary>
        public TestDikeProfile() : this(new Point2D(0, 0)) {}

        /// <summary>
        /// Initializes default <see cref="DikeProfile"/> at the world origin.
        /// </summary>
        /// <param name="name">The name of the dike profile.</param>
        public TestDikeProfile(string name) : this(name, new Point2D(0, 0)) {}

        /// <summary>
        /// Initializes default <see cref="DikeProfile"/> at the world location.
        /// </summary>
        /// <param name="point">The world coordinate of the dike profile.</param>
        public TestDikeProfile(Point2D point) : this(null, point) {}

        /// <summary>
        ///Initializes default <see cref="DikeProfile"/> at the world location.
        /// </summary>
        /// <param name="name">The name of the dike profile.</param>
        /// <param name="point">The world coordinate of the dike profile.</param>
        public TestDikeProfile(string name, Point2D point) : base(point, Enumerable.Empty<RoughnessPoint>(), Enumerable.Empty<Point2D>(), null, new ConstructionProperties
        {
            Name = name
        }) {}
    }
}