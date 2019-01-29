// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
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

namespace Core.Common.Base.Test.Geometry
{
    [TestFixture]
    public class Point3DTest
    {
        [Test]
        public void Constructor_WithParameters_ExpectedValues()
        {
            // Setup
            const double x = 1.1;
            const double y = 2.2;
            const double z = -1.1;

            // Call
            var point = new Point3D(x, y, z);

            // Assert
            Assert.AreEqual(1.1, point.X);
            Assert.AreEqual(2.2, point.Y);
            Assert.AreEqual(-1.1, point.Z);
        }

        [Test]
        public void CopyConstructor_PointNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new Point3D(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("point", paramName);
        }

        [Test]
        public void CopyConstructor_WithPointWithXandY_SetProperties()
        {
            // Setup
            const double x = 1.1;
            const double y = 2.2;
            const double z = -1.1;
            var pointToCopy = new Point3D(x, y, z);

            // Call
            var point = new Point3D(pointToCopy);

            // Assert
            Assert.AreEqual(x, point.X);
            Assert.AreEqual(y, point.Y);
            Assert.AreEqual(z, point.Z);
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ToString_HasCoordinateValues_NL_PrintCoordinateValuesInLocalCulture()
        {
            DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture();
        }

        [Test]
        [SetCulture("en-US")]
        public void ToString_HasCoordinateValues_EN_PrintCoordinateValuesInLocalCulture()
        {
            DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture();
        }

        private static void DoToString_HasCoordinateValues_PrintCoordinateValuesInLocalCulture()
        {
            // Setup
            var point = new Point3D(1.1, 2.2, 3.3);

            // Call
            string stringRepresentation = point.ToString();

            // Assert
            string expectedText = $"({point.X}, {point.Y}, {point.Z})";
            Assert.AreEqual(expectedText, stringRepresentation);
        }

        [TestFixture]
        private class Point3DEqualsTest : EqualsTestFixture<Point3D>
        {
            protected override Point3D CreateObject()
            {
                return CreatePoint3D();
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                Point3D basePoint = CreatePoint3D();
                const double offset = 1e-6;

                yield return new TestCaseData(new Point3D(basePoint.X + offset, basePoint.Y, basePoint.Z)).SetName("X");
                yield return new TestCaseData(new Point3D(basePoint.X, basePoint.Y + offset, basePoint.Z)).SetName("Y");
                yield return new TestCaseData(new Point3D(basePoint.X, basePoint.Y, basePoint.Z + offset)).SetName("Z");
            }

            private static Point3D CreatePoint3D()
            {
                var random = new Random(21);
                return new Point3D(random.NextDouble(),
                                   random.NextDouble(),
                                   random.NextDouble());
            }
        }
    }
}