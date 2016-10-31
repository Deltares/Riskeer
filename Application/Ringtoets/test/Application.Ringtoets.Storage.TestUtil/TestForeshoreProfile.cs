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

namespace Application.Ringtoets.Storage.TestUtil
{
    /// <summary>
    /// Simple foreshore profile that can be used for testing.
    /// </summary>
    public class TestForeshoreProfile : ForeshoreProfile
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestForeshoreProfile"/>.
        /// </summary>
        /// <param name="useBreakWater">If <c>true</c>, create the ForeshoreProfile with a default <see cref="BreakWater"/>.</param>
        public TestForeshoreProfile(bool useBreakWater = false) : base(new Point2D(0, 0), Enumerable.Empty<Point2D>(), useBreakWater ? new BreakWater(BreakWaterType.Dam, 10) : null, new ConstructionProperties()) { }

        /// <summary>
        /// Creates a new instance of <see cref="TestForeshoreProfile"/> with a specified<see cref="BreakWater"/>.
        /// </summary>
        /// <param name="breakWater">The <see cref="BreakWater"/> which needs to be set on the <see cref="ForeshoreProfile"/>.</param>
        public TestForeshoreProfile(BreakWater breakWater) : base(new Point2D(0, 0), Enumerable.Empty<Point2D>(), breakWater, new ConstructionProperties()) {}
    }
}