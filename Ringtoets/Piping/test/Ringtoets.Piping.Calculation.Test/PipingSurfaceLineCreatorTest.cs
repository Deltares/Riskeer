using System;
using System.Linq;

using Deltares.WTIPiping;

using NUnit.Framework;

using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Test
{
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
                new Point3D { X = 0.0, Y = 0.0, Z = 1.1 }, 
                new Point3D { X = 2.2, Y = 0.0, Z = 3.3 }, 
                new Point3D { X = 4.4, Y = 0.0, Z = 5.5 }
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.X).ToArray(), actual.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Y).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Count()), actual.Points.Select(p => p.Type));
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
                new Point3D { X = firstX, Y = 0.0, Z = 1.1 }, 
                new Point3D { X = 7.8, Y = 0.0, Z = 3.3 }, 
                new Point3D { X = 9.9, Y = 0.0, Z = 5.5 }
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            var expectedCoordinatesX = surfaceLine.Points.Select(p => p.X - firstX).ToArray();
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Y).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Count()), actual.Points.Select(p => p.Type));
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
                new Point3D { X = 1.0, Y = 1.0, Z = 2.2 }, 
                new Point3D { X = 2.0, Y = 3.0, Z = 4.4 }, // Outlier from line specified by extrema
                new Point3D { X = 3.0, Y = 4.0, Z = 7.7 },
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            var length = Math.Sqrt(2 * 2 + 3 * 3);
            const double secondCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            var expectedCoordinatesX = new[]
            {
                0.0,
                secondCoordinateFactor * length,
                length
            };
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(0, surfaceLine.Points.Count()).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Count()), actual.Points.Select(p => p.Type));
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
                new Point3D { X = 1.0, Y = 1.0, Z = 2.2 }, 
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            var expectedCoordinatesX = new[]
            {
                0.0,
            };
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(0, surfaceLine.Points.Count()).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Count()), actual.Points.Select(p => p.Type));
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
    }
}