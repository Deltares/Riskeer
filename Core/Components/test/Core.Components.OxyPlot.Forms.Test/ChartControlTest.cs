// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Components.Chart.Data;
using Core.Components.Chart.Forms;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Forms.Test
{
    [TestFixture]
    public class ChartControlTest
    {
        [Test]
        public void Constructor_PropertiesSet()
        {
            // Call
            using (var chartControl = new ChartControl())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(chartControl);
                Assert.IsInstanceOf<IChartControl>(chartControl);
                Assert.IsNull(chartControl.Data);

                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                Assert.AreEqual(Color.White, linearPlotView.BackColor);
                Assert.IsFalse(linearPlotView.Model.IsLegendVisible);
            }
        }

        [Test]
        public void GivenChartControlWithoutData_WhenDataSetToChartDataCollection_ThenChartControlUpdated()
        {
            // Given
            using (var chartControl = new ChartControl())
            {
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
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
                chartControl.Data = chartDataCollection;

                // Then
                ElementCollection<Series> series = linearPlotView.Model.Series;
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
            using (var chartControl = new ChartControl())
            {
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection1 = new ChartDataCollection("Collection 1");
                var chartDataCollection2 = new ChartDataCollection("Collection 2");

                chartDataCollection1.Add(chartPointData);
                chartDataCollection2.Add(chartLineData);
                chartDataCollection2.Add(chartAreaData);

                chartControl.Data = chartDataCollection1;

                // Precondition
                Assert.AreEqual(1, linearPlotView.Model.Series.Count);
                Assert.AreEqual("Points", linearPlotView.Model.Series[0].Title);

                // When
                chartControl.Data = chartDataCollection2;

                // Then
                Assert.AreEqual(2, linearPlotView.Model.Series.Count);
                Assert.AreEqual("Lines", linearPlotView.Model.Series[0].Title);
                Assert.AreEqual("Areas", linearPlotView.Model.Series[1].Title);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenChartDataNotifiesChange_ThenAllSeriesReused()
        {
            // Given
            using (var chartControl = new ChartControl())
            {
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
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

                chartControl.Data = chartDataCollection;

                List<Series> seriesBeforeUpdate = linearPlotView.Model.Series.ToList();

                // When
                chartLineData.Points = new Point2D[0];
                chartLineData.NotifyObservers();

                // Then
                CollectionAssert.AreEqual(seriesBeforeUpdate, linearPlotView.Model.Series);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenChartDataRemoved_ThenCorrespondingSeriesRemovedAndOtherSeriesReused()
        {
            // Given
            using (var chartControl = new ChartControl())
            {
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
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

                chartControl.Data = chartDataCollection;

                List<Series> seriesBeforeUpdate = linearPlotView.Model.Series.ToList();

                // Precondition
                Assert.AreEqual(3, seriesBeforeUpdate.Count);

                // When
                nestedChartDataCollection1.Remove(chartLineData);
                nestedChartDataCollection1.NotifyObservers();

                // Then
                ElementCollection<Series> series = linearPlotView.Model.Series;
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
            using (var chartControl = new ChartControl())
            {
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
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

                chartControl.Data = chartDataCollection;

                List<Series> seriesBeforeUpdate = linearPlotView.Model.Series.ToList();

                // Precondition
                Assert.AreEqual(3, seriesBeforeUpdate.Count);

                // When
                nestedChartDataCollection1.Insert(0, new ChartAreaData("Additional areas"));
                nestedChartDataCollection1.NotifyObservers();

                // Then
                ElementCollection<Series> series = linearPlotView.Model.Series;
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
            using (var chartControl = new ChartControl())
            {
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection = new ChartDataCollection("Root collection");

                chartDataCollection.Add(chartPointData);
                chartDataCollection.Add(chartLineData);
                chartDataCollection.Add(chartAreaData);

                chartControl.Data = chartDataCollection;

                List<Series> seriesBeforeUpdate = linearPlotView.Model.Series.ToList();

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
                ElementCollection<Series> series = linearPlotView.Model.Series;
                Assert.AreEqual(3, series.Count);
                Assert.AreEqual("Lines", series[0].Title);
                Assert.AreEqual("Areas", series[1].Title);
                Assert.AreEqual("Points", series[2].Title);
                Assert.AreEqual(0, seriesBeforeUpdate.Except(series).Count());
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void BottomAxisTitle_Always_SetsNewTitleToBottomAxis(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chartControl = new ChartControl();
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                form.Controls.Add(chartControl);

                form.Show();

                var invalidated = 0;
                linearPlotView.Invalidated += (sender, args) => invalidated++;

                // Call
                chartControl.BottomAxisTitle = newTitle;

                // Assert
                Assert.AreEqual(chartControl.BottomAxisTitle, newTitle);
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
                var chartControl = new ChartControl();
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                form.Controls.Add(chartControl);

                form.Show();

                var invalidated = 0;
                linearPlotView.Invalidated += (sender, args) => invalidated++;

                // Call
                chartControl.LeftAxisTitle = newTitle;

                // Assert
                Assert.AreEqual(chartControl.LeftAxisTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void SetChartTitle_Always_SetsNewTitleToModelAndViewInvalidated(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chartControl = new ChartControl();
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                form.Controls.Add(chartControl);

                form.Show();

                var invalidated = 0;
                linearPlotView.Invalidated += (sender, args) => invalidated++;

                // Call
                chartControl.ChartTitle = newTitle;

                // Assert
                Assert.AreEqual(chartControl.ChartTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        private static void AssertExpectedExtent(ElementCollection<Axis> modelAxes, Extent expectedExtent)
        {
            const double accuracy = 1e-8;
            Assert.AreEqual(expectedExtent.XMin, modelAxes[0].ActualMinimum, Math.Abs(expectedExtent.XMin * accuracy));
            Assert.AreEqual(expectedExtent.XMax, modelAxes[0].ActualMaximum, Math.Abs(expectedExtent.XMax * accuracy));
            Assert.AreEqual(expectedExtent.YMin, modelAxes[1].ActualMinimum, Math.Abs(expectedExtent.YMin * accuracy));
            Assert.AreEqual(expectedExtent.YMax, modelAxes[1].ActualMaximum, Math.Abs(expectedExtent.YMax * accuracy));
        }

        private static LinearPlotView GetLinearPlotView(ChartControl chartControl)
        {
            return chartControl.Controls[0].Controls.OfType<LinearPlotView>().First();
        }

        #region ZoomToAllVisibleSeries

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleSeries_WithNonChildChartData_ThrowArgumentException()
        {
            // Setup
            using (var chartControl = new ChartControl())
            {
                var chartDataCollection = new ChartDataCollection("Collection");
                var chartData = new ChartLineData("Test data");

                chartControl.Data = chartDataCollection;
                chartControl.Update();

                // Call
                void Call() => chartControl.ZoomToAllVisibleSeries(chartData);

                // Assert
                const string message = "Can only zoom to ChartData that is part of this ChartControls drawn chartData.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(Call, message).ParamName;
                Assert.AreEqual("chartData", paramName);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleSeries_ChartInFormWithEmptyDataSetAndZoomChildChartData_ViewNotInvalidated()
        {
            // Setup
            using (var form = new Form())
            {
                var chartControl = new ChartControl();
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                form.Controls.Add(chartControl);
                form.Show();

                var chartDataCollection = new ChartDataCollection("Collection");
                var chartData = new ChartPointData("Test data");
                var invalidated = 0;

                chartDataCollection.Add(chartData);

                chartControl.Data = chartDataCollection;
                chartControl.Update();

                var expectedExtent = new Extent(
                    linearPlotView.Model.Axes[0].ActualMinimum,
                    linearPlotView.Model.Axes[0].ActualMaximum,
                    linearPlotView.Model.Axes[1].ActualMinimum,
                    linearPlotView.Model.Axes[1].ActualMaximum);

                linearPlotView.Invalidated += (sender, args) => invalidated++;

                // Call
                chartControl.ZoomToAllVisibleSeries(chartData);

                // Assert
                Assert.AreEqual(0, invalidated);
                AssertExpectedExtent(linearPlotView.Model.Axes, expectedExtent);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleSeries_ChartInFormForChildChartData_ViewInvalidatedSeriesSame()
        {
            // Setup
            using (var form = new Form())
            {
                var chartControl = new ChartControl();
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                form.Controls.Add(chartControl);
                form.Show();

                var chartDataCollection = new ChartDataCollection("Collection");
                var chartData = new ChartLineData("test data")
                {
                    IsVisible = true,
                    Points = new[]
                    {
                        new Point2D(0.0, 0.0),
                        new Point2D(1.0, 1.0)
                    }
                };
                chartDataCollection.Add(chartData);
                var invalidated = 0;

                chartControl.Data = chartDataCollection;
                chartControl.Update();

                linearPlotView.Invalidated += (sender, args) => invalidated++;

                // Call
                chartControl.ZoomToAllVisibleSeries(chartData);

                // Assert
                Assert.AreEqual(1, invalidated);

                var expectedExtent = new Extent(
                    -0.01,
                    1.01,
                    -0.01,
                    1.01);

                AssertExpectedExtent(linearPlotView.Model.Axes, expectedExtent);
            }
        }

        [Test]
        public void ZoomToAllVisibleSeries_ForInvisibleChildChartData_DoNotChangeViewExtentsOfChartView()
        {
            // Setup
            using (var form = new Form())
            {
                var chartControl = new ChartControl();
                LinearPlotView linearPlotView = GetLinearPlotView(chartControl);
                form.Controls.Add(chartControl);
                form.Show();

                var chartDataCollection = new ChartDataCollection("Collection");
                var chartData = new ChartLineData("test data")
                {
                    IsVisible = false,
                    Points = new[]
                    {
                        new Point2D(3.2, 4.1),
                        new Point2D(11.2, 5.8)
                    }
                };

                chartDataCollection.Add(chartData);
                chartControl.Data = chartDataCollection;
                chartControl.Update();

                var expectedExtent = new Extent(
                    linearPlotView.Model.Axes[0].ActualMinimum,
                    linearPlotView.Model.Axes[0].ActualMaximum,
                    linearPlotView.Model.Axes[1].ActualMinimum,
                    linearPlotView.Model.Axes[1].ActualMaximum);

                // Call
                chartControl.ZoomToAllVisibleSeries(chartData);

                // Assert
                AssertExpectedExtent(linearPlotView.Model.Axes, expectedExtent);
            }
        }

        #endregion
    }
}