using System;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using Core.Common.Controls.Views;
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
        public void DefaultConstructor_Always_NoRibbonCommandHandlerSet()
        {
            // Call
            using (var plugin = new OxyPlotGuiPlugin())
            {
                // Assert
                Assert.IsInstanceOf<GuiPlugin>(plugin);
                Assert.IsNull(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithoutGui_ThrowsNullReferenceException()
        {
            // Setup
            using (var plugin = new OxyPlotGuiPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                Assert.Throws<NullReferenceException>(test);
            }
        }

        [Test]
        [RequiresSTA]
        public void Activate_WithGui_SetsRibbonCommandHandlerAndBindsActiveViewChanged()
        {
            // Setup
            using (var plugin = new OxyPlotGuiPlugin())
            {
                var mocks = new MockRepository();
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
        }

        [Test]
        public void GetViewInfoObjects_Always_ReturnsChartDataViewInfo()
        {
            // Setup
            using (var plugin = new OxyPlotGuiPlugin())
            {
                // Call
                var views = plugin.GetViewInfoObjects().ToArray();

                // Assert
                Assert.AreEqual(1, views.Length);
                Assert.AreEqual(typeof(IChartData), views[0].DataType);
                Assert.AreEqual(typeof(ChartDataView), views[0].ViewType);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [RequiresSTA]
        public void GivenConfiguredGui_WhenActiveViewChangesToIChartView_ThenRibbonSetVisibility(bool visible)
        {
            // Given
            using (var gui = new RingtoetsGui())
            using (var plugin = new OxyPlotGuiPlugin())
            {
                var mocks = new MockRepository();
                IView viewMock = visible ? (IView) new TestChartView() : new TestView();

                mocks.ReplayAll();

                gui.Plugins.Add(plugin);
                gui.Run();

                gui.DocumentViews.IgnoreActivation = false;

                // When
                gui.DocumentViews.Add(viewMock);
                gui.DocumentViews.ActiveView = viewMock;

                // Then
                Assert.AreEqual(visible ? Visibility.Visible : Visibility.Collapsed, plugin.RibbonCommandHandler.GetRibbonControl().ContextualGroups[0].Visibility);
                mocks.VerifyAll();
            }
        }
    }

    public class TestView : Control, IView
    {
        public object Data { get; set; }
    }

    public class TestChartView : Control, IChartView
    {
        public object Data { get; set; }
    }
}