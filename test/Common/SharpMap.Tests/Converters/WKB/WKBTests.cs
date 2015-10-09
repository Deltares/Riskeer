using GeoAPI.Geometries;
using NUnit.Framework;
using SharpMap.Converters.WellKnownBinary;
using SharpMap.Converters.WellKnownText;

namespace SharpMap.Tests.Converters.WKB
{
    [TestFixture]
    public class WKBTests
    {
        private readonly string multiLinestring = "MULTILINESTRING((10 10,40 50),(20 20,30 20),(20 20,50 20,50 60,20 20))";
        private readonly string linestring = "LINESTRING(20 20,20 30,30 30,30 20,40 20)";
        private readonly string polygon = "POLYGON((20 20,20 30,30 30,30 20,20 20),(21 21,21 29,29 29,29 21,21 21))";
        private readonly string point = "POINT(20.564 346.3493254)";
        private readonly string multipoint = "MULTIPOINT(20.564 346.3493254,45 32,23 54)";

        [Test]
        public void Convert()
        {
            IGeometry gML0 = GeometryFromWKT.Parse(multiLinestring);
            IGeometry gLi0 = GeometryFromWKT.Parse(linestring);
            IGeometry gPl0 = GeometryFromWKT.Parse(polygon);
            IGeometry gPn0 = GeometryFromWKT.Parse(point);
            IGeometry gMp0 = GeometryFromWKT.Parse(multipoint);
            IGeometry gML1 = GeometryFromWKB.Parse(gML0.AsBinary());
            IGeometry gLi1 = GeometryFromWKB.Parse(gLi0.AsBinary());
            IGeometry gPl1 = GeometryFromWKB.Parse(gPl0.AsBinary());
            IGeometry gPn1 = GeometryFromWKB.Parse(gPn0.AsBinary());
            IGeometry gMp1 = GeometryFromWKB.Parse(gMp0.AsBinary());

            Assert.IsTrue(Equals(gML0, gML1));
            Assert.IsTrue(Equals(gLi0, gLi1));
            Assert.IsTrue(Equals(gPl0, gPl1));
            Assert.IsTrue(Equals(gPn0, gPn1));
            Assert.IsTrue(Equals(gMp0, gMp1));
        }
    }
}