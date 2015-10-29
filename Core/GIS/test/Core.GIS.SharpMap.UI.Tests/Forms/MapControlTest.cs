using System.Threading;
using System.Windows.Forms;
using Core.Common.TestUtils;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.UI.Forms;
using NUnit.Framework;

namespace Core.GIS.SharpMap.UI.Tests.Forms
{
    [TestFixture]
    public class MapControlTest
    {
        [SetUp]
        public void SetUp()
        {
            LogHelper.ConfigureLogging();
        }

        [Test]
        public void SelectToolIsActiveByDefault()
        {
            var mapControl = new MapControl();

            Assert.IsTrue(mapControl.SelectTool.IsActive);
        }

        [Test]
        public void ChangingEnvelopeWorksCorrectly()
        {
            var layer = new VectorLayer
            {
                DataSource = new DataTableFeatureProvider("POINT(1 1)")
            };
            var map = new Map.Map
            {
                Layers =
                {
                    layer
                }
            };

            var viewEnvelope = new Envelope(10000, 10010, 10000, 10010);

            map.ZoomToFit(viewEnvelope);

            for (var i = 0; i < 10; i++)
            {
                Application.DoEvents();
                Thread.Sleep(100);
            }

            Assert.IsTrue(map.Envelope.Contains(viewEnvelope));
        }
    }
}