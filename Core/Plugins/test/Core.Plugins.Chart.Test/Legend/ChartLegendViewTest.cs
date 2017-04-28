// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Plugins.Chart.Legend;
using Core.Plugins.Chart.PresentationObjects;
using Core.Plugins.Chart.Properties;
using NUnit.Framework;

namespace Core.Plugins.Chart.Test.Legend
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
                var chartDataCollection = new ChartDataCollection("test data");

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
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void GivenChartDataContainingCollection_WhenDragDroppingFromRootToRoot_ThenDataPositionChanged(int index)
        {
            // Given
            ChartData chartLineData = CreateChartLineData();
            var rootCollection = new ChartDataCollection("test data");

            rootCollection.Add(chartLineData);
            rootCollection.Add(CreateChartLineData());
            rootCollection.Add(CreateChartLineData());

            using (var chartLegendView = new ChartLegendView
            {
                Data = rootCollection
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(chartLegendView, "treeViewControl");
                var treeNodeInfoLookup = TypeUtils.GetField<Dictionary<Type, TreeNodeInfo>>(treeViewControl, "tagTypeTreeNodeInfoLookup");
                TreeNodeInfo info = treeNodeInfoLookup[typeof(ChartDataCollection)];

                var context = new ChartDataContext(chartLineData, rootCollection);

                // When
                info.OnDrop(context, rootCollection, rootCollection, index, treeViewControl);

                // Then
                Assert.AreEqual(2 - index, rootCollection.Collection.ToList().IndexOf(chartLineData));
            }
        }

        private ChartData CreateChartLineData()
        {
            return new ChartLineData("some name");
        }
    }
}