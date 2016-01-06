using System;
using System.Linq;
using Core.Common.Gui;
using Core.Components.OxyPlot.Data;
using Core.Plugins.OxyPlot.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test
{
    [TestFixture]
    public class OxyPlotGuiPluginTest
    {
        [Test]
        [RequiresSTA]
        public void DefaultConstructor_Always_NoRibbonCommandHandlerSet()
        {
            // Call
            var plugin = new OxyPlotGuiPlugin();

            // Assert
            Assert.IsInstanceOf<GuiPlugin>(plugin);
            Assert.IsNull(plugin.RibbonCommandHandler);
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithoutGui_ThrowsNullReferenceException()
        {
            // Setup
            var plugin = new OxyPlotGuiPlugin();

            // Call
            TestDelegate test = () => plugin.Activate();

            // Assert
            Assert.Throws<NullReferenceException>(test);
        }
        [Test]
        [RequiresSTA]
        public void Activate_WithGui_SetsRibbonCommandHandlerAndBindsActiveViewChanged()
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = new OxyPlotGuiPlugin();
            var gui = mocks.StrictMock<IGui>();
            gui.Expect(g => g.ActiveViewChanged += null).IgnoreArguments();

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            plugin.Activate();

            // Assert
            Assert.IsInstanceOf<GuiPlugin>(plugin);
            Assert.NotNull(plugin.RibbonCommandHandler);
            mocks.VerifyAll();
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