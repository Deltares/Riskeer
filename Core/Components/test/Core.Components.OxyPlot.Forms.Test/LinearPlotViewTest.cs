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

using System.Linq;
using System.Windows.Forms;
using Core.Components.OxyPlot.CustomSeries;
using Core.Components.OxyPlot.Forms.Properties;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;
using TickStyle = OxyPlot.Axes.TickStyle;

namespace Core.Components.OxyPlot.Forms.Test
{
    [TestFixture]
    public class LinearPlotViewTest
    {
        [Test]
        public void DefaultConstructor_HasTwoLinearAxes()
        {
            // Call
            var view = new LinearPlotView();

            // Assert
            Assert.IsInstanceOf<PlotView>(view);
            Assert.AreEqual(DockStyle.Fill, view.Dock);

            ElementCollection<Axis> axes = view.Model.Axes;
            Assert.AreEqual(2, axes.Count);
            CollectionAssert.AllItemsAreInstancesOfType(axes, typeof(LinearAxis));
            CollectionAssert.AreEqual(new[]
            {
                Resources.ChartControl_XAxisTitle,
                Resources.ChartControl_YAxisTitle
            }, axes.Select(a => a.Title));
            CollectionAssert.AreEqual(new[]
            {
                AxisPosition.Bottom,
                AxisPosition.Left
            }, axes.Select(a => a.Position));
            CollectionAssert.AreEqual(new[]
            {
                TickStyle.None,
                TickStyle.None
            }, axes.Select(a => a.TickStyle));
            CollectionAssert.AreEqual(new[]
            {
                AxisLayer.AboveSeries,
                AxisLayer.AboveSeries
            }, axes.Select(a => a.Layer));
            CollectionAssert.AreEqual(new[]
            {
                LineStyle.Solid,
                LineStyle.Solid
            }, axes.Select(a => a.MajorGridlineStyle));
            CollectionAssert.AreEqual(new[]
            {
                LineStyle.Dot,
                LineStyle.Dot
            }, axes.Select(a => a.MinorGridlineStyle));
            CollectionAssert.AreEqual(new[]
            {
                0.1,
                0.1
            }, axes.Select(a => a.MinimumRange));
            CollectionAssert.AreEqual(new[]
            {
                1.0e12,
                1.0e12
            }, axes.Select(a => a.MaximumRange));
        }

        [Test]
        public void ZoomToAll_ViewInForm_InvalidatesView()
        {
            // Setup
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);
                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();

                // Call
                view.ZoomToAll();

                // Assert
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        public void SetExtent_RangeLargerThanMinimumRange_AxesUpdated()
        {
            // Setup
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);

                form.Show();

                const int xMin = 3;
                const int xMax = 5;
                const int yMin = 1;
                const int yMax = 2;

                // Call
                view.SetExtent(new Extent(xMin, xMax, yMin, yMax));

                // Assert
                Assert.AreEqual(xMin, view.Model.Axes[0].ActualMinimum);
                Assert.AreEqual(xMax, view.Model.Axes[0].ActualMaximum);
                Assert.AreEqual(yMin, view.Model.Axes[1].ActualMinimum);
                Assert.AreEqual(yMax, view.Model.Axes[1].ActualMaximum);
            }
        }

        [Test]
        public void SetExtent_DataRangeInXSmallerThanMinimumRange_AxesUpdatedWithCorrectionForX()
        {
            // Setup
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);

                form.Show();

                const double range = 0.02;
                const int xMin = 3;
                const int yMin = 1;
                const int yMax = 4;

                // Call
                view.SetExtent(new Extent(xMin, xMin + range, yMin, yMax));

                // Assert
                Assert.AreEqual(xMin - 0.04, view.Model.Axes[0].ActualMinimum);
                Assert.AreEqual(xMin + 0.06, view.Model.Axes[0].ActualMaximum);
                Assert.AreEqual(yMin, view.Model.Axes[1].ActualMinimum);
                Assert.AreEqual(yMax, view.Model.Axes[1].ActualMaximum);
            }
        }

        [Test]
        public void SetExtent_DataRangeInYSmallerThanMinimumRange_AxesUpdatedWithCorrectionForY()
        {
            // Setup
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);

                form.Show();

                const double range = 0.02;
                const int xMin = 3;
                const int xMax = 7;
                const int yMin = 1;

                // Call
                view.SetExtent(new Extent(xMin, xMax, yMin, yMin + range));

                // Assert
                Assert.AreEqual(xMin, view.Model.Axes[0].ActualMinimum);
                Assert.AreEqual(xMax, view.Model.Axes[0].ActualMaximum);
                Assert.AreEqual(yMin - 0.04, view.Model.Axes[1].ActualMinimum);
                Assert.AreEqual(yMin + 0.06, view.Model.Axes[1].ActualMaximum);
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void ModelTitle_Always_SetsNewTitleToModelAndInvalidatesView(string newTitle)
        {
            // Setup
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);
                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();

                // Call
                view.ModelTitle = newTitle;

                // Assert
                Assert.AreEqual(view.ModelTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void BottomAxisTitle_Always_SetsNewTitleToBottomAxisAndInvalidatesView(string newTitle)
        {
            // Setup
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);
                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();

                // Call
                view.BottomAxisTitle = newTitle;

                // Assert
                Assert.AreEqual(view.BottomAxisTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
        public void SetLeftAxisTitle_Always_SetsNewTitleToLeftAxisAndInvalidatesView(string newTitle)
        {
            // Setup
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);
                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();

                // Call
                view.LeftAxisTitle = newTitle;

                // Assert
                Assert.AreEqual(view.LeftAxisTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        public void GivenMultipleAreaSeriesAddedToView_WhenViewOpenedAndUpdated_ThenXYAxisIncludesSeriesValues()
        {
            // Given
            const int maxY = 100;
            const int minY = -25;
            const int maxX = 50;
            const int minX = -10;
            var series = new MultipleAreaSeries
            {
                Areas =
                {
                    new[]
                    {
                        new DataPoint(minX, maxY)
                    },
                    new[]
                    {
                        new DataPoint(maxX, minY)
                    }
                }
            };

            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);

                view.Model.Series.Add(series);

                // When
                form.Show();
                view.Update();
            }

            // Then
            Assert.AreEqual(maxX, series.XAxis.DataMaximum);
            Assert.AreEqual(minX, series.XAxis.DataMinimum);
            Assert.AreEqual(maxY, series.YAxis.DataMaximum);
            Assert.AreEqual(minY, series.YAxis.DataMinimum);
        }

        [Test]
        public void GivenEmptyMultipleAreaSeriesAddedToView_WhenViewOpenedAndUpdated_ThenXYAxisNotChanged()
        {
            // Given

            var series = new MultipleAreaSeries();
            using (var form = new Form())
            using (var view = new LinearPlotView())
            {
                form.Controls.Add(view);
                view.Model.Series.Add(series);

                // When
                form.Show();
                view.Update();
            }

            // Then
            Assert.AreEqual(double.NaN, series.XAxis.DataMaximum);
            Assert.AreEqual(double.NaN, series.XAxis.DataMinimum);
            Assert.AreEqual(double.NaN, series.YAxis.DataMaximum);
            Assert.AreEqual(double.NaN, series.YAxis.DataMinimum);
        }
    }
}