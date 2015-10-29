using System.Linq;
using System.Net;
using Core.GIS.SharpMap.Extensions.Layers;
using NUnit.Framework;

namespace Core.GIS.SharpMap.Extensions.Tests.Layers
{
    [TestFixture]
    public class DeltaresOpenDapDdbTileServerTest
    {
        [Test]
        public void GetHighestResTilesInExtent()
        {
            var datasets = DeltaresOpenDapDdbTileServer.GetAvailableDataSets();
            var gebcoDataSet = datasets.First(d => d.Contains("gebco_08"));
            var datasetInfo = DeltaresOpenDapDdbTileServer.GetInformationFromCatalog(gebcoDataSet);

            var tilesInWorldExtend = DeltaresOpenDapDdbTileServer.GetHighestResolutionTileUrlsInExtent(datasetInfo, -180, -90, 180, 90)
                                                                 .ToList();

            Assert.AreEqual(
                "http://opendap.deltares.nl/thredds/fileServer/opendap/deltares/delftdashboard/bathymetry/gebco_08/zl01/gebco08.zl01.00001.00001.nc",
                tilesInWorldExtend[0], "world1");
            Assert.AreEqual(10368, tilesInWorldExtend.Count, "world2");

            var tilesForNL = DeltaresOpenDapDdbTileServer.GetHighestResolutionTileUrlsInExtent(datasetInfo, 3, 50, 5, 54).ToList();

            Assert.AreEqual(
                "http://opendap.deltares.nl/thredds/fileServer/opendap/deltares/delftdashboard/bathymetry/gebco_08/zl01/gebco08.zl01.00074.00056.nc",
                tilesForNL[0], "nl1");
            Assert.AreEqual(3, tilesForNL.Count, "nl2");
        }

        [Test]
        public void GettingNonRectangularTilesWorks()
        {
            var datasets = DeltaresOpenDapDdbTileServer.GetAvailableDataSets();
            var gebcoDataSet = datasets.First(d => d.Contains("rws_vaklodingen"));
            var datasetInfo = DeltaresOpenDapDdbTileServer.GetInformationFromCatalog(gebcoDataSet);

            var tiles = DeltaresOpenDapDdbTileServer.GetHighestResolutionTileUrlsInExtent(datasetInfo,
                                                                                          159855, 582400, 160089, 582700)
                                                    .ToList();
            Assert.AreEqual(2, tiles.Count, "rd"); // would be 4 if tiles are interpreted rectangular
        }

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            try
            {
                DeltaresOpenDapDdbTileServer.GetAvailableDataSets();
            }
            catch (WebException e)
            {
                Assert.Ignore("Server down: " + e.Message);
            }
        }
    }
}