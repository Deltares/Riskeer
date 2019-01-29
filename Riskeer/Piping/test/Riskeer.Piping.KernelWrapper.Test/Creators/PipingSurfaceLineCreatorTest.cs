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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Deltares.WTIPiping;
using NUnit.Framework;
using Riskeer.Piping.KernelWrapper.Creators;
using PipingSurfaceLine = Ringtoets.Piping.Primitives.PipingSurfaceLine;

namespace Riskeer.Piping.KernelWrapper.Test.Creators
{
    [TestFixture]
    public class PipingSurfaceLineCreatorTest
    {
        [Test]
        public void Create_NormalizedLocalSurfaceLine_ReturnsSurfaceLineWithIdenticalPoints()
        {
            // Setup
            const string name = "Local coordinate surface line";
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.1),
                new Point3D(2.2, 0.0, 3.3),
                new Point3D(4.4, 0.0, 5.5)
            });

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

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
            const string name = "Local coordinate surface line";
            const double firstX = 4.6;
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(firstX, 0.0, 1.1),
                new Point3D(7.8, 0.0, 3.3),
                new Point3D(9.9, 0.0, 5.5)
            });

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            double[] expectedCoordinatesX = surfaceLine.Points.Select(p => p.X - firstX).ToArray();
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Y).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Count()), actual.Points.Select(p => p.Type));
        }

        [Test]
        public void Create_GlobalSurfaceLine_ProjectSurfaceLineIntoLZPlaneSpannedByFirstAndLastPoint()
        {
            // Setup
            const string name = "Global coordinate surface line";
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 1.0, 2.2),
                new Point3D(2.0, 3.0, 4.4), // Outlier from line specified by extrema
                new Point3D(3.0, 4.0, 7.7)
            });

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            double length = Math.Sqrt(2 * 2 + 3 * 3);
            const double secondCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            double[] expectedCoordinatesX =
            {
                0.0,
                secondCoordinateFactor * length,
                length
            };
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Points.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(Enumerable.Repeat(0, surfaceLine.Points.Count()).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Count()), actual.Points.Select(p => p.Type));
        }

        [Test]
        public void Create_SurfaceLineWithOnlyOnePoint_CreatePipingSurfaceLineWithOnePoint()
        {
            // Setup
            const string name = "Global coordinate surface line";
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 1.0, 2.2)
            });

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            double[] expectedCoordinatesX =
            {
                0.0
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
            const string name = "Surface line without points";
            var surfaceLine = new PipingSurfaceLine(name);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.IsEmpty(actual.Points);
        }

        [Test]
        public void Create_SurfaceLineWithDikeToePolderSide_CreateSurfaceLineWithDikeToePolderSideSet()
        {
            // Setup
            const string name = "Surface line without points";
            var point = new Point3D(1.0, 1.0, 2.2);
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDikeToeAtPolderAt(point);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DikeToeAtPolder);
        }

        [Test]
        public void Create_SurfaceLineWithDitchDikeSide_CreateSurfaceLineWithDitchDikeSideSet()
        {
            // Setup
            const string name = "Surface line without points";
            var point = new Point3D(1.0, 1.0, 2.2);
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDitchDikeSideAt(point);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DitchDikeSide);
        }

        [Test]
        public void Create_SurfaceLineWithDitchDikeSideAndDikeToeAtPolder_CreateSurfaceLineWithDitchDikeSideAndDikeToeAtPolderSet()
        {
            // Setup
            const string name = "Surface line without points";
            var point = new Point3D(1.0, 1.0, 2.2);
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDitchDikeSideAt(point);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DitchDikeSide);
        }

        [Test]
        public void Create_SurfaceLineWithBottomDitchDikeSide_CreateSurfaceLineWithBottomDitchDikeSideSet()
        {
            // Setup
            const string name = "Surface line without points";
            var point = new Point3D(1.0, 1.0, 2.2);
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetBottomDitchDikeSideAt(point);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.BottomDitchDikeSide);
        }

        [Test]
        public void Create_SurfaceLineWithBottomPolderDikeSide_CreateSurfaceLineWithBottomDitchPolderSideSet()
        {
            // Setup
            const string name = "Surface line without points";
            var point = new Point3D(1.0, 1.0, 2.2);
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetBottomDitchPolderSideAt(point);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.BottomDitchPolderSide);
        }

        [Test]
        public void Create_SurfaceLineWithPolderDikeSide_CreateSurfaceLineWithDitchPolderSideSet()
        {
            // Setup
            const string name = "Surface line without points";
            var point = new Point3D(1.0, 1.0, 2.2);
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDitchPolderSideAt(point);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(1, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DitchPolderSide);
        }

        [Test]
        public void Create_SurfaceLineWithMultipleCharacteristicTypesForOnePoint_CreateSurfaceLineWithPointsForEachType()
        {
            // Setup
            const string name = "Surface line without points";
            var point = new Point3D(1.0, 1.0, 2.2);
            var surfaceLine = new PipingSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                point
            });
            surfaceLine.SetDikeToeAtRiverAt(point);
            surfaceLine.SetDikeToeAtPolderAt(point);
            surfaceLine.SetDitchDikeSideAt(point);
            surfaceLine.SetBottomDitchPolderSideAt(point);
            surfaceLine.SetBottomDitchDikeSideAt(point);
            surfaceLine.SetDitchPolderSideAt(point);

            // Call
            Deltares.WTIPiping.PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            Assert.AreEqual(5, actual.Points.Count);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DikeToeAtPolder);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.DitchDikeSide);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.BottomDitchPolderSide);
            AssertPointsAreEqual(new Point3D(0.0, 0.0, 2.2), actual.BottomDitchDikeSide);
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