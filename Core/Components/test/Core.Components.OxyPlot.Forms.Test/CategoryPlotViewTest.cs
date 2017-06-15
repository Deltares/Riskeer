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

            PlotModel plotModel = plotView.Model;

            ElementCollection<Axis> axes = plotModel.Axes;
            Assert.AreEqual(1, axes.Count);
            CollectionAssert.AllItemsAreInstancesOfType(axes, typeof(CategoryAxis));

            Assert.AreEqual(0, plotModel.LegendBorderThickness);
            Assert.AreEqual(LegendOrientation.Horizontal, plotModel.LegendOrientation);
            Assert.AreEqual(LegendPlacement.Outside, plotModel.LegendPlacement);
            Assert.AreEqual(LegendPosition.TopCenter, plotModel.LegendPosition);
        }

        [Test]
        [TestCase("Title")]
        [TestCase("Test")]
        [TestCase("Label")]
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
                Assert.AreEqual(view.ModelTitle, newTitle);
                Assert.AreEqual(1, invalidated);
            }
        }
    }
}