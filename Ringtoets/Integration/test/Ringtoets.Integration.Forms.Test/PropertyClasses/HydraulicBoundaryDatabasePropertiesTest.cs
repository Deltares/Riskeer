using Core.Common.Gui.PropertyBag;
using NUnit.Framework;
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
            Assert.IsInstanceOf<ObjectProperties<HydraulicBoundaryDatabase>>(properties);
            Assert.IsNull(properties.Data);
        }

        [Test]
        public void GetProperties_WithData_ReturnExpectedValues()
        {
            // Setup
            var hydraulicDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "Test"
            };

            var properties = new HydraulicBoundaryDatabaseProperties
            {
                Data = hydraulicDatabase
            };

            // Call & Assert
            Assert.AreEqual(hydraulicDatabase.FilePath, properties.FilePath);
        }
    }
}