// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using Core.Plugins.Chart.Legend;
using Core.Plugins.Chart.PresentationObjects;
using Core.Plugins.Chart.Properties;
using NUnit.Framework;
using Rhino.Mocks;
using GuiResources = Core.Common.Gui.Properties.Resources;

namespace Core.Plugins.Chart.Test.Legend
{
    [TestFixture]
    public class ChartDataContextTreeNodeInfoTest
    {
        private ChartLegendView chartLegendView;
        private TreeNodeInfo info;

        private static IEnumerable<TestCaseData> ChartDataLegendImages
        {
            get
            {
                yield return new TestCaseData(new ChartPointData("test"), Resources.PointsIcon);
                yield return new TestCaseData(new ChartLineData("test"), Resources.LineIcon);
                yield return new TestCaseData(new ChartAreaData("test"), Resources.AreaIcon);
                yield return new TestCaseData(new ChartMultipleAreaData("test data"), Resources.AreaIcon);
                yield return new TestCaseData(new ChartDataCollection("test"), GuiResources.folder);
            }
        }

        private static IEnumerable<ChartData> DragChartData
        {
            get
            {
                return new ChartData[]
                {
                    new ChartPointData("test"),
                    new ChartLineData("test"),
                    new ChartAreaData("Test"),
                    new ChartDataCollection("test")
                };
            }
        }

        private static IEnumerable<ChartData> NoChartDataCollection
        {
            get
            {
                return new ChartData[]
                {
                    new ChartPointData("test"),
                    new ChartLineData("test"),
                    new ChartAreaData("Test"),
                    new ChartMultipleAreaData("test data")
                };
            }
        }

        private static IEnumerable<TestCaseData> IsCheckedChartData
        {
            get
            {
                yield return new TestCaseData(new ChartPointData("test"), false);
                yield return new TestCaseData(new ChartPointData("test"), true);
                yield return new TestCaseData(new ChartLineData("test"), false);
                yield return new TestCaseData(new ChartLineData("test"), true);
                yield return new TestCaseData(new ChartAreaData("test"), false);
                yield return new TestCaseData(new ChartAreaData("test"), true);
                yield return new TestCaseData(new ChartMultipleAreaData("test data"), false);
                yield return new TestCaseData(new ChartMultipleAreaData("test data"), true);
            }
        }

        [SetUp]
        public void SetUp()
        {
            chartLegendView = new ChartLegendView();

            TreeViewControl treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
            Dictionary<Type, TreeNodeInfo> treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");

            info = treeNodeInfoLookup[typeof(ChartDataContext)];
        }

