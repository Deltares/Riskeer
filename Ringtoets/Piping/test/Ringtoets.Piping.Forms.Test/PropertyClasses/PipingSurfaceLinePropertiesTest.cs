using Core.Common.Gui;
using NUnit.Framework;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.PropertyClasses;

namespace Ringtoets.Piping.Forms.Test.PropertyClasses
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