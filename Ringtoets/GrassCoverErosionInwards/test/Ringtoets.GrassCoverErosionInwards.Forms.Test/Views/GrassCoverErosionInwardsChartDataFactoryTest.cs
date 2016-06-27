﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using NUnit.Framework;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.Views;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.Views
{
    [TestFixture]
    public class GrassCoverErosionInwardsChartDataFactoryTest
    {
        [Test]
        public void Create_DikeProfileGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsChartDataFactory.Create((RoughnessPoint[]) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dikeGeometry", exception.ParamName);
        }

        [Test]
        public void Create_GivenDikeProfileGeometry_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0.0, 0.0), CreateDikeProfileGeometry(), new Point2D[0]);

            // Call
            ChartData data = GrassCoverErosionInwardsChartDataFactory.Create(dikeProfile.DikeGeometry);

            // Assert
            Assert.IsInstanceOf<ChartLineData>(data);
            ChartLineData chartLineData = (ChartLineData)data;
            Assert.AreEqual(3, chartLineData.Points.Count());
            Assert.AreEqual(Resources.DikeProfile_DisplayName, data.Name);

            AssertEqualPointCollections(dikeProfile.DikeGeometry.Select(dg => dg.Point), chartLineData.Points);
            AssertEqualStyle(chartLineData.Style, Color.SaddleBrown, 2, DashStyle.Solid);
        }

        [Test]
        public void Create_DikeProfileForshoreGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsChartDataFactory.Create((Point2D[]) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("foreshoreGeometry", exception.ParamName);
        }

        [Test]
        public void Create_GivenDikeProfileForshoreGeometry_ReturnsChartDataWithDefaultStyling()
        {
            var dikeProfile = new DikeProfile(new Point2D(0.0, 0.0), new RoughnessPoint[0], CreateForshoreGeometry());

            // Call
            ChartData data = GrassCoverErosionInwardsChartDataFactory.Create(dikeProfile.ForeshoreGeometry);

            // Assert
            Assert.IsInstanceOf<ChartLineData>(data);
            ChartLineData chartLineData = (ChartLineData)data;
            Assert.AreEqual(3, chartLineData.Points.Count());
            Assert.AreEqual(Resources.Foreshore_DisplayName, data.Name);

            AssertEqualPointCollections(dikeProfile.ForeshoreGeometry, chartLineData.Points);
            AssertEqualStyle(chartLineData.Style, Color.DarkOrange, 2, DashStyle.Solid);
        }

        private void AssertEqualPointCollections(IEnumerable<Point2D> points, IEnumerable<Point2D> chartPoints)
        {
            CollectionAssert.AreEqual(points, chartPoints);
        }

        private void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        }

        private RoughnessPoint[] CreateDikeProfileGeometry()
        {
            return new[]
            {
                new RoughnessPoint(new Point2D(2.0, 3.0), 4.0),
                new RoughnessPoint(new Point2D(3.0, 4.0), 4.0),
                new RoughnessPoint(new Point2D(4.0, 5.0), 4.0)
            };
        }

        private Point2D[] CreateForshoreGeometry()
        {
            return new[]
            {
                new Point2D(8.0, 5.0), 
                new Point2D(9.0, 4.0), 
                new Point2D(10.0, 3.0), 
            };
        }
    }
}