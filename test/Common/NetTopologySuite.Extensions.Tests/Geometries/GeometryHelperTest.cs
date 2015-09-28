using System;
using System.Linq;
using DelftTools.TestUtils;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.IO;
using GisSharpBlog.NetTopologySuite.Utilities;
using NetTopologySuite.Extensions.Features;
using NetTopologySuite.Extensions.Geometries;
using NUnit.Framework;
using SharpTestsEx;
using Assert = NUnit.Framework.Assert;

namespace NetTopologySuite.Extensions.Tests.Geometries
{
    [TestFixture]
    public class GeometryHelperTest
    {
        [Test]
        public void GetNearestFeatureReturnsLastNearestFeature()
        {
            var features = new[]
                               {
                                   new Feature {Geometry = new Point(0, 0)},
                                   new Feature {Geometry = new Point(2, 0)},
                                   new Feature {Geometry = new Point(2, 2)}
                               };

            var feature1 = GeometryHelper.GetNearestFeature(new Coordinate(1, 1), features, 3);

            feature1
                .Should("the last feature is chosen if more than 1 featres with the same distance are found")
                    .Be.EqualTo(features[2]);
        }

        [Test]
        public void GetNearestFeatureReturnsNullWhenNoNearestFeaturesCanBeFound()
        {
            var features = new[]
                               {
                                   new Feature {Geometry = new Point(0, 0)},
                                   new Feature {Geometry = new Point(2, 0)},
                                   new Feature {Geometry = new Point(2, 2)}
                               };

            var feature2 = GeometryHelper.GetNearestFeature(new Coordinate(1, 1), features, 0.5);

            feature2
                .Should("tolerance is too small")
                    .Be.Null();
        }

        [Test]
        public void SplitPolygonVerticalAt()
        {
            //create a polygon
            var polygon = new Polygon(new LinearRing(new[]{new Coordinate(0, 0), 
                                           new Coordinate(10, 10), 
                                           new Coordinate(20, 0),
                                           new Coordinate(0, 0)}));
            var polygonHalfs = GeometryHelper.SplitGeometryVerticalAt(polygon, 10);
            Assert.AreEqual(
                new[] {new Coordinate(0, 0), new Coordinate(10, 10), new Coordinate(10, 0), new Coordinate(0, 0)},
                polygonHalfs.First.Coordinates);

            Assert.AreEqual(
                new[] { new Coordinate(10, 10), new Coordinate(20, 0), new Coordinate(10, 0), new Coordinate(10, 10) },
                polygonHalfs.Second.Coordinates);
        }

        [Test]
        public void SplitPolygonVerticalAtInvalidLocationThrowsException()
        {
            //create a polygon
            int callCount = 0;
            try
            {
                var polygon = new Polygon(new LinearRing(new[]{new Coordinate(0, 0), 
                                           new Coordinate(10, 10), 
                                           new Coordinate(20, 0),
                                           new Coordinate(0, 0)}));
                var polygonHalfs = GeometryHelper.SplitGeometryVerticalAt(polygon, 30.01);
                Assert.AreEqual(
                    new[] { new Coordinate(0, 0), new Coordinate(10, 10), new Coordinate(10, 0), new Coordinate(0, 0) },
                    polygonHalfs.First.Coordinates);

                Assert.AreEqual(
                    new[] { new Coordinate(10, 10), new Coordinate(20, 0), new Coordinate(10, 0), new Coordinate(10, 10) },
                    polygonHalfs.Second.Coordinates);
            }
            catch (Exception ex)
            {
                callCount++;
                Assert.IsTrue(ex is ArgumentOutOfRangeException);
                //can't use expected message since the message is only known at runtime (the decimal sep)
                string expectedMessage = string.Format("Splitpoint at x {0:00.00} not within polygon. \r\nParameter name: splitPointX", 30.01);
                Assert.AreEqual(expectedMessage, ex.Message);
            }
            //make sure the exception was thrown once
            Assert.AreEqual(1,callCount);
            
        }

