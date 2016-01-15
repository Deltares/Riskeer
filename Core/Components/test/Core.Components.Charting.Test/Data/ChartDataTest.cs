using System;
using System.Collections.Generic;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    [TestFixture]
    public class ChartDataTest
    {
        [Test]
        public void Constructor_PointsNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate test = () => new TestChartData(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        } 

        [Test]
        public void Constructor_WithPoints_PointsCopySet()
        {
            // Setup
            var points = new List<Tuple<double, double>>
            {
                new Tuple<double, double>(3.2, 1.1)
            };

            // Call
            var data = new TestChartData(points);

            // Assert
            Assert.IsTrue(data.IsVisible);
            CollectionAssert.AreEqual(points, data.Points);
            Assert.AreNotSame(points, data.Points);
        } 
    }

    public class TestChartData : ChartData {
        public TestChartData(IEnumerable<Tuple<double, double>> points) : base(points) {}
    }
}