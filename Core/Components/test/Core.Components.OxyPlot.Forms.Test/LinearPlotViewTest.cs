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

using System.Linq;
using System.Windows.Forms;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using Core.Components.OxyPlot.Forms.Properties;
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

            var axes = view.Model.Axes;
            Assert.AreEqual(2, axes.Count);
            CollectionAssert.AllItemsAreInstancesOfType(axes, typeof(LinearAxis));
            CollectionAssert.AreEqual(new [] {Resources.ChartControl_XAxisTitle, Resources.ChartControl_YAxisTitle} , axes.Select(a => a.Title));
            CollectionAssert.AreEqual(new [] {AxisPosition.Bottom, AxisPosition.Left} , axes.Select(a => a.Position));
            CollectionAssert.AreEqual(new [] {TickStyle.None, TickStyle.None} , axes.Select(a => a.TickStyle));
            CollectionAssert.AreEqual(new [] {new[] { 0.0 }, new[] { 0.0 }} , axes.Select(a => a.ExtraGridlines));
            CollectionAssert.AreEqual(new [] {1, 1} , axes.Select(a => a.ExtraGridlineThickness));
            CollectionAssert.AreEqual(new [] {AxisLayer.AboveSeries, AxisLayer.AboveSeries} , axes.Select(a => a.Layer));
            CollectionAssert.AreEqual(new [] {LineStyle.Solid, LineStyle.Solid} , axes.Select(a => a.MajorGridlineStyle));
            CollectionAssert.AreEqual(new [] {LineStyle.Dot,LineStyle.Dot} , axes.Select(a => a.MinorGridlineStyle));
            CollectionAssert.AreEqual(new [] {0.1, 0.1} , axes.Select(a => a.MinimumRange));
        }

        [Test]
        public void ZoomToAll_ViewInForm_InvalidatesView()
        {
            // Setup
            var form = new Form();
            var view = new LinearPlotView();
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
}