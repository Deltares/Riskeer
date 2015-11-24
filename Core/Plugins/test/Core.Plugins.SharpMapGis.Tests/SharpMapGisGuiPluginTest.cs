using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui;
using Core.GIS.SharpMap.Layers;
using Core.Plugins.SharpMapGis.Gui;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.SharpMapGis.Tests
{
    [TestFixture]
    public class SharpMapGisGuiPluginTest
    {
        /// <summary>
        /// Returns a contextmenu in case the data is a vectorlayer.
        /// </summary>
        [Test]
        public void GetContextMenuTest()
        {
            var mocks = new MockRepository();
            var gui = mocks.Stub<IGui>();
            var viewManager = mocks.Stub<IViewList>();
            var project = new Project();
            var sharpMapGisPluginGui = new SharpMapGisGuiPlugin
            {
                Gui = gui
            };

            Expect.Call(gui.ToolWindowViews).Return(viewManager).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewManager).Repeat.Any();
            Expect.Call(gui.Plugins).Return(new[]
            {
                sharpMapGisPluginGui
            }).Repeat.Any();

            var layerNode = new TreeNode();
            var treeView = new TreeView();
            treeView.Nodes.Add(layerNode);
            var vectorLayer = new VectorLayer("testLayer");

            mocks.ReplayAll();

            var application = new RingtoetsApplication { Project = project };

            gui.Application = application;

            sharpMapGisPluginGui.Gui = gui;

            sharpMapGisPluginGui.InitializeMapLegend();

            treeView.Parent = sharpMapGisPluginGui.MapLegendView;
            Assert.IsNotNull(sharpMapGisPluginGui.GetContextMenu(layerNode, vectorLayer));

            mocks.VerifyAll();
        }
    }
}