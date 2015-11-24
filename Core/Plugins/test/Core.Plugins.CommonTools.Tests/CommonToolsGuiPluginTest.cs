using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.MainWindow;
using Core.Plugins.CommonTools.Gui;
using Core.Plugins.CommonTools.Gui.Forms.Charting;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Tests
{
    [TestFixture]
    public class CommonToolsGuiPluginTest
    {
        private static readonly MockRepository mocks = new MockRepository();

        [Test]
        public void ChartLegendViewIsUpdatedForCurrentActiveView()
        {
            var gui = mocks.Stub<IGui>();
            gui.ApplicationCore = mocks.Stub<ApplicationCore>();
            var pluginGui = new CommonToolsGuiPlugin();
            var mainWindow = mocks.Stub<IMainWindow>();
            var toolWindowViews = new TestViewList();
            var documentViews = new TestViewList();
            var chartView = new ChartView();

            gui.Expect(g => g.DocumentViews).Return(documentViews).Repeat.Any();
            gui.Expect(g => g.ToolWindowViews).Return(toolWindowViews).Repeat.Any();
            gui.Expect(g => g.MainWindow).Return(mainWindow).Repeat.Any();
            mainWindow.Expect(w => w.Visible).Return(true).Repeat.Any();

            mocks.ReplayAll();

            documentViews.Add(chartView);

            pluginGui.Gui = gui;
            pluginGui.Activate();

            documentViews.ActiveView = chartView;

            var chartLegendView = gui.ToolWindowViews.OfType<ChartLegendView>().FirstOrDefault();

            Assert.IsNotNull(chartLegendView);
            Assert.AreEqual(chartView.Data, chartLegendView.Data);

            mocks.VerifyAll();
        }

        [Test]
        public void TestGetObjectProperties()
        {
            var guiPlugin = new CommonToolsGuiPlugin();
            var propertyInfos = guiPlugin.GetPropertyInfos().ToList();

            var propertyInfo = propertyInfos.First(pi => pi.ObjectType == typeof(IChart));
            Assert.AreEqual(typeof(ChartProperties), propertyInfo.PropertyType);
        }
    }
}