        [Test]
        public void SplitPolygonVerticalAtEdge()
        {
            //create a polygon
            var polygon = new Polygon(new LinearRing(new[]{new Coordinate(0, 0), 
                                           new Coordinate(10, 10), 
                                           new Coordinate(10, 5),
                                           new Coordinate(20, 0),
                                            new Coordinate(0,0)}));
            
            var polygonHalfs = GeometryHelper.SplitGeometryVerticalAt(polygon, 10);
            Assert.AreEqual(
                new[] { new Coordinate(0, 0), new Coordinate(10, 10), new Coordinate(10, 5), new Coordinate(10, 0), new Coordinate(0, 0) },
                polygonHalfs.First.Coordinates);

            Assert.AreEqual(
                new[] { new Coordinate(10, 10), new Coordinate(10, 5), new Coordinate(10, 5), new Coordinate(20, 0), new Coordinate(10, 0), new Coordinate(10, 5) },
                polygonHalfs.Second.Coordinates);
        }
        
        [Test]
        public void Difference()
        {
            var polygon =
                new Polygon(
                    new LinearRing(new[]
                                       {
                                           new Coordinate(0, 0), new Coordinate(5, 0), new Coordinate(5, 5),
                                           new Coordinate(0, 5), new Coordinate(0, 0)
                                       }));
            var other =
                new Polygon(
                    new LinearRing(new[]
                                       {
                                           new Coordinate(0, 2), new Coordinate(8, 2), new Coordinate(1, 1),
                                           new Coordinate(0, 1), new Coordinate(0, 2)
                                       }));
            
            var error = polygon.Difference(other);
        }

        [Test]
        public void NormalizePolygon()
        {
            var polygonWithRedundantPoints = new Polygon(
                new LinearRing(new[]
                                   {
                                       new Coordinate(0, 0), new Coordinate(1, 1), new Coordinate(2, 2),
                                       new Coordinate(3, 1), new Coordinate(4, 0), new Coordinate(2, 0),
                                       new Coordinate(2, 0),
                                       new Coordinate(0, 0)
                                   }));
            var expectedPolygon = new Polygon(
                new LinearRing(new[]
                                   {
                                       new Coordinate(0, 0), new Coordinate(2, 2),
                                       new Coordinate(4, 0), new Coordinate(0, 0)
                                   }));

            var actualPolygon = GeometryHelper.NormalizeGeometry(polygonWithRedundantPoints);
            Assert.AreEqual(expectedPolygon.Coordinates.Length, actualPolygon.Coordinates.Length);
            Assert.AreEqual(expectedPolygon, actualPolygon);
        }

        [Test]
        public void NormalizePolygonWithEndPointAsStartPoint()
        {
            var polygonWithRedundantPoints = new Polygon(
                new LinearRing(new[]
                                   {
                                       new Coordinate(1, 0), new Coordinate(0, 0), new Coordinate(0,1),
                                       new Coordinate(2, 1), new Coordinate(2, 0), new Coordinate(1, 0)
                                   }));
            var expectedPolygon = new Polygon(
                new LinearRing(new[]
                                   {
                                       new Coordinate(0, 0), new Coordinate(0, 1),
                                       new Coordinate(2, 1), new Coordinate(2, 0),new Coordinate(0,0)
                                   }));
            
            var actualPolygon = GeometryHelper.NormalizeGeometry(polygonWithRedundantPoints);
            Assert.AreEqual(expectedPolygon.Coordinates.Length, actualPolygon.Coordinates.Length);
            Assert.AreEqual(expectedPolygon, actualPolygon);
        }

        [Test]
        public void TestPointIsInLine()
        {
            Assert.IsFalse(GeometryHelper.PointIsOnLineBetweenPreviousAndNext(new Coordinate(0, 0), new Coordinate(2, 2),
                                                               new Coordinate(1, 1))); 

        }

        [Test]
        public void Normalize()
        {
            var polygon =
                new WKTReader().Read(
                    "POLYGON((2 -10,1.4 -7,3.4 -7,2.6 -3,0.6 -3,0 0,0 1,10 1,10 0,9 -5,8.6 -7,8 -10,2 -10))");
            var normal = GeometryHelper.NormalizeGeometry(polygon);
            
            Assert.AreEqual(polygon.Coordinates.Length-2,normal.Coordinates.Length);
        }

        [Test]
        public void NormalizeLineString()
        {
            var line =
                new WKTReader().Read(
                    "LINESTRING(10 10,5 5,0 0)");
            var normal = GeometryHelper.NormalizeGeometry(line);

            Assert.AreEqual(new[]{new Coordinate(10,10),new Coordinate(0,0)}, normal.Coordinates);
        }

