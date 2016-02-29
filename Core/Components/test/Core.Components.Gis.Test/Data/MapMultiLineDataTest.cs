using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;

using NUnit.Framework;

namespace Core.Components.Gis.Test.Data
{
    [TestFixture]
    public class MapMultiLineDataTest
    {
        [Test]
        public void Constructor_NullCollection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MapMultiLineData(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(test, string.Format("A point collection is required when creating a subclass of {0}.", typeof(PointBasedMapData)));
        }

        [Test]
        public void Constructor_WithEmptyCollection_CreatesNewMapLineData()
        {
            // Setup
            var collection = new Collection<IEnumerable<Point2D>>();

            // Call
            var data = new MapMultiLineData(collection);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(collection, data.Lines);
        }

        [Test]
        public void Constructor_WithPoints_CreatesNewMapLineData()
        {
            // Setup
            var lines = CreateTestLines();

            // Call
            var data = new MapMultiLineData(lines);

            // Assert
            Assert.IsInstanceOf<MapData>(data);
            Assert.AreNotSame(lines, data.Lines);
            CollectionAssert.AreEqual(lines, data.Lines);
        }

        private static IEnumerable<IEnumerable<Point2D>> CreateTestLines()
        {
            return new Collection<IEnumerable<Point2D>>
            {
                new Collection<Point2D> {
                    new Point2D(0.0, 1.1),
                    new Point2D(1.0, 2.1),
                    new Point2D(1.6, 1.6)
                }
            };
        }
    }
}