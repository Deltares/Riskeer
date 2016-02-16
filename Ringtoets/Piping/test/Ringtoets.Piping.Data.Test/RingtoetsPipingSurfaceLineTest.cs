using System;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Geometry;

using NUnit.Framework;
using Ringtoets.Piping.Data.Exceptions;

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
                new Point3D
                {
                    X = 1.1, Y = 2.2, Z = 3.3
                }
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
                new Point3D
                {
                    X = 1.1, Y = 2.2, Z = 3.3
                },
                new Point3D
                {
                    X = 4.4, Y = 5.5, Z = 6.6
                },
                new Point3D
                {
                    X = 7.7, Y = 8.8, Z = 9.9
                },
                new Point3D
                {
                    X = 10.10, Y = 11.11, Z = 12.12
                },
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
            surfaceLine.SetGeometry(new[]{ new Point3D { X = 1.1, Y = 2.2, Z = originalZ} });


            // Call
            Point2D[] lzCoordinates = surfaceLine.ProjectGeometryToLZ().ToArray();

            // Assert
            CollectionAssert.AreEqual(new[]{ new Point2D { X = 0.0, Y = originalZ } }, lzCoordinates);
        }

        [Test]
        public void ProjectGeometryToLZ_GeometryWithMultiplePoints_ProjectPointsOntoLzPlaneKeepingOriginalZ()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D { X = 1.0, Y = 1.0, Z = 2.2 }, 
                new Point3D { X = 2.0, Y = 3.0, Z = 4.4 }, // Outlier from line specified by extrema
                new Point3D { X = 3.0, Y = 4.0, Z = 7.7 },
            });

            // Call
            Point2D[] actual = surfaceLine.ProjectGeometryToLZ().ToArray();

            // Assert
            var length = Math.Sqrt(2 * 2 + 3 * 3);
            const double secondCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            var expectedCoordinatesX = new[]
            {
                0.0,
                secondCoordinateFactor * length,
                length
            };
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Select(p => p.Y).ToArray());
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
                new Point3D { X = 0.0, Y = 0.0, Z = 2.2 }, 
                new Point3D { X = l, Y = 0.0, Z = testZ },
                new Point3D { X = 3.0, Y = 0.0, Z = 7.7 },
            });

            // Call
            var result = surfaceLine.GetZAtL(l);

            // Assert
            Assert.AreEqual(testZ, result);
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
            StringAssert.StartsWith(Properties.Resources.RingtoetsPipingSurfaceLine_Collection_of_points_for_geometry_is_null, exception.Message);
        }

        [Test]
        public void SetGeometry_GeometryContainsNullPoint_ThrowsArgumentException()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => surfaceLine.SetGeometry(new Point3D[] { null });

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            StringAssert.StartsWith(Properties.Resources.RingtoetsPipingSurfaceLine_A_point_in_the_collection_was_null, exception.Message);
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
                new Point3D { X = 0.0, Y = 0.0, Z = 2.2 }, 
                new Point3D { X = l, Y = 0.0, Z = testZ },
                new Point3D { X = l, Y = 0.0, Z = testZ+1 },
                new Point3D { X = 3.0, Y = 0.0, Z = 7.7 },
            });

            // Call
            TestDelegate test = () => surfaceLine.GetZAtL(l);

            // Assert
            var exception = Assert.Throws<RingtoetsPipingSurfaceLineException>(test);
            var message = string.Format(Properties.Resources.RingtoetsPipingSurfaceLine_Cannot_determine_reliable_z_when_surface_line_is_vertical_in_l, l);
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
    }
}