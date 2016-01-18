using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewManager;
using Core.Components.Charting.Data;
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
                Assert.IsInstanceOf<IToolViewController>(plugin);
                Assert.IsInstanceOf<IDocumentViewController>(plugin);
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
        public void Activate_WithGui_InitializesComponentsAndBindsActiveViewChanged()
        {
            // Setup
            var mocks = new MockRepository();

            using (var plugin = new OxyPlotGuiPlugin())
            {
                var gui = mocks.StrictMock<IGui>();
                var dockingManger = mocks.Stub<IDockingManager>();
                var toolWindows = new ViewList(dockingManger, null);
                var view = mocks.StrictMock<IView>();

                gui.Expect(g => g.ActiveViewChanged += null).IgnoreArguments();
                gui.Expect(g => g.ActiveViewChanged -= null).IgnoreArguments();
                gui.Expect(g => g.ToolWindowViews).Return(toolWindows).Repeat.Twice();
                gui.Expect(g => g.OpenToolView(Arg<LegendView>.Matches(c => true)));
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
                var views = plugin.GetViewInfoObjects().ToArray();

                // Assert
                Assert.AreEqual(1, views.Length);
                Assert.AreEqual(typeof(ICollection<ChartData>), views[0].DataType);
                Assert.AreEqual(typeof(ChartDataView), views[0].ViewType);
                Assert.AreEqual("Diagram", views[0].GetViewName(view, null));
            }
        }

        [Test]
        public void CloseToolView_Always_CloseToolView()
        {
            // Setup
            using (var plugin = new OxyPlotGuiPlugin())
            {
                var mocks = new MockRepository();
                var gui = mocks.StrictMock<IGui>();
                var view = mocks.StrictMock<IView>();
                gui.Expect(g => g.CloseToolView(view));
                
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                plugin.CloseToolView(view);

                // Assert
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ActiveView_Always_ReturnGuiActiveView()
        {
            // Setup
            var mocks = new MockRepository();
            var gui = mocks.StrictMock<IGui>();
            var view = mocks.StrictMock<IView>();

            gui.Expect(g => g.ActiveView).Return(view);

            mocks.ReplayAll();

            using (var plugin = new OxyPlotGuiPlugin())
            {
                plugin.Gui = gui;

                // Call
                var result = plugin.ActiveView;

                // Assert
                Assert.AreSame(view, result);
            }
        }

        [Test]
        [RequiresSTA]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenConfiguredGui_WhenOpenToolView_UpdateComponentsWithDataFromActiveView(bool isChartViewActive)
        {
            // Given
            using (var gui = new RingtoetsGui())
            {
                var plugin = new OxyPlotGuiPlugin();
                gui.MainWindow = new MainWindow(gui);
                var mocks = new MockRepository();
                IView viewMock = isChartViewActive ? (IView)new TestChartView() : new TestView();
                viewMock.Data = new BaseChart();

                mocks.ReplayAll();

                gui.Plugins.Add(plugin);
                gui.Run();

                gui.DocumentViews.Add(viewMock);
                gui.DocumentViews.ActiveView = viewMock;
                var legendView = gui.ToolWindowViews.First(t => t is LegendView);

                legendView.Data = null;

                // When
                plugin.OpenToolView(legendView);

                // Then
                Assert.AreSame(isChartViewActive ? viewMock.Data : null, legendView.Data);
                mocks.VerifyAll();
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
            {
                var plugin = new OxyPlotGuiPlugin();
                gui.MainWindow = new MainWindow(gui);
                var mocks = new MockRepository();
                IView viewMock = visible ? (IView) new TestChartView() : new TestView();

                mocks.ReplayAll();

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
        }
    }
}