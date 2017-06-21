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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Components.Stack.Data;
using Core.Components.Stack.Forms;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

namespace Core.Components.OxyPlot.Forms.Test
{
    [TestFixture]
    public class StackChartControlTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var chart = new StackChartControl();

            // Assert
            Assert.IsInstanceOf<Control>(chart);
            Assert.IsInstanceOf<IStackChartControl>(chart);

            Assert.IsNull(chart.Data);

            CategoryPlotView plotView = chart.Controls.OfType<CategoryPlotView>().First();
            Assert.AreEqual(Color.White, plotView.BackColor);
            Assert.IsTrue(plotView.Model.IsLegendVisible);
        }

        [Test]
        public void GivenChartControlWithoutData_WhenDataSetToStackChartData_ThenChartControlUpdated()
        {
            // Given
            using (var chart = new StackChartControl())
            {
                var data = new StackChartData();
                data.AddColumn("Column 1");
                data.AddColumn("Column 2");
                data.AddRow("Row 1", new List<double>
                {
                    0.4,
                    0.2
                });
                data.AddRow("Row 2", new List<double>
                {
                    0.6,
                    0.8
                });

                // When
                chart.Data = data;

                // Then
                CategoryPlotView plotView = chart.Controls.OfType<CategoryPlotView>().First();
                AssertSeriesAndColumns(plotView);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenDataSetToOtherStackChartData_ThenChartControlUpdated()
        {
            // Given
            using (var chart = new StackChartControl())
            {
                var data = new StackChartData();
                data.AddColumn("Column 1");
                data.AddColumn("Column 2");
                data.AddRow("Row 1", new List<double>
                {
                    0.4,
                    0.2
                });
                data.AddRow("Row 2", new List<double>
                {
                    0.6,
                    0.8
                });

                chart.Data = data;

                // Precondition
                CategoryPlotView plotView = chart.Controls.OfType<CategoryPlotView>().First();
                AssertSeriesAndColumns(plotView);

                // When
                var newData = new StackChartData();
                newData.AddColumn("Column 3");
                newData.AddColumn("Column 4");
                newData.AddRow("Row 3", new List<double>
                {
                    0.5,
                    0.7
                });
                newData.AddRow("Row 4", new List<double>
                {
                    0.5,
                    0.3
                });
                chart.Data = newData;

                // Then
                ElementCollection<Series> series = plotView.Model.Series;
                Assert.AreEqual(2, series.Count);
                Assert.AreEqual("Row 3", series[0].Title);
                Assert.AreEqual("Row 4", series[1].Title);

                CategoryAxis axis = plotView.Model.Axes.OfType<CategoryAxis>().First();

                Assert.AreEqual(2, axis.Labels.Count);
                Assert.AreEqual("Column 3", axis.Labels[0]);
                Assert.AreEqual("Column 4", axis.Labels[1]);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenDataSetToNull_ThenChartControlUpdated()
        {
            // Given
            using (var chart = new StackChartControl())
            {
                var data = new StackChartData();
                data.AddColumn("Column 1");
                data.AddColumn("Column 2");
                data.AddRow("Row 1", new List<double>
                {
                    0.4,
                    0.2
                });
                data.AddRow("Row 2", new List<double>
                {
                    0.6,
                    0.8
                });

                chart.Data = data;

                // Precondition
                CategoryPlotView plotView = chart.Controls.OfType<CategoryPlotView>().First();
                AssertSeriesAndColumns(plotView);

                // When
                chart.Data = null;

                // Then
                ElementCollection<Series> series = plotView.Model.Series;
                CollectionAssert.IsEmpty(series);

                CategoryAxis axis = plotView.Model.Axes.OfType<CategoryAxis>().First();

                CollectionAssert.IsEmpty(axis.Labels);
            }
        }

        [Test]
        public void GivenChartControlWithData_WhenDataNotified_ThenChartControlUpdated()
        {
            // Given
            using (var chart = new StackChartControl())
            {
                var data = new StackChartData();
                data.AddColumn("Column 1");
                data.AddColumn("Column 2");
                data.AddRow("Row 1", new List<double>
                {
                    0.4,
                    0.2
                });
                data.AddRow("Row 2", new List<double>
                {
                    0.6,
                    0.8
                });

                chart.Data = data;

                CategoryPlotView plotView = chart.Controls.OfType<CategoryPlotView>().First();
                AssertSeriesAndColumns(plotView);

                // When
                data.Clear();
                data.AddColumn("New column 1");
                data.AddColumn("New column 2");
                data.AddRow("New row 1", new List<double>
                {
                    0.3,
                    0.8
                });
                data.AddRow("New row 2", new List<double>
                {
                    0.8,
                    0.2
                });
                data.NotifyObservers();

                // Then
                ElementCollection<Series> series = plotView.Model.Series;
                Assert.AreEqual(2, series.Count);
                Assert.AreEqual("New row 1", series[0].Title);
                Assert.AreEqual("New row 2", series[1].Title);

                CategoryAxis axis = plotView.Model.Axes.OfType<CategoryAxis>().First();

                Assert.AreEqual(2, axis.Labels.Count);
                Assert.AreEqual("New column 1", axis.Labels[0]);
                Assert.AreEqual("New column 2", axis.Labels[1]);
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
                var chart = new StackChartControl();
                CategoryPlotView view = chart.Controls.OfType<CategoryPlotView>().First();
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

        private static void AssertSeriesAndColumns(CategoryPlotView plotView)
        {
            ElementCollection<Series> series = plotView.Model.Series;
            Assert.AreEqual(2, series.Count);
            Assert.AreEqual("Row 1", series[0].Title);
            Assert.AreEqual("Row 2", series[1].Title);

            CategoryAxis axis = plotView.Model.Axes.OfType<CategoryAxis>().First();

            Assert.AreEqual(2, axis.Labels.Count);
            Assert.AreEqual("Column 1", axis.Labels[0]);
            Assert.AreEqual("Column 2", axis.Labels[1]);
        }
    }
}