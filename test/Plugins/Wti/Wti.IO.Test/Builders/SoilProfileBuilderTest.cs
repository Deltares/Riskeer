using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wti.Data;
using Wti.IO.Builders;
using Wti.IO.Properties;

namespace Wti.IO.Test.Builders
{
    public class SoilProfileBuilderTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("name")]
        public void Constructor_WithNameInvalidX_ThrowsArgumentExcpetion(string name)
        {
            // Call
            TestDelegate test = () => new SoilProfileBuilder(name, double.NaN);

            // Assert
            var exception = Assert.Throws<ArgumentException>(test);
            Assert.AreEqual(Resources.Error_SoilProfileBuilderCantDetermineIntersectAtDoubleNaN, exception.Message);
        }

        [Test]
        [TestCase(null)]
        [TestCase("name")]
        public void Constructor_WithNameValidX_ReturnsNewInstance(string name)
        {
            // Call
            var builder = new SoilProfileBuilder(name, 0.0);

            // Assert
            Assert.NotNull(builder);
        }

        [Test]
        public void Build_WithOutLayers_ThrowsArgumentException()
        {
            // Setup
            var profileName = "SomeProfile";
            var builder = new SoilProfileBuilder(profileName, 0.0);

            // Call
            TestDelegate test = () => builder.Build();

            // Assert
            Assert.Throws<ArgumentException>(test);
        }
    }
}