using System.Collections;
using System.Drawing;
using System.IO;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.GIS.SharpMap.Api.Collections;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Data.Providers;
using Core.GIS.SharpMap.Layers;
using Core.GIS.SharpMap.Rendering.Thematics;
using Core.GIS.SharpMap.Styles;
using Core.Plugins.SharpMapGis.Gui.Forms.MapLegendView;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.SharpMapGis.Test.Forms
{
    [TestFixture]
    public class MapLayerTreeViewNodePresenterTest
    {
        private static readonly MockRepository mocks = new MockRepository();
        private static ITreeNodePresenter mapLayerNodePresenter;

        private ITreeView treeView;

        [SetUp]
        public void Setup()
        {
            mapLayerNodePresenter = new MapLayerTreeViewNodePresenter(new MapLegendView(null));
            treeView = mocks.Stub<ITreeView>();
            mapLayerNodePresenter.TreeView = treeView;
        }

        [Test]
        public void LoadingDataFromShapeFileAndCreateNodeOnMapLayer()
        {
            var vectorLayer = GetRiverLayer();
            var node = mocks.Stub<ITreeNode>();

            mocks.ReplayAll();

            mapLayerNodePresenter.UpdateNode(null, node, vectorLayer);

            Assert.AreEqual(vectorLayer, node.Tag);
            Assert.AreEqual(vectorLayer.Name, node.Text);
            Assert.AreEqual(true, node.ShowCheckBox);

            mocks.VerifyAll();
        }

        [Test]
        public void LayerInImmutableGroupLayerCanNotBeDragged()
        {
            var groupLayer = new GroupLayer();
            var vectorLayer = GetRiverLayer();
            groupLayer.Layers.Add(vectorLayer);

            groupLayer.LayersReadOnly = true;

            var node = mocks.Stub<ITreeNode>();
            var parentNode = mocks.Stub<ITreeNode>();
            parentNode.Tag = groupLayer;

            node.Expect(c => c.Parent).Return(parentNode);
            treeView.Expect(tv => tv.GetNodeByTag(null)).Return(node).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();

            // No drag of layers
            Assert.AreEqual(DragOperations.None, mapLayerNodePresenter.CanDrag(vectorLayer));

            mocks.VerifyAll();
        }

        [Test]
        public void CanNotDropIntoImmutableGroupLayer()
        {
            var vectorLayer = GetRiverLayer();
            var groupLayer = new GroupLayer
            {
                LayersReadOnly = true
            };
            var sourceNode = mocks.Stub<ITreeNode>();
            var targetNode = mocks.Stub<ITreeNode>();

            sourceNode.Tag = vectorLayer;
            targetNode.Tag = groupLayer;

            mocks.ReplayAll();

            // No drop into layers
            Assert.AreEqual(DragOperations.None, mapLayerNodePresenter.CanDrop(vectorLayer, sourceNode, targetNode, DragOperations.Move));

            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_TreeViewHasSorter_ReturnFalse()
        {
            // Setup
            var vectorLayer = GetRiverLayer();
            var sourceNode = mocks.Stub<ITreeNode>();
            var targetNode = mocks.Stub<ITreeNode>();

            mapLayerNodePresenter.TreeView.TreeViewNodeSorter = mocks.Stub<IComparer>();

            mocks.ReplayAll();

            // Precondition
            Assert.IsNotNull(mapLayerNodePresenter.TreeView.TreeViewNodeSorter);

            // Call
            var insertionAllowed = mapLayerNodePresenter.CanInsert(vectorLayer, sourceNode, targetNode);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanInsert_TreeViewDoesNotHaveSorter_ReturnTrue()
        {
            // Setup
            var vectorLayer = GetRiverLayer();
            var sourceNode = mocks.Stub<ITreeNode>();
            var targetNode = mocks.Stub<ITreeNode>();

            mocks.ReplayAll();

            // Precondition
            Assert.IsNull(mapLayerNodePresenter.TreeView.TreeViewNodeSorter);

            // Call
            var insertionAllowed = mapLayerNodePresenter.CanInsert(vectorLayer, sourceNode, targetNode);

            // Assert
            Assert.IsTrue(insertionAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void TestCanRemove()
        {
            // Create a group layer
            var groupLayer = mocks.Stub<GroupLayer>();
            var groupLayerTreeNode = mocks.Stub<ITreeNode>();
            groupLayerTreeNode.Tag = groupLayer;

            // Create a nested layer for the group layer
            var nestedLayer1 = mocks.Stub<Layer>();
            var nestedLayerTreeNode1 = mocks.Stub<ITreeNode>();
            nestedLayer1.CanBeRemovedByUser = true;
            nestedLayerTreeNode1.Tag = nestedLayer1;
            groupLayer.Layers = new EventedList<ILayer>
            {
                nestedLayer1
            };

            // Create a nested group layer
            var nestedGroupLayer = mocks.Stub<GroupLayer>();
            var nestedGroupLayerTreeNode = mocks.Stub<ITreeNode>();
            nestedGroupLayerTreeNode.Tag = nestedGroupLayer;
            groupLayer.Layers.Add(nestedGroupLayer);

            // Create a nested layer for the nested group layer
            var nestedLayer2 = mocks.Stub<Layer>();
            var nestedLayerTreeNode2 = mocks.Stub<ITreeNode>();
            nestedLayer2.CanBeRemovedByUser = true;
            nestedLayerTreeNode2.Tag = nestedLayer2;
            nestedGroupLayer.Layers = new EventedList<ILayer>
            {
                nestedLayer2
            };

            nestedLayerTreeNode1.Expect(c => c.Parent).Return(groupLayerTreeNode).Repeat.Any();
            treeView.Expect(tv => tv.GetNodeByTag(nestedLayer1)).Return(nestedLayerTreeNode1).Repeat.Any();

            nestedLayerTreeNode2.Expect(c => c.Parent).Return(nestedGroupLayerTreeNode).Repeat.Any();
            treeView.Expect(tv => tv.GetNodeByTag(nestedLayer2)).Return(nestedLayerTreeNode2).Repeat.Any();

            mocks.ReplayAll();

            /* Situation:
             * 
             *  groupLayer                 groupLayerTreeNode                  
             *  |                          |
             *  |-- nestedLayer1           |-- nestedLayerTreeNode1             
             *  |                          |
             *  |-- nestedGroupLayer       |-- nestedGroupLayerTreeNode
             *      |                          |
             *      |-- nestedLayer2           |-- nestedLayerTreeNode2
             *      
             */

            Assert.IsTrue(mapLayerNodePresenter.CanRemove(groupLayer, nestedLayer1));
            Assert.IsTrue(mapLayerNodePresenter.CanRemove(nestedGroupLayer, nestedLayer2));

            groupLayer.LayersReadOnly = true;
            Assert.IsFalse(mapLayerNodePresenter.CanRemove(groupLayer, nestedLayer1));
            Assert.IsTrue(mapLayerNodePresenter.CanRemove(nestedGroupLayer, nestedLayer2));

            nestedGroupLayer.LayersReadOnly = true;
            Assert.IsFalse(mapLayerNodePresenter.CanRemove(groupLayer, nestedLayer1));
            Assert.IsFalse(mapLayerNodePresenter.CanRemove(nestedGroupLayer, nestedLayer2));

            mocks.VerifyAll();
        }

        [Test]
        public void TestGetChildNodeObjects()
        {
            var colorBlend = new ColorBlend(new[]
            {
                Color.Black,
                Color.White
            }, new[]
            {
                0.0f,
                1.0f
            });
            var gradientTheme = new GradientTheme("aa", 0, 3, new VectorStyle(), new VectorStyle(), colorBlend,
                                                  colorBlend, colorBlend, 3);

            // VectorLayer:
            var vectorLayer = new VectorLayer
            {
                Theme = gradientTheme
            };

            var result = mapLayerNodePresenter.GetChildNodeObjects(vectorLayer);
            var enumerator = result.GetEnumerator();

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.Current is GradientThemeItem);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.Current is GradientThemeItem);

            Assert.IsTrue(enumerator.MoveNext());
            Assert.IsTrue(enumerator.Current is GradientThemeItem);
        }

        private static VectorLayer GetRiverLayer()
        {
            var path = TestHelper.GetDataDir() + @"\rivers.shp";
            var shapeFile = new ShapeFile(path);

            return new VectorLayer(Path.GetFileName(path), shapeFile);
        }
    }
}