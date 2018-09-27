// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

                LinearPlotView plotView = chart.Controls.OfType<LinearPlotView>().Single();
                Assert.AreEqual(Color.White, plotView.BackColor);
                Assert.IsFalse(plotView.Model.IsLegendVisible);
            }
        }

        [Test]
        public void GivenChartControlWithoutData_WhenDataSetToChartDataCollection_ThenChartControlUpdated()
        {
            // Given
            using (var chart = new ChartControl())
            {
                LinearPlotView plotView = chart.Controls.OfType<LinearPlotView>().Single();
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
                ElementCollection<Series> series = plotView.Model.Series;
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
                LinearPlotView plotView = chart.Controls.OfType<LinearPlotView>().Single();
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
                LinearPlotView plotView = chart.Controls.OfType<LinearPlotView>().Single();
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

                List<Series> seriesBeforeUpdate = plotView.Model.Series.ToList();

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
                LinearPlotView plotView = chart.Controls.OfType<LinearPlotView>().Single();
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

                List<Series> seriesBeforeUpdate = plotView.Model.Series.ToList();

                // Precondition
                Assert.AreEqual(3, seriesBeforeUpdate.Count);

                // When
                nestedChartDataCollection1.Remove(chartLineData);
                nestedChartDataCollection1.NotifyObservers();

                // Then
                ElementCollection<Series> series = plotView.Model.Series;
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
                LinearPlotView plotView = chart.Controls.OfType<LinearPlotView>().Single();
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

                List<Series> seriesBeforeUpdate = plotView.Model.Series.ToList();

                // Precondition
                Assert.AreEqual(3, seriesBeforeUpdate.Count);

                // When
                nestedChartDataCollection1.Insert(0, new ChartAreaData("Additional areas"));
                nestedChartDataCollection1.NotifyObservers();

                // Then
                ElementCollection<Series> series = plotView.Model.Series;
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
                LinearPlotView plotView = chart.Controls.OfType<LinearPlotView>().Single();
                var chartPointData = new ChartPointData("Points");
                var chartLineData = new ChartLineData("Lines");
                var chartAreaData = new ChartAreaData("Areas");
                var chartDataCollection = new ChartDataCollection("Root collection");

                chartDataCollection.Add(chartPointData);
                chartDataCollection.Add(chartLineData);
                chartDataCollection.Add(chartAreaData);

                chart.Data = chartDataCollection;

                List<Series> seriesBeforeUpdate = plotView.Model.Series.ToList();

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
                ElementCollection<Series> series = plotView.Model.Series;
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
        [TestCase("Title")]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void BottomAxisTitle_Always_SetsNewTitleToBottomAxis(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
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
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
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
        public void SetChartTitle_Always_SetsNewTitleToModelAndViewInvalidated(string newTitle)
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
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

        private void AssertExpectedExtent(ElementCollection<Axis> modelAxes, Extent expectedExtent)
        {
            const double accuracy = 1e-8;
            Assert.AreEqual(expectedExtent.XMin, modelAxes[0].ActualMinimum, Math.Abs(expectedExtent.XMin * accuracy));
            Assert.AreEqual(expectedExtent.XMax, modelAxes[0].ActualMaximum, Math.Abs(expectedExtent.XMax * accuracy));
            Assert.AreEqual(expectedExtent.YMin, modelAxes[1].ActualMinimum, Math.Abs(expectedExtent.YMin * accuracy));
            Assert.AreEqual(expectedExtent.YMax, modelAxes[1].ActualMaximum, Math.Abs(expectedExtent.YMax * accuracy));
        }

        #region ZoomToAllVisibleLayers

        [Test]
        public void ZoomToAllVisibleLayers_ChartInForm_ViewInvalidatedSeriesSame()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var testData = new ChartLineData("test data")
                {
                    Points = new[]
                    {
                        new Point2D(2, 3)
                    }
                };
                var collection = new ChartDataCollection("collection");
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
                var invalidated = 0;

                collection.Add(testData);

                chart.Data = collection;

                List<Series> series = view.Model.Series.ToList();

                form.Controls.Add(chart);
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();
                view.Update();

                // Call
                chart.ZoomToAllVisibleLayers();

                // Assert
                Assert.AreEqual(1, invalidated);
                CollectionAssert.AreEqual(series, view.Model.Series);
            }
        }

        [Test]
        public void ZoomToAllVisibleLayers_ChartWithoutDataInForm_ViewNotInvalidated()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                var testData = new ChartLineData("test data");
                var collection = new ChartDataCollection("collection");
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
                var invalidated = 0;

                collection.Add(testData);

                chart.Data = collection;

                List<Series> series = view.Model.Series.ToList();

                form.Controls.Add(chart);
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();
                view.Update();

                // Call
                chart.ZoomToAllVisibleLayers();

                // Assert
                Assert.AreEqual(0, invalidated);
                CollectionAssert.AreEqual(series, view.Model.Series);
            }
        }

        [Test]
        public void ZoomToAllVisibleLayers_NotAllLayersVisible_ZoomToVisibleLayersExtent()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
                form.Controls.Add(chart);
                form.Show();

                var collection = new ChartDataCollection("collection");
                collection.Add(new ChartLineData("test data")
                {
                    Points = new[]
                    {
                        new Point2D(1, 5),
                        new Point2D(4, 3)
                    }
                });
                collection.Add(new ChartPointData("test data")
                {
                    Points = new[]
                    {
                        new Point2D(8, 2),
                        new Point2D(1, 1),
                        new Point2D(1, 4)
                    }
                });
                collection.Add(new ChartAreaData("test data")
                {
                    IsVisible = false,
                    Points = new[]
                    {
                        new Point2D(1, 2),
                        new Point2D(2, 3),
                        new Point2D(3, 3),
                        new Point2D(2, -1)
                    }
                });
                chart.Data = collection;
                chart.Update();

                var expectedExtent = new Extent(1 - 0.07, 8 + 0.07, 1 - 0.04, 5 + 0.04);

                // Precondition
                Assert.AreEqual(3, view.Model.Series.Count, "Precondition failed: expected 3 series.");
                Assert.IsFalse(view.Model.Series.All(l => l.IsVisible), "Precondition failed: not all series should be visible.");

                // Call
                chart.ZoomToAllVisibleLayers();

                // Assert
                AssertExpectedExtent(view.Model.Axes, expectedExtent);
            }
        }

        [Test]
        [TestCase(5.0, 5.0)]
        [TestCase(45.3, 1.0)]
        [TestCase(1.0, 122.9)]
        [TestCase(1.0e12 * 0.55, 1.0e12 * 0.55)]
        public void ZoomToAllVisibleLayers_LayersOfVariousDimensions_ZoomToVisibleLayersExtent(double xMax, double yMax)
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
                form.Controls.Add(chart);
                form.Show();

                var collection = new ChartDataCollection("collection");
                collection.Add(new ChartLineData("test data")
                {
                    IsVisible = true,
                    Points = new[]
                    {
                        new Point2D(0.0, 0.0),
                        new Point2D(xMax, yMax)
                    }
                });

                chart.Data = collection;
                chart.Update();

                double xMargin = xMax * 0.01;
                double yMargin = yMax * 0.01;
                var expectedExtent = new Extent(-xMargin, xMax + xMargin, -yMargin, yMax + yMargin);

                // Call
                chart.ZoomToAllVisibleLayers();

                // Assert
                AssertExpectedExtent(view.Model.Axes, expectedExtent);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleLayers_WithNonChildChartData_ThrowArgumentException()
        {
            // Setup
            using (var chart = new ChartControl())
            {
                var chartDataCollection = new ChartDataCollection("Collection");
                var chartData = new ChartLineData("Test data");

                chart.Data = chartDataCollection;
                chart.Update();

                // Call
                TestDelegate call = () => chart.ZoomToAllVisibleLayers(chartData);

                // Assert
                const string message = "Can only zoom to ChartData that is part of this ChartControls drawn chartData.";
                string paramName = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, message).ParamName;
                Assert.AreEqual("chartData", paramName);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleLayers_ChartInFormWithEmptyDataSetAndZoomChildChartData_ViewNotInvalidated()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
                form.Controls.Add(chart);
                form.Show();

                var chartDataCollection = new ChartDataCollection("Collection");
                var chartData = new ChartPointData("Test data");
                var invalidated = 0;

                chartDataCollection.Add(chartData);

                chart.Data = chartDataCollection;
                chart.Update();

                var expectedExtent = new Extent(
                    view.Model.Axes[0].ActualMinimum,
                    view.Model.Axes[0].ActualMaximum,
                    view.Model.Axes[1].ActualMinimum,
                    view.Model.Axes[1].ActualMaximum);

                view.Invalidated += (sender, args) => invalidated++;

                // Call
                chart.ZoomToAllVisibleLayers(chartData);

                // Assert
                Assert.AreEqual(0, invalidated);
                AssertExpectedExtent(view.Model.Axes, expectedExtent);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void ZoomToAllVisibleLayers_ChartInFormForChildChartData_ViewInvalidatedLayersSame()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
                form.Controls.Add(chart);
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

                chart.Data = chartDataCollection;
                chart.Update();

                view.Invalidated += (sender, args) => invalidated++;

                // Call
                chart.ZoomToAllVisibleLayers(chartData);

                // Assert
                Assert.AreEqual(1, invalidated);

                var expectedExtent = new Extent(
                    -0.01,
                    1.01,
                    -0.01,
                    1.01);

                AssertExpectedExtent(view.Model.Axes, expectedExtent);
            }
        }

        [Test]
        public void ZoomToAllVisibleLayers_ForInvisibleChildChartData_DoNotChangeViewExtentsOfChartView()
        {
            // Setup
            using (var form = new Form())
            {
                var chart = new ChartControl();
                LinearPlotView view = chart.Controls.OfType<LinearPlotView>().Single();
                form.Controls.Add(chart);
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
                chart.Data = chartDataCollection;
                chart.Update();

                var expectedExtent = new Extent(
                    view.Model.Axes[0].ActualMinimum,
                    view.Model.Axes[0].ActualMaximum,
                    view.Model.Axes[1].ActualMinimum,
                    view.Model.Axes[1].ActualMaximum);

                // Call
                chart.ZoomToAllVisibleLayers(chartData);

                // Assert
                AssertExpectedExtent(view.Model.Axes, expectedExtent);
            }
        }

        #endregion
    }
}