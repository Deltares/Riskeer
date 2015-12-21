using System.Linq;
using Core.Common.Base.Plugin;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewManager;
using Core.Plugins.Charting.Forms;
using Core.Plugins.Charting.Property;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Charting.Test
{
    [TestFixture]
    public class ChartingGuiPluginTest
    {
        [Test]
        [RequiresSTA]
        public void ChartLegendViewIsUpdatedForCurrentActiveView()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var applicationCore = mocks.Stub<ApplicationCore>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var dockingManager = mocks.Stub<IDockingManager>();
            using (var toolWindowViews = new ViewList(dockingManager, ViewLocation.Bottom))
            using (var documentViews = new ViewList(dockingManager, ViewLocation.Bottom))
            {
                var chartView = new ChartView();

                ChartLegendView chartLegendView;
                using (var pluginGui = new ChartingGuiPlugin())
                {
                    gui.Expect(g => g.ApplicationCore).Return(applicationCore).Repeat.Any();
                    gui.Expect(g => g.DocumentViews).Return(documentViews).Repeat.Any();
                    gui.Expect(g => g.ToolWindowViews).Return(toolWindowViews).Repeat.Any();
                    gui.Expect(g => g.MainWindow).Return(mainWindow).Repeat.Any();
                    mainWindow.Expect(w => w.Visible).Return(true).Repeat.Any();

                    mocks.ReplayAll();

                    documentViews.Add(chartView);

                    pluginGui.Gui = gui;
                    pluginGui.Activate();

                    documentViews.ActiveView = chartView;

                    chartLegendView = gui.ToolWindowViews.OfType<ChartLegendView>().FirstOrDefault();

                    Assert.IsNotNull(chartLegendView);
                    Assert.AreSame(chartLegendView, pluginGui.ChartLegendView);
                    Assert.AreEqual(chartView.Data, chartLegendView.Data);
                }
                Assert.True(chartLegendView.IsDisposed);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void TestGetObjectProperties()
        {
            using (var guiPlugin = new ChartingGuiPlugin())
            {
                var propertyInfos = guiPlugin.GetPropertyInfos().ToList();

                var chartPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(IChart));
                var lineChartPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(ILineChartSeries));
                var pointChartPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(IPointChartSeries));
                var areaChartPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(IAreaChartSeries));
                var polygonChartPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(IPolygonChartSeries));
                var barChartPropertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(BarSeries));

                Assert.AreEqual(6, propertyInfos.Count);
                Assert.AreEqual(typeof(ChartProperties), chartPropertyInfo.PropertyType);
                Assert.AreEqual(typeof(LineChartSeriesProperties), lineChartPropertyInfo.PropertyType);
                Assert.AreEqual(typeof(PointChartSeriesProperties), pointChartPropertyInfo.PropertyType);
                Assert.AreEqual(typeof(AreaChartSeriesProperties), areaChartPropertyInfo.PropertyType);
                Assert.AreEqual(typeof(PolygonChartSeriesProperties), polygonChartPropertyInfo.PropertyType);
                Assert.AreEqual(typeof(BarSeriesProperties), barChartPropertyInfo.PropertyType);
            }
        }

        [Test]
        public void TestGetViewInfoObjectsContent()
        {
            using (var guiPlugin = new ChartingGuiPlugin())
            {
                var viewInfos = guiPlugin.GetViewInfoObjects().ToList();

                Assert.NotNull(viewInfos);
                Assert.IsTrue(viewInfos.Any(vi => vi.DataType == typeof(Chart) && vi.ViewType == typeof(ChartView)));
            }
        } 
    }
}