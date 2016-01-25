using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using NUnit.Framework;

namespace Core.Components.Charting.Test.Data
{
    public class PointBasedChartDataTest
    {
        [Test]
        public void Constructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestPointBasedChartData(null);
            
            // Assert
            var expectedMessage = "A point collection is required when creating a subclass of Core.Components.Charting.Data.PointBasedChartData.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_WithPoints_PropertiesSet()
        {
            // Setup
            var points = new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.0, 1.0),
                Tuple.Create(2.5, 1.1)
            };
            
            // Call
            var data = new TestPointBasedChartData(points);

            // Assert
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
            Assert.IsTrue(data.IsVisible);
        }
    }

    public class TestPointBasedChartData : PointBasedChartData
    {
        public TestPointBasedChartData(IEnumerable<Tuple<double,double>> points) : base(points) { }
    }
}