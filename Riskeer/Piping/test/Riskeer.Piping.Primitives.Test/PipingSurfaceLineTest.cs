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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.Common.Data;

namespace Riskeer.Piping.Primitives.Test
{
    [TestFixture]
    public class PipingSurfaceLineTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingSurfaceLine(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            const string name = "Some name";

            // Call
            var surfaceLine = new PipingSurfaceLine(name);

            // Assert
            Assert.IsInstanceOf<MechanismSurfaceLineBase>(surfaceLine);
            Assert.AreEqual(name, surfaceLine.Name);
        }

        [Test]
        public void CopyProperties_WithSurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);

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
            PipingSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();

            PipingSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();
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
            PipingSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();
            var surfaceLineToUpdateFrom = new PipingSurfaceLine(surfaceLine.Name)
            {
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
            PipingSurfaceLine surfaceLine = CreateSurfaceLineWithCharacteristicPoints();

            var expectedIntersectionPoint = new Point2D(123, 456);
            PipingSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();
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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            PipingSurfaceLine surfaceLineToUpdateFrom = CreateSurfaceLineWithCharacteristicPoints();

            // Call
            surfaceLine.CopyProperties(surfaceLineToUpdateFrom);

            // Assert
            AssertPropertiesUpdated(surfaceLineToUpdateFrom, surfaceLine);
        }

        [TestFixture]
        private class PipingSurfaceLineEqualsTest : EqualsTestFixture<PipingSurfaceLine, TestSurfaceLine>
        {
            protected override PipingSurfaceLine CreateObject()
            {
                return CreateSurfaceLineWithCharacteristicPoints();
            }

            protected override TestSurfaceLine CreateDerivedObject()
            {
                PipingSurfaceLine baseLine = CreateSurfaceLineWithCharacteristicPoints();
                return new TestSurfaceLine(baseLine);
            }

            private static IEnumerable<TestCaseData> GetUnequalTestCases()
            {
                PipingSurfaceLine differentName = CreateSurfaceLineWithCharacteristicPoints("Different Name");
                yield return new TestCaseData(differentName).SetName("Name");

                PipingSurfaceLine differentGeometry = CreateSurfaceLineWithCharacteristicPoints();
                differentGeometry.SetGeometry(new[]
                {
                    new Point3D(3, 4, 5)
                });
                yield return new TestCaseData(differentGeometry)
                    .SetName("Geometry");

                PipingSurfaceLine differentReferenceLineIntersectionWorldPoint = CreateSurfaceLineWithCharacteristicPoints();
                differentReferenceLineIntersectionWorldPoint.ReferenceLineIntersectionWorldPoint = new Point2D(1, 1);
                yield return new TestCaseData(differentReferenceLineIntersectionWorldPoint)
                    .SetName("WorldIntersectionPoint");

                PipingSurfaceLine differentBottomDitchDikeSide = CreateSurfaceLineWithCharacteristicPoints();
                Point3D[] points = differentBottomDitchDikeSide.Points.ToArray();
                differentBottomDitchDikeSide.SetBottomDitchDikeSideAt(points[4]);
                yield return new TestCaseData(differentBottomDitchDikeSide)
                    .SetName("BottomDitchDikeSide");

                PipingSurfaceLine differentBottomDitchPolderSide = CreateSurfaceLineWithCharacteristicPoints();
                points = differentBottomDitchPolderSide.Points.ToArray();
                differentBottomDitchPolderSide.SetBottomDitchPolderSideAt(points[5]);
                yield return new TestCaseData(differentBottomDitchPolderSide)
                    .SetName("BottomDitchPolderSide");

                PipingSurfaceLine differentDikeToeAtPolder = CreateSurfaceLineWithCharacteristicPoints();
                points = differentDikeToeAtPolder.Points.ToArray();
                differentDikeToeAtPolder.SetDikeToeAtPolderAt(points[5]);
                yield return new TestCaseData(differentDikeToeAtPolder)
                    .SetName("DikeToeAtPolder");

                PipingSurfaceLine differentDikeToeAtRiver = CreateSurfaceLineWithCharacteristicPoints();
                points = differentDikeToeAtRiver.Points.ToArray();
                differentDikeToeAtRiver.SetDikeToeAtRiverAt(points[5]);
                yield return new TestCaseData(differentDikeToeAtRiver)
                    .SetName("DikeToeAtRiver");

                PipingSurfaceLine differentDitchDikeSide = CreateSurfaceLineWithCharacteristicPoints();
                points = differentDitchDikeSide.Points.ToArray();
                differentDitchDikeSide.SetDitchDikeSideAt(points[1]);
                yield return new TestCaseData(differentDitchDikeSide)
                    .SetName("DitchDikeSide");

                PipingSurfaceLine differentDitchPolderSide = CreateSurfaceLineWithCharacteristicPoints();
                points = differentDitchPolderSide.Points.ToArray();
                differentDitchPolderSide.SetDitchPolderSideAt(points[1]);
                yield return new TestCaseData(differentDitchPolderSide)
                    .SetName("DitchPolderSide");
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
                var surfaceLine = new PipingSurfaceLine(string.Empty);
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
                var surfaceLine = new PipingSurfaceLine(string.Empty);

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
                var surfaceLine = new PipingSurfaceLine(string.Empty);

                // Call
                TestDelegate test = () => SetCharacteristicPoint(surfaceLine, null);

                // Assert
                const string expectedMessage = "Cannot find a point in geometry using a null point.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
            }

            protected abstract void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, Point3D point);
            protected abstract Point3D GetCharacteristicPoint(PipingSurfaceLine surfaceLine);
            protected abstract string CharacteristicPointDescription();
        }

        [TestFixture]
        public class SetDitchPolderSideAtTest : SetCharacteristicPointTest
        {
            protected override void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDitchPolderSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(PipingSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetBottomDitchPolderSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(PipingSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetBottomDitchDikeSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(PipingSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDitchDikeSideAt(point);
            }

            protected override Point3D GetCharacteristicPoint(PipingSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDikeToeAtRiverAt(point);
            }

            protected override Point3D GetCharacteristicPoint(PipingSurfaceLine surfaceLine)
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
            protected override void SetCharacteristicPoint(PipingSurfaceLine surfaceLine, Point3D point)
            {
                surfaceLine.SetDikeToeAtPolderAt(point);
            }

            protected override Point3D GetCharacteristicPoint(PipingSurfaceLine surfaceLine)
            {
                return surfaceLine.DikeToeAtPolder;
            }

            protected override string CharacteristicPointDescription()
            {
                return "Teen dijk binnenwaarts";
            }
        }

        private class TestSurfaceLine : PipingSurfaceLine
        {
            public TestSurfaceLine(PipingSurfaceLine profile) : base(string.Empty)
            {
                CopyProperties(profile);
            }
        }

        private static PipingSurfaceLine CreateSurfaceLineWithCharacteristicPoints(string name = "Name A")
        {
            var surfaceLine = new PipingSurfaceLine(name)
            {
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

        private static void CreateTestGeometry(Point3D testPoint, PipingSurfaceLine surfaceLine)
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

        private static void AssertPropertiesUpdated(PipingSurfaceLine expectedSurfaceLine, PipingSurfaceLine actualSurfaceLine)
        {
            Assert.AreEqual(expectedSurfaceLine.Name, actualSurfaceLine.Name);
            TestHelper.AssertAreEqualButNotSame(expectedSurfaceLine.ReferenceLineIntersectionWorldPoint,
                                                actualSurfaceLine.ReferenceLineIntersectionWorldPoint);
            CollectionAssert.AreEqual(expectedSurfaceLine.Points, actualSurfaceLine.Points);
            TestHelper.AssertCollectionAreNotSame(expectedSurfaceLine.Points, actualSurfaceLine.Points);

            IEnumerable<Point3D> actualSurfaceLinePoints = actualSurfaceLine.Points;
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.BottomDitchDikeSide, actualSurfaceLine.BottomDitchDikeSide);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.BottomDitchPolderSide, actualSurfaceLine.BottomDitchPolderSide);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DikeToeAtPolder, actualSurfaceLine.DikeToeAtPolder);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DikeToeAtRiver, actualSurfaceLine.DikeToeAtRiver);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DitchPolderSide, actualSurfaceLine.DitchPolderSide);
            AssertAreEqualAndFromSurfaceLine(actualSurfaceLinePoints, expectedSurfaceLine.DitchDikeSide, actualSurfaceLine.DitchDikeSide);
        }

        private static void AssertAreEqualAndFromSurfaceLine(IEnumerable<Point3D> actualSurfaceLinePoints, Point3D expectedPoint, Point3D actualPoint)
        {
            Assert.AreEqual(expectedPoint, actualPoint);
            if (actualPoint != null)
            {
                Assert.IsTrue(actualSurfaceLinePoints.Contains(actualPoint, new ReferenceEqualityComparer<Point3D>()));
            }
        }
    }
}