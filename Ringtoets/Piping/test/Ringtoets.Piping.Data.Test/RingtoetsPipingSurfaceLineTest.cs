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
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Primitives.Exceptions;
using PipingPrimitivesResources = Ringtoets.Piping.Primitives.Properties.Resources;

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
            Assert.AreEqual(string.Empty, surfaceLine.Name);
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
            Assert.AreSame(sourceData[0], surfaceLine.StartingWorldPoint);
            Assert.AreSame(sourceData[0], surfaceLine.EndingWorldPoint);
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
            Assert.AreSame(sourceData[0], surfaceLine.StartingWorldPoint);
            Assert.AreSame(sourceData[3], surfaceLine.EndingWorldPoint);
        }

        [Test]
        public void ProjectGeometryToLZ_EmptyCollection_ReturnEmptyCollection()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
            var surfaceLine = new RingtoetsPipingSurfaceLine();
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
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            StringAssert.StartsWith(PipingPrimitivesResources.RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null, exception.Message);
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
            StringAssert.StartsWith(PipingPrimitivesResources.RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null, exception.Message);
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
            Assert.AreEqual(PipingPrimitivesResources.RingtoetsPipingSurfaceLine_SurfaceLine_has_no_Geometry, exceptionMessage);
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
            string message = string.Format(PipingPrimitivesResources.RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l, l);
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
        public void SetDitchPolderSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            surfaceLine.SetDitchPolderSideAt(testPoint);

            // Assert
            Assert.AreEqual(testPoint, surfaceLine.DitchPolderSide);
            Assert.AreNotSame(testPoint, surfaceLine.DitchPolderSide);
        }

        [Test]
        public void SetDitchPolderSideAt_GeometryEmpty_ThrowsInvalidOperationException()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDitchPolderSideAt(testPoint);

            // Assert
            string expectedMessage = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                                   testPoint, Resources.CharacteristicPoint_DitchPolderSide);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void SetDitchPolderSideAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDitchPolderSideAt(null);

            // Assert
            const string expectedMessage = "Cannot find a point in geometry using a null point.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void SetBottomDitchPolderSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            surfaceLine.SetBottomDitchPolderSideAt(testPoint);

            // Assert
            Assert.AreEqual(testPoint, surfaceLine.BottomDitchPolderSide);
            Assert.AreNotSame(testPoint, surfaceLine.BottomDitchPolderSide);
        }

        [Test]
        public void SetBottomDitchPolderSideAt_GeometryEmpty_ThrowsInvalidOperationException()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchPolderSideAt(testPoint);

            // Assert
            string expectedMessage = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                                   testPoint, Resources.CharacteristicPoint_BottomDitchPolderSide);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void SetBottomDitchPolderSideAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchPolderSideAt(null);

            // Assert
            const string expectedMessage = "Cannot find a point in geometry using a null point.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void SetBottomDitchDikeSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            surfaceLine.SetBottomDitchDikeSideAt(testPoint);

            // Assert
            Assert.AreEqual(testPoint, surfaceLine.BottomDitchDikeSide);
            Assert.AreNotSame(testPoint, surfaceLine.BottomDitchDikeSide);
        }

        [Test]
        public void SetBottomDitchDikeSideAt_GeometryEmpty_ThrowsInvalidOperationException()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchDikeSideAt(testPoint);

            // Assert
            string expectedMessage = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                                   testPoint, Resources.CharacteristicPoint_BottomDitchDikeSide);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void SetBottomDitchDikeSideAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchDikeSideAt(null);

            // Assert
            const string expectedMessage = "Cannot find a point in geometry using a null point.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void SetDitchDikeSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            surfaceLine.SetDitchDikeSideAt(testPoint);

            // Assert
            Assert.AreEqual(testPoint, surfaceLine.DitchDikeSide);
            Assert.AreNotSame(testPoint, surfaceLine.DitchDikeSide);
        }

        [Test]
        public void SetDitchDikeSideAt_GeometryEmpty_ThrowsInvalidOperationException()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDitchDikeSideAt(testPoint);

            // Assert
            string message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                           testPoint, Resources.CharacteristicPoint_DitchDikeSide);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void SetDitchDikeSideAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDitchDikeSideAt(null);

            // Assert
            const string expectedMessage = "Cannot find a point in geometry using a null point.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void SetDikeToeAtRiverAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            surfaceLine.SetDikeToeAtRiverAt(testPoint);

            // Assert
            Assert.AreEqual(testPoint, surfaceLine.DikeToeAtRiver);
            Assert.AreNotSame(testPoint, surfaceLine.DikeToeAtRiver);
        }

        [Test]
        public void SetDikeToeAtRiverAt_GeometryEmpty_ThrowsInvalidOperationException()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtRiverAt(testPoint);

            // Assert
            string expectedMessage = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                                   testPoint, Resources.CharacteristicPoint_DikeToeAtRiver);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void SetDikeToeAtRiverAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtRiverAt(null);

            // Assert
            const string expectedMessage = "Cannot find a point in geometry using a null point.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void SetDikeToeAtPolderAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            const double testX = 1.0;
            const double testY = 2.2;
            const double testZ = 4.4;
            var testPoint = new Point3D(testX, testY, testZ);
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            CreateTestGeometry(testPoint, surfaceLine);

            // Call
            surfaceLine.SetDikeToeAtPolderAt(testPoint);

            // Assert
            Assert.AreEqual(testPoint, surfaceLine.DikeToeAtPolder);
            Assert.AreNotSame(testPoint, surfaceLine.DikeToeAtPolder);
        }

        [Test]
        public void SetDikeToeAtPolderAt_GeometryEmpty_ThrowsInvalidOperationException()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtPolderAt(testPoint);

            // Assert
            string expectedMessage = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_to_assign_as_characteristic_point_1_,
                                                   testPoint, Resources.CharacteristicPoint_DikeToeAtPolder);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void SetDikeToeAtPolderAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtPolderAt(null);

            // Assert
            const string expectedMessage = "Cannot find a point in geometry using a null point.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
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
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(expectedGeometry, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
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
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
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
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(expectedIntersectionPoint, surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
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
            Assert.AreEqual(surfaceLineToUpdateFrom.Name, surfaceLine.Name);
            Assert.AreEqual(surfaceLineToUpdateFrom.ReferenceLineIntersectionWorldPoint,
                            surfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(surfaceLineToUpdateFrom.Points, surfaceLine.Points);
            AssertCharacteristicPoints(surfaceLineToUpdateFrom, surfaceLine);
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

        private static void AssertCharacteristicPoints(RingtoetsPipingSurfaceLine expectedSurfaceLine, RingtoetsPipingSurfaceLine actualSurfaceLine)
        {
            Assert.AreEqual(expectedSurfaceLine.BottomDitchDikeSide, actualSurfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(expectedSurfaceLine.BottomDitchPolderSide, actualSurfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtPolder, actualSurfaceLine.DikeToeAtPolder);
            Assert.AreEqual(expectedSurfaceLine.DikeToeAtRiver, actualSurfaceLine.DikeToeAtRiver);
            Assert.AreEqual(expectedSurfaceLine.DitchPolderSide, actualSurfaceLine.DitchPolderSide);
            Assert.AreEqual(expectedSurfaceLine.DitchDikeSide, actualSurfaceLine.DitchDikeSide);
        }

        private static void CreateTestGeometry(Point3D testPoint, RingtoetsPipingSurfaceLine surfaceLine)
        {
            var random = new Random(21);
            var points = new[]
            {
                new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble()),
                new Point3D(testPoint.X, testPoint.Y, testPoint.Z),
                new Point3D(2 + random.NextDouble(), random.NextDouble(), random.NextDouble())
            };
            surfaceLine.SetGeometry(points);
        }
    }
}