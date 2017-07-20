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
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.Exceptions;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
        public void GetZAtL_GeometryIsEmpty_ThrowInvalidOperationException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
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

            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
        public void GetZAtL_SurfaceLineDoesNotContainsPointAtL_ThrowsArgumentOutOfRange(double l)
        {
            // Setup
            double testZ = new Random(22).NextDouble();

            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
        public void GetZAtL_SurfaceLineVerticalAtL_ThrowsRingtoetsPipingSurfaceLineException()
        {
            // Setup
            double testZ = new Random(22).NextDouble();

            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
            var exception = Assert.Throws<RingtoetsPipingSurfaceLineException>(test);
            string message = $"Kan geen hoogte bepalen op het punt met de lokale coördinaat {l}, omdat de profielschematisatie verticaal loopt op dat punt.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void ToString_ReturnName()
        {
            // Setup
            const string niceName = "Nice name";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = niceName
            };

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                testPoint
            });

            // Call
            Point2D localPoint = surfaceLine.GetLocalPointFromGeometry(testPoint);

            // Assert
            Assert.AreEqual(new Point2D(double.NaN, double.NaN), localPoint);
        }

        [Test]
        public void CopyProperties_WithSurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate call = () => surfaceLine.CopyProperties(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("fromSurfaceLine", paramName);
        }

        [Test]
        public void CopyProperties_LineWithUpdatedGeometricPoints_PropertiesUpdated()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();

            RingtoetsPipingSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();
            var expectedGeometry = new List<Point3D>
            {
                new Point3D(0, 1, 2),
                new Point3D(3, 4, 5),
                new Point3D(6, 7, 8)
            };
            expectedGeometry.AddRange(surfaceLine.Points);
            surfaceLineToUpdateFrom.SetGeometry(expectedGeometry);

            // Call
            surfaceLine.CopyProperties(surfaceLineToUpdateFrom);

            // Assert
            AssertPropertiesUpdated(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void CopyProperties_LineUpdatedWithRemovedCharacteristicPoints_PropertiesUpdated()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();
            var surfaceLineToUpdateFrom = new RingtoetsPipingSurfaceLine
            {
                Name = surfaceLine.Name,
                ReferenceLineIntersectionWorldPoint = surfaceLine.ReferenceLineIntersectionWorldPoint
            };
            surfaceLineToUpdateFrom.SetGeometry(surfaceLine.Points);

            // Call
            surfaceLine.CopyProperties(surfaceLineToUpdateFrom);

            // Assert
            AssertPropertiesUpdated(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void CopyProperties_LineWithUpdatedReferenceLineWorldPoint_PropertiesUpdated()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();

            var expectedIntersectionPoint = new Point2D(123, 456);
            RingtoetsPipingSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint = expectedIntersectionPoint;

            // Call
            surfaceLine.CopyProperties(surfaceLineToUpdateFrom);

            // Assert
            AssertPropertiesUpdated(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void CopyProperties_LineWithUpdatedGeometryAndReferenceLineIntersectionAndCharacteristicPoints_PropertiesUpdated()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            RingtoetsPipingSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();

            // Call
            surfaceLine.CopyProperties(surfaceLineToUpdateFrom);

            // Assert
            AssertPropertiesUpdated(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var surfaceLineOne = new RingtoetsPipingSurfaceLine();

            // Call
            bool isLineOneEqualToLineOne = surfaceLineOne.Equals(surfaceLineOne);

            // Assert
            Assert.IsTrue(isLineOneEqualToLineOne);
        }

        [Test]
        public void Equal_SameReference_ReturnsTrue()
        {
            // Setup
            var surfaceLineOne = new RingtoetsPipingSurfaceLine();
            RingtoetsPipingSurfaceLine surfaceLineTwo = surfaceLineOne;

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsTrue(isLineOneEqualToLineTwo);
            Assert.IsTrue(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_ToNull_ReturnsFalse()
        {
            // Setup
            var surfaceLineOne = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };

            // Call
            bool isLineOneEqualToNull = surfaceLineOne.Equals(null);

            // Assert
            Assert.IsFalse(isLineOneEqualToNull);
        }

        [Test]
        public void Equals_ToDifferentType_ReturnsFalse()
        {
            // Setup
            var surfaceLineOne = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };

            var differentType = new object();

            // Call
            bool isSurfaceLineEqualToDifferentType = surfaceLineOne.Equals(differentType);
            bool isDifferentTypeEqualToSurfaceLine = differentType.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isSurfaceLineEqualToDifferentType);
            Assert.IsFalse(isDifferentTypeEqualToSurfaceLine);
        }

        [Test]
        public void Equals_DifferentNames_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineOne.Name = "Name A";

            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineTwo.Name = "Name B";

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentGeometries_ReturnsFalse()
        {
            // Setup
            var surfaceLineOne = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
            surfaceLineOne.SetGeometry(new[]
            {
                new Point3D(1, 2, 3)
            });

            var surfaceLineTwo = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A"
            };
            surfaceLineTwo.SetGeometry(new[]
            {
                new Point3D(3, 4, 5)
            });

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentReferenceLineIntersectionWorldPoint_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineOne.ReferenceLineIntersectionWorldPoint = new Point2D(0, 0);

            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineTwo.ReferenceLineIntersectionWorldPoint = new Point2D(1, 1);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentBottomDitchDikeSide_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetBottomDitchDikeSideAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentBottomDitchPolderSide_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetBottomDitchPolderSideAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentDikeToeAtPolder_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetDikeToeAtPolderAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentDikeToeAtRiver_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetDikeToeAtRiverAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentDitchDikeSide_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetDitchDikeSideAt(points[1]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentDitchPolderSide_ReturnsFalse()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetDitchPolderSideAt(points[1]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_NamesGeometriesAndReferenceLineIntersectionWorldPointAndCharacteristicPointsEqual_ReturnsTrue()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsTrue(isLineOneEqualToLineTwo);
            Assert.IsTrue(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_TransitivePropertyWithSameNamesAndGeometry_ReturnsTrue()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsPipingSurfaceLine surfaceLineThree = CreateSurfaceLineWithCharacteristicPoints();

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineThree = surfaceLineTwo.Equals(surfaceLineThree);
            bool isLineOneEqualToLineThree = surfaceLineOne.Equals(surfaceLineThree);

            // Assert
            Assert.IsTrue(isLineOneEqualToLineTwo);
            Assert.IsTrue(isLineTwoEqualToLineThree);
            Assert.IsTrue(isLineOneEqualToLineThree);
        }

        [Test]
        public void GetHashCode_EqualSurfaceLines_ReturnSameHashCode()
        {
            // Setup
            var surfaceLineOne = new RingtoetsPipingSurfaceLine();
            var surfaceLineTwo = new RingtoetsPipingSurfaceLine();

            // Call
            int hashCodeOne = surfaceLineOne.GetHashCode();
            int hashCodeTwo = surfaceLineTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        public abstract class SetCharacteristicPointTest
        {
            [Test]
            public void PointInGeometry_PointSetFromGeometry()
            {
                // Setup
                const double testX = 1.0;
                const double testY = 2.2;
                const double testZ = 4.4;
                var testPoint = new Point3D(testX, testY, testZ);
                var surfaceLine = new RingtoetsPipingSurfaceLine();
                CreateTestGeometry(testPoint, surfaceLine);

                // Call
                SetCharacteristicPoint(surfaceLine, testPoint);

                // Assert
                Assert.AreEqual(testPoint, GetCharacteristicPoint(surfaceLine));
                Assert.AreNotSame(testPoint, GetCharacteristicPoint(surfaceLine));
            }

            [Test]
            public void GeometryEmpty_ThrowsInvalidOperationException()
            {
                // Setup
                var random = new Random(21);
                var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
                var surfaceLine = new RingtoetsPipingSurfaceLine();

                // Call
                TestDelegate test = () => SetCharacteristicPoint(surfaceLine, testPoint);

                // Assert
                string expectedMessage = $"De geometrie bevat geen punt op locatie {testPoint} om als '{CharacteristicPointDescription()}' in te stellen.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
            }

            [Test]
            public void Null_ThrowsArgumentNullException()
            {
                // Setup
                var surfaceLine = new RingtoetsPipingSurfaceLine();

                // Call
                TestDelegate test = () => SetCharacteristicPoint(surfaceLine, null);

                // Assert
                const string expectedMessage = "Cannot find a point in geometry using a null point.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
            }

            protected abstract void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D point);
            protected abstract Point3D GetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine);
            protected abstract string CharacteristicPointDescription();
        }

        [TestFixture]
        public class SetDitchPolderSideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDitchPolderSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine)
            {
                return surfaceLine.DitchPolderSide;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Insteek sloot polderzijde";
            }
        }

        [TestFixture]
        public class SetBottomDitchPolderSideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetBottomDitchPolderSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine)
            {
                return surfaceLine.BottomDitchPolderSide;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Slootbodem polderzijde";
            }
        }

        [TestFixture]
        public class SetBottomDitchDikeSideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetBottomDitchDikeSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine)
            {
                return surfaceLine.BottomDitchDikeSide;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Slootbodem dijkzijde";
            }
        }

        [TestFixture]
        public class SetDitchDikeSideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDitchDikeSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine)
            {
                return surfaceLine.DitchDikeSide;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Insteek sloot dijkzijde";
            }
        }

        [TestFixture]
        public class SetDikeToeAtRiverAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDikeToeAtRiverAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine)
            {
                return surfaceLine.DikeToeAtRiver;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Teen dijk buitenwaarts";
            }
        }

        [TestFixture]
        public class SetDikeToeAtPolderAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDikeToeAtPolderAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsPipingSurfaceLine surfaceLine)
            {
                return surfaceLine.DikeToeAtPolder;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Teen dijk binnenwaarts";
            }
        }

        private static RingtoetsPipingSurfaceLine CreateSurfaceLineWithCharacteristicPoints()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Name A",
                ReferenceLineIntersectionWorldPoint = new Point2D(0, 0)
            };
            var geometry = new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3)
            };
            surfaceLine.SetGeometry(geometry);
            surfaceLine.SetBottomDitchDikeSideAt(geometry[0]);
            surfaceLine.SetBottomDitchPolderSideAt(geometry[1]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[2]);
            surfaceLine.SetDikeToeAtRiverAt(geometry[3]);
            surfaceLine.SetDitchDikeSideAt(geometry[4]);
            surfaceLine.SetDitchPolderSideAt(geometry[5]);

            return surfaceLine;
        }

        private static void CreateTestGeometry(Point3D testPoint, RingtoetsPipingSurfaceLine surfaceLine)
        {
            var random = new Random(21);
            var points = new[]
            {
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(testPoint),
                new Point3D(2 + random.NextDouble(), random.NextDouble(), random.NextDouble())
            };
            surfaceLine.SetGeometry(points);
        }

        private static void AssertPropertiesUpdated(RingtoetsPipingSurfaceLine expectedSurfaceLine, RingtoetsPipingSurfaceLine actualSurfaceLine)
        {
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.ReferenceLineIntersectionWorldPoint,
                                                actualSurfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(expectedSurfaceLine.Points, actualSurfaceLine.Points);
            TestHelper.AssertCollectionAreNotSame(expectedSurfaceLine.Points, actualSurfaceLine.Points);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.BottomDitchDikeSide, actualSurfaceLine.BottomDitchDikeSide);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.BottomDitchPolderSide, actualSurfaceLine.BottomDitchPolderSide);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.DikeToeAtPolder, actualSurfaceLine.DikeToeAtPolder);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.DikeToeAtRiver, actualSurfaceLine.DikeToeAtRiver);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.DitchPolderSide, actualSurfaceLine.DitchPolderSide);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.DitchDikeSide, actualSurfaceLine.DitchDikeSide);
        }
    }
}