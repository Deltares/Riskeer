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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Components.Stack.Forms;
using NUnit.Framework;

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
    }
}