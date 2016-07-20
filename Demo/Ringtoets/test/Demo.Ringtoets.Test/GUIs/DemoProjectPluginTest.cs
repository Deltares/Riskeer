using System.Linq;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
using Core.Components.Gis.Data;
using Demo.Ringtoets.GUIs;
using Demo.Ringtoets.Views;
using NUnit.Framework;

namespace Demo.Ringtoets.Test.GUIs
{
    [TestFixture]
    public class DemoProjectPluginTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultConstructor_DefaultValues()
        {
            // Call
            using (var plugin = new DemoProjectPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsInstanceOf<IRibbonCommandHandler>(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        public void GetViewInfoObjects_Always_ReturnsChartDataViewInfo()
        {
            // Setup
            using (var plugin = new DemoProjectPlugin())
            {
                // Call
                var views = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(2, views.Length);

                ViewInfo chartViewInfo = views[0];
                Assert.AreEqual(typeof(ChartDataCollection), chartViewInfo.DataType);
                Assert.AreEqual(typeof(ChartDataView), chartViewInfo.ViewType);
                Assert.AreEqual("Diagram", chartViewInfo.GetViewName(new ChartDataView(), null));

                ViewInfo mapViewInfo = views[1];
                Assert.AreEqual(typeof(MapData), mapViewInfo.DataType);
                Assert.AreEqual(typeof(MapDataView), mapViewInfo.ViewType);
                Assert.AreEqual("Kaart", mapViewInfo.GetViewName(new MapDataView(), null));
            }
        }
    }
}