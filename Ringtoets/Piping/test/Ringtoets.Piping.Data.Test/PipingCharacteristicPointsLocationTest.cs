using System;
using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingCharacteristicPointsLocationTest
    {
        [Test]
        public void Constructor_WithNullName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingCharacteristicPointsLocation(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase("")]
        [TestCase("Name")]
        public void Constructor_WithName_ThrowsArgumentNullException(string name)
        {
            // Call
            var location = new PipingCharacteristicPointsLocation(name);

            // Assert
            Assert.AreEqual(name, location.Name);
        }
    }
}