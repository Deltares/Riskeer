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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
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
        public void TrySetDitchPolderSide_NoPointInGeometry_PointSetAndReturnFalse()
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
        public void TrySetBottomDitchDikeSide_NoPointInGeometry_PointSetAndReturnFalse()
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
        public void TrySetBottomDitchPolderSide_NoPointInGeometry_PointSetAndReturnFalse()
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
        public void TrySetDitchDikeSide_NoPointInGeometry_PointSetAndReturnFalse()
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
        public void TrySetDikeToeAtPolder_NoPointInGeometry_PointSetAndReturnFalse()
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
        public void TrySetDikeToeAtRiver_NoPointInGeometry_PointSetAndReturnFalse()
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
    }
}