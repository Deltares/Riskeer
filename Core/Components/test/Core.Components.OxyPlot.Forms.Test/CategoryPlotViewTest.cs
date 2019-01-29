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
using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.WindowsForms;

namespace Core.Components.OxyPlot.Forms.Test
{
    [TestFixture]
    public class CategoryPlotViewTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var plotView = new CategoryPlotView();

            // Assert
            Assert.IsInstanceOf<PlotView>(plotView);
            Assert.AreEqual(DockStyle.Fill, plotView.Dock);

            Assert.IsNull(plotView.ModelTitle);
            Assert.IsNull(plotView.VerticalAxisTitle);

            PlotModel plotModel = plotView.Model;

            ElementCollection<Axis> axes = plotModel.Axes;
            Assert.AreEqual(2, axes.Count);

            Axis categoryAxis = axes.First(ax => ax.GetType() == typeof(CategoryAxis));
            Assert.AreEqual(1, categoryAxis.MinorStep);
            Assert.AreEqual(90, categoryAxis.Angle);

            Assert.AreEqual(-0.5, categoryAxis.AbsoluteMinimum);
            Assert.IsFalse(categoryAxis.IsPanEnabled);
            Assert.IsFalse(categoryAxis.IsZoomEnabled);

            Axis linearAxis = axes.First(ax => ax.GetType() == typeof(LinearAxis));
            Assert.AreEqual(0, linearAxis.MinimumPadding);
            Assert.IsFalse(linearAxis.IsPanEnabled);
            Assert.IsFalse(linearAxis.IsZoomEnabled);
            Assert.IsNull(linearAxis.Title);

            Assert.AreEqual(0, plotModel.LegendBorderThickness);
            Assert.AreEqual(LegendOrientation.Horizontal, plotModel.LegendOrientation);
            Assert.AreEqual(LegendPlacement.Outside, plotModel.LegendPlacement);
            Assert.AreEqual(LegendPosition.TopCenter, plotModel.LegendPosition);
        }

        [Test]
        [TestCase("Title")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("  ")]
        public void ModelTitle_Always_SetsNewTitleToModelAndInvalidatesView(string newTitle)
        {
            // Setup
            using (var form = new Form())
            using (var view = new CategoryPlotView())
            {
                form.Controls.Add(view);
                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();

                // Call
                view.ModelTitle = newTitle;

                // Assert
                Assert.AreEqual(newTitle, view.Model.Title);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        [TestCase("Title")]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("  ")]
        public void VerticalAxisTitle_AlwaysSetsNewVerticalAxisTitleToModelAndInvalidatesView(string newTitle)
        {
            // Setup 
            using (var form = new Form())
            using (var view = new CategoryPlotView())
            {
                form.Controls.Add(view);
                var invalidated = 0;
                view.Invalidated += (sender, args) => invalidated++;

                form.Show();

                // Call
                view.VerticalAxisTitle = newTitle;

                // Assert
                PlotModel plotModel = view.Model;

                ElementCollection<Axis> axes = plotModel.Axes;
                var categoryAxis = (LinearAxis) axes.First(ax => ax.GetType() == typeof(LinearAxis));
                Assert.AreEqual(newTitle, categoryAxis.Title);
                Assert.AreEqual(1, invalidated);
            }
        }

        [Test]
        public void AddLabels_LabelsNull_ThrowsArgumentNullException()
        {
            // Setup
            var plotView = new CategoryPlotView();

            // Call
            TestDelegate test = () => plotView.AddLabels(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("labels", exception.ParamName);
        }

        [Test]
        public void AddLabel_WithLabel_LabelAddedAndAbsoluteMaximumSet()
        {
            // Setup
            const string label = "Test";
            var plotView = new CategoryPlotView();

            // Call
            plotView.AddLabels(new[]
            {
                label
            });

            // Assert
            CategoryAxis axis = plotView.Model.Axes.OfType<CategoryAxis>().First();
            Assert.AreEqual(1, axis.Labels.Count);
            Assert.AreEqual(label, axis.Labels[0]);
            Assert.AreEqual(0.5, axis.AbsoluteMaximum);
        }

        [Test]
        public void AddLabel_EmptyLabels_AbsoluteMaximumSet()
        {
            // Setup
            var plotView = new CategoryPlotView();

            // Call
            plotView.AddLabels(Enumerable.Empty<string>());

            // Assert
            CategoryAxis axis = plotView.Model.Axes.OfType<CategoryAxis>().First();
            Assert.AreEqual(0, axis.Labels.Count);
            Assert.AreEqual(0, axis.AbsoluteMaximum);
        }

        [Test]
        public void ClearLabels_Always_LabelsCleared()
        {
            // Setup
            var plotView = new CategoryPlotView();
            plotView.AddLabels(new[]
            {
                "Test"
            });

            // Precondition
            CategoryAxis axis = plotView.Model.Axes.OfType<CategoryAxis>().First();
            Assert.AreEqual(1, axis.Labels.Count);

            // Call
            plotView.ClearLabels();

            // Assert
            CollectionAssert.IsEmpty(axis.Labels);
        }
    }
}