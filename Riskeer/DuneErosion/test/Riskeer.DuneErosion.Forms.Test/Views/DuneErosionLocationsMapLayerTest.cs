using System;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Data.TestUtil;
using Riskeer.DuneErosion.Forms.TestUtil;
using Riskeer.DuneErosion.Forms.Views;

namespace Riskeer.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneErosionLocationsMapLayerTest
    {
        [Test]
        public void Constructor_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new DuneErosionLocationsMapLayer(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test")
            });

            // Call
            var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism);

            // Assert
            Assert.IsInstanceOf<IDisposable>(mapLayer);
            AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
        }

        private static void AssertDuneLocationsMapData(DuneErosionFailureMechanism failureMechanism,
                                                       MapData mapData)
        {
            Assert.IsInstanceOf<MapPointData>(mapData);
            Assert.AreEqual("Hydraulische belastingen", mapData.Name);

            var duneLocationsMapData = (MapPointData) mapData;
            DuneErosionMapFeaturesTestHelper.AssertDuneLocationFeaturesData(failureMechanism, duneLocationsMapData.Features);
        }
    }
}