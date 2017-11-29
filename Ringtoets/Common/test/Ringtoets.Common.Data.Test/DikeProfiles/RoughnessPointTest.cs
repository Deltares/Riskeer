// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;

namespace Ringtoets.Common.Data.Test.DikeProfiles
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
            Assert.AreEqual(new RoundedDouble(2, roughness), roughnessPoint.Roughness);
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

        [Test]
        public void GetHashCode_EqualRoughnessPoint_ReturnsSameHashCode()
        {
            // Setup
            var geometryPoint = new Point2D(0, 0);
            const double roughness = 3.14;

            var roughnessPointOne = new RoughnessPoint(geometryPoint, roughness);
            var roughnessPointTwo = new RoughnessPoint(geometryPoint, roughness);

            // Call
            int hashCodeOne = roughnessPointOne.GetHashCode();
            int hashCodeTwo = roughnessPointTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        [TestFixture]
        private class RoughnessPointEqualsTest : EqualsGuidelinesTestFixture<RoughnessPoint, DerivedRoughnessPoint>
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
                RoughnessPoint basePoint = CreateRoughnessPoint();

                yield return new TestCaseData(new RoughnessPoint(basePoint.Point,
                                                                 basePoint.Roughness + 10))
                    .SetName("Roughness");
                yield return new TestCaseData(new RoughnessPoint(new Point2D(1, 1),
                                                                 basePoint.Roughness))
                    .SetName("Point");
            }
        }

        private static RoughnessPoint CreateRoughnessPoint()
        {
            return new RoughnessPoint(new Point2D(0, 0), 3.14);
        }

        private class DerivedRoughnessPoint : RoughnessPoint
        {
            public DerivedRoughnessPoint(RoughnessPoint roughnessPoint)
                : base(roughnessPoint.Point, roughnessPoint.Roughness) {}
        }
    }
}