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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
using LandwardDirection = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.LandwardDirection;
using WtiStabilityLandwardDirection = Deltares.WTIStability.Data.Geo.LandwardDirection;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Creators.Input
{
    [TestFixture]
    public class SurfaceLineCreatorTest
    {
        [Test]
        public void Create_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SurfaceLineCreator.Create(null, LandwardDirection.PositiveX);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void Create_SurfaceLineWithoutPoints_CreateSurfaceLineWithoutPoints()
        {
            // Setup
            const string name = "Surface line without points";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);

            // Call
            SurfaceLine2 actual = SurfaceLineCreator.Create(surfaceLine, LandwardDirection.PositiveX);

            // Assert
            AssertGeneralValues(name, actual);
            CollectionAssert.IsEmpty(actual.Geometry.Points);
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
            SurfaceLine2 actual = SurfaceLineCreator.Create(surfaceLine, LandwardDirection.PositiveX);

            // Assert
            AssertGeneralValues(name, actual);
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.X).ToArray(), actual.Geometry.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Geometry.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(CharacteristicPointType.None, surfaceLine.Points.Count()), actual.CharacteristicPoints.Select(p => p.CharacteristicPointType));
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
            SurfaceLine2 actual = SurfaceLineCreator.Create(surfaceLine, LandwardDirection.PositiveX);

            // Assert
            AssertGeneralValues(name, actual);

            double[] expectedCoordinatesX = surfaceLine.Points.Select(p => p.X - firstX).ToArray();

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Geometry.Points.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Geometry.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(CharacteristicPointType.None, surfaceLine.Points.Count()), actual.CharacteristicPoints.Select(p => p.CharacteristicPointType));
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
            SurfaceLine2 actual = SurfaceLineCreator.Create(surfaceLine, LandwardDirection.PositiveX);

            // Assert
            AssertGeneralValues(name, actual);

            double length = Math.Sqrt(2 * 2 + 3 * 3);
            const double secondCoordinateFactor = (2.0 * 1.0 + 3.0 * 2.0) / (2.0 * 2.0 + 3.0 * 3.0);
            double[] expectedCoordinatesX =
            {
                0.0,
                secondCoordinateFactor * length,
                length
            };

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Geometry.Points.Select(p => p.X).ToArray(), new DoubleWithToleranceComparer(1e-2));
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Geometry.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(CharacteristicPointType.None, surfaceLine.Points.Count()), actual.CharacteristicPoints.Select(p => p.CharacteristicPointType));
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
            SurfaceLine2 actual = SurfaceLineCreator.Create(surfaceLine, LandwardDirection.PositiveX);

            // Assert
            AssertGeneralValues(name, actual);

            double[] expectedCoordinatesX =
            {
                0.0
            };

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Geometry.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Geometry.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(CharacteristicPointType.None, surfaceLine.Points.Count()), actual.CharacteristicPoints.Select(p => p.CharacteristicPointType));
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
            SurfaceLine2 actual = SurfaceLineCreator.Create(surfaceLine, LandwardDirection.PositiveX);

            // Assert
            AssertGeneralValues(name, actual);

            double[] expectedCoordinatesX = surfaceLine.Points.Select(p => p.X).ToArray();
            double[] expectedCoordinatesZ = surfaceLine.Points.Select(p => p.Z).ToArray();

            CollectionAssert.AreEqual(expectedCoordinatesX, actual.Geometry.Points.Select(p => p.X));
            CollectionAssert.AreEqual(expectedCoordinatesZ, actual.Geometry.Points.Select(p => p.Z));

            CharacteristicPointSet actualCharacteristicPoints = actual.CharacteristicPoints;
            Assert.IsTrue(actualCharacteristicPoints.All(cp => ReferenceEquals(actualCharacteristicPoints, cp.PointSet)));
            CollectionAssert.AreEqual(expectedCoordinatesX, actualCharacteristicPoints.Geometry.Points.Select(p => p.X));
            CollectionAssert.AreEqual(expectedCoordinatesZ, actualCharacteristicPoints.Geometry.Points.Select(p => p.Z));

            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.BottomDitchDikeSide).LocationEquals(ToGeometryPoint(geometry[0])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.BottomDitchPolderSide).LocationEquals(ToGeometryPoint(geometry[1])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.DikeToeAtPolder).LocationEquals(ToGeometryPoint(geometry[2])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.DikeToeAtRiver).LocationEquals(ToGeometryPoint(geometry[3])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.DikeTopAtPolder).LocationEquals(ToGeometryPoint(geometry[4])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.DitchDikeSide).LocationEquals(ToGeometryPoint(geometry[5])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.DitchPolderSide).LocationEquals(ToGeometryPoint(geometry[6])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.SurfaceLevelInside).LocationEquals(ToGeometryPoint(geometry[7])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.SurfaceLevelOutside).LocationEquals(ToGeometryPoint(geometry[8])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.ShoulderBaseInside).LocationEquals(ToGeometryPoint(geometry[9])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.ShoulderTopInside).LocationEquals(ToGeometryPoint(geometry[10])));
            Assert.IsTrue(actualCharacteristicPoints.GetGeometryPoint(CharacteristicPointType.DikeTopAtRiver).LocationEquals(ToGeometryPoint(geometry[11])));
        }

        [Test]
        public void Create_InvalidLandwardDirection_ThrowInvalidEnumArgumentException()
        {
            // Setup
            const string name = "Surface line with landward direction";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);

            // Call
            TestDelegate test = () => SurfaceLineCreator.Create(surfaceLine, (LandwardDirection) 99);

            // Assert
            string message = $"The value of argument 'landwardDirection' ({99}) is invalid for Enum type '{typeof(LandwardDirection).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [TestCase(LandwardDirection.PositiveX, WtiStabilityLandwardDirection.PositiveX)]
        [TestCase(LandwardDirection.NegativeX, WtiStabilityLandwardDirection.NegativeX)]
        public void Create_ValidLandwardDirection_CreateSurfaceLineWithLandwardDirection(LandwardDirection landwardDirection,
                                                                                         WtiStabilityLandwardDirection expectedLandwardDirection)
        {
            // Setup
            const string name = "Surface line with landward direction";
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(name);

            // Call
            SurfaceLine2 actual = SurfaceLineCreator.Create(surfaceLine, landwardDirection);

            // Assert
            AssertGeneralValues(name, actual);
            Assert.AreEqual(expectedLandwardDirection, actual.LandwardDirection);
        }

        private static void AssertGeneralValues(string name, SurfaceLine2 actual)
        {
            Assert.AreEqual(name, actual.Name); // Unused property
            CollectionAssert.IsEmpty(actual.Geometry.CalcPoints); // Internal property
        }

        private static GeometryPoint ToGeometryPoint(Point3D point)
        {
            return new GeometryPoint(point.X, point.Z);
        }
    }
}