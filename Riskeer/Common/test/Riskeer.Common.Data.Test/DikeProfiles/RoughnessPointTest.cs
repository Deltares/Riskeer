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

using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.DikeProfiles
{
    [TestFixture]
    public class RoughnessPointTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double roughness = 1.23456789;
            var point = new Point2D(1.1826, 2.2692);

            // Call
            var roughnessPoint = new RoughnessPoint(point, roughness);

            // Assert
            Assert.AreEqual(new Point2D(1.18, 2.27), roughnessPoint.Point);
            Assert.AreEqual(2, roughnessPoint.Roughness.NumberOfDecimalPlaces);
            Assert.AreEqual(roughness, roughnessPoint.Roughness, roughnessPoint.Roughness.GetAccuracy());
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

        [TestFixture]
        private class RoughnessPointEqualsTest : EqualsTestFixture<RoughnessPoint, DerivedRoughnessPoint>
        {
            protected override RoughnessPoint CreateObject()
            {
                return CreateRoughnessPoint();
            }

            protected override DerivedRoughnessPoint CreateDerivedObject()
            {
                RoughnessPoint basePoint = CreateRoughnessPoint();
                return new DerivedRoughnessPoint(basePoint);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                var random = new Random(21);
                double offset = random.NextDouble();
                RoughnessPoint basePoint = CreateRoughnessPoint();

                yield return new TestCaseData(new RoughnessPoint(basePoint.Point,
                                                                 basePoint.Roughness + offset))
                    .SetName("Roughness");
                yield return new TestCaseData(new RoughnessPoint(new Point2D(basePoint.Point.X + offset, basePoint.Point.Y),
                                                                 basePoint.Roughness))
                    .SetName("Point");
            }

            private static RoughnessPoint CreateRoughnessPoint()
            {
                return new RoughnessPoint(new Point2D(0, 0), 3.14);
            }
        }

        private class DerivedRoughnessPoint : RoughnessPoint
        {
            public DerivedRoughnessPoint(RoughnessPoint roughnessPoint)
                : base(roughnessPoint.Point, roughnessPoint.Roughness) {}
        }
    }
}