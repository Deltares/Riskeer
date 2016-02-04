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
    public class LineDataTreeNodeInfoTest
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

            info = treeNodeInfos.First(tni => tni.TagType == typeof(LineData));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(LineData), info.TagType);
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
            var lineData = mocks.StrictMock<LineData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var text = info.Text(lineData);

            // Assert
            Assert.AreEqual(Resources.ChartData_Line_data_label, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var lineData = mocks.StrictMock<LineData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var image = info.Image(lineData);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.LineIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_Always_ReturnsDragOperationsMove()
        {
            // Setup
            var treeNode = new TreeNode();
            var lineData = mocks.StrictMock<LineData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var dragOperations = info.CanDrag(lineData, treeNode);

            // Assert
            Assert.AreEqual(DragOperations.Move, dragOperations);

            mocks.VerifyAll();
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var lineData = mocks.StrictMock<LineData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var canCheck = info.CanCheck(lineData);

            // Assert
            Assert.IsTrue(canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfLineData(bool isVisible)
        {
            // Setup
            var lineData = mocks.StrictMock<LineData>(Enumerable.Empty<Tuple<double, double>>());

            lineData.IsVisible = isVisible;

            mocks.ReplayAll();

            // Call
            var canCheck = info.IsChecked(lineData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void LineDataNodeWithoutParent_SetsLineDataVisibility(bool initialVisibleState)
        {
            // Setup
            var lineData = mocks.StrictMock<LineData>(Enumerable.Empty<Tuple<double, double>>());
            var lineDataTreeNode = new TreeNode
            {
                Tag = lineData
            };

            mocks.ReplayAll();

            lineData.IsVisible = initialVisibleState;
            lineDataTreeNode.Checked = !initialVisibleState;

            // Call
            info.OnNodeChecked(lineDataTreeNode);

            // Assert
            Assert.AreEqual(!initialVisibleState, lineData.IsVisible);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_LineDataNodeWithObservableParent_SetsLineDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var observable = mocks.StrictMock<IObservable>();
            var lineData = mocks.StrictMock<LineData>(Enumerable.Empty<Tuple<double, double>>());
            var lineDataTreeNode = new TreeNode
            {
                Tag = lineData
            };
            var parentNode = new TreeNode
            {
                Tag = observable
            };

            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            lineData.IsVisible = initialVisibleState;
            parentNode.Nodes.Add(lineDataTreeNode);
            lineDataTreeNode.Checked = !initialVisibleState;

            // Call
            info.OnNodeChecked(lineDataTreeNode);

            // Assert
            Assert.AreEqual(!initialVisibleState, lineData.IsVisible);

            mocks.VerifyAll();
        }
    }
}