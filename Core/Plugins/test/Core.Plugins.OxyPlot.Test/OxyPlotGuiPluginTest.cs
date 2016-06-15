using System;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
using Core.Components.Charting.TestUtil;
using Core.Components.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Forms;
using Core.Plugins.OxyPlot.Legend;
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
        public void Activate_WithoutGui_ThrowsArgumentNullException()
        {
            // Setup
            using (var plugin = new OxyPlotGuiPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                Assert.Throws<ArgumentNullException>(test);
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void Activate_WithGui_InitializesComponentsWithIChartViewDataAndBindsActiveViewChanged(bool useChartView)
        {
            // Setup
            var mocks = new MockRepository();
            IView view;

            if (useChartView)
            {
                var chartView = mocks.Stub<IChartView>();
                var chart = mocks.Stub<IChartControl>();
                chart.Data = new TestChartData("test data");
                chartView.Stub(v => v.Chart).Return(chart);
                view = chartView;
            }
            else
            {
                view = mocks.StrictMock<IView>();
            }

            using (var plugin = new OxyPlotGuiPlugin())
            {
                var gui = mocks.StrictMock<IGui>();

                gui.Stub(g => g.IsToolWindowOpen<LegendView>()).Return(false);

                gui.Expect(g => g.OpenToolView(Arg<LegendView>.Matches(c => true)));
                gui.Expect(g => g.ActiveViewChanged += null).IgnoreArguments();
                gui.Expect(g => g.ActiveViewChanged -= null).IgnoreArguments();
                gui.Expect(g => g.ActiveView).Return(view);

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                plugin.Activate();

                // Assert
                Assert.IsInstanceOf<GuiPlugin>(plugin);
                Assert.NotNull(plugin.RibbonCommandHandler);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void GetViewInfoObjects_Always_ReturnsChartDataViewInfo()
        {
            // Setup
            using (var plugin = new OxyPlotGuiPlugin())
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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [RequiresSTA]
        public void GivenConfiguredGui_WhenActiveViewChangesToViewWithChart_ThenRibbonSetVisibility(bool visible)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new ApplicationCore(), new GuiCoreSettings()))
            {
                var plugin = new OxyPlotGuiPlugin();
                var testChartView = new TestChartView();
                var chart = new ChartControl();
                IView viewMock = visible ? (IView) testChartView : new TestView();

                testChartView.Data = chart;

                gui.Plugins.Add(plugin);
                plugin.Gui = gui;
                gui.Run();

                // When
                gui.DocumentViews.Add(viewMock);
                gui.DocumentViews.ActiveView = viewMock;

                // Then
                Assert.AreEqual(visible ? Visibility.Visible : Visibility.Collapsed, plugin.RibbonCommandHandler.GetRibbonControl().ContextualGroups[0].Visibility);
                mocks.VerifyAll();
            }

            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }
    }
}