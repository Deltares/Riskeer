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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
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
            using (var chart = new ChartControl())
            {
                // Assert
                Assert.IsInstanceOf<Control>(chart);
                Assert.AreEqual(75, chart.MinimumSize.Height);
                Assert.AreEqual(50, chart.MinimumSize.Width);
                Assert.IsNotNull(chart.Data);
                CollectionAssert.IsEmpty(chart.Data.List);
                Assert.IsTrue(chart.IsPanningEnabled);
                Assert.IsFalse(chart.IsRectangleZoomingEnabled);
            }
        }

        [Test]
        public void TogglePanning_Always_PanningEnabled()
        {
            // Setup
            using (var chart = new ChartControl())
            {
                // Precondition
                Assert.IsTrue(chart.IsPanningEnabled);

                // Call
                chart.TogglePanning();

                // Assert
                Assert.IsTrue(chart.IsPanningEnabled);
                Assert.IsFalse(chart.IsRectangleZoomingEnabled);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleRectangleZooming_Always_ChangesState(bool isRectangleZooming)
        {
            // Setup
            using (var chart = new ChartControl())
            {
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
        }

        [Test]
        public void ZoomToAll_ChartInForm_ViewInvalidatedSeriesSame()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var testData = new ChartLineData(Enumerable.Empty<Point2D>(), "test data");
                var view = TypeUtils.GetField<PlotView>(chart, "view");
                var invalidated = 0;

                chart.Data.Add(testData);
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
        }

        [Test]
        public void UpdateObserver_ChartInForm_ViewInvalidatedSeriesRenewed()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var testData = new ChartLineData(Enumerable.Empty<Point2D>(), "test data");
                var view = TypeUtils.GetField<PlotView>(chart, "view");
                var invalidated = 0;

                chart.Data.Add(testData);
                chart.UpdateObserver();
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

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void BottomAxisTitle_Always_SetsNewTitleToBottomAxis(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var view = TypeUtils.GetField<PlotView>(chart, "view");
                form.Controls.Add(chart);

                form.Show();

                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                // Call
                chart.BottomAxisTitle = newTitle;

                // Assert
                Assert.AreEqual(chart.BottomAxisTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void SetLeftAxisTitle_Always_SetsNewTitleToLeftAxis(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var view = TypeUtils.GetField<PlotView>(chart, "view");
                form.Controls.Add(chart);

                form.Show();

                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                // Call
                chart.LeftAxisTitle = newTitle;

                // Assert
                Assert.AreEqual(chart.LeftAxisTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void SetModelTitle_Always_SetsNewTitleToModelAndViewInvalidated(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var view = TypeUtils.GetField<PlotView>(chart, "view");
                form.Controls.Add(chart);

                form.Show();

                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                // Call
                chart.ChartTitle = newTitle;

                // Assert
                Assert.AreEqual(chart.ChartTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        public void ResetChartData_Always_SetsDataToNull()
        {
            // Setup
            using (var chart = new ChartControl())
            {
                // Precondition
                Assert.IsNotNull(chart.Data);

                // Call
                chart.ResetChartData();

                // Assert
                Assert.IsNull(chart.Data);
            }
        }
    }
}