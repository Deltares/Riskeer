using System.Collections.Generic;
using System.Windows.Forms;
using Application.Ringtoets.Forms.ViewManager;
using Core.Common.BaseDelftTools;
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
        private MockRepository mocks;

        [SetUp]
        public void Setup()
        {
            mocks = new MockRepository();
        }

        /// <summary>
        /// Returns a contextmenu in case the data is a vectorlayer.
        /// </summary>
        [Test]
        public void GetContextMenuTest()
        {
            var gui = mocks.Stub<IGui>();
            var viewManager = mocks.Stub<IViewList>();
            var application = mocks.Stub<IApplication>();
            var project = new Project();
            var sharpMapGisPluginGui = new SharpMapGisGuiPlugin
            {
                Gui = gui
            };

            Expect.Call(application.Project).Return(project).Repeat.Any();
            Expect.Call(application.FileExporters).Return(new List<IFileExporter>()).Repeat.Any();
            gui.Application = application;
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

            sharpMapGisPluginGui.Gui = gui;

            sharpMapGisPluginGui.InitializeMapLegend();

            treeView.Parent = sharpMapGisPluginGui.MapLegendView;
            Assert.IsNotNull(sharpMapGisPluginGui.GetContextMenu(layerNode, vectorLayer));

            mocks.VerifyAll();
        }

        [Test]
        public void Activate()
        {
            //just a check to make sure activation does not crash..can very well because of the ammount of depencies
            var gui = mocks.Stub<IGui>();
            var dockingManager = mocks.Stub<IDockingManager>();
            var viewList = new ViewList(dockingManager, null);
            var application = mocks.Stub<IApplication>();
            var project = new Project();
            var sharpMapGisPluginGui = new SharpMapGisGuiPlugin
            {
                Gui = gui
            };

            Expect.Call(application.Project).Return(project).Repeat.Any();
            Expect.Call(application.FileExporters).Return(new List<IFileExporter>()).Repeat.Any();

            Expect.Call(gui.ToolWindowViews).Return(viewList).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewList).Repeat.Any();
            Expect.Call(gui.Plugins).Return(new[]
            {
                sharpMapGisPluginGui
            }).Repeat.Any();
            gui.Application = application;

            mocks.ReplayAll();

            sharpMapGisPluginGui.Activate();
            Assert.IsTrue(sharpMapGisPluginGui.IsActive);
        }
    }
}