        [Test]
        public void GetIntersectionArea()
        {
            var polygon = new WKTReader().Read(
                    "POLYGON((2 -10,1.4 -7,3.4 -7,2.6 -3,0.6 -3,0 0,0 1,10 1,10 0,9 -5,8.6 -7,8 -10,2 -10))");

            var polygon2 = new WKTReader().Read(
                    "POLYGON((3 -10,2.4 -7,4.4 -7,3.6 -3,1.6 -3,1 0,1 1,11 1,11 0,10 -5,9.6 -7,9 -10,3 -10))");

            //self area
            Assert.AreEqual(polygon.Area, GeometryHelper.GetSampledIntersectionArea(polygon, polygon), 3);

            var actualArea = GeometryHelper.GetIntersectionArea(polygon, polygon2);
            Assert.AreEqual(71, actualArea, 0.1);
            Assert.AreEqual(actualArea, GeometryHelper.GetSampledIntersectionArea(polygon, polygon2), 3);
        }

        [Test]
        public void GetIntersectionAreaCircles()
        {
            var factory = new GeometricShapeFactory {Size = 20};
            var circle = factory.CreateCircle();
            factory.Centre = new Coordinate(0, 5);
            var circle2 = factory.CreateCircle();

            //self area
            Assert.AreEqual(circle.Area, GeometryHelper.GetSampledIntersectionArea(circle, circle), 10);

            var actualArea = GeometryHelper.GetIntersectionArea(circle, circle2);
            Assert.AreEqual(102.68839131234732, actualArea, 0.1);
            Assert.AreEqual(actualArea, GeometryHelper.GetSampledIntersectionArea(circle, circle2), 10);
        }

        [Test]
        [Category(TestCategory.Jira)] //TOOLS-7021
        public void DistanceShouldNotThrowForGeometryNull()
        {
            var lineString = new LineString(new[] { new Coordinate(0, 0), new Coordinate(10, 0) });
            Coordinate nullCoordinate = null;
            Point nullGeometry = null;

            var distance = GeometryHelper.Distance(null, new Coordinate(0, 0));
            Assert.AreEqual(double.MaxValue, distance);

            distance = GeometryHelper.Distance(lineString, nullCoordinate);
            Assert.AreEqual(double.MaxValue, distance);

            distance = GeometryHelper.Distance(null, new Point(0, 0));
            Assert.AreEqual(double.MaxValue, distance);

            distance = GeometryHelper.Distance(lineString, nullGeometry);
            Assert.AreEqual(double.MaxValue, distance);
        }

        [Test]
        [Category(TestCategory.Jira)] //TOOLS-7021
        public void GetNearestFeatureShouldNotThrowForGeometryNull()
        {
            var nearestFeature = GeometryHelper.GetNearestFeature(new Coordinate(0, 0),
                                                                  new[] { 
                                                                            new Feature {Geometry = null},
                                                                            new Feature {Geometry = null},
                                                                            new Feature {Geometry = null}
                                                                         },
                                                                  2);
            Assert.IsNull(nearestFeature);

            var featureToBeFound = new Feature { Geometry = new Point(0.2, 0.2) };
            nearestFeature = GeometryHelper.GetNearestFeature(new Coordinate(0, 0),
                                                              new[] 
                                                                  { 
                                                                      new Feature {Geometry = null},
                                                                      new Feature {Geometry = new Point(1, 0)},
                                                                      featureToBeFound
                                                                  },
                                                              2);
            Assert.IsNotNull(nearestFeature);
            Assert.IsTrue(featureToBeFound == nearestFeature);
        }

        [Test]
        [Category(TestCategory.Jira)] //TOOLS-7021
        public void GetFeaturesInRangeShouldNotThrowForGeometryNull()
        {
            var features = GeometryHelper.GetFeaturesInRange(new Coordinate(0, 0),
                                                             new[]
                                                                 {
                                                                     new Feature {Geometry = null},
                                                                     new Feature {Geometry = null},
                                                                     new Feature {Geometry = null}
                                                                 }, 
                                                             5).ToArray();
            Assert.IsEmpty(features);

            var feature1 = new Feature { Geometry = new Point(0.2, 1) };
            var feature2 = new Feature { Geometry = new Point(1, 0, 2) };
            features = GeometryHelper.GetFeaturesInRange(new Coordinate(0, 0),
                                                         new[]
                                                             {
                                                                 feature1,
                                                                 new Feature {Geometry = null},
                                                                 feature2
                                                             },
                                                         5).ToArray();
            Assert.AreEqual(2, features.Count());
            Assert.Contains(feature1, features);
            Assert.Contains(feature2, features);
        }
    }
}