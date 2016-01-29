using System.Collections.Generic;
using System.Linq;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class LegendTreeViewTest
    {
        [Test]
        public void DefaultConstructor_SetsFourNodePresenters()
        {
            // Call
            var view = new LegendTreeView();

            // Assert
            Assert.AreEqual(4, view.TreeViewController.TreeNodeInfos.Count());
            Assert.AreEqual(typeof(PointData), view.TreeViewController.TreeNodeInfos.ElementAt(0).TagType);
            Assert.AreEqual(typeof(LineData), view.TreeViewController.TreeNodeInfos.ElementAt(1).TagType);
            Assert.AreEqual(typeof(AreaData), view.TreeViewController.TreeNodeInfos.ElementAt(2).TagType);
            Assert.AreEqual(typeof(ChartDataCollection), view.TreeViewController.TreeNodeInfos.ElementAt(3).TagType);
            Assert.IsNull(view.ChartData);
        }

        [Test]
        public void Dispose_DataSet_DataSetToNull()
        {
            // Setup
            var tree = new LegendTreeView();
            var data = new TestChartData();

            tree.ChartData = data;

            // Call
            tree.Dispose();

            // Assert
            Assert.IsNull(tree.ChartData);
        }

        private class TestChartData : ChartDataCollection
        {
            public TestChartData() : base(new List<ChartData>()) {}
        }
    }
}