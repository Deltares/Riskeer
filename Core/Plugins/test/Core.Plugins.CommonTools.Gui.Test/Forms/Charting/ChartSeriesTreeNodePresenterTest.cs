using System;
using System.Drawing;
using Core.Common.Controls.Charting;
using Core.Common.Controls.Charting.Series;
using Core.Common.Controls.TreeView;
using NUnit.Framework;
using Core.Plugins.CommonTools.Gui.Forms.Charting;
using Steema.TeeChart.Styles;

namespace Core.Plugins.CommonTools.Gui.Test.Forms.Charting
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
            var chartSeries = new TestChartSeries(new CustomPoint());
            chartSeries.Title = name;

            // Call
            nodePresenter.OnNodeRenamed(chartSeries, newName);

            // Assert
            Assert.AreSame(newName, chartSeries.Title);
        }

        [Test]
        public void RemoveNodeData_NodeNotInChartSeries_DoesNotThrow()
        {
            // Assert
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var chartSeries = new TestChartSeries(new CustomPoint());
            var chart = new Chart();
            chartSeries.Chart = chart;

            // Precondition
            CollectionAssert.DoesNotContain(chart.Series, chartSeries);

            // Call
            TestDelegate test = () => nodePresenter.RemoveNodeData(null, chartSeries);

            // Assert
            Assert.DoesNotThrow(test);
        }

        [Test]
        public void RemoveNodeData_NodeInChartSeries_ChartSeriesRemoved()
        {
            // Assert
            var nodePresenter = new ChartSeriesTreeNodePresenter();
            var chartSeries = new TestChartSeries(new CustomPoint());
            var chart = new Chart();
            chartSeries.Chart = chart;
            chart.AddChartSeries(chartSeries);

            // Precondition
            CollectionAssert.Contains(chart.Series, chartSeries);

            // Call
            nodePresenter.RemoveNodeData(null, chartSeries);

            // Assert
            CollectionAssert.DoesNotContain(chart.Series, chartSeries);
        }
    }

    public class TestChartSeries : ChartSeries {
        public TestChartSeries(CustomPoint series) : base(series) {}
        public override Color Color { get; set; }
    }
}