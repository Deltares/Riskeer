using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class PointDataTreeNodeInfoTest
    {
        private MockRepository mocks;
        private LegendView legendView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            legendView = new LegendView();

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(legendView, "treeViewControl");
            var treeNodeInfos = TypeUtils.GetField<IEnumerable<TreeNodeInfo>>(treeViewControl, "treeNodeInfos");

            info = treeNodeInfos.First(tni => tni.TagType == typeof(PointData));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PointData), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.ContextMenuStrip);
            Assert.IsNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsTextFromResource()
        {
            // Setup
            var pointData = mocks.StrictMock<PointData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var text = info.Text(pointData);

            // Assert
            Assert.AreEqual(Resources.ChartData_Point_data_label, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var pointData = mocks.StrictMock<PointData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var image = info.Image(pointData);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.PointsIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_Always_ReturnsDragOperationsMove()
        {
            // Setup
            var treeNode = new TreeNode();
            var pointData = mocks.StrictMock<PointData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var dragOperations = info.CanDrag(pointData, treeNode);

            // Assert
            Assert.AreEqual(DragOperations.Move, dragOperations);

            mocks.VerifyAll();
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var pointData = mocks.StrictMock<PointData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var canCheck = info.CanCheck(pointData);

            // Assert
            Assert.IsTrue(canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfPointsData(bool isVisible)
        {
            // Setup
            var pointData = mocks.StrictMock<PointData>(Enumerable.Empty<Tuple<double, double>>());

            pointData.IsVisible = isVisible;

            mocks.ReplayAll();

            // Call
            var canCheck = info.IsChecked(pointData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void PointDataNodeWithoutParent_SetsPointDataVisibility(bool initialVisibleState)
        {
            // Setup
            var pointData = mocks.StrictMock<PointData>(Enumerable.Empty<Tuple<double, double>>());
            var pointDataTreeNode = new TreeNode
            {
                Tag = pointData
            };

            mocks.ReplayAll();

            pointData.IsVisible = initialVisibleState;
            pointDataTreeNode.Checked = !initialVisibleState;

            // Call
            info.OnNodeChecked(pointDataTreeNode);

            // Assert
            Assert.AreEqual(!initialVisibleState, pointData.IsVisible);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_PointDataNodeWithObservableParent_SetsPointDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var observable = mocks.StrictMock<IObservable>();
            var pointData = mocks.StrictMock<PointData>(Enumerable.Empty<Tuple<double, double>>());
            var pointDataTreeNode = new TreeNode
            {
                Tag = pointData
            };
            var parentNode = new TreeNode
            {
                Tag = observable
            };

            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            pointData.IsVisible = initialVisibleState;
            parentNode.Nodes.Add(pointDataTreeNode);
            pointDataTreeNode.Checked = !initialVisibleState;

            // Call
            info.OnNodeChecked(pointDataTreeNode);

            // Assert
            Assert.AreEqual(!initialVisibleState, pointData.IsVisible);

            mocks.VerifyAll();
        }
    }
}