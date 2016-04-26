using System;
using Application.Ringtoets.Storage.Create;
using Application.Ringtoets.Storage.DbContext;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;

namespace Application.Ringtoets.Storage.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryLocationCreateExtensionsTest
    {
        [Test]
        public void Create_WithoutCollector_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", 2, 3);

            // Call
            TestDelegate test = () => hydraulicBoundaryLocation.Create(null);

            // Assert
            var parameterName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("collector", parameterName);
        }

        [Test]
        public void Create_WithCollector_ReturnsHydraulicBoundaryLocationEntityWithPropertiesSet()
        {
            // Setup
            var testName = "testName";
            var random = new Random(21);
            var coordinateX = random.NextDouble();
            var coordinateY = random.NextDouble();
            var id = random.Next(0,150);
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, testName, coordinateX, coordinateY);
            var collector = new CreateConversionCollector();

            // Call
            var entity = hydraulicBoundaryLocation.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(testName, entity.Name);
            Assert.AreEqual(Convert.ToDecimal(coordinateX), entity.LocationX);
            Assert.AreEqual(Convert.ToDecimal(coordinateY), entity.LocationY);
            Assert.AreEqual(id, entity.LocationId);
            Assert.IsNull(entity.DesignWaterLevel);
        }

        [Test]
        public void Create_WithCollectorAndDesignWaterLevel_ReturnsHydraulicBoundaryLocationEntityWithDesignWaterLevelSet()
        {
            // Setup
            var random = new Random(21);
            var waterLevel = random.NextDouble();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(-1, "testName", random.NextDouble(), random.NextDouble())
            {
                DesignWaterLevel = waterLevel
            };
            var collector = new CreateConversionCollector();

            // Call
            var entity = hydraulicBoundaryLocation.Create(collector);

            // Assert
            Assert.IsNotNull(entity);
            Assert.AreEqual(Convert.ToDecimal(waterLevel), entity.DesignWaterLevel);
        }
    }
}