using System;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Plugin.FileImporter;

namespace Ringtoets.Piping.Plugin.Test.FileImporter
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
            var result = surfaceLine.TrySetDitchPolderSide(null);

            // Assert
            Assert.IsFalse(result);
        } 

        [Test]
        public void TrySetDitchPolderSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () =>
            {
                result = surfaceLine.TrySetDitchPolderSide(new Point3D(x, y, z));
            };

            // Assert
            var message = string.Format("Karakteristiek punt van profielmeting 'testName' is overgeslagen. De geometrie bevat geen punt op locatie ({0},{1},{2}) om als '{3}' in te stellen.",
                x, y, z, Data.Properties.Resources.CharacteristicPoint_DitchPolderSide);
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDitchPolderSide_NoPointInGeometry_PointSetAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new [] { point });

            // Call
            var result = surfaceLine.TrySetDitchPolderSide(point);

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
            var result = surfaceLine.TrySetBottomDitchDikeSide(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchDikeSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () =>
            {
                result = surfaceLine.TrySetBottomDitchDikeSide(new Point3D(x, y, z));
            };

            // Assert
            var message = string.Format("Karakteristiek punt van profielmeting 'testName' is overgeslagen. De geometrie bevat geen punt op locatie ({0},{1},{2}) om als '{3}' in te stellen.",
                x, y, z, Data.Properties.Resources.CharacteristicPoint_BottomDitchDikeSide);
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchDikeSide_NoPointInGeometry_PointSetAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[] { point });

            // Call
            var result = surfaceLine.TrySetBottomDitchDikeSide(point);

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
            var result = surfaceLine.TrySetBottomDitchPolderSide(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchPolderSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () =>
            {
                result = surfaceLine.TrySetBottomDitchPolderSide(new Point3D(x, y, z));
            };

            // Assert
            var message = string.Format("Karakteristiek punt van profielmeting 'testName' is overgeslagen. De geometrie bevat geen punt op locatie ({0},{1},{2}) om als '{3}' in te stellen.",
                x, y, z, Data.Properties.Resources.CharacteristicPoint_BottomDitchPolderSide);
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetBottomDitchPolderSide_NoPointInGeometry_PointSetAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[] { point });

            // Call
            var result = surfaceLine.TrySetBottomDitchPolderSide(point);

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
            var result = surfaceLine.TrySetDitchDikeSide(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDitchDikeSide_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () =>
            {
                result = surfaceLine.TrySetDitchDikeSide(new Point3D(x, y, z));
            };

            // Assert
            var message = string.Format("Karakteristiek punt van profielmeting 'testName' is overgeslagen. De geometrie bevat geen punt op locatie ({0},{1},{2}) om als '{3}' in te stellen.",
                x, y, z, Data.Properties.Resources.CharacteristicPoint_DitchDikeSide);
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDitchDikeSide_NoPointInGeometry_PointSetAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[] { point });

            // Call
            var result = surfaceLine.TrySetDitchDikeSide(point);

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
            var result = surfaceLine.TrySetDikeToeAtPolder(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtPolder_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () =>
            {
                result = surfaceLine.TrySetDikeToeAtPolder(new Point3D(x, y, z));
            };

            // Assert
            var message = string.Format("Karakteristiek punt van profielmeting 'testName' is overgeslagen. De geometrie bevat geen punt op locatie ({0},{1},{2}) om als '{3}' in te stellen.",
                x, y, z, Data.Properties.Resources.CharacteristicPoint_DikeToeAtPolder);
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtPolder_NoPointInGeometry_PointSetAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[] { point });

            // Call
            var result = surfaceLine.TrySetDikeToeAtPolder(point);

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
            var result = surfaceLine.TrySetDikeToeAtRiver(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtRiver_NoPointInGeometry_LogAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "testName"
            };

            var result = true;

            // Call
            Action test = () =>
            {
                result = surfaceLine.TrySetDikeToeAtRiver(new Point3D(x, y, z));
            };

            // Assert
            var message = string.Format("Karakteristiek punt van profielmeting 'testName' is overgeslagen. De geometrie bevat geen punt op locatie ({0},{1},{2}) om als '{3}' in te stellen.",
                x, y, z, Data.Properties.Resources.CharacteristicPoint_DikeToeAtRiver);
            TestHelper.AssertLogMessageIsGenerated(test, message, 1);
            Assert.IsFalse(result);
        }

        [Test]
        public void TrySetDikeToeAtRiver_NoPointInGeometry_PointSetAndReturnFalse()
        {
            // Setup
            var random = new Random(21);
            var x = random.NextDouble();
            var y = random.NextDouble();
            var z = random.NextDouble();
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            var point = new Point3D(x, y, z);

            surfaceLine.SetGeometry(new[] { point });

            // Call
            var result = surfaceLine.TrySetDikeToeAtRiver(point);

            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(point, surfaceLine.DikeToeAtRiver);
        }
    }
}