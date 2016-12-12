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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.Utils.Reflection;
using Core.Components.Charting.Data;
using Core.Components.Charting.Forms;
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
                Assert.IsInstanceOf<IChartControl>(chart);
                Assert.AreEqual(100, chart.MinimumSize.Height);
                Assert.AreEqual(100, chart.MinimumSize.Width);
                Assert.IsNull(chart.Data);
                Assert.IsTrue(chart.IsPanningEnabled);
                Assert.IsFalse(chart.IsRectangleZoomingEnabled);

                var view = TypeUtils.GetField<PlotView>(chart, "plotView");
                Assert.AreEqual(Color.White, view.BackColor);
                Assert.IsFalse(view.Model.IsLegendVisible);
            }
        }

        [Test]
        public void GivenChartControlWithoutData_WhenDataSetToChartDataCollection_ThenChartControlUpdated()
        {
            // Given
            using (var chart = new ChartControl())
            {
                var plotView = chart.Controls.OfType<LinearPlotView>().First();
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection = new ChartDataCollection("Root collection");
                var nestedChartDataCollection1 = new ChartDataCollection("Nested collection 1");
                var nestedChartDataCollection2 = new ChartDataCollection("Nested collection 2");

                chartDataCollection.Add(chartPointData);
                chartDataCollection.Add(nestedChartDataCollection1);
                nestedChartDataCollection1.Add(chartLineData);
                nestedChartDataCollection1.Add(nestedChartDataCollection2);
                nestedChartDataCollection2.Add(chartAreaData);

                // When
                chart.Data = chartDataCollection;

                // Then
                var series = plotView.Model.Series;
                Assert.AreEqual(3, series.Count);
                Assert.AreEqual("Points", series[0].Title);
                Assert.AreEqual("Lines", series[1].Title);
                Assert.AreEqual("Areas", series[2].Title);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenDataSetToOtherChartDataCollection_ThenChartControlUpdated()
        {
            // Given
            using (var chart = new ChartControl())
            {
                var plotView = chart.Controls.OfType<LinearPlotView>().First();
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection1 = new ChartDataCollection("Collection 1");
                var chartDataCollection2 = new ChartDataCollection("Collection 2");

                chartDataCollection1.Add(chartPointData);
                chartDataCollection2.Add(chartLineData);
                chartDataCollection2.Add(chartAreaData);

                chart.Data = chartDataCollection1;

                // Precondition
                Assert.AreEqual(1, plotView.Model.Series.Count);
                Assert.AreEqual("Points", plotView.Model.Series[0].Title);

                // When
                chart.Data = chartDataCollection2;

                // Then
                Assert.AreEqual(2, plotView.Model.Series.Count);
                Assert.AreEqual("Lines", plotView.Model.Series[0].Title);
                Assert.AreEqual("Areas", plotView.Model.Series[1].Title);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenChartDataNotifiesChange_ThenAllSeriesReused()
        {
            // Given
            using (var chart = new ChartControl())
            {
                var plotView = chart.Controls.OfType<LinearPlotView>().First();
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection = new ChartDataCollection("Root collection");
                var nestedChartDataCollection1 = new ChartDataCollection("Nested collection 1");
                var nestedChartDataCollection2 = new ChartDataCollection("Nested collection 2");

                chartDataCollection.Add(chartPointData);
                chartDataCollection.Add(nestedChartDataCollection1);
                nestedChartDataCollection1.Add(chartLineData);
                nestedChartDataCollection1.Add(nestedChartDataCollection2);
                nestedChartDataCollection2.Add(chartAreaData);

                chart.Data = chartDataCollection;

                var seriesBeforeUpdate = plotView.Model.Series.ToList();

                // When
                chartLineData.Points = new Point2D[0];
                chartLineData.NotifyObservers();

                // Then
                CollectionAssert.AreEqual(seriesBeforeUpdate, plotView.Model.Series);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenChartDataRemoved_ThenCorrespondingSeriesRemovedAndOtherSeriesReused()
        {
            // Given
            using (var chart = new ChartControl())
            {
                var plotView = chart.Controls.OfType<LinearPlotView>().First();
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection = new ChartDataCollection("Root collection");
                var nestedChartDataCollection1 = new ChartDataCollection("Nested collection 1");
                var nestedChartDataCollection2 = new ChartDataCollection("Nested collection 2");

                chartDataCollection.Add(chartPointData);
                chartDataCollection.Add(nestedChartDataCollection1);
                nestedChartDataCollection1.Add(chartLineData);
                nestedChartDataCollection1.Add(nestedChartDataCollection2);
                nestedChartDataCollection2.Add(chartAreaData);

                chart.Data = chartDataCollection;

                var seriesBeforeUpdate = plotView.Model.Series.ToList();

                // Precondition
                Assert.AreEqual(3, seriesBeforeUpdate.Count);

                // When
                nestedChartDataCollection1.Remove(chartLineData);
                nestedChartDataCollection1.NotifyObservers();

                // Then
                var series = plotView.Model.Series;
                Assert.AreEqual(2, series.Count);
                Assert.AreEqual("Points", series[0].Title);
                Assert.AreEqual("Areas", series[1].Title);
                Assert.AreEqual(0, series.Except(seriesBeforeUpdate).Count());
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenChartDataAdded_ThenCorrespondingSeriesAddedAndOtherSeriesReused()
        {
            // Given
            using (var chart = new ChartControl())
            {
                var plotView = chart.Controls.OfType<LinearPlotView>().First();
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection = new ChartDataCollection("Root collection");
                var nestedChartDataCollection1 = new ChartDataCollection("Nested collection 1");
                var nestedChartDataCollection2 = new ChartDataCollection("Nested collection 2");

                chartDataCollection.Add(chartPointData);
                chartDataCollection.Add(nestedChartDataCollection1);
                nestedChartDataCollection1.Add(chartLineData);
                nestedChartDataCollection1.Add(nestedChartDataCollection2);
                nestedChartDataCollection2.Add(chartAreaData);

                chart.Data = chartDataCollection;

                var seriesBeforeUpdate = plotView.Model.Series.ToList();

                // Precondition
                Assert.AreEqual(3, seriesBeforeUpdate.Count);

                // When
                nestedChartDataCollection1.Insert(0, new ChartAreaData("Additional areas"));
                nestedChartDataCollection1.NotifyObservers();

                // Then
                var series = plotView.Model.Series;
                Assert.AreEqual(4, series.Count);
                Assert.AreEqual("Points", series[0].Title);
                Assert.AreEqual("Additional areas", series[1].Title);
                Assert.AreEqual("Lines", series[2].Title);
                Assert.AreEqual("Areas", series[3].Title);
                Assert.AreEqual(0, seriesBeforeUpdate.Except(series).Count());
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenChartDataMoved_ThenCorrespondingSeriesMovedAndAllSeriesReused()
        {
            // Given
            using (var chart = new ChartControl())
            {
                var plotView = chart.Controls.OfType<LinearPlotView>().First();
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection = new ChartDataCollection("Root collection");

                chartDataCollection.Add(chartPointData);
                chartDataCollection.Add(chartLineData);
                chartDataCollection.Add(chartAreaData);

                chart.Data = chartDataCollection;

                var seriesBeforeUpdate = plotView.Model.Series.ToList();

                // Precondition
                Assert.AreEqual(3, seriesBeforeUpdate.Count);
                Assert.AreEqual("Points", seriesBeforeUpdate[0].Title);
                Assert.AreEqual("Lines", seriesBeforeUpdate[1].Title);
                Assert.AreEqual("Areas", seriesBeforeUpdate[2].Title);

                // When
                chartDataCollection.Remove(chartPointData);
                chartDataCollection.Add(chartPointData);
                chartDataCollection.NotifyObservers();

                // Then
                var series = plotView.Model.Series;
                Assert.AreEqual(3, series.Count);
                Assert.AreEqual("Lines", series[0].Title);
                Assert.AreEqual("Areas", series[1].Title);
                Assert.AreEqual("Points", series[2].Title);
                Assert.AreEqual(0, seriesBeforeUpdate.Except(series).Count());
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
                var testData = new ChartLineData("test data");
                var collection = new ChartDataCollection("collection");
                var view = TypeUtils.GetField<PlotView>(chart, "plotView");
                var invalidated = 0;

                collection.Add(testData);

                chart.Data = collection;

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
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void BottomAxisTitle_Always_SetsNewTitleToBottomAxis(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var view = TypeUtils.GetField<PlotView>(chart, "plotView");
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
                var view = TypeUtils.GetField<PlotView>(chart, "plotView");
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
                var view = TypeUtils.GetField<PlotView>(chart, "plotView");
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
    }
}