        [TearDown]
        public void TearDown()
        {
            chartLegendView.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(ChartDataContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNull(info.ContextMenuStrip);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanCheck);
            Assert.IsNotNull(info.IsChecked);
            Assert.IsNotNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNotNull(info.CanDrop);
            Assert.IsNotNull(info.CanInsert);
            Assert.IsNotNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsNameFromWrappedChartData()
        {
            // Setup
            ChartDataContext context = GetContext(new TestChartData());

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual(context.WrappedData.Name, text);
        }

        [Test]
        [TestCaseSource("ChartDataLegendImages")]
        public void Image_WrappedDataChartPointData_ReturnPointsIcon(ChartData chartData, Image expectedImage)
        {
            // Setup            
            ChartDataContext context = GetContext(chartData);

            // Call
            Image image = info.Image(context);

            // Assert
            TestHelper.AssertImagesAreEqual(expectedImage, image);
        }

        [Test]
        public void ChildNodeObjects_ChartDataCollection_ReturnsItemsFromChartDataCollectionList()
        {
            // Setup
            TestChartData chartData1 = new TestChartData();
            TestChartData chartData2 = new TestChartData();
            TestChartData chartData3 = new TestChartData();
            ChartDataCollection chartDataCollection = new ChartDataCollection("test data");

            chartDataCollection.Add(chartData1);
            chartDataCollection.Add(chartData2);
            chartDataCollection.Add(chartData3);

            ChartDataContext context = GetContext(chartDataCollection);

            // Call
            var objects = info.ChildNodeObjects(context);

            // Assert
            var expectedChildren = new[]
            {
                new ChartDataContext(chartData3, new ChartDataCollection("test")),
                new ChartDataContext(chartData2, new ChartDataCollection("test")),
                new ChartDataContext(chartData1, new ChartDataCollection("test"))
            };
            CollectionAssert.AreEqual(expectedChildren, objects);
        }

        [Test]
        [TestCaseSource("NoChartDataCollection")]
        public void ChildNodeObjects_OtherThanChartDataCollection_ReturnsEmptyArray(ChartData chartData)
        {
            // Setup
            ChartDataContext context = GetContext(chartData);

            // Call
            var objects = info.ChildNodeObjects(context);

            // Assert
            CollectionAssert.IsEmpty(objects);
        }

        [Test]
        public void CanDrag_WrappedDataChartMultipleAreaData_ReturnsFalse()
        {
            // Setup
            ChartMultipleAreaData multipleChartAreaData = new ChartMultipleAreaData("test data");
            ChartDataContext context = GetContext(multipleChartAreaData);

            // Call
            bool canDrag = info.CanDrag(context, null);

            // Assert
            Assert.IsFalse(canDrag);
        }

        [Test]
        [TestCaseSource("DragChartData")]
        public void CanDrag_WrappedDataOtherThanChartMultipleAreaData_ReturnsTrue(ChartData chartData)
        {
            // Setup
            ChartDataContext context = GetContext(chartData);

            // Call
            bool canDrag = info.CanDrag(context, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanCheck_WrappedDataChartDataCollection_ReturnsFalse()
        {
            // Setup
            ChartDataContext context = GetContext(new ChartDataCollection("test data"));

            // Call
            bool canCheck = info.CanCheck(context);

            // Assert
            Assert.IsFalse(canCheck);
        }

        [Test]
        [TestCaseSource("NoChartDataCollection")]
        public void CanCheck_WrappedDataOtherThanChartDataCollection_ReturnsTrue(ChartData chartData)
        {
            // Setup
            ChartDataContext context = GetContext(chartData);

            // Call
            bool canCheck = info.CanCheck(context);

            // Assert
            Assert.IsTrue(canCheck);
        }

        [Test]
        [TestCaseSource("IsCheckedChartData")]
        public void IsChecked_Always_ReturnsAccordingToVisibleStateOfChartData(ChartData chartData, bool isVisible)
        {
            // Setup
            ChartDataContext context = GetContext(chartData);
            context.WrappedData.IsVisible = isVisible;

            // Call
            bool isChecked = info.IsChecked(context);

            // Assert
            Assert.AreEqual(isVisible, isChecked);
        }

        [Test]
        [TestCaseSource("IsCheckedChartData")]
        public void OnNodeChecked_Always_SetsPointDataVisibilityAndNotifiesParentObservers(ChartData chartData, bool initialVisibleState)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            ChartDataContext context = GetContext(chartData);
            context.WrappedData.IsVisible = initialVisibleState;

            context.WrappedData.Attach(observer);

            // Call
            info.OnNodeChecked(context, null);

            // Assert
            Assert.AreEqual(!initialVisibleState, context.WrappedData.IsVisible);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_TargetIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            ChartData chartData = new TestChartData();
            ChartDataCollection chartDataCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, chartDataCollection);
            ChartDataContext targetContext = GetContext(chartDataCollection);

            // Call
            bool canDrop = info.CanDrop(context, targetContext);

            // Assert
            Assert.IsTrue(canDrop);
        }

        [Test]
        public void CanDrop_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            ChartData chartData = new TestChartData();
            ChartData chartData2 = new TestChartData();

            ChartDataContext context = GetContext(chartData);
            ChartDataContext targetContext = GetContext(chartData2);

            // Call
            bool canDrop = info.CanDrop(context, targetContext);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanDrop_TargetDataIsCollection_ReturnsFalse()
        {
            // Setup
            ChartData chartData = new TestChartData();
            ChartDataCollection rootCollection = new ChartDataCollection("test");
            ChartDataCollection targetCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, rootCollection);
            ChartDataContext targetContext = GetContext(targetCollection, rootCollection);

            // Call
            bool canDrop = info.CanDrop(context, targetContext);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        public void CanInsert_TargetIsSameAsSourceParent_ReturnsTrue()
        {
            // Setup
            ChartData chartData = new TestChartData();
            ChartDataCollection chartDataCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, chartDataCollection);
            ChartDataContext targetContext = GetContext(chartDataCollection);

            // Call
            bool canInsert = info.CanInsert(context, targetContext);

            // Assert
            Assert.IsTrue(canInsert);
        }

