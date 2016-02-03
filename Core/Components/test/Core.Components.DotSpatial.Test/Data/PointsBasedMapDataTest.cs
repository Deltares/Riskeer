using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Data;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Test.Data
{
    [TestFixture]
    public class PointsBasedMapDataTest
    {
        [Test]
        public void Constructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestPointBasedMapData(null);

            // Assert
            var expectedMessage = "A point collection is required when creating a subclass of Core.Components.DotSpatial.Data.PointBasedMapData.";
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
            var data = new TestPointBasedMapData(points);

            // Assert
            Assert.AreNotSame(points, data.Points);
            CollectionAssert.AreEqual(points, data.Points);
            Assert.IsTrue(data.IsVisible);
        }

        private class TestPointBasedMapData : PointBasedMapData
        {
            public TestPointBasedMapData(IEnumerable<Tuple<double, double>> points) : base(points) { }
        }
    }
}
