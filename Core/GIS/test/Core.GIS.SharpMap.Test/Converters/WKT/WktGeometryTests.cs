using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Converters.Geometries;
using Core.GIS.SharpMap.Converters.WellKnownText;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.Converters.WKT
{
    [TestFixture]
    public class WktGeometryTests
    {
        [Test]
        public void ParseGeometryCollection()
        {
            string geomCollection = "GEOMETRYCOLLECTION(POINT(10 10),POINT(30 30),LINESTRING(15 15,20 20))";
            IGeometryCollection geom = GeometryFromWKT.Parse(geomCollection) as IGeometryCollection;
            Assert.IsNotNull(geom);
            Assert.AreEqual(3, geom.NumGeometries);
            Assert.IsTrue(geom[0] is IPoint);
            Assert.IsTrue(geom[1] is IPoint);
            Assert.IsTrue(geom[2] is ILineString);
            Assert.AreEqual(geomCollection, geom.AsText());
            geom = GeometryFromWKT.Parse("GEOMETRYCOLLECTION EMPTY") as IGeometryCollection;
            Assert.IsNotNull(geom);
            Assert.AreEqual(0, geom.NumGeometries);
            geomCollection = "GEOMETRYCOLLECTION(POINT(10 10),LINESTRING EMPTY,POINT(20 49))";
            geom = GeometryFromWKT.Parse(geomCollection) as IGeometryCollection;
            Assert.IsNotNull(geom);
            Assert.IsTrue(geom[1].IsEmpty);
            Assert.AreEqual(3, geom.NumGeometries);
            Assert.AreEqual(geomCollection, geom.AsText());
            Assert.AreEqual("GEOMETRYCOLLECTION EMPTY", GeometryFactory.CreateGeometryCollection().AsText());
        }

        [Test]
        public void ParseMultipolygon()
        {
            string multipolygon = "MULTIPOLYGON(((0 0,10 0,10 10,0 10,0 0)),((5 5,7 5,7 7,5 7,5 5)))";
            IMultiPolygon geom = GeometryFromWKT.Parse(multipolygon) as IMultiPolygon;
            Assert.IsNotNull(geom);
            Assert.AreEqual(2, geom.NumGeometries);
            Assert.AreEqual(GeometryFactory.CreatePoint(5, 5), geom[0].Centroid);
            Assert.AreEqual(multipolygon, geom.AsText());
            Assert.IsNotNull(GeometryFromWKT.Parse("MULTIPOLYGON EMPTY"));
            Assert.IsTrue(GeometryFromWKT.Parse("MULTIPOLYGON EMPTY").IsEmpty);
            geom = GeometryFromWKT.Parse("MULTIPOLYGON(((0 0,10 0,10 10,0 10,0 0)),EMPTY,((5 5,7 5,7 7,5 7,5 5)))") as IMultiPolygon;
            Assert.IsNotNull(geom);
            Assert.IsTrue(geom[1].IsEmpty);
            Assert.AreEqual(GeometryFactory.CreatePoint(5, 5), (geom.Geometries[2] as IPolygon).Shell.EndPoint);
            Assert.AreEqual(GeometryFactory.CreatePoint(5, 5), (geom.Geometries[2] as IPolygon).Shell.StartPoint);
            Assert.AreEqual((geom.Geometries[2] as IPolygon).Shell.StartPoint, (geom.Geometries[2] as IPolygon).Shell.EndPoint);
            Assert.AreEqual(3, geom.NumGeometries);
            Assert.AreEqual("MULTIPOLYGON EMPTY", GeometryFactory.CreateMultiPolygon().AsText());
        }

        [Test]
        public void ParseLineString()
        {
            string linestring = "LINESTRING(20 20,20 30,30 30,30 20,40 20)";
            ILineString geom = GeometryFromWKT.Parse(linestring) as ILineString;
            Assert.IsNotNull(geom);
            Assert.AreEqual(40, geom.Length);
            Assert.IsFalse(geom.IsRing);
            Assert.AreEqual(linestring, geom.AsText());
            Assert.IsTrue((GeometryFromWKT.Parse("LINESTRING EMPTY") as ILineString).IsEmpty);
            Assert.AreEqual("LINESTRING EMPTY", GeometryFactory.CreateLineString(null).AsText());
        }

        [Test]
        public void ParseMultiLineString()
        {
            string multiLinestring = "MULTILINESTRING((10 10,40 50),(20 20,30 20),(20 20,50 20,50 60,20 20))";
            IMultiLineString geom = GeometryFromWKT.Parse(multiLinestring) as IMultiLineString;
            Assert.IsNotNull(geom);
            Assert.AreEqual(3, geom.NumGeometries);
            Assert.AreEqual(180, geom.Length);
            Assert.AreEqual(120, geom.Geometries[2].Length);
            Assert.IsFalse((geom.Geometries[0] as ILineString).IsClosed, "[0].IsClosed");
            Assert.IsFalse((geom.Geometries[1] as ILineString).IsClosed, "[1].IsClosed");
            Assert.IsTrue((geom.Geometries[2] as ILineString).IsClosed, "[2].IsClosed");
            Assert.IsTrue((geom.Geometries[0] as ILineString).IsSimple, "[0].IsSimple");
            Assert.IsTrue((geom.Geometries[1] as ILineString).IsSimple, "[1].IsSimple");
            Assert.IsTrue((geom.Geometries[2] as ILineString).IsSimple, "[2].IsSimple");
            Assert.IsTrue((geom.Geometries[2] as ILineString).IsRing, "Third line is a ring");
            Assert.AreEqual(multiLinestring, geom.AsText());
            Assert.IsTrue(GeometryFromWKT.Parse("MULTILINESTRING EMPTY").IsEmpty);
            geom = GeometryFromWKT.Parse("MULTILINESTRING((10 10,40 50),(20 20,30 20),EMPTY,(20 20,50 20,50 60,20 20))") as IMultiLineString;
            Assert.IsNotNull(geom);
            Assert.IsTrue(geom[2].IsEmpty);
            Assert.AreEqual(4, geom.NumGeometries);
            Assert.AreEqual("MULTILINESTRING EMPTY", GeometryFactory.CreateMultiLineString(null).AsText());
        }

        [Test]
        public void ParsePolygon()
        {
            string polygon = "POLYGON((20 20,20 30,30 30,30 20,20 20))";
            IPolygon geom = GeometryFromWKT.Parse(polygon) as IPolygon;
            Assert.IsNotNull(geom);
            Assert.AreEqual(40, geom.ExteriorRing.Length);
            Assert.AreEqual(100, geom.Area);
            Assert.AreEqual(polygon, geom.AsText());
            //Test interior rings
            polygon = "POLYGON((20 20,20 30,30 30,30 20,20 20),(21 21,29 21,29 29,21 29,21 21),(23 23,23 27,27 27,27 23,23 23))";
            geom = GeometryFromWKT.Parse(polygon) as IPolygon;
            Assert.IsNotNull(geom);
            Assert.AreEqual(40, geom.Shell.Length);
            Assert.AreEqual(2, geom.Holes.Length);
            //Assert.AreEqual(52, geom.Area);
            //Assert.AreEqual(geom.Shell.Area - geom.Holes[0].Area + geom.Holes[1].Area, geom.Area);
            Assert.AreEqual(polygon, geom.AsText());
            //Test empty geometry WKT
            Assert.IsTrue(GeometryFromWKT.Parse("POLYGON EMPTY").IsEmpty);
            Assert.AreEqual("POLYGON EMPTY", GeometryFactory.CreatePolygon(null, null).AsText());
        }

        [Test]
        public void ParsePoint()
        {
            string point = "POINT(20.564 346.3493254)";
            IPoint geom = GeometryFromWKT.Parse(point) as IPoint;
            Assert.IsNotNull(geom);
            Assert.AreEqual(20.564, geom.X);
            Assert.AreEqual(346.3493254, geom.Y);
            Assert.AreEqual(point, geom.AsText());
            Assert.IsTrue(GeometryFromWKT.Parse("POINT EMPTY").IsEmpty);
            Assert.AreEqual("POINT EMPTY", GeometryFactory.CreatePoint(null).AsText());
        }

        [Test]
        public void ParseMultiPoint()
        {
            string multipoint = "MULTIPOINT(20.564 346.3493254,45 32,23 54)";
            IMultiPoint geom = GeometryFromWKT.Parse(multipoint) as IMultiPoint;
            Assert.IsNotNull(geom);
            Assert.AreEqual(20.564, (geom.Geometries[0] as IPoint).X);
            Assert.AreEqual(54, (geom.Geometries[2] as IPoint).Y);
            Assert.AreEqual(multipoint, geom.AsText());
            Assert.IsTrue(GeometryFromWKT.Parse("MULTIPOINT EMPTY").IsEmpty);
            Assert.AreEqual("MULTIPOINT EMPTY", GeometryFactory.CreateMultiPoint(null).AsText());
        }
    }
}