// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Components.OxyPlot.CustomSeries;
using NSubstitute;
using NUnit.Framework;
using OxyPlot;

namespace Core.Components.OxyPlot.Test.CustomSeries
{
    [TestFixture]
    public class MultipleAreaSeriesTest
    {
        [Test]
        public void DefaultConstructor_RetunsDefaultValues()
        {
            // Call
            var series = new MultipleAreaSeries();

            // Assert
            Assert.AreEqual(OxyColors.Fuchsia, series.Fill);
            Assert.AreEqual(OxyColors.Fuchsia, series.Color);
            Assert.AreEqual(0, series.StrokeThickness);
            CollectionAssert.IsEmpty(series.Areas);
        }

        [Test]
        public void Render_NoContext_ThrowsArgumentNullException()
        {
            // Setup
            var series = new MultipleAreaSeries();

            // Call
            TestDelegate test = () => series.Render(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("rc", paramName);
        }

        [Test]
        public void Render_NoAreas_NoCallForDrawPolygon()
        {
            // Setup
            var renderContext = Substitute.For<IRenderContext>();

            var series = new MultipleAreaSeries();

            // Call
            series.Render(renderContext);

            // Assert
            renderContext.DidNotReceiveWithAnyArgs().DrawPolygon(null, default, default, default);
        }

        [Test]
        public void Render_EmptyAreas_NoCallForDrawPolygon()
        {
            // Setup
            var renderContext = Substitute.For<IRenderContext>();

            var series = new MultipleAreaSeries
            {
                Areas =
                {
                    new DataPoint[0],
                    new DataPoint[0],
                    new DataPoint[0]
                }
            };

            // Call
            series.Render(renderContext);

            // Assert
            renderContext.DidNotReceiveWithAnyArgs().DrawPolygon(null, default, default, default);
        }

        [Test]
        public void Render_NonEmptyArea_RendersThePoints()
        {
            // Setup
            var random = new Random(21);
            int pointCount = random.Next(5, 455);
            var series = new MultipleAreaSeries();
            var model = new PlotModel();
            model.Series.Add(series);

            var renderContext = Substitute.For<IRenderContext>();
            renderContext.SetClip(Arg.Any<OxyRect>()).Returns(true);

            var area = new DataPoint[pointCount];
            series.Areas.Add(area);

            for (var i = 0; i < pointCount; i++)
            {
                area[i] = new DataPoint(random.Next(-50, 50), random.Next(-50, 50));
            }

            ((IPlotModel) model).Update(false);

            // Call
            series.Render(renderContext);

            // Assert
            renderContext.Received(1).DrawPolygon(
                Arg.Is<ScreenPoint[]>(sp => sp.Length == pointCount),
                Arg.Is<OxyColor>(c => c == series.Fill),
                Arg.Is<OxyColor>(c => c == series.Color),
                Arg.Is<double>(d => d == series.StrokeThickness),
                Arg.Any<double[]>(),
                Arg.Any<LineJoin>(),
                Arg.Any<bool>());
        }

        [Test]
        public void Render_MultipleNonEmptyArea_RendersTheAreas()
        {
            // Setup
            var random = new Random(21);
            int areaCount = random.Next(5, 455);
            var series = new MultipleAreaSeries();
            var model = new PlotModel();
            model.Series.Add(series);

            var renderContext = Substitute.For<IRenderContext>();
            renderContext.SetClip(Arg.Any<OxyRect>()).Returns(true);

            for (var i = 0; i < areaCount; i++)
            {
                series.Areas.Add(new[]
                {
                    new DataPoint(random.Next(-50, 50), random.Next(-50, 50))
                });
            }

            ((IPlotModel) model).Update(false);

            // Call
            series.Render(renderContext);

            // Assert
            renderContext.Received(areaCount).DrawPolygon(
                Arg.Is<ScreenPoint[]>(sp => sp.Length == 1),
                Arg.Is<OxyColor>(c => c == series.Fill),
                Arg.Is<OxyColor>(c => c == series.Color),
                Arg.Is<double>(d => d == series.StrokeThickness),
                Arg.Any<double[]>(),
                Arg.Any<LineJoin>(),
                Arg.Any<bool>());
        }
    }
}