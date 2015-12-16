using System.Linq;
using Core.Common.Base.Plugin;
using Core.Common.Controls.Swf;
using Core.Common.Controls.Swf.Charting;
using Core.Common.Gui;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Utils;
using Core.Plugins.CommonTools.Gui.Forms;
using Core.Plugins.CommonTools.Gui.Forms.Charting;
using Core.Plugins.CommonTools.Gui.Property.Charting;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.CommonTools.Gui.Test
{
    [TestFixture]
    public class CommonToolsGuiPluginTest
    {
        private static readonly MockRepository mocks = new MockRepository();

        [Test]
        public void ChartLegendViewIsUpdatedForCurrentActiveView()
        {
            var gui = mocks.Stub<IGui>();
            var applicationCore = mocks.Stub<ApplicationCore>();
            var pluginGui = new CommonToolsGuiPlugin();
            var mainWindow = mocks.Stub<IMainWindow>();
            var toolWindowViews = new TestViewList();
            var documentViews = new TestViewList();
            var chartView = new ChartView();

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

        [Test]
        public void TestGetViewInfoObjectsContent()
        {
            var guiPlugin = new CommonToolsGuiPlugin();
            var viewInfos = guiPlugin.GetViewInfoObjects().ToList();

            Assert.NotNull(viewInfos);
            Assert.IsTrue(viewInfos.Any(vi => vi.DataType == typeof(RichTextFile) && vi.ViewType == typeof(RichTextView)));
            Assert.IsTrue(viewInfos.Any(vi => vi.DataType == typeof(Url) && vi.ViewType == typeof(HtmlPageView)));
            Assert.IsTrue(viewInfos.Any(vi => vi.DataType == typeof(Chart) && vi.ViewType == typeof(ChartView)));
        }
    }
}