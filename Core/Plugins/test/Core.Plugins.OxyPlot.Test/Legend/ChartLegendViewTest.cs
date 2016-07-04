using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Legend;
using Core.Plugins.OxyPlot.Properties;
using NUnit.Framework;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartLegendViewTest
    {
        [Test]
        public void DefaultConstructor_CreatesUserControl()
        {
            // Call 
            using (var view = new ChartLegendView())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(Resources.General_Chart, view.Text);
            }
        }

        [Test]
        public void Data_ChartControl_DataSet()
        {
            // Setup 
            using (var view = new ChartLegendView())
            {
                var chartDataCollection = new ChartDataCollection(new List<ChartData>(), "test data");

                // Call
                view.Data = chartDataCollection;

                // Assert
                Assert.AreSame(chartDataCollection, view.Data);
            }
        }

        [Test]
        public void Data_ForNull_NullSet()
        {
            // Setup 
            using (var view = new ChartLegendView())
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }
        }

        [Test]
        public void Data_OtherObject_ThrowsInvalidCastException()
        {
            // Setup 
            using (var view = new ChartLegendView())
            {
                // Call
                TestDelegate test = () => view.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(test);
            }
        }

        [Test]
        public void Dispose_Always_DataSetToNull()
        {
            // Setup
            var legendView = new ChartLegendView
            {
                Data = new ChartDataCollection(new List<ChartData>(), "test data")
            };

            var treeViewControl = TypeUtils.GetField<TreeViewControl>(legendView, "treeViewControl");

            // Call
            legendView.Dispose();

            // Assert
            Assert.IsNull(legendView.Data);
            Assert.IsNull(treeViewControl.Data);
            Assert.IsTrue(treeViewControl.IsDisposed);
        }

        [Test]
        public void GivenChartDataContainingCollection_WhenDragDroppingFromCollectionToRoot_ThenDataMoved()
        {
            // Given
            var chartLineData = CreateChartLineData();

            var innerCollection = new ChartDataCollection(new List<ChartData>
            {
                chartLineData
            }, "collection");
            var rootCollection = new ChartDataCollection(new List<ChartData>
            {
                CreateChartLineData(),
                innerCollection
            }, "test data");

            var chartLegendView = new ChartLegendView
            {
                Data = rootCollection
            };

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");
            var info = treeNodeInfoLookup[typeof(ChartDataCollection)];

            // When
            info.OnDrop(chartLineData, rootCollection, innerCollection, 0, treeViewControl);

            // Then
            CollectionAssert.DoesNotContain(innerCollection.List, chartLineData);
            CollectionAssert.Contains(rootCollection.List, chartLineData);
        }

        [Test]
        public void GivenChartDataContainingCollection_WhenDragDroppingFromRootToCollection_ThenDataMoved()
        {
            // Given
            var chartLineData = CreateChartLineData();

            var innerCollection = new ChartDataCollection(new List<ChartData>
            {
                CreateChartLineData()
            }, "collection");
            var rootCollection = new ChartDataCollection(new List<ChartData>
            {
                chartLineData,
                innerCollection
            }, "test data");

            var chartLegendView = new ChartLegendView
            {
                Data = rootCollection
            };

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");
            var info = treeNodeInfoLookup[typeof(ChartDataCollection)];

            // When
            info.OnDrop(chartLineData, innerCollection, rootCollection, 0, treeViewControl);

            // Then
            CollectionAssert.DoesNotContain(rootCollection.List, chartLineData);
            CollectionAssert.Contains(innerCollection.List, chartLineData);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenChartDataContainingCollection_WhenDragDroppingFromRootToRoot_ThenDataPositionChanged(int index)
        {
            // Given
            var chartLineData = CreateChartLineData();

            var rootCollection = new ChartDataCollection(new List<ChartData>
            {
                chartLineData,
                CreateChartLineData(),
                CreateChartLineData()
            }, "test data");

            var chartLegendView = new ChartLegendView
            {
                Data = rootCollection
            };

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");
            var info = treeNodeInfoLookup[typeof(ChartDataCollection)];

            // When
            info.OnDrop(chartLineData, rootCollection, rootCollection, index, treeViewControl);

            // Then
            Assert.AreEqual(2 - index, rootCollection.List.IndexOf(chartLineData));
        }

        private ChartData CreateChartLineData()
        {
            return new ChartLineData(Enumerable.Empty<Point2D>(), "some name");
        }
    }
}