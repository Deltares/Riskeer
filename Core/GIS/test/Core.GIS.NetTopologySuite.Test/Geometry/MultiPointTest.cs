using Core.GIS.GeoAPI.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using NUnit.Framework;

namespace Core.GIS.NetTopologySuite.Test.Geometry
{
    [TestFixture]
    public class MultiPointTest
    {
        [Test]
        public void GetHashCodeShoulBeComputed()
        {
            var multiPoint = new MultiPoint(new IPoint[]
            {
                new Point(1.0, 2.0),
                new Point(3.0, 4.0)
            });

            Assert.AreEqual(2024009049, multiPoint.GetHashCode());
        }
    }
}