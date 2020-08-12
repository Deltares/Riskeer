using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class ReadHrdLocationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const long locationId = 1;
            const string name = "NW_1_208_vk_00063";
            const double xCoordinate = 70778.1;
            const double yCoordinate = 441562.2;

            // Call
            var readHrdLocation = new ReadHrdLocation(locationId,
                                                      name,
                                                      xCoordinate,
                                                      yCoordinate);

            // Assert
            Assert.AreEqual(locationId, readHrdLocation.HrdLocationId);
            Assert.AreEqual(name, readHrdLocation.Name);
            Assert.AreEqual(xCoordinate, readHrdLocation.CoordinateX);
            Assert.AreEqual(yCoordinate, readHrdLocation.CoordinateY);
        }
    }
}