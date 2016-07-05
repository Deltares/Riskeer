using System;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Deltares.WTIPiping;
using NUnit.Framework;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.KernelWrapper.Test
{
    [TestFixture]
    public class PipingSurfaceLineCreatorTest
    {
        [Test]
        public void Create_NormalizedLocalSurfaceLine_ReturnsSurfaceLineWithIdenticalPoints()
        {
            // Setup
            const string name = "Local coordinate surfaceline";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D (0.0, 0.0, 1.1), 
                new Point3D (2.2, 0.0, 3.3), 
                new Point3D (4.4, 0.0, 5.5)
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.X).ToArray(), actual.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Y).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Length), actual.Points.Select(p => p.Type));
        }

        [Test]
        public void Create_LocalSurfaceLineNotNormalized_TranslateAllPointsToMakeFirstCoordinateZeroX()
        {
            // Setup
            const string name = "Local coordinate surfaceline";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            const double firstX = 4.6;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D (firstX, 0.0, 1.1), 
                new Point3D (7.8, 0.0, 3.3), 
                new Point3D (9.9, 0.0, 5.5)
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            var expectedCoordinatesX = surfaceLine.Points.Select(p => p.X - firstX).ToArray();
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Y).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Length), actual.Points.Select(p => p.Type));
        }

        [Test]
        public void Create_GlobalSurfaceLine_ProjectSurfaceLineIntoLZPlaneSpannedByFirstAndLastPoint()
        {
            // Setup
            const string name = "Global coordinate surfaceline";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 1.0, 2.2),
                new Point3D(2.0, 3.0, 4.4), // Outlier from line specified by extrema
                new Point3D(3.0, 4.0, 7.7)
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            var length = Math.Sqrt(2*2 + 3*3);
            const double secondCoordinateFactor = (2.0*1.0 + 3.0*2.0)/(2.0*2.0 + 3.0*3.0);
            var expectedCoordinatesX = new[]
            {
                0.0,
                secondCoordinateFactor*length,
                length
            };
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(Enumerable.Repeat(0, surfaceLine.Points.Length).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Length), actual.Points.Select(p => p.Type));
        }

        [Test]
        public void Create_SurfaceLineWithOnlyOnePoint_CreatePipingSurfaceLineWithOnePoint()
        {
            // Setup
            const string name = "Global coordinate surfaceline";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 1.0, 2.2)
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            var expectedCoordinatesX = new[]
            {
                0.0
            };
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(0, surfaceLine.Points.Length).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Length), actual.Points.Select(p => p.Type));
        }

        [Test]
        public void Create_SurfaceLineWithoutPoints_CreateSurfaceLineWithoutPoints()
        {
            // Setup
            const string name = "Surfaceline without points";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.IsEmpty(actual.Points);
        }

        [Test]
        public void Create_SurfaceLineWithDikeToePolderSide_CreateSurfaceLineWithDikeToePolderSideSet()
        {
            // Setup
            const string name = "Surfaceline without points";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            var point = new Point3D(1.0, 1.0, 2.2);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDikeToeAtPolderAt(point);

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DikeToeAtPolder);
        }

        [Test]
        public void Create_SurfaceLineWithDitchDikeSide_CreateSurfaceLineWithDitchDikeSideSet()
        {
            // Setup
            const string name = "Surfaceline without points";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            var point = new Point3D(1.0, 1.0, 2.2);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDitchDikeSideAt(point);

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DitchDikeSide);
        }

        [Test]
        public void Create_SurfaceLineWithBottomDitchDikeSide_CreateSurfaceLineWithBottomDitchDikeSideSet()
        {
            // Setup
            const string name = "Surfaceline without points";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            var point = new Point3D(1.0, 1.0, 2.2);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetBottomDitchDikeSideAt(point);

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.BottomDitchDikeSide);
        }

        [Test]
        public void Create_SurfaceLineWithBottomPolderDikeSide_CreateSurfaceLineWithBottomDitchPolderSideSet()
        {
            // Setup
            const string name = "Surfaceline without points";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            var point = new Point3D(1.0, 1.0, 2.2);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetBottomDitchPolderSideAt(point);

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.BottomDitchPolderSide);
        }

        [Test]
        public void Create_SurfaceLineWithPolderDikeSide_CreateSurfaceLineWithDitchPolderSideSet()
        {
            // Setup
            const string name = "Surfaceline without points";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            var point = new Point3D(1.0, 1.0, 2.2);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDitchPolderSideAt(point);

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DitchPolderSide);
        }

        private void AssertPointsAreEqual(Point3D point, PipingPoint otherPoint)
        {
            if (point == null)
            {
                Assert.IsNull(otherPoint);
                return;
            }
            if (otherPoint == null)
            {
                Assert.Fail("Expected value for otherPoint.");
            }
            Assert.AreEqual(point.X, otherPoint.X);
            Assert.AreEqual(point.Y, otherPoint.Y);
            Assert.AreEqual(point.Z, otherPoint.Z);
        }
    }
}