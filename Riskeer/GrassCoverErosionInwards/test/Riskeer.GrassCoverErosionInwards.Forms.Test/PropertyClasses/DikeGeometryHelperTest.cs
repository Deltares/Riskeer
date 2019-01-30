// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Riskeer.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DikeGeometryHelperTest
    {
        [Test]
        public void GetRoughnesses_DikeGeometryWithNoPoints_ReturnsEmptyCollection()
        {
            // Setup
            IEnumerable<RoughnessPoint> dikeGeometry = Enumerable.Empty<RoughnessPoint>();

            // Call
            IEnumerable<RoundedDouble> roughnesses = DikeGeometryHelper.GetRoughnesses(dikeGeometry);

            // Assert
            CollectionAssert.IsEmpty(roughnesses);
        }

        [Test]
        public void GetRoughnesses_DikeGeometryWithOnePoint_ReturnsEmptyCollection()
        {
            // Setup
            var dikeGeometry = new[]
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 0.6)
            };

            // Call
            IEnumerable<RoundedDouble> roughnesses = DikeGeometryHelper.GetRoughnesses(dikeGeometry);

            // Assert
            CollectionAssert.IsEmpty(roughnesses);
        }

        [Test]
        public void GetRoughnesses_DikeGeometryWithMultiplePoints_ReturnsCollectionOfAllButLastRoughness()
        {
            // Setup
            var dikeGeometry = new[]
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 0.6),
                new RoughnessPoint(new Point2D(3.3, 4.4), 0.7),
                new RoughnessPoint(new Point2D(5.5, 6.6), 0.8)
            };

            // Call
            IEnumerable<RoundedDouble> roughnesses = DikeGeometryHelper.GetRoughnesses(dikeGeometry);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new RoundedDouble(2, 0.6),
                new RoundedDouble(2, 0.7)
            }, roughnesses);
        }
    }
}