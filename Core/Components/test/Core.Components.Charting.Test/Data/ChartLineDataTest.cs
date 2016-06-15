using System;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartLineDataTest
    {
        [Test]
        public void Constructor_NullPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartLineData(null, "test data");

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("     ")]
        public void Constructor_InvalidName_ThrowsArgumentException(string invalidName)
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            TestDelegate test = () => new ChartLineData(points, invalidName);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "A name must be set to map data");
        }

        [Test]
        public void Constructor_WithEmptyPoints_CreatesNewICharData()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>();

            // Call
            var data = new ChartLineData(points, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(points, data.Points);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewICharData()
        {
            // Setup
            var points = CreateTestPoints();

            // Call
            var data = new ChartLineData(points, "test data");

            // Assert
            Assert.IsInstanceOf<ChartData>(data);
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
        }

        private Collection<Tuple<double, double>> CreateTestPoints()
        {
            return new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.0, 1.1),    
                Tuple.Create(1.0, 2.1),
                Tuple.Create(1.6, 1.6)    
            };
        }
    }
}