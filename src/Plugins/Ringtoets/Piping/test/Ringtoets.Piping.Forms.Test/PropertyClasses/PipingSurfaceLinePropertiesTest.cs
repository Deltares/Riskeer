using DelftTools.Shell.Gui;

using NUnit.Framework;

using Wti.Data;
using Wti.Forms.PropertyClasses;

namespace Wti.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class PipingSurfaceLinePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new PipingSurfaceLineProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<RingtoetsPipingSurfaceLine>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            const string expectedName = "<some nice name>";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = expectedName
            };

            var properties = new PipingSurfaceLineProperties
            {
                Data = surfaceLine
            };

            // Call & Assert
            Assert.AreEqual(expectedName, properties.Name);
            CollectionAssert.AreEqual(surfaceLine.Points, properties.Points);
        }
    }
}