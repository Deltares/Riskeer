using System;
using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Gis.Geometries;
using NUnit.Framework;

namespace Core.Components.Gis.Test.Geometries
{
    [TestFixture]
    public class MapGeometryTest
    {
        [Test]
        public void ParameteredConstructor_WithPoints_PointsSet()
        {
            // Setup
            var list1 = new List<Point2D>
            {
                new Point2D(1.1, 2.2),
                new Point2D(3.3, 4.4),
                new Point2D(5.5, 6.6)
            };

            var list2 = new List<Point2D>
            {
                new Point2D(7.7, 8.8),
                new Point2D(9.9, 10.1),
                new Point2D(11.11, 12.12)
            };

            var list3 = new List<Point2D>
            {
                new Point2D(13.13, 14.14),
                new Point2D(15.15, 16.16),
                new Point2D(17.17, 18.18)
            };

            var geometriesList = new List<IEnumerable<Point2D>>
            {
                list1,
                list2,
                list3
            };

            // Call
            var mapGeometry = new MapGeometry(geometriesList);

            // Assert
            CollectionAssert.AreEqual(geometriesList, mapGeometry.PointCollections);
        }

        [Test]
        public void ParameteredConstructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapGeometry(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "MapGeometry cannot be created without points.");
        }	
    }
}