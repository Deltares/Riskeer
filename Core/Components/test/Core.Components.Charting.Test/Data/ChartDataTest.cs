using System;
using Core.Common.Base;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartDataTest
    {
        [Test]
        [TestCase(null)]
        [TestCase("   ")]
        [TestCase("")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Call
            TestDelegate call = () => new TestChartData(invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "A name must be set to map data");
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            var name = "Some name";

            // Call
            var data = new TestChartData(name);

            // Assert
            Assert.IsInstanceOf<Observable>(data);
            Assert.AreEqual(name, data.Name);
        }

        [Test]
        public void Name_SetName_ReturnsNewName()
        {
            // setup
            var name = "Some name";
            var newName = "Something";
            var data = new TestChartData(name);

            // Precondition
            Assert.AreEqual(name, data.Name);

            // Call
            data.Name = newName;

            // Assert
            Assert.AreNotEqual(name, data.Name);
            Assert.AreEqual(newName, data.Name);

        }
    }

    public class TestChartData : ChartData
    {
        public TestChartData(string name) : base(name) { }
    }
}