        [Test]
        public void CanInsert_TargetParentNotSameAsSourceParent_ReturnsFalse()
        {
            // Setup
            ChartData chartData = new TestChartData();
            ChartData chartData2 = new TestChartData();

            ChartDataContext context = GetContext(chartData);
            ChartDataContext targetContext = GetContext(chartData2);

            // Call
            bool canInsert = info.CanInsert(context, targetContext);

            // Assert
            Assert.IsFalse(canInsert);
        }

        [Test]
        public void CanInsert_TargetDataIsCollection_ReturnsFalse()
        {
            // Setup
            ChartData chartData = new TestChartData();
            ChartDataCollection rootCollection = new ChartDataCollection("test");
            ChartDataCollection targetCollection = new ChartDataCollection("test");

            ChartDataContext context = GetContext(chartData, rootCollection);
            ChartDataContext targetContext = GetContext(targetCollection, rootCollection);

            // Call
            bool canDrop = info.CanInsert(context, targetContext);

            // Assert
            Assert.IsFalse(canDrop);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void OnDrop_ChartDataMovedToPositionInsideRange_SetsNewReverseOrder(int position)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var chartData1 = new TestChartData();
            var chartData2 = new TestChartData();
            var chartData3 = new TestChartData();
            var chartDataCollection = new ChartDataCollection("test data");

            chartDataCollection.Add(chartData1);
            chartDataCollection.Add(chartData2);
            chartDataCollection.Add(chartData3);

            var context1 = GetContext(chartData1);
            var collectionContext = GetContext(chartDataCollection);

            chartDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                info.OnDrop(context1, collectionContext, collectionContext, position, treeViewControl);

                // Assert
                var reversedIndex = 2 - position;
                ChartDataCollection wrappedCollectionData = (ChartDataCollection) collectionContext.WrappedData;
                Assert.AreSame(context1.WrappedData, wrappedCollectionData.Collection.ElementAt(reversedIndex));

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(-50)]
        [TestCase(-1)]
        [TestCase(4)]
        [TestCase(50)]
        public void OnDrop_ChartDataMovedToPositionOutsideRange_ThrowsException(int position)
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var chartData1 = new ChartLineData("line");
            var chartData2 = new ChartAreaData("area");
            var chartData3 = new ChartPointData("point");
            var chartData4 = new ChartMultipleAreaData("multiple area");
            var chartDataCollection = new ChartDataCollection("test data");

            chartDataCollection.Add(chartData1);
            chartDataCollection.Add(chartData2);
            chartDataCollection.Add(chartData3);
            chartDataCollection.Add(chartData4);

            chartDataCollection.Attach(observer);
            chartLegendView.Data = chartDataCollection;

            var context1 = GetContext(chartData1);
            var collectionContext = GetContext(chartDataCollection);

            chartDataCollection.Attach(observer);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => info.OnDrop(context1, collectionContext, collectionContext, position, treeViewControl);

                // Assert
                Assert.Throws<ArgumentOutOfRangeException>(test);

                mocks.VerifyAll(); // Expect no update observer.
            }
        }

        private static ChartDataContext GetContext(ChartData chartData, ChartDataCollection chartDataCollection = null)
        {
            return new ChartDataContext(chartData, chartDataCollection ?? new ChartDataCollection("test"));
        }
    }
}