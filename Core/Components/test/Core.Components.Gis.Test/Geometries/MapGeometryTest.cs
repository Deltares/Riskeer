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
            var points = new List<Point2D>
            {
                new Point2D(0.1, 1.3),
                new Point2D(2.1, 5.3),
                new Point2D(3.8, 1.1)
            };

            // Call
            var mapGeometry = new MapGeometry(points);

            // Assert
            CollectionAssert.AreEqual(points, mapGeometry.Points);
        }

        [Test]
        public void ParameteredConstructor_WithoutPoints_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MapGeometry(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "MapGeometry can't be created without points.");
        }	
    }
}