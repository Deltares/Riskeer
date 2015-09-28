using DelftTools.TestUtils;
using NUnit.Framework;
using SharpMap.Extensions.Layers;
using SharpMap.UI.Forms;
using SharpTestsEx;

namespace SharpMap.Extensions.Tests.Layers
{
    [TestFixture]
    public class OpenStreetMapLayerTest
    {
        [Test]
        [Category(TestCategory.WindowsForms)]
        public void ShowWithOsmLayer()
        {
            var map = new Map();

            var layer = new OpenStreetMapLayer();
            map.Layers.Add(layer);

            var mapControl = new MapControl {Map = map};

            WindowsFormsTestHelper.ShowModal(mapControl);
        }

        [Test]
        public void CacheDirectoryIsDefined()
        {
            OpenStreetMapLayer.CacheLocation
                .Should("Cache directory is defined")
                .EndWith(@"cache_open_street_map");
        }
    }
}