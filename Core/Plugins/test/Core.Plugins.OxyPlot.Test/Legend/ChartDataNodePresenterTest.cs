using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartDataNodePresenterTest
    {
        [Test]
        public void UpdateNode_NodeDataIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var nodePresenter = new TestChartDataNodePresenter(null, null);
            var legendTreeView = new LegendTreeView();
            var testNode = new TreeNode(legendTreeView);

            // Call
            TestDelegate test = () => nodePresenter.UpdateNode(null, testNode, null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void UpdateNode_NodeIsNull_ArgumentNullException()
        {
            // Setup
            var nodePresenter = new TestChartDataNodePresenter(null, null);

            // Call
            TestDelegate test = () => nodePresenter.UpdateNode(null,null, new TestPointBasedChartData());

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpdateNode_WithNodeAndData_NodeUpdated(bool visible)
        {
            // Setup
            Bitmap testIcon = Resources.PointsIcon;
            var testText = "SomeText";
            var nodePresenter = new TestChartDataNodePresenter(testText, testIcon);
            var testPointBasedChartData = new TestPointBasedChartData();
            var legendTreeView = new LegendTreeView();
            testPointBasedChartData.IsVisible = visible;
            var testNode = new TreeNode(legendTreeView);

            // Call
            nodePresenter.UpdateNode(null, testNode, testPointBasedChartData);

            // Assert
            Assert.AreEqual(visible, testNode.Checked);
            Assert.AreSame(testText, testNode.Text);
            Assert.AreSame(testIcon, testNode.Image);
            Assert.IsTrue(testNode.ShowCheckBox);
        }

        [Test]
        public void CanDrag_Always_ReturnsMove()
        {
            // Setup
            var nodePresenter = new TestChartDataNodePresenter(null, null);

            // Call
            var operation = nodePresenter.CanDrag(null);

            // Assert
            Assert.AreEqual(DragOperations.Move, operation);
        }

        [Test]
        public void OnNodeChecked_NodeIsNull_ThrowsArgumentNullException()
        {
            // Setup
            var nodePresenter = new TestChartDataNodePresenter(null, null);

            // Call
            TestDelegate test = () => nodePresenter.OnNodeChecked(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void OnNodeChecked_NodeDataIsNull_ThrowsArgumentException()
        {
            // Setup
            var legendTreeView = new LegendTreeView();
            var testNode = new TreeNode(legendTreeView);
            var nodePresenter = new TestChartDataNodePresenter(null, null);

            // Call
            TestDelegate test = () => nodePresenter.OnNodeChecked(testNode);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_WithNodeWithoutParent_DataVisibilityUpdated(bool visibility)
        {
            // Setup
            var nodePresenter = new TestChartDataNodePresenter(null, null);
            var legendTreeView = new LegendTreeView();
            var data = new TestPointBasedChartData();
            var testNode = new TreeNode(legendTreeView)
            {
                Tag = data,
                Checked = visibility,
            };

            // Call
            nodePresenter.OnNodeChecked(testNode);

            // Assert
            Assert.AreEqual(visibility, data.IsVisible);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_WithNodeWithParent_DataVisibilityUpdatedParentNotified(bool visibility)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            var observable = mocks.Stub<Observable>();
            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            observable.Attach(observer);

            var nodePresenter = new TestChartDataNodePresenter(null, null);
            var legendTreeView = new LegendTreeView();
            var data = new TestPointBasedChartData();
            var parent = new TreeNode(legendTreeView)
            {
                Tag = observable
            };
            var testNode = new TreeNode(legendTreeView)
            {
                Tag = data,
                Checked = visibility,
            };
            parent.Nodes.Add(testNode);

            // Call
            nodePresenter.OnNodeChecked(testNode);

            // Assert
            Assert.AreEqual(visibility, data.IsVisible);
            mocks.VerifyAll();
        }
    }

    public class TestChartDataNodePresenter : ChartDataNodePresenter<TestPointBasedChartData> {
        private readonly string text;
        private readonly Bitmap icon;

        public TestChartDataNodePresenter(string text, Bitmap icon)
        {
            this.text = text;
            this.icon = icon;
        }

        protected override string Text
        {
            get
            {
                return text;
            }
        }

        protected override Bitmap Icon
        {
            get
            {
                return icon;
            }
        }
    }

    public class TestPointBasedChartData : PointBasedChartData {
        public TestPointBasedChartData() : base(Enumerable.Empty<Tuple<double, double>>()) {}
    }
}