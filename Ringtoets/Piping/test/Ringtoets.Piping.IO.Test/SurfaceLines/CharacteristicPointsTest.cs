using System;
using NUnit.Framework;
using Ringtoets.Piping.IO.SurfaceLines;

namespace Ringtoets.Piping.IO.Test.SurfaceLines
{
    [TestFixture]
    public class CharacteristicPointsTest
    {
        [Test]
        public void Constructor_WithNullName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new CharacteristicPoints(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase("")]
        [TestCase("Name")]
        public void Constructor_WithName_ThrowsArgumentNullException(string name)
        {
            // Call
            var location = new CharacteristicPoints(name);

            // Assert
            Assert.AreEqual(name, location.Name);
        }
    }
}