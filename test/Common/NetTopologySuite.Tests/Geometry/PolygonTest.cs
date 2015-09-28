using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;

namespace NetTopologySuite.Tests.Geometry
{
    [TestFixture]
    public class PolygonTest
    {
        [Test]
        public void CompareUsingHashcode()
        {
            var polygon = new Polygon(new LinearRing(new[] { new Coordinate(1.0, 2.0), new Coordinate(2.0, 3.0), new Coordinate(3.0, 4.0), new Coordinate(1.0, 2.0) }));

            Assert.AreEqual(495991521, polygon.GetHashCode());
        }
    }
}