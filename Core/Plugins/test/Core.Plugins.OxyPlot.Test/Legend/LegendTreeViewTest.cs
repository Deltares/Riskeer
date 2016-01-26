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
        public void GivenTreeWithNodes_WhenTreeDisposed_ThenNodesDetachedFromIObservableData()
        {
            // Given
            var tree = new LegendTreeView();
            var data = new TestChartData();

            tree.ChartData = data;

            // When
            tree.Dispose();

            // Then
            var dataObservers = TypeUtils.GetField<ICollection<IObserver>>(data, "observers");
            var pointObservers = TypeUtils.GetField<ICollection<IObserver>>(TestChartData.Point, "observers");
            var lineObservers = TypeUtils.GetField<ICollection<IObserver>>(TestChartData.Line, "observers");

            CollectionAssert.IsEmpty(dataObservers);
            CollectionAssert.IsEmpty(pointObservers);
            CollectionAssert.IsEmpty(lineObservers);
        }

        private class TestChartData : ChartDataCollection
        {
            public static readonly PointData Point = new PointData(Enumerable.Empty<Tuple<double, double>>());
            public static readonly LineData Line = new LineData(Enumerable.Empty<Tuple<double, double>>());

            public TestChartData() : base(new List<ChartData>
            {
                Point,
                Line
            }) {}
        }
    }
}