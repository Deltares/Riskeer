using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.CoordinateSystems.Transformations;
using SharpMap.Extensions.CoordinateSystems;

namespace SharpMap.Extensions.Tests.CoordinateSystems
{
    [TestFixture]
    public class GeodeticDistanceTest
    {
        [Test]
        public void DistanceAmsterdamGroningen()
        {
            Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();

            var rd = Map.CoordinateSystemFactory.CreateFromEPSG(28992);
            var amsterdam = new Coordinate(119843, 487715);
            var groningen = new Coordinate(233883, 582065);
            var distance = GeodeticDistance.Distance(rd, amsterdam, groningen);
            Assert.AreEqual(147000, distance, 1000.0);
        }

        [Test]
        public void DistanceAmsterdamNewYork()
        {
            Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();

            var webMercator = Map.CoordinateSystemFactory.CreateFromEPSG(3857);
            var amsterdam = new Coordinate(547900, 6835651);
            var newyork = new Coordinate(-8218509, 4952200); 
            var distance = GeodeticDistance.Distance(webMercator, amsterdam, newyork);
            Assert.AreEqual(5870000, distance, 1000.0);
        }
    }
}