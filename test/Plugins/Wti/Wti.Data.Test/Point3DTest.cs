using NUnit.Framework;

namespace Wti.Data.Test
{
    [TestFixture]
    public class Point3DTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var point = new Point3D();

            // Assert
            Assert.AreEqual(0, point.X);
            Assert.AreEqual(0, point.Y);
            Assert.AreEqual(0, point.Z);
        }

        [Test]
        public void AutomaticProperties_SetAndGetValuesAgain_ReturnedValueShouldBeSameAsSetValue()
        {
            // Setup
            var point = new Point3D();

            // Call
            point.X = 1.1;
            point.Y = 2.2;
            point.Z = -1.1;

            // Assert
            Assert.AreEqual(1.1, point.X);
            Assert.AreEqual(2.2, point.Y);
            Assert.AreEqual(-1.1, point.Z);
        }
    }
}