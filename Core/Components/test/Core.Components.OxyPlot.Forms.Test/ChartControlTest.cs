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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Components.Charting.TestUtil;
using NUnit.Framework;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Forms.Test
{
    [TestFixture]
    public class ChartControlTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var chart = new ChartControl();

            // Assert
            Assert.IsInstanceOf<Control>(chart);
            Assert.AreEqual(75, chart.MinimumSize.Height);
            Assert.AreEqual(50, chart.MinimumSize.Width);
            Assert.IsNull(chart.Data);
            Assert.IsTrue(chart.IsPanningEnabled);
            Assert.IsFalse(chart.IsRectangleZoomingEnabled);
        }

        [Test]
        public void Data_NotKnownChartData_ThrowsNotSupportedException()
        {
            // Setup
            var chart = new ChartControl();
            var testData = new TestChartData();

            // Call
            TestDelegate test = () => chart.Data = testData;

            // Assert
            Assert.Throws<NotSupportedException>(test);
        }

        [Test]
        public void Data_Null_ReturnsNull()
        {
            // Setup
            var chart = new ChartControl();

            // Call
            chart.Data = null;

            // Assert
            Assert.IsNull(chart.Data);
        }

        [Test]
        public void Data_KnownChartData_ChartControlAttachedSeriesAdded()
        {
            // Setup
            var chart = new ChartControl();
            var testData = new ChartLineData(Enumerable.Empty<Tuple<double, double>>(), "test data");
            var observers = TypeUtils.GetField<ICollection<IObserver>>(testData, "observers");
            var view = TypeUtils.GetField<PlotView>(chart, "view");

            // Call
            chart.Data = testData;

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                chart
            }, observers);
            Assert.AreEqual(1, view.Model.Series.Count);
        }

        [Test]
        public void Data_NewDataSet_ChartControlDetachedFromOldAttachedToNewSeriesUpdated()
        {
            // Setup
            var chart = new ChartControl();
            var testDataOld = new ChartLineData(Enumerable.Empty<Tuple<double, double>>(), "test data");
            var testDataNew = new ChartLineData(Enumerable.Empty<Tuple<double, double>>(), "test data");
            var observersOld = TypeUtils.GetField<ICollection<IObserver>>(testDataOld, "observers");
            var observersNew = TypeUtils.GetField<ICollection<IObserver>>(testDataNew, "observers");
            var view = TypeUtils.GetField<PlotView>(chart, "view");

            // Call
            chart.Data = testDataOld;
            chart.Data = testDataNew;

            // Assert
            CollectionAssert.IsEmpty(observersOld);
            CollectionAssert.AreEqual(new[]
            {
                chart
            }, observersNew);
            Assert.AreEqual(1, view.Model.Series.Count);
        }

        [Test]
        public void Data_DataSetNewValueIsNull_ChartControlDetachedSeriesCleared()
        {
            // Setup
            var chart = new ChartControl();
            var testData = new ChartLineData(Enumerable.Empty<Tuple<double, double>>(), "test data");
            var observers = TypeUtils.GetField<ICollection<IObserver>>(testData, "observers");
            var view = TypeUtils.GetField<PlotView>(chart, "view");

            chart.Data = testData;

            // Precondition
            Assert.AreEqual(1, view.Model.Series.Count);

            // Call
            chart.Data = null;

            // Assert
            CollectionAssert.IsEmpty(observers);
            CollectionAssert.IsEmpty(view.Model.Series);
        }

        [Test]
        public void TogglePanning_Always_PanningEnabled()
        {
            // Setup
            var chart = new ChartControl();

            // Precondition
            Assert.IsTrue(chart.IsPanningEnabled);

            // Call
            chart.TogglePanning();

            // Assert
            Assert.IsTrue(chart.IsPanningEnabled);
            Assert.IsFalse(chart.IsRectangleZoomingEnabled);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleRectangleZooming_Always_ChangesState(bool isRectangleZooming)
        {
            // Setup
            var chart = new ChartControl();
            if (isRectangleZooming)
            {
                chart.ToggleRectangleZooming();
            }

            // Precondition
            Assert.AreEqual(isRectangleZooming, chart.IsRectangleZoomingEnabled);
            Assert.AreEqual(!isRectangleZooming, chart.IsPanningEnabled);

            // Call
            chart.ToggleRectangleZooming();

            // Assert
            Assert.IsTrue(chart.IsRectangleZoomingEnabled);
        }

        [Test]
        public void ZoomToAll_ChartInForm_ViewInvalidatedSeriesSame()
        {
            // Setup
            var form = new Form();
            var chart = new ChartControl();
            var testData = new ChartLineData(Enumerable.Empty<Tuple<double, double>>(), "test data");
            var view = TypeUtils.GetField<PlotView>(chart, "view");
            var invalidated = 0;

            chart.Data = testData;
            var series = view.Model.Series.ToList();

            form.Controls.Add(chart);
            view.Invalidated += (sender, args) => invalidated++;

            form.Show();

            // Call
            chart.ZoomToAll();

            // Assert
            Assert.AreEqual(1, invalidated);
            CollectionAssert.AreEqual(series, view.Model.Series);
        }

        [Test]
        public void UpdateObserver_ChartInForm_ViewInvalidatedSeriesRenewed()
        {
            // Setup
            var form = new Form();
            var chart = new ChartControl();
            var testData = new ChartLineData(Enumerable.Empty<Tuple<double, double>>(), "test data");
            var view = TypeUtils.GetField<PlotView>(chart, "view");
            var invalidated = 0;

            chart.Data = testData;
            var series = view.Model.Series.ToList();

            form.Controls.Add(chart);
            view.Invalidated += (sender, args) => invalidated++;

            form.Show();

            // Call
            chart.UpdateObserver();

            // Assert
            Assert.AreEqual(1, invalidated);
            Assert.AreEqual(1, view.Model.Series.Count);
            Assert.AreNotSame(series[0], view.Model.Series[0]);
        }
    }
}