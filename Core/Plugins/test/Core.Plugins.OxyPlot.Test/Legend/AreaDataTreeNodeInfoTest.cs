using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class AreaDataTreeNodeInfoTest
    {
        private MockRepository mocks;
        private LegendTreeView legendTreeView;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            legendTreeView = new LegendTreeView();
            info = legendTreeView.TreeViewController.TreeNodeInfos.First(tni => tni.TagType == typeof(AreaData));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(AreaData), info.TagType);
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
            var areaData = mocks.StrictMock<AreaData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var text = info.Text(areaData);

            // Assert
            Assert.AreEqual(Resources.ChartDataNodePresenter_Area_data_label, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var areaData = mocks.StrictMock<AreaData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var image = info.Image(areaData);

            // Assert
            TestHelper.AssertImagesAreEqual(Resources.AreaIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_Always_ReturnsDragOperationsMove()
        {
            // Setup
            var treeNode = new TreeNode();
            var areaData = mocks.StrictMock<AreaData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var dragOperations = info.CanDrag(areaData, treeNode);

            // Assert
            Assert.AreEqual(DragOperations.Move, dragOperations);

            mocks.VerifyAll();
        }

        [Test]
        public void CanCheck_Always_ReturnsTrue()
        {
            // Setup
            var areaData = mocks.StrictMock<AreaData>(Enumerable.Empty<Tuple<double, double>>());

            mocks.ReplayAll();

            // Call
            var canCheck = info.CanCheck(areaData);

            // Assert
            Assert.IsTrue(canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfAreaData(bool isVisible)
        {
            // Setup
            var areaData = mocks.StrictMock<AreaData>(Enumerable.Empty<Tuple<double, double>>());

            areaData.IsVisible = isVisible;

            mocks.ReplayAll();

            // Call
            var canCheck = info.IsChecked(areaData);

            // Assert
            Assert.AreEqual(isVisible, canCheck);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_AreaDataNodeWithoutParent_SetsAreaDataVisibility(bool initialVisibleState)
        {
            // Setup
            var areaData = mocks.StrictMock<AreaData>(Enumerable.Empty<Tuple<double, double>>());
            var areaDataTreeNode = new TreeNode
            {
                Tag = areaData
            };

            mocks.ReplayAll();

            areaData.IsVisible = initialVisibleState;
            areaDataTreeNode.Checked = !initialVisibleState;

            // Call
            info.OnNodeChecked(areaDataTreeNode);

            // Assert
            Assert.AreEqual(!initialVisibleState, areaData.IsVisible);

            mocks.VerifyAll();
        }

        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_AreaDataNodeWithObservableParent_SetsAreaDataVisibilityAndNotifiesParentObservers(bool initialVisibleState)
        {
            // Setup
            var observable = mocks.StrictMock<IObservable>();
            var areaData = mocks.StrictMock<AreaData>(Enumerable.Empty<Tuple<double, double>>());
            var areaDataTreeNode = new TreeNode
            {
                Tag = areaData
            };
            var parentNode = new TreeNode
            {
                Tag = observable
            };

            observable.Expect(o => o.NotifyObservers());

            mocks.ReplayAll();

            areaData.IsVisible = initialVisibleState;
            parentNode.Nodes.Add(areaDataTreeNode);
            areaDataTreeNode.Checked = !initialVisibleState;

            // Call
            info.OnNodeChecked(areaDataTreeNode);

            // Assert
            Assert.AreEqual(!initialVisibleState, areaData.IsVisible);

            mocks.VerifyAll();
        }
    }
}