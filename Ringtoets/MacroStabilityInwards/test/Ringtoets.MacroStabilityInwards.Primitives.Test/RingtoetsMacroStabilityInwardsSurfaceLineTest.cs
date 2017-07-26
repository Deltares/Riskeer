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
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives.Exceptions;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class RingtoetsMacroStabilityInwardsSurfaceLineTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            TestHelper.AssertAreEqualButNotSame(sourceData[0], surfaceLine.StartingWorldPoint);
            TestHelper.AssertAreEqualButNotSame(sourceData[3], surfaceLine.EndingWorldPoint);
        }

        [Test]
        public void ProjectGeometryToLZ_EmptyCollection_ReturnEmptyCollection()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

            // Call
            RoundedPoint2DCollection lzCoordinates = surfaceLine.ProjectGeometryToLZ();

            // Assert
            CollectionAssert.IsEmpty(lzCoordinates);
            Assert.AreEqual(2, lzCoordinates.NumberOfDecimalPlaces);
        }

        [Test]
        public void ProjectGeometryToLZ_GeometryWithOnePoint_ReturnSinglePointAtZeroXAndOriginalZ()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            const double originalZ = 3.3;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.1, 2.2, originalZ)
            });

            // Call
            RoundedPoint2DCollection lzCoordinates = surfaceLine.ProjectGeometryToLZ();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.0, originalZ)
            }, lzCoordinates);
            Assert.AreEqual(2, lzCoordinates.NumberOfDecimalPlaces);
        }

        [Test]
        public void ProjectGeometryToLZ_GeometryWithMultiplePoints_ProjectPointsOntoLzPlaneKeepingOriginalZ()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 1.0, 2.2),
                new Point3D(2.0, 3.0, 4.4), // Outlier from line specified by extrema
                new Point3D(3.0, 4.0, 7.7)
            });

            // Call
            RoundedPoint2DCollection actual = surfaceLine.ProjectGeometryToLZ();

            // Assert
            double length = Math.Sqrt(2 * 2 + 3 * 3);
            const double secondCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            var expectedCoordinatesX = new[]
            {
                0.0,
                secondCoordinateFactor * length,
                length
            };
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(actual.GetAccuracy()));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Select(p => p.Y).ToArray());
            Assert.AreEqual(2, actual.NumberOfDecimalPlaces);
        }

        [Test]
        public void SetGeometry_GeometryIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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

            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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

            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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
        public void GetZAtL_SurfaceLineVerticalAtL_ThrowsRingtoetsMacroStabilityInwardsSurfaceLineException()
        {
            // Setup
            double testZ = new Random(22).NextDouble();

            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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
            var exception = Assert.Throws<RingtoetsMacroStabilityInwardsSurfaceLineException>(test);
            string message = $"Kan geen hoogte bepalen op het punt met de lokale coördinaat {l}, omdat de profielschematisatie verticaal loopt op dat punt.";
            Assert.AreEqual(message, exception.Message);
        }

        [Test]
        public void ToString_ReturnName()
        {
            // Setup
            const string niceName = "Nice name";
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();

            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();
            var surfaceLineToUpdateFrom = new RingtoetsMacroStabilityInwardsSurfaceLine
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();

            var expectedIntersectionPoint = new Point2D(123, 456);
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();
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
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();

            // Call
            surfaceLine.CopyProperties(surfaceLineToUpdateFrom);

            // Assert
            AssertPropertiesUpdated(surfaceLineToUpdateFrom, surfaceLine);
        }

        [Test]
        public void Equals_DerivedClassWithEqualProperties_ReturnsTrue()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine profile = CreateSurfaceLineWithCharacteristicPoints();
            var derivedLayer = new TestSurfaceLine(profile);

            // Call
            bool areEqual = profile.Equals(derivedLayer);

            // Assert
            Assert.IsTrue(areEqual);
        }

        [Test]
        public void Equals_ToItself_ReturnsTrue()
        {
            // Setup
            var surfaceLineOne = new RingtoetsMacroStabilityInwardsSurfaceLine();

            // Call
            bool isLineOneEqualToLineOne = surfaceLineOne.Equals(surfaceLineOne);

            // Assert
            Assert.IsTrue(isLineOneEqualToLineOne);
        }

        [Test]
        public void Equal_SameReference_ReturnsTrue()
        {
            // Setup
            var surfaceLineOne = new RingtoetsMacroStabilityInwardsSurfaceLine();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = surfaceLineOne;

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
            var surfaceLineOne = new RingtoetsMacroStabilityInwardsSurfaceLine
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
            var surfaceLineOne = new RingtoetsMacroStabilityInwardsSurfaceLine
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineOne.Name = "Name A";

            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
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
            var surfaceLineOne = new RingtoetsMacroStabilityInwardsSurfaceLine
            {
                Name = "Name A"
            };
            surfaceLineOne.SetGeometry(new[]
            {
                new Point3D(1, 2, 3)
            });

            var surfaceLineTwo = new RingtoetsMacroStabilityInwardsSurfaceLine
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineOne.ReferenceLineIntersectionWorldPoint = new Point2D(0, 0);

            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            surfaceLineTwo.ReferenceLineIntersectionWorldPoint = new Point2D(1, 1);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentSurfaceLevelInside_ReturnsFalse()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetSurfaceLevelInsideAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentTrafficLoadInside_ReturnsFalse()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetTrafficLoadInsideAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentTrafficLoadOutside_ReturnsFalse()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetTrafficLoadOutsideAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentDikeTopAtPolder_ReturnsFalse()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetDikeTopAtPolderAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentShoulderBaseInside_ReturnsFalse()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetShoulderBaseInsideAt(points[5]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentShoulderTopInside_ReturnsFalse()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetShoulderTopInsideAt(points[7]);

            // Call
            bool isLineOneEqualToLineTwo = surfaceLineOne.Equals(surfaceLineTwo);
            bool isLineTwoEqualToLineOne = surfaceLineTwo.Equals(surfaceLineOne);

            // Assert
            Assert.IsFalse(isLineOneEqualToLineTwo);
            Assert.IsFalse(isLineTwoEqualToLineOne);
        }

        [Test]
        public void Equals_DifferentSurfaceLevelOutside_ReturnsFalse()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            Point3D[] points = surfaceLineTwo.Points.ToArray();
            surfaceLineTwo.SetSurfaceLevelOutsideAt(points[5]);

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineOne = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineTwo = CreateSurfaceLineWithCharacteristicPoints();
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLineThree = CreateSurfaceLineWithCharacteristicPoints();

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
            var surfaceLineOne = new RingtoetsMacroStabilityInwardsSurfaceLine();
            var surfaceLineTwo = new RingtoetsMacroStabilityInwardsSurfaceLine();

            // Call
            int hashCodeOne = surfaceLineOne.GetHashCode();
            int hashCodeTwo = surfaceLineTwo.GetHashCode();

            // Assert
            Assert.AreEqual(hashCodeOne, hashCodeTwo);
        }

        [TestFixture]
        public class SetDitchPolderSideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDitchPolderSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetBottomDitchPolderSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetBottomDitchDikeSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDitchDikeSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.DitchDikeSide;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Insteek sloot dijkzijde";
            }
        }

        [TestFixture]
        public class SetDikeTopAtPolderAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDikeTopAtPolderAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.DikeTopAtPolder;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Kruin binnentalud";
            }
        }

        [TestFixture]
        public class SetShoulderBaseInsideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetShoulderBaseInsideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.ShoulderBaseInside;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Insteek binnenberm";
            }
        }

        [TestFixture]
        public class SetShoulderTopInsideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetShoulderTopInsideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.ShoulderTopInside;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Kruin binnenberm";
            }
        }

        [TestFixture]
        public class SetSurfaceLevelInsideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetSurfaceLevelInsideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.SurfaceLevelInside;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Maaiveld binnenwaarts";
            }
        }

        [TestFixture]
        public class SetSurfaceLevelOutsideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetSurfaceLevelOutsideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.SurfaceLevelOutside;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Maaiveld buitenwaarts";
            }
        }

        [TestFixture]
        public class SetTrafficLoadInsideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetTrafficLoadInsideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.TrafficLoadInside;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Verkeersbelasting kant binnenwaarts";
            }
        }

        [TestFixture]
        public class SetTrafficLoadOutsideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetTrafficLoadOutsideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.TrafficLoadOutside;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Verkeersbelasting kant buitenwaarts";
            }
        }

        [TestFixture]
        public class SetDikeToeAtRiverAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDikeToeAtRiverAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDikeToeAtPolderAt(point);
            }

            protected override Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
            {
                return surfaceLine.DikeToeAtPolder;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Teen dijk binnenwaarts";
            }
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
                var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
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
                var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

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
                var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

                // Call
                TestDelegate test = () => SetCharacteristicPoint(surfaceLine, null);

                // Assert
                const string expectedMessage = "Cannot find a point in geometry using a null point.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
            }

            protected abstract void SetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, Point3D point);
            protected abstract Point3D GetCharacteristicPoint(RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine);
            protected abstract string CharacteristicPointDescription();
        }

        private class TestSurfaceLine : RingtoetsMacroStabilityInwardsSurfaceLine
        {
            public TestSurfaceLine(RingtoetsMacroStabilityInwardsSurfaceLine profile)
            {
                CopyProperties(profile);
            }
        }

        private static void AssertPropertiesUpdated(RingtoetsMacroStabilityInwardsSurfaceLine expectedSurfaceLine,
                                                    RingtoetsMacroStabilityInwardsSurfaceLine actualSurfaceLine)
        {
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.ReferenceLineIntersectionWorldPoint,
                                                actualSurfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(expectedSurfaceLine.Points, actualSurfaceLine.Points);
            TestHelper.AssertCollectionAreNotSame(expectedSurfaceLine.Points, actualSurfaceLine.Points);

            Point3D[] actualSurfaceLinePoints = actualSurfaceLine.Points;
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.SurfaceLevelOutside, actualSurfaceLine.SurfaceLevelOutside);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.TrafficLoadOutside, actualSurfaceLine.TrafficLoadOutside);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.TrafficLoadInside, actualSurfaceLine.TrafficLoadInside);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DikeTopAtPolder, actualSurfaceLine.DikeTopAtPolder);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.ShoulderBaseInside, actualSurfaceLine.ShoulderBaseInside);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.ShoulderTopInside, actualSurfaceLine.ShoulderTopInside);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.BottomDitchDikeSide, actualSurfaceLine.BottomDitchDikeSide);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.BottomDitchPolderSide, actualSurfaceLine.BottomDitchPolderSide);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DikeToeAtPolder, actualSurfaceLine.DikeToeAtPolder);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DikeToeAtRiver, actualSurfaceLine.DikeToeAtRiver);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DitchPolderSide, actualSurfaceLine.DitchPolderSide);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DitchDikeSide, actualSurfaceLine.DitchDikeSide);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.SurfaceLevelInside, actualSurfaceLine.SurfaceLevelInside);
        }

        private static void AssertAreEqualAndFromSurfaceLine(Point3D[] actualSurfaceLinePoints, Point3D expectedPoint, Point3D actualPoint)
        {
            Assert.AreEqual(expectedPoint, actualPoint);
            if (actualPoint != null)
            {
                Assert.IsTrue(actualSurfaceLinePoints.Contains(actualPoint, new ReferenceEqualityComparer<Point3D>()));
            }
        }

        private static RingtoetsMacroStabilityInwardsSurfaceLine CreateSurfaceLineWithCharacteristicPoints()
        {
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine
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
                new Point3D(5, 0, 3),
                new Point3D(6, 0, 4),
                new Point3D(7, 0, 5),
                new Point3D(8, 0, 5),
                new Point3D(9, 0, 4),
                new Point3D(10, 0, 3),
                new Point3D(11, 0, 2),
                new Point3D(12, 0, 1)
            };
            surfaceLine.SetGeometry(geometry);

            surfaceLine.SetSurfaceLevelOutsideAt(geometry[0]);
            surfaceLine.SetTrafficLoadOutsideAt(geometry[1]);
            surfaceLine.SetTrafficLoadInsideAt(geometry[2]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[3]);
            surfaceLine.SetShoulderBaseInsideAt(geometry[4]);
            surfaceLine.SetShoulderTopInsideAt(geometry[5]);
            surfaceLine.SetBottomDitchDikeSideAt(geometry[6]);
            surfaceLine.SetBottomDitchPolderSideAt(geometry[7]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[8]);
            surfaceLine.SetDikeToeAtRiverAt(geometry[9]);
            surfaceLine.SetDitchDikeSideAt(geometry[10]);
            surfaceLine.SetDitchPolderSideAt(geometry[11]);
            surfaceLine.SetSurfaceLevelInsideAt(geometry[12]);

            return surfaceLine;
        }

        private static void CreateTestGeometry(Point3D testPoint, RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine)
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
    }
}