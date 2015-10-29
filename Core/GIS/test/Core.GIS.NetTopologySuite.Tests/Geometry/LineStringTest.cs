using Core.GIS.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Core.GIS.NetTopologySuite.Tests.Geometry
{
    [TestFixture]
    public class LineStringTest
    {
        [Test]
        public void GetHashCodeMustBeComputed()
        {
            var lineString = new LineString(new ICoordinate[]
            {
                new Coordinate(1.0, 2.0),
                new Coordinate(3.0, 4.0)
            });

            Assert.AreEqual(2024009049, lineString.GetHashCode());
        }
    }
}