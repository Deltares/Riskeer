using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.CoordinateSystems;
using Core.GIS.SharpMap.Extensions.CoordinateSystems;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Test.CoordinateSystems
{
    [TestFixture]
    public class CoordinateSystemValidatorTest
    {
        [SetUp]
        public void InitializeMap()
        {
            Map.Map.CoordinateSystemFactory = new OgrCoordinateSystemFactory();
        }

        [Test]
        public void Wgs84CoordinatesValidateAsWgs84Coordinates()
        {
            var testCoordinates = new[]
            {
                new Coordinate(52.155556, 5.387778)
            };
            var factory = new OgrCoordinateSystemFactory();
            var wgs84CS = factory.CreateFromEPSG(4326);
            Assert.IsTrue(CoordinateSystemValidator.CanAssignCoordinateSystem(testCoordinates, wgs84CS));
        }

        [Test]
        public void RdCoordinatesSucceedAsRdCoordinates()
        {
            var testCoordinates = new[]
            {
                new Coordinate(135000, 463000),
                new Coordinate(155000, 463000)
            };
            var factory = new OgrCoordinateSystemFactory();
            var rd = factory.CreateFromEPSG(28992);
            Assert.IsTrue(CoordinateSystemValidator.CanAssignCoordinateSystem(testCoordinates, rd));
        }

        [Test]
        public void RdCoordinatesFailAsWgs84Coordinates()
        {
            var testCoordinates = new[]
            {
                new Coordinate(135000, 463000),
                new Coordinate(155000, 463000)
            };
            var factory = new OgrCoordinateSystemFactory();
            var wgs84CS = factory.CreateFromEPSG(4326);
            Assert.IsFalse(CoordinateSystemValidator.CanAssignCoordinateSystem(testCoordinates, wgs84CS));
        }

        [Test]
        public void WebMercatorCoordinatesValidateAsWebMercatorAndFailAsWgs84Coordinates()
        {
            var amsterdam = new Coordinate(547900, 6835651);
            var newyork = new Coordinate(-8218509, 4952200);
            var testCoordinates = new[]
            {
                amsterdam,
                newyork
            };

            var factory = new OgrCoordinateSystemFactory();

            // check against webmercator
            var webMercator = factory.CreateFromEPSG(3857);
            Assert.IsTrue(CoordinateSystemValidator.CanAssignCoordinateSystem(testCoordinates, webMercator));

            // check against wgs84
            var wgs84CS = factory.CreateFromEPSG(4326);
            Assert.IsFalse(CoordinateSystemValidator.CanAssignCoordinateSystem(testCoordinates, wgs84CS));
        }
    }
}