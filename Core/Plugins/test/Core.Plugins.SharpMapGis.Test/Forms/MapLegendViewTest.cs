using System;
using System.IO;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Gui;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Map;
using Core.Plugins.SharpMapGis.Gui;
using Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using Core.Plugins.SharpMapGis.Gui.Properties;
using NUnit.Framework;
using Rhino.Mocks;
using TreeNode = Core.Common.Controls.TreeView.TreeNode;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.SharpMapGis.Test.Forms
{
    [TestFixture]
    public class MapLegendViewTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Init()
        {
            var map = new Map();
            Form form = CreateFormWithMapLegendView(map);
            form.Dispose();
        }

        [Test]
        public void RemoveFromMapLegendViewDisposesLayerResources()
        {
            var mapLegendView = CreateMapLegendView();
            var map = new Map();
            mapLegendView.Map = map;

            string path = TestHelper.GetDataDir() + "rivers.shp";

            var shapeFile = mocks.StrictMock<ShapeFile>(path);

            mocks.ReplayAll();

            shapeFile.Expect(sf => sf.GetExtents()).Return(null).Repeat.Any();
            shapeFile.Expect(sf => sf.FeaturesChanged += null).IgnoreArguments();
            shapeFile.Expect(sf => sf.FeaturesChanged -= null).IgnoreArguments().Repeat.Twice();
            shapeFile.Expect(sf => sf.CoordinateSystemChanged += null).IgnoreArguments();
            shapeFile.Expect(sf => sf.CoordinateSystemChanged -= null).IgnoreArguments().Repeat.Twice();
            shapeFile.Expect(sf => sf.AddNewFeatureFromGeometryDelegate = null).IgnoreArguments();
            shapeFile.Expect(sf => sf.AddNewFeatureFromGeometryDelegate).Return(null);
            shapeFile.Expect(sf => sf.Dispose()); //this is what we are checking
            shapeFile.Replay();

            var vectorLayer = new VectorLayer(Path.GetFileName(path), shapeFile);
            map.Layers.Add(vectorLayer);

            TypeUtils.CallPrivateMethod<MapLegendView>(mapLegendView, "RemoveLayer", vectorLayer);

            mocks.VerifyAll();
        }

        [Test]
        public void HideAllLayersButThisShouldWorkInMap()
        {
            //should work for layers on Map level
            var map = new Map();
            var layer1 = new VectorLayer();
            var layer2 = new VectorLayer();
            map.Layers.Add(layer1);
            map.Layers.Add(layer2);

            MapLegendView.HideAllLayersButThisOne(layer1, map);

            Assert.IsTrue(layer1.Visible);
            Assert.IsFalse(layer2.Visible);
        }

        [Test]
        public void HideAllLayersButThisShouldWorkInGroupLayer()
        {
            var map = new Map();
            var layer1 = new VectorLayer();
            var groupLayer = new GroupLayer();
            var nestedLayer1 = new VectorLayer();
            var nestedLayer2 = new VectorLayer();
            groupLayer.Layers.Add(nestedLayer1);
            groupLayer.Layers.Add(nestedLayer2);

            map.Layers.Add(layer1);
            map.Layers.Add(groupLayer);

            MapLegendView.HideAllLayersButThisOne(nestedLayer1, map);
            Assert.IsTrue(layer1.Visible);
            Assert.IsTrue(groupLayer.Visible);
            //only the other layer in the group should be turned off
            Assert.IsFalse(nestedLayer2.Visible);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException), ExpectedMessage = "Layer kees not part of map map1")]
        public void HideAllLayersButThisOneThrowsExceptionIfLayerIsNotInMap()
        {
            var map = new Map
            {
                Name = "map1"
            };
            var layer1 = new VectorLayer
            {
                Name = "kees"
            };
            MapLegendView.HideAllLayersButThisOne(layer1, map);
        }

        [Test]
        public void GetContextMenu_ForMap_ReturnContextMenuStripWithSixItems()
        {
            // Setup
            var mapLegendView = CreateMapLegendView();
            var mapMock = mocks.StrictMock<Map>();

            mocks.Replay(mapMock);

            // Call
            var contextMenu = mapLegendView.GetContextMenu(mapMock);

            // Assert
            Assert.AreEqual(6, contextMenu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, "Voeg kaartlaag toe", null, null);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 1, "Toevoegen groeplaag", null, null);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, "Zoom naar alles", null, Resources.MapZoomToExtentsImage);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 4, "Zoomen tot kaartbereik", null, null);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 5, "Veranderen kaartcoördinatenstelsel...", null, Resources.GlobePencil);

            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[2]);
        }

        [Test]
        public void GetContextMenu_ForLayer_ReturnContextMenuStripWithTwelveItems()
        {
            // Setup
            var mapLegendView = CreateMapLegendView();
            var layerMock = mocks.StrictMock<Layer>();

            mocks.Replay(layerMock);

            // Call
            var contextMenu = mapLegendView.GetContextMenu(layerMock);

            // Assert
            Assert.AreEqual(12, contextMenu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, "&Verwijderen", null, Resources.DeleteHS);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, "&Hernoemen", null, null);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 5, "Volgorde", null, Resources.LayersStack);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 6, "&Zoomen naar omvang", null, Resources.MapZoomToExtentsImage);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 7, "Synchroniseer zoomniveau met kaart", null, Resources.LayersUngroup);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 9, "Labels weergeven", null, null);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 10, "Weergeven in de legenda", null, null);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 11, "Alle andere lagen verbergen", null, null);

            var toolStripDropDown = contextMenu.Items[5] as ToolStripDropDownItem;

            Assert.NotNull(toolStripDropDown);
            TestHelper.AssertDropDownItemContainsItem(toolStripDropDown, 0, "Op de voorgrond plaatsen", null, Resources.LayersStackArrange);
            TestHelper.AssertDropDownItemContainsItem(toolStripDropDown, 1, "Naar de achtergrond verplaatsen", null, Resources.LayersStackArrangeBack);
            TestHelper.AssertDropDownItemContainsItem(toolStripDropDown, 3, "Naar voren halen", null, null);
            TestHelper.AssertDropDownItemContainsItem(toolStripDropDown, 4, "Naar achteren sturen", null, null);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { contextMenu.Items[1], contextMenu.Items[4], contextMenu.Items[8], toolStripDropDown.DropDownItems[2] }, typeof(ToolStripSeparator));
        }

        [Test]
        public void GetContextMenu_ForLayerNullLayerSelected_ShowAttributeTableDisabled()
        {
            // Setup
            var layerMock = mocks.StrictMock<Layer>();
            var mapLegendView = CreateMapLegendView();

            // Call
            var contextMenu = mapLegendView.GetContextMenu(layerMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, "Attributentabel openen", null, Resources.Table, false);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_ForLayerShowAttributeTableSetOnSelectedNode_ShowAttributeTableItemSetToCorrectState(bool showAttributeTableEnabled)
        {
            // Setup
            var layerMock = mocks.Stub<Layer>();
            var mapLegendView = CreateMapLegendView();

            layerMock.ShowAttributeTable = showAttributeTableEnabled;

            var treeView = (TreeView) mapLegendView.Controls.Find("TreeView", true)[0];
            var selectedNode = new TreeNode(treeView);
            var parentNode = new TreeNode(treeView);
            parentNode.Nodes.Add(selectedNode);
            selectedNode.Tag = layerMock;
            treeView.Nodes.Add(parentNode);
            treeView.SelectedNode = selectedNode;

            // Call
            var contextMenu = mapLegendView.GetContextMenu(layerMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, "Attributentabel openen", null, Resources.Table, showAttributeTableEnabled);

            mocks.VerifyAll();
        }

        private MapLegendView CreateMapLegendView()
        {
            var project = new Project();
            var gui = mocks.Stub<IGui>();
            var applicationCore = new ApplicationCore();

            //create stubbed version of plugingui
            SharpMapGisGuiPlugin guiPlugin = new SharpMapGisGuiPlugin();

            Expect.Call(gui.ApplicationCore).Return(applicationCore).Repeat.Any();
            Expect.Call(gui.Plugins).Return(new[]
            {
                guiPlugin
            }).Repeat.Any();
            Expect.Call(gui.ToolWindowViews).Return(mocks.Stub<IViewList>()).Repeat.Any();
            Expect.Call(gui.DocumentViews).Return(mocks.Stub<IViewList>()).Repeat.Any();

            gui.Project = project;

            mocks.ReplayAll();

            guiPlugin.Gui = gui;

            guiPlugin.Activate();

            var mapLegendView = guiPlugin.MapLegendView;
            mapLegendView.Dock = DockStyle.Fill;

            return mapLegendView;
        }

        private Form CreateFormWithMapLegendView(Map map)
        {
            var mapLegendView = CreateMapLegendView();
            mapLegendView.Map = map;
            mapLegendView.Dock = DockStyle.Fill;
            Form form = new Form();
            form.Controls.Add(mapLegendView);
            return form;
        }
    }
}