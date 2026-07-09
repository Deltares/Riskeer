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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
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
    public class MultipleLineSeriesTest
    {
        [Test]
        public void DefaultConstructor_RetunsDefaultValues()
        {
            // Call
            var series = new MultipleLineSeries();

            // Assert
            Assert.AreEqual(OxyColors.Fuchsia, series.Color);
            Assert.AreEqual(0, series.StrokeThickness);
            Assert.AreEqual(LineStyle.Solid, series.LineStyle);
            CollectionAssert.IsEmpty(series.Lines);
        }

        [Test]
        public void Render_NoContext_ThrowsArgumentNullException()
        {
            var series = new MultipleLineSeries();

            // Call
            TestDelegate test = () => series.Render(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("rc", paramName);
        }

        [Test]
        public void Render_NoLines_NoCallForDrawLine()
        {
            // Setup
            var renderContext = Substitute.For<IRenderContext>();

            var series = new MultipleLineSeries();

            // Call
            series.Render(renderContext);

            // Assert
            renderContext.DidNotReceiveWithAnyArgs().DrawLine(null, default, default, null, default, default);
        }

        [Test]
        public void Render_EmptyLines_NoCallForDrawLine()
        {
            // Setup
            var renderContext = Substitute.For<IRenderContext>();

            var series = new MultipleLineSeries
            {
                Lines =
                {
                    new DataPoint[0],
                    new DataPoint[0],
                    new DataPoint[0]
                }
            };

            // Call
            series.Render(renderContext);

            // Assert
            renderContext.DidNotReceiveWithAnyArgs().DrawLine(null, default, default, null, default, default);
        }

        [Test]
        [TestCase(null, new[]
        {
            3.0,
            2.3
        })]
        [TestCase(LineStyle.DashDashDot, null)]
        [TestCase(LineStyle.DashDashDot, new[]
        {
            4.1,
            1.25
        })]
        public void Render_NonEmptyLine_RendersThePoints(LineStyle? style, double[] dashes)
        {
            // Setup
            var random = new Random(21);
            int pointCount = random.Next(5, 455);
            var series = new MultipleLineSeries();
            if (style.HasValue)
            {
                series.LineStyle = style.Value;
            }

            series.Dashes = dashes;
            var model = new PlotModel();
            model.Series.Add(series);

            var renderContext = Substitute.For<IRenderContext>();
            renderContext.SetClip(Arg.Any<OxyRect>()).Returns(true);

            var line = new DataPoint[pointCount];
            series.Lines.Add(line);

            for (var i = 0; i < pointCount; i++)
            {
                line[i] = new DataPoint(random.Next(-50, 50), random.Next(-50, 50));
            }

            ((IPlotModel) model).Update(false);

            // Call
            series.Render(renderContext);

            // Assert
            double[] expectedDashes = dashes ?? style.Value.GetDashArray();
            renderContext.Received(1).DrawLine(
                Arg.Is<ScreenPoint[]>(sp => sp.Length == pointCount),
                Arg.Is<OxyColor>(c => c == series.Color),
                Arg.Is<double>(d => d == series.StrokeThickness),
                Arg.Is<double[]>(d => Equals(d, expectedDashes)),
                Arg.Any<LineJoin>(),
                Arg.Any<bool>());
        }

        [Test]
        [TestCase(null, new[]
        {
            3.0,
            2.3
        })]
        [TestCase(LineStyle.DashDashDot, null)]
        [TestCase(LineStyle.DashDashDot, new[]
        {
            4.1,
            1.25
        })]
        public void Render_MultipleNonEmptyLine_RendersTheLines(LineStyle? style, double[] dashes)
        {
            // Setup
            var random = new Random(21);
            int lineCount = random.Next(5, 455);
            var series = new MultipleLineSeries();
            if (style.HasValue)
            {
                series.LineStyle = style.Value;
            }

            series.Dashes = dashes;
            var model = new PlotModel();
            model.Series.Add(series);

            var renderContext = Substitute.For<IRenderContext>();
            renderContext.SetClip(Arg.Any<OxyRect>()).Returns(true);

            for (var i = 0; i < lineCount; i++)
            {
                series.Lines.Add(new[]
                {
                    new DataPoint(random.Next(-50, 50), random.Next(-50, 50))
                });
            }

            ((IPlotModel) model).Update(false);

            // Call
            series.Render(renderContext);

            // Assert
            double[] expectedDashes = dashes ?? style.Value.GetDashArray();
            renderContext.Received(lineCount).DrawLine(
                Arg.Is<ScreenPoint[]>(sp => sp.Length == 1),
                Arg.Is<OxyColor>(c => c == series.Color),
                Arg.Is<double>(d => d == series.StrokeThickness),
                Arg.Is<double[]>(d => Equals(d, expectedDashes)),
                Arg.Any<LineJoin>(),
                Arg.Any<bool>());
        }
    }
}