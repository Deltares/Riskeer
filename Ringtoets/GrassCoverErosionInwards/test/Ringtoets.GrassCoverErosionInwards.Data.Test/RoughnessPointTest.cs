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
using Core.Common.Base.Geometry;
using NUnit.Framework;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class RoughnessPointTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var point = new Point2D(1.1, 2.2);
            const double roughness = 5.5;

            // Call
            var roughnessPoint = new RoughnessPoint(point, roughness);

            // Assert
            Assert.AreEqual(point, roughnessPoint.Point);
            Assert.AreEqual(roughness, roughnessPoint.Roughness);
        }

        [Test]
        public void Constructor_PointNull_ThrowsArgumentNullException()
        {
            // Setup & Call
            TestDelegate test = () => new RoughnessPoint(null, 0.0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("point", exception.ParamName);
        }
    }
}
