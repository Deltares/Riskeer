using System;
using System.Diagnostics;
using System.Linq;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.NetTopologySuite.IO;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;
using NUnit.Framework;

namespace Core.GIS.NetTopologySuite.Test.Geometry
{
    [TestFixture]
    public class GeometryTest
    {
        [Test]
        public void DifferenceDoesNotCrashForCertainPolygons()
        {
            //TEST demonstrates problem in NTS 1.7.1 was the reason for upgrade to 1.7.3
            var g1 = new WKTReader().Read("POLYGON ((5 -10, 2 -10, 0 0, 0 1, 5 1, 5 -10))");
            var g2 = new WKTReader().Read("POLYGON ((0.6 -3, 2.6 -3, 3.4 -7, 1.4 -7, 0.6 -3))");

            //this used to crash..
            g1.Difference(g2);
        }

        [Test]
        public void DifferenceProblemForOtherPolygons()
        {
            var wktReader = new WKTReader(new GeometryFactory(new PrecisionModel(1000)));
            var g1 = wktReader.Read("POLYGON((0.0000001 -3,5 -3,5 -7,0.000001 -7,0.0000001 -3))");
            var g2 = wktReader.Read("POLYGON((5 -10,2 -10,0.000001 0,0.000001 1,5 1,5 -10))");

            g1.Difference(g2);
        }

        /// <summary>
        /// Crashes with default precision model.
        /// </summary>
        [Test]
        public void IntersectTwoLines()
        {
            var wktReader = new WKTReader(new GeometryFactory(new PrecisionModel(PrecisionModel.MaximumPreciseValue)));
            var g1 = wktReader.Read("LINESTRING(280 0.01, 285 -0.07)");
            var g2 = wktReader.Read("LINESTRING(-900.0 0, 1520.0 0)");

            var intersection = g1.Intersection(g2);

            Assert.AreEqual(1, intersection.Coordinates.Count());
        }

        [Test]
        public void LineGeometryBuffer()
        {
            double tolerance = 20.0;
            var line = new LineString(new ICoordinate[]
            {
                new Coordinate(0, 100),
                new Coordinate(100, 0)
            });
            var bufferedEnvelope = line.Envelope.Buffer(tolerance, 1);

            Assert.IsTrue(bufferedEnvelope.Contains(new Point(90, 105)));
            Assert.IsFalse(bufferedEnvelope.Contains(new Point(200, 80)));
            Assert.IsTrue(bufferedEnvelope.Contains(new Point(109, 109)));
            Assert.IsFalse(bufferedEnvelope.Contains(new Point(111, 111)));
        }

        [Test]
        public void GetHashCodeShouldBeComputedLazyAndShouldBeFast()
        {
            var geometryCount = 1000;
            var geometries = new IGeometry[geometryCount];

            for (int i = 0; i < geometryCount; i++)
            {
                geometries[i] = new Polygon(new LinearRing(new[]
                {
                    new Coordinate(1.0, 2.0),
                    new Coordinate(2.0, 3.0),
                    new Coordinate(3.0, 4.0),
                    new Coordinate(1.0, 2.0)
                }));
            }

            var polygon = new Polygon(new LinearRing(new[]
            {
                new Coordinate(1.0, 2.0),
                new Coordinate(2.0, 3.0),
                new Coordinate(3.0, 4.0),
                new Coordinate(1.0, 2.0)
            }));

            // computes hash code every call
            var t0 = DateTime.Now;
            for (int i = 0; i < geometryCount; i++)
            {
                geometries[i].GetHashCode();
            }
            var t1 = DateTime.Now;

            var dt1 = t1 - t0;

            // computes hash code only first time (lazy)
            t0 = DateTime.Now;
            for (int i = 0; i < geometryCount; i++)
            {
                polygon.GetHashCode();
            }
            t1 = DateTime.Now;

            var dt2 = t1 - t0;

            Assert.IsTrue(dt2.TotalMilliseconds < 15*dt1.TotalMilliseconds);
        }

        [Test]
        public void GetHashCodeTakesYIntoAccount()
        {
            var point = new Point(1, 2);

            var c1 = point.GetHashCode();

            point.Y = 3;
            Assert.AreNotEqual(c1, point.GetHashCode());
        }

        [Test]
        public void GeometryTransformScaleTest()
        {
            var wktReader = new WKTReader(new GeometryFactory(new PrecisionModel(1000)));
            var geometry = wktReader.Read("POLYGON((0 -3,5 -3,5 -7,0 -7,0 -3))");

            var scale = 5.0;

            var scaledGeometry = GeometryTransform.Scale(geometry, scale);

            Assert.AreEqual(scaledGeometry.Coordinates.Length, geometry.Coordinates.Length);
            Assert.AreEqual(scaledGeometry.Centroid, geometry.Centroid);

            Assert.AreEqual(-10.0, scaledGeometry.Coordinates[0].X);
            Assert.AreEqual(5.0, scaledGeometry.Coordinates[0].Y);
            Assert.AreEqual(15.0, scaledGeometry.Coordinates[1].X);
            Assert.AreEqual(5.0, scaledGeometry.Coordinates[1].Y);
            Assert.AreEqual(15.0, scaledGeometry.Coordinates[2].X);
            Assert.AreEqual(-15.0, scaledGeometry.Coordinates[2].Y);
            Assert.AreEqual(-10.0, scaledGeometry.Coordinates[3].X);
            Assert.AreEqual(-15.0, scaledGeometry.Coordinates[3].Y);
            Assert.AreEqual(-10.0, scaledGeometry.Coordinates[4].X);
            Assert.AreEqual(5.0, scaledGeometry.Coordinates[4].Y);
        }

        [Test]
        public void IntersectsIgnoresZCoordinates()
        {
            var wktReader = new WKTReader(new GeometryFactory(new PrecisionModel(1000)));

            var polygon = wktReader.Read("POLYGON((0 0,1 1,1 -1,0 0))");
            var disjointLine = wktReader.Read("LINESTRING(0 1,1 2)");
            var intersectingLine = wktReader.Read("LINESTRING(0 1,1 0)");

            Assert.IsFalse(polygon.Intersects(disjointLine));
            Assert.IsTrue(polygon.Intersects(intersectingLine));

            foreach (var coordinate in polygon.Coordinates)
            {
                coordinate.Z = 1;
            }
            foreach (var coordinate in disjointLine.Coordinates)
            {
                coordinate.Z = 1;
            }
            foreach (var coordinate in intersectingLine.Coordinates)
            {
                coordinate.Z = -1;
            }

            Assert.IsFalse(polygon.Intersects(disjointLine));
            Assert.IsTrue(polygon.Intersects(intersectingLine));
        }

        [Test]
        public void UnionPolygons()
        {
            var wktReader = new WKTReader(new GeometryFactory(new PrecisionModel(1000)));

            var polygon1 = wktReader.Read("POLYGON((0 0,1 1,1 -1,0 0))");
            var polygon2 = wktReader.Read("POLYGON((1 1,2 0,1 -1,1 1))");

            var union = polygon1.Union(polygon2);

            var wkt = new WKTWriter().Write(union);

            Trace.WriteLine(wkt);

            Assert.AreEqual("POLYGON((0 0,1 1,2 0,1 -1,0 0))", wkt, "polygons are merged");
        }
    }
}