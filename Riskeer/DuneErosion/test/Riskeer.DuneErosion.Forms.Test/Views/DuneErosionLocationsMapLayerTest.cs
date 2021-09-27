using System;
using Core.Common.Base;
using Core.Components.Gis.Data;
using NUnit.Framework;
using Rhino.Mocks;
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

        [Test]
        public void GivenMapLayerWithDuneLocations_WhenChangingDuneLocationsDataAndObserversNotified_ThenMapDataUpdated()
        {
            // Given
            var failureMechanism = new DuneErosionFailureMechanism();
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test1")
            });

            var mapLayer = new DuneErosionLocationsMapLayer(failureMechanism);

            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            mapLayer.MapData.Attach(observer);

            // Precondition
            AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);

            // When
            failureMechanism.SetDuneLocations(new[]
            {
                new TestDuneLocation("test2")
            });
            failureMechanism.DuneLocations.NotifyObservers();

            // Then
            AssertDuneLocationsMapData(failureMechanism, mapLayer.MapData);
            mocks.VerifyAll();
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