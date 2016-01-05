using System.Linq;
using Core.Common.Gui;
using Core.Components.OxyPlot.Data;
using Core.Plugins.OxyPlot.Forms;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class OxyPlotGuiPluginTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultConstructor_Always_RibbonCommandHandlerAssigned()
        {
            // Call
            var plugin = new OxyPlotGuiPlugin();

            // Assert
            Assert.IsInstanceOf<GuiPlugin>(plugin);
            Assert.NotNull(plugin.RibbonCommandHandler);
        }

        [Test]
        [RequiresSTA]
        public void GetViewInfoObjects_Always_ReturnsChartDataViewInfo()
        {
            // Setup
            var plugin = new OxyPlotGuiPlugin();

            // Call
            var views = plugin.GetViewInfoObjects().ToArray();

            // Assert
            Assert.AreEqual(1, views.Length);
            Assert.AreEqual(typeof(IChartData), views[0].DataType);
            Assert.AreEqual(typeof(ChartDataView), views[0].ViewType);
        }
    }
}