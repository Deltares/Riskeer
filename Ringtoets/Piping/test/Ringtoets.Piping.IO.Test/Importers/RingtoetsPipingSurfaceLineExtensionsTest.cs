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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Piping.IO.Importers;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.IO.Test.Importers
{
    [TestFixture]
    public class RingtoetsPipingSurfaceLineExtensionsTest
    {
        [Test]
        public void TrySetDitchPolderSide_Null_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            bool result = surfaceLine.TrySetDitchPolderSide(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDitchPolderSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () => result = surfaceLine.TrySetDitchPolderSide(testPoint);

            // Assert
            string message = $"Karakteristiek punt van profielschematisatie 'testName' is overgeslagen. De geometrie bevat geen punt op locatie {testPoint} om als 'Insteek sloot polderzijde' in te stellen.";
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDitchPolderSide_PointInGeometry_PointSetAndReturnTrue()
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            double z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[]
            {
                point
            });

            // Call
            bool result = surfaceLine.TrySetDitchPolderSide(point);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(point, surfaceLine.DitchPolderSide);
        }

        [Test]
        public void TrySetBottomDitchDikeSide_Null_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            bool result = surfaceLine.TrySetBottomDitchDikeSide(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchDikeSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () => result = surfaceLine.TrySetBottomDitchDikeSide(testPoint);

            // Assert
            string message = $"Karakteristiek punt van profielschematisatie 'testName' is overgeslagen. De geometrie bevat geen punt op locatie {testPoint} om als 'Slootbodem dijkzijde' in te stellen.";
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchDikeSide_PointInGeometry_PointSetAndReturnTrue()
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            double z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[]
            {
                point
            });

            // Call
            bool result = surfaceLine.TrySetBottomDitchDikeSide(point);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(point, surfaceLine.BottomDitchDikeSide);
        }

        [Test]
        public void TrySetBottomDitchPolderSide_Null_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            bool result = surfaceLine.TrySetBottomDitchPolderSide(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchPolderSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () => result = surfaceLine.TrySetBottomDitchPolderSide(testPoint);

            // Assert
            string message = $"Karakteristiek punt van profielschematisatie 'testName' is overgeslagen. De geometrie bevat geen punt op locatie {testPoint} om als 'Slootbodem polderzijde' in te stellen.";
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchPolderSide_PointInGeometry_PointSetAndReturnTrue()
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            double z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[]
            {
                point
            });

            // Call
            bool result = surfaceLine.TrySetBottomDitchPolderSide(point);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(point, surfaceLine.BottomDitchPolderSide);
        }

        [Test]
        public void TrySetDitchDikeSide_Null_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            bool result = surfaceLine.TrySetDitchDikeSide(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDitchDikeSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () => result = surfaceLine.TrySetDitchDikeSide(testPoint);

            // Assert
            string message = $"Karakteristiek punt van profielschematisatie 'testName' is overgeslagen. De geometrie bevat geen punt op locatie {testPoint} om als 'Insteek sloot dijkzijde' in te stellen.";
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDitchDikeSide_PointInGeometry_PointSetAndReturnTrue()
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            double z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[]
            {
                point
            });

            // Call
            bool result = surfaceLine.TrySetDitchDikeSide(point);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(point, surfaceLine.DitchDikeSide);
        }

        [Test]
        public void TrySetDikeToeAtPolder_Null_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            bool result = surfaceLine.TrySetDikeToeAtPolder(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtPolder_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () => result = surfaceLine.TrySetDikeToeAtPolder(testPoint);

            // Assert
            string message = $"Karakteristiek punt van profielschematisatie 'testName' is overgeslagen. De geometrie bevat geen punt op locatie {testPoint} om als 'Teen dijk binnenwaarts' in te stellen.";
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtPolder_PointInGeometry_PointSetAndReturnTrue()
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            double z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[]
            {
                point
            });

            // Call
            bool result = surfaceLine.TrySetDikeToeAtPolder(point);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(point, surfaceLine.DikeToeAtPolder);
        }

        [Test]
        public void TrySetDikeToeAtRiver_Null_ReturnsFalse()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            bool result = surfaceLine.TrySetDikeToeAtRiver(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtRiver_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var testPoint = new Point3D(random.NextDouble(), random.NextDouble(), random.NextDouble());
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () => result = surfaceLine.TrySetDikeToeAtRiver(testPoint);

            // Assert
            string message = $"Karakteristiek punt van profielschematisatie 'testName' is overgeslagen. De geometrie bevat geen punt op locatie {testPoint} om als 'Teen dijk buitenwaarts' in te stellen.";
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtRiver_PointInGeometry_PointSetAndReturnTrue()
        {
            // Setup
            var random = new Random(21);
            double x = random.NextDouble();
            double y = random.NextDouble();
            double z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[]
            {
                point
            });

            // Call
            bool result = surfaceLine.TrySetDikeToeAtRiver(point);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(point, surfaceLine.DikeToeAtRiver);
        }

        [Test]
        public void SetCharacteristicPoints_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Setup
            var points = new CharacteristicPoints("swapped dike toes")
            {
                DikeToeAtPolder = new Point3D(3, 2, 5),
                DikeToeAtRiver = new Point3D(3.4, 3, 8),
                DitchDikeSide = new Point3D(4.4, 6, 8),
                BottomDitchDikeSide = new Point3D(5.1, 6, 6.5),
                BottomDitchPolderSide = new Point3D(8.5, 7.2, 4.2),
                DitchPolderSide = new Point3D(9.6, 7.5, 3.9)
            };

            // Call
            TestDelegate test = () => ((RingtoetsPipingSurfaceLine) null).SetCharacteristicPoints(points);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void SetCharacteristicPoints_CharacteristicPointsNull_NoCharacteristicPointsSet()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(3, 2, 5),
                new Point3D(3.4, 3, 8),
                new Point3D(4.4, 6, 8),
                new Point3D(5.1, 6, 6.5),
                new Point3D(8.5, 7.2, 4.2),
                new Point3D(9.6, 7.5, 3.9)
            });

            // Call
            surfaceLine.SetCharacteristicPoints(null);

            // Assert
            Assert.IsNull(surfaceLine.DikeToeAtRiver);
            Assert.IsNull(surfaceLine.DikeToeAtPolder);
            Assert.IsNull(surfaceLine.DitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchDikeSide);
            Assert.IsNull(surfaceLine.BottomDitchPolderSide);
            Assert.IsNull(surfaceLine.DitchPolderSide);
        }

        [Test]
        public void SetCharacteristicPoints_DikeToesReversed_ThrowsTransformException()
        {
            // Setup
            var name = "swapped dike toes";
            var points = new CharacteristicPoints(name)
            {
                DikeToeAtPolder = new Point3D(3, 2, 5),
                DikeToeAtRiver = new Point3D(3.4, 3, 8),
                DitchDikeSide = new Point3D(4.4, 6, 8),
                BottomDitchDikeSide = new Point3D(5.1, 6, 6.5),
                BottomDitchPolderSide = new Point3D(8.5, 7.2, 4.2),
                DitchPolderSide = new Point3D(9.6, 7.5, 3.9)
            };
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(CharacteristicPointsToGeometry(points));

            // Call
            TestDelegate test = () => surfaceLine.SetCharacteristicPoints(points);

            // Assert
            var exception = Assert.Throws<SurfaceLineTransformException>(test);
            Assert.AreEqual($"Het uittredepunt moet landwaarts van het intredepunt liggen voor locatie '{name}'.", exception.Message);
        }

        [Test]
        [TestCaseSource(nameof(DifferentValidCharacteristicPointConfigurations))]
        public void SetCharacteristicPoints_ValidSituations_PointsAreSet(CharacteristicPoints points)
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(CharacteristicPointsToGeometry(points));

            // Call
            surfaceLine.SetCharacteristicPoints(points);

            // Assert
            Assert.AreEqual(points.DikeToeAtRiver, surfaceLine.DikeToeAtRiver);
            Assert.AreEqual(points.DikeToeAtPolder, surfaceLine.DikeToeAtPolder);
            Assert.AreEqual(points.DitchDikeSide, surfaceLine.DitchDikeSide);
            Assert.AreEqual(points.BottomDitchDikeSide, surfaceLine.BottomDitchDikeSide);
            Assert.AreEqual(points.BottomDitchPolderSide, surfaceLine.BottomDitchPolderSide);
            Assert.AreEqual(points.DitchPolderSide, surfaceLine.DitchPolderSide);
        }

        private static IEnumerable<Point3D> CharacteristicPointsToGeometry(CharacteristicPoints points)
        {
            return new[]
            {
                points.DikeToeAtRiver,
                points.DikeToeAtPolder,
                points.DitchDikeSide,
                points.BottomDitchDikeSide,
                points.BottomDitchPolderSide,
                points.DitchPolderSide
            }.Where(p => p != null);
        }

        private static IEnumerable<TestCaseData> DifferentValidCharacteristicPointConfigurations
        {
            get
            {
                var dikeToeAtRiver = new Point3D(3, 2, 5);
                var dikeToeAtPolder = new Point3D(3.4, 3, 8);
                var ditchDikeSide = new Point3D(4.4, 6, 8);
                var bottomDitchDikeSide = new Point3D(5.1, 6, 6.5);
                var bottomDitchPolderSide = new Point3D(8.5, 7.2, 4.2);
                var ditchPolderSide = new Point3D(9.6, 7.5, 3.9);

                var name = "All present";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DikeToeAtRiver";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DikeToeAtPolder";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DitchDikeSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing BottomDitchDikeSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing BottomDitchPolderSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    DitchPolderSide = ditchPolderSide
                }).SetName(name);

                name = "Missing DitchPolderSide";
                yield return new TestCaseData(new CharacteristicPoints(name)
                {
                    DikeToeAtRiver = dikeToeAtRiver,
                    DikeToeAtPolder = dikeToeAtPolder,
                    DitchDikeSide = ditchDikeSide,
                    BottomDitchDikeSide = bottomDitchDikeSide,
                    BottomDitchPolderSide = bottomDitchPolderSide
                }).SetName(name);
            }
        }
    }
}