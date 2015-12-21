using System;
using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Plugins.Charting.Forms;
using Core.Plugins.Charting.Properties;
using NUnit.Framework;
using Rhino.Mocks;
using Steema.TeeChart.Styles;

namespace Core.Plugins.Charting.Test.Forms
{
    [TestFixture]
    public class ChartSeriesTreeNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_Always_PropertiesSet()
        {
            // Call
            var nodePresenter = new ChartSeriesTreeNodePresenter();

            // Assert
            Assert.IsInstanceOf<TreeViewNodePresenterBase<ChartSeries>>(nodePresenter);
        }

        [Test]
        public void CanRenameNode_Always_ReturnsTrue()
        {
            // Setup
            var nodePresenter = new ChartSeriesTreeNodePresenter();

            // Call
            var result = nodePresenter.CanRenameNode(null);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void OnNodeRenamed_NoChartSeries_DoesNotThrow()
        {
            // Setup
            var nodePresenter = new ChartSeriesTreeNodePresenter();

            // Call
            TestDelegate testDelegate = () => nodePresenter.OnNodeRenamed(null, String.Empty);

            // Assert
            Assert.DoesNotThrow(testDelegate);
        }

        [Test]
        public void OnNodeRenamed_NewNodeName_ChartSeriesTitleSetToNodeName()
        {
            // Setup
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var name = "<some name>";
            var newName = "<some new name>";
            var chartSeries = new TestChartSeries();
            chartSeries.Title = name;

            // Call
            nodePresenter.OnNodeRenamed(chartSeries, newName);

            // Assert
            Assert.AreEqual(newName, chartSeries.Title);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void UpdateNode_NoKnownSeriesType_PropertiesSetImageNull(bool visible)
        {
            // Setup
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var node = new TreeNode(null);
            var testTitle = "<some title>";
            var chartSeries = new TestChartSeries
            {
                Title = testTitle,
                Visible = visible
            };

            // Call
            nodePresenter.UpdateNode(null, node, chartSeries);

            // Assert
            Assert.AreEqual(testTitle, node.Text);
            Assert.AreEqual(visible, node.Checked);
            Assert.IsTrue(node.ShowCheckBox);
            Assert.IsNull(node.Image);
            Assert.AreSame(chartSeries, node.Tag);
        }

        [Test]
        [TestCase(typeof(AreaChartSeries), "Area")]
        [TestCase(typeof(LineChartSeries), "Line")]
        [TestCase(typeof(PointChartSeries), "Points")]
        [TestCase(typeof(PolygonChartSeries), "Polygon")]
        [TestCase(typeof(BarSeries), "Bars")]
        public void UpdateNode_KnownSeriesType_NodeImageSet(Type chartType, string resourceName)
        {
            // Setup
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var node = new TreeNode(null);
            var chartSeries = Activator.CreateInstance(chartType);

            // Call
            nodePresenter.UpdateNode(null, node, chartSeries);
            Image expectedImage = Resources.ResourceManager.GetObject(resourceName) as Image;
            // Assert
            TestHelper.AssertImagesAreEqual(expectedImage, node.Image);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void OnNodeChecked_NodeChecked_VisibleTrue(bool visible)
        {
            // Setup
            var mocks = new MockRepository();
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var node = mocks.StrictMock<ITreeNode>();
            var chartSeries = new TestChartSeries();
            node.Expect(n => n.Tag).Return(chartSeries);
            node.Expect(n => n.Checked).Return(visible);

            mocks.ReplayAll();

            // Call
            nodePresenter.OnNodeChecked(node);

            // Assert
            Assert.AreEqual(visible, chartSeries.Visible);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_Always_ReturnMove()
        {
            // Setup
            var nodePresenter = new ChartSeriesTreeNodePresenter();

            // Call
            var result = nodePresenter.CanDrag(null);

            // Assert
            Assert.AreEqual(DragOperations.Move, result);
        }

        [Test]
        public void RemoveNodeData_NodeNotInChartSeries_ReturnsFalse()
        {
            // Assert
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var chartSeries = new TestChartSeries();
            var chart = new Chart();
            chartSeries.Chart = chart;

            // Precondition
            CollectionAssert.DoesNotContain(chart.Series, chartSeries);

            // Call
            var result = nodePresenter.RemoveNodeData(null, chartSeries);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void RemoveNodeData_NodeInChartSeries_ChartSeriesRemoved()
        {
            // Assert
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var chartSeries = new TestChartSeries();
            var chart = new Chart();
            chartSeries.Chart = chart;
            chart.AddChartSeries(chartSeries);

            // Precondition
            CollectionAssert.Contains(chart.Series, chartSeries);

            // Call
            var result = nodePresenter.RemoveNodeData(null, chartSeries);

            // Assert
            CollectionAssert.DoesNotContain(chart.Series, chartSeries);
            Assert.IsTrue(result);
        }
    }

    public class TestChartSeries : ChartSeries {
        public TestChartSeries() : base(new CustomPoint()) { }
        public override Color Color { get; set; }
    }
}