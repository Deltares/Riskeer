﻿using System;
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
    public class BaseChartTest
    {
        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var chart = new BaseChart();

            // Assert
            Assert.IsInstanceOf<Control>(chart);
            Assert.AreEqual(75, chart.MinimumSize.Height);
            Assert.AreEqual(50, chart.MinimumSize.Width);
            Assert.IsNull(chart.Data);
            Assert.IsFalse(chart.IsPanningEnabled);
            Assert.IsFalse(chart.IsRectangleZoomingEnabled);
        }

        [Test]
        public void Data_NotKnownChartData_ThrowsNotSupportedException()
        {
            // Setup
            var chart = new BaseChart();
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
            var chart = new BaseChart();

            // Call
            chart.Data = null;

            // Assert
            Assert.IsNull(chart.Data);
        }

        [Test]
        public void Data_KnownChartData_BaseChartAttachedSeriesAdded()
        {
            // Setup
            var chart = new BaseChart();
            var testData = new LineData(Enumerable.Empty<Tuple<double,double>>());
            var observers = TypeUtils.GetField<ICollection<IObserver>>(testData, "observers");
            var view = TypeUtils.GetField<PlotView>(chart, "view");

            // Call
            chart.Data = testData;

            // Assert
            CollectionAssert.AreEqual(new []{chart}, observers);
            Assert.AreEqual(1, view.Model.Series.Count);
        }

        [Test]
        public void Data_NewDataSet_BaseChartDetachedFromOldAttachedToNewSeriesUpdated()
        {
            // Setup
            var chart = new BaseChart();
            var testDataOld = new LineData(Enumerable.Empty<Tuple<double, double>>());
            var testDataNew = new LineData(Enumerable.Empty<Tuple<double, double>>());
            var observersOld = TypeUtils.GetField<ICollection<IObserver>>(testDataOld, "observers");
            var observersNew = TypeUtils.GetField<ICollection<IObserver>>(testDataNew, "observers");
            var view = TypeUtils.GetField<PlotView>(chart, "view");

            // Call
            chart.Data = testDataOld;
            chart.Data = testDataNew;

            // Assert
            CollectionAssert.IsEmpty(observersOld);
            CollectionAssert.AreEqual(new[] { chart }, observersNew);
            Assert.AreEqual(1, view.Model.Series.Count);
        }

        [Test]
        public void Data_DataSetNewValueIsNull_BaseChartDetachedSeriesCleared()
        {
            // Setup
            var chart = new BaseChart();
            var testData = new LineData(Enumerable.Empty<Tuple<double, double>>());
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
        [TestCase(true)]
        [TestCase(false)]
        public void TogglePanning_Always_ChangesState(bool isPanning)
        {
            // Setup
            var chart = new BaseChart();
            if (isPanning)
            {
                chart.TogglePanning();
            }

            // Precondition
            Assert.AreEqual(isPanning, chart.IsPanningEnabled);

            // Call
            chart.TogglePanning();

            // Assert
            Assert.AreNotEqual(isPanning, chart.IsPanningEnabled);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleRectangleZooming_Always_ChangesState(bool isRectangleZooming)
        {
            // Setup
            var chart = new BaseChart();
            if (isRectangleZooming)
            {
                chart.ToggleRectangleZooming();
            }

            // Precondition
            Assert.AreEqual(isRectangleZooming, chart.IsRectangleZoomingEnabled);

            // Call
            chart.ToggleRectangleZooming();

            // Assert
            Assert.AreNotEqual(isRectangleZooming, chart.IsRectangleZoomingEnabled);
        }

        [Test]
        public void ZoomToAll_ChartInForm_ViewInvalidatedSeriesSame()
        {
            // Setup
            var form = new Form();
            var chart = new BaseChart();
            var testData = new LineData(Enumerable.Empty<Tuple<double, double>>());
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
            var chart = new BaseChart();
            var testData = new LineData(Enumerable.Empty<Tuple<double, double>>());
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