// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class SurfaceLineCreatorTest
    {
        [Test]
        public void Create_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => SurfaceLineCreator.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void Create_SurfaceLineWithoutPoints_CreateSurfaceLineWithoutPoints()
        {
            // Setup
            const string name = "Surface line without points";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);

            // Call
            SurfaceLine actual = SurfaceLineCreator.Create(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(actual.CharacteristicPoints);
        }

        [Test]
        public void Create_NormalizedLocalSurfaceLine_ReturnsSurfaceLineWithIdenticalPoints()
        {
            // Setup
            const string name = "Local coordinate surface line";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0.0, 0.0, 1.1),
                new Point3D(2.2, 0.0, 3.3),
                new Point3D(4.4, 0.0, 5.5)
            });

            // Call
            SurfaceLine actual = SurfaceLineCreator.Create(surfaceLine);

            // Assert
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.X), actual.CharacteristicPoints.Select(p => p.GeometryPoint.X));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z), actual.CharacteristicPoints.Select(p => p.GeometryPoint.Z));
            Assert.IsTrue(actual.CharacteristicPoints.All(cp => cp.CharacteristicPoint == CharacteristicPointType.None));
        }

        [Test]
        public void Create_LocalSurfaceLineNotNormalized_TranslateAllPointsToMakeFirstCoordinateZeroX()
        {
            // Setup
            const string name = "Local coordinate surface line";
            const double firstX = 4.6;
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(firstX, 0.0, 1.1),
                new Point3D(7.8, 0.0, 3.3),
                new Point3D(9.9, 0.0, 5.5)
            });

            // Call
            SurfaceLine actual = SurfaceLineCreator.Create(surfaceLine);

            // Assert
            double[] expectedCoordinatesX = surfaceLine.Points.Select(p => p.X - firstX).ToArray();

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.CharacteristicPoints.Select(p => p.GeometryPoint.X), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z), actual.CharacteristicPoints.Select(p => p.GeometryPoint.Z));
            Assert.IsTrue(actual.CharacteristicPoints.All(cp => cp.CharacteristicPoint == CharacteristicPointType.None));
        }

        [Test]
        public void Create_GlobalSurfaceLine_ProjectSurfaceLineIntoLZPlaneSpannedByFirstAndLastPoint()
        {
            // Setup
            const string name = "Global coordinate surface line";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 1.0, 2.2),
                new Point3D(2.0, 3.0, 4.4), // Outlier from line specified by extrema
                new Point3D(3.0, 4.0, 7.7)
            });

            // Call
            SurfaceLine actual = SurfaceLineCreator.Create(surfaceLine);

            // Assert
            double length = Math.Sqrt(2 * 2 + 3 * 3);
            const double secondCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            double[] expectedCoordinatesX =
            {
                0.0,
                secondCoordinateFactor * length,
                length
            };

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.CharacteristicPoints.Select(p => p.GeometryPoint.X), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z), actual.CharacteristicPoints.Select(p => p.GeometryPoint.Z));
            Assert.IsTrue(actual.CharacteristicPoints.All(cp => cp.CharacteristicPoint == CharacteristicPointType.None));
        }

        [Test]
        public void Create_SurfaceLineWithOnlyOnePoint_CreateSurfaceLineWithOnePoint()
        {
            // Setup
            const string name = "Global coordinate surface line";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.0, 1.0, 2.2)
            });

            // Call
            SurfaceLine actual = SurfaceLineCreator.Create(surfaceLine);

            // Assert
            double[] expectedCoordinatesX =
            {
                0.0
            };

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.CharacteristicPoints.Select(p => p.GeometryPoint.X));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z), actual.CharacteristicPoints.Select(p => p.GeometryPoint.Z));
            Assert.IsTrue(actual.CharacteristicPoints.All(cp => cp.CharacteristicPoint == CharacteristicPointType.None));
        }

        [Test]
        public void Create_SurfaceLineWithAllCharacteristicPoints_CreateSurfaceLineWithGeometryAndCharacteristicPoints()
        {
            // Setup
            const string name = "Surface line with characteristic points";
            var geometry = new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(1, 0, 2),
                new Point3D(2, 0, 3),
                new Point3D(3, 0, 0),
                new Point3D(4, 0, 2),
                new Point3D(5, 0, 3),
                new Point3D(6, 0, 0),
                new Point3D(7, 0, 2),
                new Point3D(8, 0, 3),
                new Point3D(9, 0, 0),
                new Point3D(10, 0, 2),
                new Point3D(11, 0, 3)
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);
            surfaceLine.SetGeometry(geometry);
            surfaceLine.SetBottomDitchDikeSideAt(geometry[0]);
            surfaceLine.SetBottomDitchPolderSideAt(geometry[1]);
            surfaceLine.SetDikeToeAtPolderAt(geometry[2]);
            surfaceLine.SetDikeToeAtRiverAt(geometry[3]);
            surfaceLine.SetDikeTopAtPolderAt(geometry[4]);
            surfaceLine.SetDitchDikeSideAt(geometry[5]);
            surfaceLine.SetDitchPolderSideAt(geometry[6]);
            surfaceLine.SetSurfaceLevelInsideAt(geometry[7]);
            surfaceLine.SetSurfaceLevelOutsideAt(geometry[8]);
            surfaceLine.SetShoulderBaseInsideAt(geometry[9]);
            surfaceLine.SetShoulderTopInsideAt(geometry[10]);
            surfaceLine.SetDikeTopAtRiverAt(geometry[11]);

            // Call
            SurfaceLine actual = SurfaceLineCreator.Create(surfaceLine);

            // Assert
            IEnumerable<double> expectedCoordinatesX = surfaceLine.Points.Select(p => p.X);
            IEnumerable<double> expectedCoordinatesZ = surfaceLine.Points.Select(p => p.Z);

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.CharacteristicPoints.Select(p => p.GeometryPoint.X));
            CollectionAssert.AreEqual(expectedCoordinatesZ, actual.CharacteristicPoints.Select(p => p.GeometryPoint.Z));

            Assert.AreEqual(CharacteristicPointType.BottomDitchDikeSide, actual.CharacteristicPoints.ElementAt(0).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.BottomDitchPolderSide, actual.CharacteristicPoints.ElementAt(1).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.DikeToeAtPolder, actual.CharacteristicPoints.ElementAt(2).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.DikeToeAtRiver, actual.CharacteristicPoints.ElementAt(3).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.DikeTopAtPolder, actual.CharacteristicPoints.ElementAt(4).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.DitchDikeSide, actual.CharacteristicPoints.ElementAt(5).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.DitchPolderSide, actual.CharacteristicPoints.ElementAt(6).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.SurfaceLevelInside, actual.CharacteristicPoints.ElementAt(7).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.SurfaceLevelOutside, actual.CharacteristicPoints.ElementAt(8).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.ShoulderBaseInside, actual.CharacteristicPoints.ElementAt(9).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.ShoulderTopInside, actual.CharacteristicPoints.ElementAt(10).CharacteristicPoint);
            Assert.AreEqual(CharacteristicPointType.DikeTopAtRiver, actual.CharacteristicPoints.ElementAt(11).CharacteristicPoint);
        }
    }
}