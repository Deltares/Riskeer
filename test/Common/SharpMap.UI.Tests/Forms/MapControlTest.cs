using System.Linq;
using System.Threading;
using System.Windows.Forms;
using DelftTools.TestUtils;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.UI.Forms;
using SharpTestsEx;

namespace SharpMap.UI.Tests.Forms
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
            var layer = new VectorLayer { DataSource = new DataTableFeatureProvider("POINT(1 1)") };
            var map = new Map { Layers = { layer } };

            var mapControl = new MapControl { Map = map };

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
