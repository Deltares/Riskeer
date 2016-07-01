﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Components.OxyPlot.CustomSeries;
using NUnit.Framework;
using OxyPlot;
using OxyPlot.Axes;
using Rhino.Mocks;

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
            Assert.AreEqual(OxyColors.Automatic, series.Fill);
            Assert.AreEqual(OxyColors.Fuchsia, series.ActualFill);
            Assert.AreEqual(OxyColors.Automatic, series.Color);
            Assert.AreEqual(OxyColors.Fuchsia, series.ActualColor);
            Assert.AreEqual(0, series.StrokeThickness);
            Assert.IsEmpty(series.Areas);
        }

        [Test]
        public void Render_NoAreas_NoCallForRenderArea()
        {
            // Setup
            var mocks = new MockRepository();
            IRenderContext renderContext = mocks.StrictMock<IRenderContext>();
            mocks.ReplayAll();

            var series = new MultipleAreaSeries();

            // Call
            series.Render(renderContext);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Render_EmptyAreas_NoCallForRenderArea()
        {
            // Setup
            var mocks = new MockRepository();
            IRenderContext renderContext = mocks.StrictMock<IRenderContext>();
            mocks.ReplayAll();

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
            mocks.VerifyAll();
        }

        [Test]
        public void Render_NonEmptyArea_RendersThePoints()
        {
            // Setup
            var random = new Random(21);
            var pointCount = random.Next(5, 455);
            var series = new MultipleAreaSeries();
            var model = new PlotModel();
            model.Series.Add(series);

            var mocks = new MockRepository();
            var renderContext = mocks.Stub<IRenderContext>();
            renderContext.Stub(rc => rc.SetClip(OxyRect.Create(0, 0, 0, 0))).Return(true);
            renderContext.Stub(rc => rc.ResetClip());
            renderContext.Expect(rc => rc.DrawPolygon(
                Arg<ScreenPoint[]>.Matches(sp => sp.Length == pointCount),
                Arg<OxyColor>.Is.Equal(series.ActualFill),
                Arg<OxyColor>.Is.Equal(series.ActualColor),
                Arg<double>.Is.Equal(series.StrokeThickness),
                Arg<double[]>.Is.Anything,
                Arg<LineJoin>.Is.Anything,
                Arg<bool>.Is.Anything));

            mocks.ReplayAll();
            var area = new DataPoint[pointCount];
            series.Areas.Add(area);

            for (var i = 0; i < pointCount; i++)
            {
                area[i] = new DataPoint(random.Next(-50,50), random.Next(-50,50));
            }

            ((IPlotModel)model).Update(false);

            // Call
            series.Render(renderContext);

            // Assert
            mocks.VerifyAll();
        } 

        [Test]
        public void Render_MultipleNonEmptyArea_RendersTheAreas()
        {
            // Setup
            var random = new Random(21);
            var areaCount = random.Next(5, 455);
            var series = new MultipleAreaSeries();
            var model = new PlotModel();
            model.Series.Add(series);

            var mocks = new MockRepository();
            var renderContext = mocks.Stub<IRenderContext>();
            renderContext.Stub(rc => rc.SetClip(OxyRect.Create(0, 0, 0, 0))).Return(true);
            renderContext.Stub(rc => rc.ResetClip());
            renderContext.Expect(rc => rc.DrawPolygon(
                Arg<ScreenPoint[]>.Matches(sp => sp.Length > 0),
                Arg<OxyColor>.Is.Equal(series.ActualFill),
                Arg<OxyColor>.Is.Equal(series.ActualColor),
                Arg<double>.Is.Equal(series.StrokeThickness),
                Arg<double[]>.Is.Anything,
                Arg<LineJoin>.Is.Anything,
                Arg<bool>.Is.Anything)).Repeat.Times(areaCount);

            mocks.ReplayAll();
            
            for (var i = 0; i < areaCount; i++)
            {
                series.Areas.Add(new []
                {
                    new DataPoint(random.Next(-50, 50), random.Next(-50, 50))
                });
            }

            ((IPlotModel)model).Update(false);

            // Call
            series.Render(renderContext);

            // Assert
            mocks.VerifyAll();
        }
    }
}