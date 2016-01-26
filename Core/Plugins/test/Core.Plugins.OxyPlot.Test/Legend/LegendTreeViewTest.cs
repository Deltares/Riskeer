using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
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
            Assert.AreEqual(4, view.NodePresenters.Count());
            Assert.IsInstanceOf<LineDataNodePresenter>(view.NodePresenters.ElementAt(0));
            Assert.IsInstanceOf<PointDataNodePresenter>(view.NodePresenters.ElementAt(1));
            Assert.IsInstanceOf<AreaDataNodePresenter>(view.NodePresenters.ElementAt(2));
            Assert.IsInstanceOf<ChartNodePresenter>(view.NodePresenters.ElementAt(3));
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
            Assert.IsNull(tree.Data);
        }

        private class TestChartData : ChartDataCollection
        {
            public TestChartData() : base(new List<ChartData>()) {}
        }
    }
}