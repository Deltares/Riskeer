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

using System.Collections.Generic;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class DikeGeometryHelperTest
    {
        [Test]
        public void GetRoughnesses_DikeGeometryWithNoPoints_ReturnsEmptyCollection()
        {
            // Setup
            var dikeGeometry = new List<RoughnessPoint>();

            // Call
            var roughnesses = DikeGeometryHelper.GetRoughnesses(dikeGeometry);

            // Assert
            CollectionAssert.IsEmpty(roughnesses);
        }

        [Test]
        public void GetRoughnesses_DikeGeometryWithOnePoint_ReturnsEmptyCollection()
        {
            // Setup
            var dikeGeometry = new List<RoughnessPoint>
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 3.3)
            };

            // Call
            var roughnesses = DikeGeometryHelper.GetRoughnesses(dikeGeometry);

            // Assert
            CollectionAssert.IsEmpty(roughnesses);
        }

        [Test]
        public void GetRoughnesses_DikeGeometryWithMultiplePoints_ReturnsCollectionOfAllButLastRoughness()
        {
            // Setup
            var dikeGeometry = new List<RoughnessPoint>
            {
                new RoughnessPoint(new Point2D(1.1, 2.2), 3.3),
                new RoughnessPoint(new Point2D(4.4, 5.5), 6.6),
                new RoughnessPoint(new Point2D(7.7, 8.8), 9.9)
            };

            // Call
            var roughnesses = DikeGeometryHelper.GetRoughnesses(dikeGeometry);

            // Assert
            CollectionAssert.AreEqual(new[] { new RoundedDouble(2, 3.3), new RoundedDouble(2, 6.6) }, roughnesses);
        }
    }
}
