using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.HydraulicBoundary;
using Ringtoets.Integration.Forms.PropertyClasses;

namespace Ringtoets.Integration.Forms.Test.PropertyClasses
{
    [TestFixture]
    public class HydraulicBoundaryDatabasePropertiesTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var properties = new HydraulicBoundaryDatabaseProperties();

            // Assert
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabaseContext>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var mocks = new MockRepository();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(hydraulicBoundaryDatabase, assessmentSectionBaseMock);
            hydraulicBoundaryDatabaseContext.BoundaryDatabase.FilePath = "Test";

            var properties = new HydraulicBoundaryDatabaseProperties
            {
                Data = hydraulicBoundaryDatabaseContext
            };

            // Call & Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseContext.BoundaryDatabase.FilePath, properties.FilePath);
        }
    }
}