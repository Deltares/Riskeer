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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Exceptions;

namespace Riskeer.Common.Data.Test
{
    [TestFixture]
    public class MechanismSurfaceLineBaseTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestMechanismSurfaceLine(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var surfaceLine = new TestMechanismSurfaceLine();

            // Assert
            Assert.IsInstanceOf<Observable>(surfaceLine);
            Assert.IsEmpty(surfaceLine.Name);
            CollectionAssert.IsEmpty(surfaceLine.Points);
            Assert.IsNull(surfaceLine.StartingWorldPoint);
            Assert.IsNull(surfaceLine.EndingWorldPoint);
            Assert.IsNull(surfaceLine.ReferenceLineIntersectionWorldPoint);
        }

        [Test]
        public void ReferenceLineIntersectionWorldPoint_SetNewValue_GetNewlySetValue()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();

            var point = new Point2D(1.2, 3.4);

            // Call
            surfaceLine.ReferenceLineIntersectionWorldPoint = point;

            // Assert
            Assert.AreEqual(point, surfaceLine.ReferenceLineIntersectionWorldPoint);
        }

        [Test]
        public void SetGeometry_EmptyCollection_PointsSetEmptyAndNullStartAndEndWorldPoints()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();

            IEnumerable<Point3D> sourceData = Enumerable.Empty<Point3D>();

            // Call
            surfaceLine.SetGeometry(sourceData);

            // Assert
            CollectionAssert.IsEmpty(surfaceLine.Points);
            Assert.IsNull(surfaceLine.StartingWorldPoint);
            Assert.IsNull(surfaceLine.EndingWorldPoint);
        }

        [Test]
        public void SetGeometry_CollectionOfOnePoint_InitializeStartAndEndWorldPointsToSameInstanceAndInitializePoints()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();

            var sourceData = new[]
            {
                new Point3D(1.1, 2.2, 3.3)
            };

            // Call
            surfaceLine.SetGeometry(sourceData);

            // Assert
            Assert.AreNotSame(sourceData, surfaceLine.Points);
            CollectionAssert.AreEqual(sourceData, surfaceLine.Points);
            TestHelper.AssertAreEqualButNotSame(sourceData[0], surfaceLine.StartingWorldPoint);
            TestHelper.AssertAreEqualButNotSame(sourceData[0], surfaceLine.EndingWorldPoint);
        }

        [Test]
        public void SetGeometry_CollectionOfMultiplePoints_InitializeStartAndEndWorldPointsInitializePoints()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();

            var sourceData = new[]
            {
                new Point3D(1.1, 2.2, 3.3),
                new Point3D(4.4, 5.5, 6.6),
                new Point3D(7.7, 8.8, 9.9),
                new Point3D(10.10, 11.11, 12.12)
            };

            // Call
            surfaceLine.SetGeometry(sourceData);

            // Assert
            Assert.AreNotSame(sourceData, surfaceLine.Points);
            CollectionAssert.AreEqual(sourceData, surfaceLine.Points);
            Assert.AreEqual(sourceData[0], surfaceLine.StartingWorldPoint);
            Assert.AreEqual(sourceData[3], surfaceLine.EndingWorldPoint);
        }

        [Test]
        public void SetGeometry_GeometryIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            StringAssert.StartsWith("De geometrie die opgegeven werd voor de profielschematisatie heeft geen waarde.", exception.Message);
        }

        [Test]
        public void SetGeometry_GeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(new Point3D[]
            {
                null
            });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            StringAssert.StartsWith("Een punt in de geometrie voor de profielschematisatie heeft geen waarde.", exception.Message);
        }

        [Test]
        public void GetZAtL_GeometryIsEmpty_ThrowsInvalidOperationException()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();
            var l = (RoundedDouble) new Random(21).NextDouble();

            // Call
            TestDelegate test = () => surfaceLine.GetZAtL(l);

            // Assert
            string exceptionMessage = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual("De profielschematisatie heeft geen geometrie.", exceptionMessage);
        }

        [Test]
        public void GetZAtL_SurfaceLineContainsPointAtL_ReturnsZOfPoint()
        {
            // Setup
            double testZ = new Random(22).NextDouble();

            var surfaceLine = new TestMechanismSurfaceLine();
            var l = (RoundedDouble) 2.0;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 2.2),
                new Point3D(l, 0.0, testZ),
                new Point3D(3.0, 0.0, 7.7)
            });

            // Call
            double result = surfaceLine.GetZAtL(l);

            // Assert
            Assert.AreEqual(testZ, result, 1e-2);
        }

        [Test]
        [SetCulture("nl-NL")]
        [TestCase(-1)]
        [TestCase(-5e-3)]
        [TestCase(3.1 + 5e-3)]
        [TestCase(4.0)]
        public void GetZAtL_SurfaceLineDoesNotContainsPointAtL_ThrowsArgumentOutOfRangeException(double l)
        {
            // Setup
            double testZ = new Random(22).NextDouble();

            var surfaceLine = new TestMechanismSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 0.0, 2.2),
                new Point3D(2.0, 0.0, testZ),
                new Point3D(4.1, 0.0, 7.7)
            });

            // Call
            TestDelegate test = () => surfaceLine.GetZAtL((RoundedDouble) l);

            // Assert
            const string expectedMessage = "Kan geen hoogte bepalen. De lokale coördinaat moet in het bereik [0,0, 3,1] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void GetZAtL_SurfaceLineVerticalAtL_ThrowsMechanismSurfaceLineException()
        {
            // Setup
            double testZ = new Random(22).NextDouble();

            var surfaceLine = new TestMechanismSurfaceLine();
            var l = (RoundedDouble) 2.0;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 2.2),
                new Point3D(l, 0.0, testZ),
                new Point3D(l, 0.0, testZ + 1),
                new Point3D(3.0, 0.0, 7.7)
            });

            // Call
            TestDelegate test = () => surfaceLine.GetZAtL(l);

            // Assert
            var exception = Assert.Throws<MechanismSurfaceLineException>(test);
            string message = $"Kan geen hoogte bepalen op het punt met de lokale coördinaat {l}, omdat de profielschematisatie verticaal loopt op dat punt.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void ToString_ReturnName()
        {
            // Setup
            const string niceName = "Nice name";
            var surfaceLine = new TestMechanismSurfaceLine(niceName);

            // Call
            string text = surfaceLine.ToString();

            // Assert
            Assert.AreEqual(niceName, text);
        }

        [Test]
        [TestCase(5.0)]
        [TestCase(1.375)]
        [TestCase(-0.005)]
        [TestCase(-5)]
        public void ValidateInRange_PointNotInRange_ReturnsFalse(double invalidValue)
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new TestMechanismSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            bool valid = surfaceLine.ValidateInRange(invalidValue);

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        [TestCase(-0e-3)]
        [TestCase(1.37)]
        [TestCase(1.0)]
        public void ValidateInRange_PointInRange_ReturnsTrue(double validValue)
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new TestMechanismSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            bool valid = surfaceLine.ValidateInRange(validValue);

            // Assert
            Assert.IsTrue(valid);
        }

        [Test]
        public void GetLocalPointFromGeometry_ValidSurfaceLine_ReturnsLocalPoint()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new TestMechanismSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            Point2D localPoint = surfaceLine.GetLocalPointFromGeometry(testPoint);

            // Assert
            Assert.AreEqual(new Point2D(0.04, 4.4), localPoint);
        }

        [Test]
        public void GetLocalPointFromGeometry_NoPointsOnSurfaceLine_ReturnsPointWithNanValues()
        {
            // Setup
            var surfaceLine = new TestMechanismSurfaceLine();

            // Call
            Point2D localPoint = surfaceLine.GetLocalPointFromGeometry(new Point3D(1.0, 2.2, 4.4));

            // Assert
            Assert.AreEqual(new Point2D(double.NaN, double.NaN), localPoint);
        }

        [Test]
        public void GetLocalPointFromGeometry_OnePointOnSurfaceLine_ReturnsPointWithNanValues()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new TestMechanismSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                testPoint
            });

            // Call
            Point2D localPoint = surfaceLine.GetLocalPointFromGeometry(testPoint);

            // Assert
            Assert.AreEqual(new Point2D(double.NaN, double.NaN), localPoint);
        }

        private class TestMechanismSurfaceLine : MechanismSurfaceLineBase
        {
            public TestMechanismSurfaceLine(string name = "") : base(name) {}
        }

        private static void CreateTestGeometry(Point3D testPoint, TestMechanismSurfaceLine surfaceLine)
        {
            var random = new Random(21);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(testPoint),
                new Point3D(2 + random.NextDouble(), random.NextDouble(), random.NextDouble())
            });
        }
    }
}