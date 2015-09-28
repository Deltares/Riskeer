using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.TestUtils;
using DeltaShell.Gui.Forms.ViewManager;
using DeltaShell.Plugins.SharpMapGis.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using NUnit.Framework;
using Rhino.Mocks;
using SharpMap;
using SharpMap.Data.Providers;
using SharpMap.Layers;

namespace DeltaShell.Plugins.SharpMapGis.Tests
{
    [TestFixture]
    public class SharpMapGisGuiPluginTest
    {
        private MockRepository mocks;
        [SetUp]
        public void Setup()
        {
            mocks= new MockRepository();    
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
            var sharpMapGisPluginGui = new SharpMapGisGuiPlugin { Gui = gui };

            Expect.Call(application.Project).Return(project).Repeat.Any();
            Expect.Call(application.FileExporters).Return(new List<IFileExporter>()).Repeat.Any();
            gui.Application = application;
            Expect.Call(gui.ToolWindowViews).Return(viewManager).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewManager).Repeat.Any();
            Expect.Call(gui.Plugins).Return(new[] { sharpMapGisPluginGui }).Repeat.Any();

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
        [NUnit.Framework.Category(TestCategory.WindowsForms)]
        public void EventBubbling()
        {
            var mapView = new MapView();

            var gui = (IGui)mocks.Stub(typeof(IGui));

            var guiPlugin = new SharpMapGisGuiPlugin() {Gui = gui};
            Expect.Call(gui.Plugins).Return(new[] { guiPlugin }).Repeat.Any();

            mocks.ReplayAll();
            //         var gisGuiService = (IGisGuiService)mocks.Stub(typeof(IGisGuiService));
            
            var mapLegendView = new MapLegendView(gui);

            var map = new Map(new Size(2, 1));

            var propertyGrid = new PropertyGrid { SelectedObject = gui };
 
            mapView.Map = map;
            mapLegendView.Map = map;

            const string path = @"..\..\..\..\..\test-data\DeltaShell\DeltaShell.Plugins.SharpMapGis.Tests\roads.shp";
            var shapeFile = new ShapeFile(path, false);
            var vectorLayer = new VectorLayer(Path.GetFileNameWithoutExtension(path), shapeFile);
/*
            VectorStyle style = new VectorStyle();
            vectorLayer.Style = style;
*/
            map.Layers.Add(vectorLayer);

            propertyGrid.SelectedObject = new VectorLayerProperties { Data = map.Layers[0] };

            int changeCount = 0;

            ((INotifyPropertyChanged)map).PropertyChanged +=
                delegate
                    {
                        // Assert.AreEqual(e.PropertyName, "Line");
                        changeCount++;
                    };


            var form1 = new Form();
            mapView.Dock = DockStyle.Fill;
            form1.Controls.Add(mapView);

            var form2 = new Form();
            mapLegendView.Dock = DockStyle.Fill;
            form2.Controls.Add(mapLegendView);

            var form3 = new Form();
            propertyGrid.Dock = DockStyle.Fill;
            form3.Controls.Add(propertyGrid);

            WindowsFormsTestHelper.ShowModal(form1);
            WindowsFormsTestHelper.ShowModal(form2);
            WindowsFormsTestHelper.ShowModal(form3);
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
            var sharpMapGisPluginGui = new SharpMapGisGuiPlugin { Gui = gui };
            
            Expect.Call(application.Project).Return(project).Repeat.Any();
            Expect.Call(application.FileExporters).Return(new List<IFileExporter>()).Repeat.Any();

            Expect.Call(gui.ToolWindowViews).Return(viewList).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(viewList).Repeat.Any();
            Expect.Call(gui.Plugins).Return(new[] { sharpMapGisPluginGui }).Repeat.Any();
            gui.Application = application;

            mocks.ReplayAll();
            
            sharpMapGisPluginGui.Activate();
            Assert.IsTrue(sharpMapGisPluginGui.IsActive);
        }
    }
}