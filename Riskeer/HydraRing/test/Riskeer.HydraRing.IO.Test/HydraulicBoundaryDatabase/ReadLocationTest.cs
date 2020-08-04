using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class ReadLocationTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const long locationId = 1;
            const string assessmentSectionName = "10-1";
            const string hrdFileName = "01_Bovenrijn_selectie_mu2017.sqlite";

            // Call
            var readLocation = new ReadLocation(locationId, assessmentSectionName, hrdFileName);

            // Assert
            Assert.AreEqual(locationId, readLocation.LocationId);
            Assert.AreEqual(assessmentSectionName, readLocation.AssessmentSectionName);
            Assert.AreEqual(hrdFileName, readLocation.HrdFileName);
        }
    }
}
