using System.Linq;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Components.Charting.Data;
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
                var view = new ChartDataView();

                // Call
                var views = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(1, views.Length);
                Assert.AreEqual(typeof(ChartDataCollection), views[0].DataType);
                Assert.AreEqual(typeof(ChartDataView), views[0].ViewType);
                Assert.AreEqual("Diagram", views[0].GetViewName(view, null));
            }
        }
    }
}