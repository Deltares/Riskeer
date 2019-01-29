// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using Core.Components.Chart.Data;
using Core.Components.Chart.TestUtil;
using Core.Plugins.Chart.Legend;
using Core.Plugins.Chart.PresentationObjects;
using Core.Plugins.Chart.Properties;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Chart.Test.Legend
{
    [TestFixture]
    public class ChartLegendViewTest
    {
        [Test]
        public void Constructor_WithoutContextMenuBuilderProvider_CreatesUserControl()
        {
            // Call 
            var exception = Assert.Throws<ArgumentNullException>(() => new ChartLegendView(null));

            // Assert
            Assert.AreEqual("contextMenuBuilderProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_WithBuilderProvider_CreatesUserControl()
        {
            // Setup 
            var mocks = new MockRepository();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            // Call 
            using (var view = new ChartLegendView(menuBuilderProvider))
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(view);
                Assert.IsInstanceOf<IView>(view);
                Assert.IsInstanceOf<ISelectionProvider>(view);
                Assert.IsNull(view.Data);
                Assert.AreEqual(Resources.General_Chart, view.Text);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Data_ChartDataCollection_DataSet()
        {
            // Setup 
            var mocks = new MockRepository();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            using (var view = new ChartLegendView(menuBuilderProvider))
            {
                var chartDataCollection = new ChartDataCollection("test data");

                // Call
                view.Data = chartDataCollection;

                // Assert
                Assert.AreSame(chartDataCollection, view.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Data_ForNull_NullSet()
        {
            // Setup 
            var mocks = new MockRepository();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            using (var view = new ChartLegendView(menuBuilderProvider))
            {
                // Call
                view.Data = null;

                // Assert
                Assert.IsNull(view.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Data_OtherObject_ThrowsInvalidCastException()
        {
            // Setup 
            var mocks = new MockRepository();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            using (var view = new ChartLegendView(menuBuilderProvider))
            {
                // Call
                TestDelegate test = () => view.Data = new object();

                // Assert
                Assert.Throws<InvalidCastException>(test);
            }

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Selection_NestedNodeData_ReturnsWrappedObjectData()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            ChartData chartData = CreateChartData();
            var chartDataCollection = new ChartDataCollection("collection");
            chartDataCollection.Add(chartData);

            using (var view = new ChartLegendView(contextMenuBuilderProvider)
            {
                Data = chartDataCollection
            })
            {
                var context = new ChartDataContext(chartData, chartDataCollection);

                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");
                WindowsFormsTestHelper.Show(treeViewControl);
                treeViewControl.TrySelectNodeForData(context);

                // Call
                object selection = view.Selection;

                // Assert
                Assert.AreSame(chartData, selection);
            }

            WindowsFormsTestHelper.CloseAll();

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Selection_RootNodeData_ReturnsObjectData()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            ChartData chartData = CreateChartData();
            var chartDataCollection = new ChartDataCollection("collection");
            chartDataCollection.Add(chartData);

            using (var view = new ChartLegendView(contextMenuBuilderProvider)
            {
                Data = chartDataCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");
                WindowsFormsTestHelper.Show(treeViewControl);
                treeViewControl.TrySelectNodeForData(chartDataCollection);

                // Call
                object selection = view.Selection;

                // Assert
                Assert.AreSame(chartDataCollection, selection);
            }

            WindowsFormsTestHelper.CloseAll();

            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenChartLegendView_WhenSelectedNodeChanged_SelectionChangedFired()
        {
            // Given
            ChartData chartData = CreateChartData();
            var chartDataCollection = new ChartDataCollection("collection");
            chartDataCollection.Add(chartData);

            var mocks = new MockRepository();
            var contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            using (var view = new ChartLegendView(contextMenuBuilderProvider)
            {
                Data = chartDataCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");
                WindowsFormsTestHelper.Show(treeViewControl);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // When
                var context = new ChartDataContext(chartData, chartDataCollection);
                treeViewControl.TrySelectNodeForData(context);

                // Then
                Assert.AreEqual(1, selectionChangedCount);
            }

            WindowsFormsTestHelper.CloseAll();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenChartLegendView_WhenSettingData_SelectionChangedFired()
        {
            // Given
            var mocks = new MockRepository();
            var contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            using (var view = new ChartLegendView(contextMenuBuilderProvider)
            {
                Data = new ChartDataCollection("collection")
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(view, "treeViewControl");
                WindowsFormsTestHelper.Show(treeViewControl);

                var selectionChangedCount = 0;
                view.SelectionChanged += (sender, args) => selectionChangedCount++;

                // When
                view.Data = new ChartDataCollection("collection");

                // Then
                Assert.AreEqual(1, selectionChangedCount);
            }

            WindowsFormsTestHelper.CloseAll();
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenChartDataContainingCollection_WhenDragDroppingFromRootToRoot_ThenDataPositionChanged(int index)
        {
            // Given
            var mocks = new MockRepository();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            ChartData chartData = CreateChartData();
            var rootCollection = new ChartDataCollection("test data");

            rootCollection.Add(chartData);
            rootCollection.Add(CreateChartData());
            rootCollection.Add(CreateChartData());

            using (var chartLegendView = new ChartLegendView(menuBuilderProvider)
            {
                Data = rootCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
                var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");
                TreeNodeInfo info = treeNodeInfoLookup[typeof(ChartDataCollection)];

                var context = new ChartDataContext(chartData, rootCollection);

                // When
                info.OnDrop(context, rootCollection, rootCollection, index, treeViewControl);

                // Then
                Assert.AreEqual(2 - index, rootCollection.Collection.ToList().IndexOf(chartData));
            }

            mocks.VerifyAll();
        }

        private static ChartData CreateChartData()
        {
            return new TestChartData("some name");
        }
    }
}