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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data.Exceptions;
using Ringtoets.Piping.Data.Properties;

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
            Assert.AreEqual(String.Empty, surfaceLine.Name);
            CollectionAssert.IsEmpty(surfaceLine.Points);
            Assert.IsNull(surfaceLine.StartingWorldPoint);
            Assert.IsNull(surfaceLine.EndingWorldPoint);
        }

        [Test]
        public void SetGeometry_EmptyCollection_PointsSetEmptyAndNullStartAndEndWorldPoints()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var sourceData = Enumerable.Empty<Point3D>();

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
                new Point3D(10.10, 11.11, 12.12),
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
            IEnumerable<Point2D> lzCoordinates = surfaceLine.ProjectGeometryToLZ();

            // Assert
            CollectionAssert.IsEmpty(lzCoordinates);
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
            Point2D[] lzCoordinates = surfaceLine.ProjectGeometryToLZ().ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.0, originalZ)
            }, lzCoordinates);
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
                new Point3D(3.0, 4.0, 7.7),
            });

            // Call
            Point2D[] actual = surfaceLine.ProjectGeometryToLZ().ToArray();

            // Assert
            var length = Math.Sqrt(2*2 + 3*3);
            const double secondCoordinateFactor = (2.0*1.0 + 3.0*2.0)/(2.0*2.0 + 3.0*3.0);
            var expectedCoordinatesX = new[]
            {
                0.0,
                secondCoordinateFactor*length,
                length
            };
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Select(p => p.Y).ToArray());
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
            StringAssert.StartsWith(Resources.RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null, exception.Message);
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
            StringAssert.StartsWith(Resources.RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null, exception.Message);
        }

        [Test]
        public void GetZAtL_GeometryIsEmpty_ThrowInvalidOperationException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var l = new Random(21).NextDouble();

            // Call
            TestDelegate test = () => surfaceLine.GetZAtL(l);

            // Assert
            var exceptionMessage = Assert.Throws<InvalidOperationException>(test).Message;
            Assert.AreEqual(Resources.RingtoetsPipingSurfaceLine_SurfaceLine_has_no_Geometry, exceptionMessage);
        }

        [Test]
        public void GetZAtL_SurfaceLineContainsPointAtL_ReturnsZOfPoint()
        {
            // Setup
            var testZ = new Random(22).NextDouble();

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var l = 2.0;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 2.2),
                new Point3D(l, 0.0, testZ),
                new Point3D(3.0, 0.0, 7.7),
            });

            // Call
            var result = surfaceLine.GetZAtL(l);

            // Assert
            Assert.AreEqual(testZ, result);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(-1e-6)]
        [TestCase(3.1 + 1e-6)]
        [TestCase(4.0)]
        public void GetZAtL_SurfaceLineDoesNotContainsPointAtL_ThrowsArgumentOutOfRange(double l)
        {
            // Setup
            var testZ = new Random(22).NextDouble();

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 0.0, 2.2),
                new Point3D(2.0, 0.0, testZ),
                new Point3D(4.1, 0.0, 7.7),
            });

            // Call
            TestDelegate test = () => surfaceLine.GetZAtL(l);

            // Assert
            var expectedMessage = string.Format("Kan geen hoogte bepalen. L moet in het bereik van [{0}, {1}] liggen.",
                                                0, 3.1);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, expectedMessage);
        }

        [Test]
        public void GetZAtL_SurfaceLineVerticalAtL_ThrowsRingtoetsPipingSurfaceLineException()
        {
            // Setup
            var testZ = new Random(22).NextDouble();

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var l = 2.0;
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
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l, l);
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
            var text = surfaceLine.ToString();

            // Assert
            Assert.AreEqual(niceName, text);
        }

        [Test]
        public void SetDitchPolderSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            var testX = 1.0;
            var testY = 2.2;
            var testZ = 4.4;
            Point3D testPoint = new Point3D(testX, testY, testZ);
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
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            Point3D testPoint = new Point3D(x, y, z);
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDitchPolderSideAt(testPoint);

            // Assert
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point_3_,
                x, y, z, Resources.CharacteristicPoint_DitchPolderSide);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void SetDitchPolderSideAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDitchPolderSideAt(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot find a point in geometry using a null point.");
        }

        [Test]
        public void SetBottomDitchPolderSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            var testX = 1.0;
            var testY = 2.2;
            var testZ = 4.4;
            Point3D testPoint = new Point3D(testX, testY, testZ);
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
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            Point3D testPoint = new Point3D(x, y, z);
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchPolderSideAt(testPoint);

            // Assert
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point_3_,
                x, y, z, Resources.CharacteristicPoint_BottomDitchPolderSide);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void SetBottomDitchPolderSideAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchPolderSideAt(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot find a point in geometry using a null point.");
        }

        [Test]
        public void SetBottomDitchDikeSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            var testX = 1.0;
            var testY = 2.2;
            var testZ = 4.4;
            Point3D testPoint = new Point3D(testX, testY, testZ);
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
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            Point3D testPoint = new Point3D(x, y, z);
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchDikeSideAt(testPoint);

            // Assert
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point_3_,
                x, y, z, Resources.CharacteristicPoint_BottomDitchDikeSide);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void SetBottomDitchDikeSideAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetBottomDitchDikeSideAt(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot find a point in geometry using a null point.");
        }

        [Test]
        public void SetDitchDikeSideAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            var testX = 1.0;
            var testY = 2.2;
            var testZ = 4.4;
            Point3D testPoint = new Point3D(testX, testY, testZ);
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
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            Point3D testPoint = new Point3D(x, y, z);
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDitchDikeSideAt(testPoint);

            // Assert
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point_3_,
                x, y, z, Resources.CharacteristicPoint_DitchDikeSide);
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
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot find a point in geometry using a null point.");
        }

        [Test]
        public void SetDikeToeAtRiverAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            var testX = 1.0;
            var testY = 2.2;
            var testZ = 4.4;
            Point3D testPoint = new Point3D(testX, testY, testZ);
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
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            Point3D testPoint = new Point3D(x, y, z);
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtRiverAt(testPoint);

            // Assert
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point_3_,
                x, y, z, Resources.CharacteristicPoint_DikeToeAtRiver);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void SetDikeToeAtRiverAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtRiverAt(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot find a point in geometry using a null point.");
        }

        [Test]
        public void SetDikeToeAtPolderAt_PointInGeometry_PointSetFromGeometry()
        {
            // Setup
            var testX = 1.0;
            var testY = 2.2;
            var testZ = 4.4;
            Point3D testPoint = new Point3D(testX, testY, testZ);
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
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            Point3D testPoint = new Point3D(x, y, z);
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtPolderAt(testPoint);

            // Assert
            var message = string.Format(Resources.RingtoetsPipingSurfaceLine_SetCharacteristicPointAt_Geometry_does_not_contain_point_at_0_1_2_to_assign_as_characteristic_point_3_,
                x, y, z, Resources.CharacteristicPoint_DikeToeAtPolder);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void SetDikeToeAtPolderAt_Null_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetDikeToeAtPolderAt(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, "Cannot find a point in geometry using a null point.");
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