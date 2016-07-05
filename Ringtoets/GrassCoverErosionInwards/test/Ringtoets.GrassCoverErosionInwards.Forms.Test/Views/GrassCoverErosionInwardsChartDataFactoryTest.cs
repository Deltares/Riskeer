// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
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
            TestDelegate call = () => GrassCoverErosionInwardsChartDataFactory.Create((RoughnessPoint[]) null, "dike profile name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dikeGeometry", exception.ParamName);
        }

        [Test]
        public void Create_GivenDikeProfileGeometry_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            var dikeProfile = new DikeProfile(new Point2D(0.0, 0.0), CreateDikeProfileGeometry(), new Point2D[0])
            {
                Name = "dike profile"
            };

            // Call
            ChartData data = GrassCoverErosionInwardsChartDataFactory.Create(dikeProfile.DikeGeometry, dikeProfile.Name);

            // Assert
            Assert.IsInstanceOf<ChartLineData>(data);
            ChartLineData chartLineData = (ChartLineData) data;
            Assert.AreEqual(3, chartLineData.Points.Count());
            var expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                             dikeProfile.Name,
                                             Resources.DikeProfile_DisplayName);
            Assert.AreEqual(expectedName, data.Name);

            AssertEqualPointCollections(dikeProfile.DikeGeometry.Select(dg => dg.Point), chartLineData.Points);
            AssertEqualStyle(chartLineData.Style, Color.SaddleBrown, 2, DashStyle.Solid);
        }

        [Test]
        public void Create_DikeProfileForshoreGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsChartDataFactory.Create((Point2D[]) null, "dike profile name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("foreshoreGeometry", exception.ParamName);
        }

        [Test]
        public void Create_GivenDikeProfileForshoreGeometry_ReturnsChartDataWithDefaultStyling()
        {
            var dikeProfile = new DikeProfile(new Point2D(0.0, 0.0), new RoughnessPoint[0], CreateForshoreGeometry())
            {
                Name = "dike profile"
            };

            // Call
            ChartData data = GrassCoverErosionInwardsChartDataFactory.Create(dikeProfile.ForeshoreGeometry.ToArray(), dikeProfile.Name);

            // Assert
            Assert.IsInstanceOf<ChartLineData>(data);
            ChartLineData chartLineData = (ChartLineData) data;
            Assert.AreEqual(3, chartLineData.Points.Count());
            var expectedName = string.Format(Resources.GrassCoverErosionInwardsChartDataFactory_Create_DataIdentifier_0_DataTypeDisplayName_1_,
                                             dikeProfile.Name,
                                             Resources.Foreshore_DisplayName);
            Assert.AreEqual(expectedName, data.Name);

            AssertEqualPointCollections(dikeProfile.ForeshoreGeometry, chartLineData.Points);
            AssertEqualStyle(chartLineData.Style, Color.DarkOrange, 2, DashStyle.Solid);
        }

        [Test]
        public void Create_DikeHeightNaN_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsChartDataFactory.Create((RoundedDouble) double.NaN, new RoughnessPoint[0], "dike profile name");

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("dikeHeight", exception.ParamName);
        }

        [Test]
        public void Create_DikeHeightNotNaNDikeProfileGeometryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionInwardsChartDataFactory.Create((RoundedDouble) 12.0, null, "dike profile name");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("dikeGeometry", exception.ParamName);
        }

        [Test]
        public void Create_GivenDikeHeightAndDikeGeometry_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            var dikeGeometry = CreateDikeProfileGeometry();
            RoundedDouble dikeHeight = (RoundedDouble) 12.0;
            string name = "dike profile";

            // Call
            ChartData data = GrassCoverErosionInwardsChartDataFactory.Create(dikeHeight, dikeGeometry, name);

            // Assert
            Assert.IsInstanceOf<ChartLineData>(data);
            ChartLineData chartLineData = (ChartLineData) data;
            Assert.AreEqual(2, chartLineData.Points.Count());
            Assert.AreEqual(Resources.DikeHeight_ChartName, data.Name);

            var dikeHeightPoints = new[]
            {
                new Point2D(dikeGeometry.First().Point.X, 12.0),
                new Point2D(dikeGeometry.Last().Point.X, 12.0),
            };

            AssertEqualPointCollections(dikeHeightPoints, chartLineData.Points);
            AssertEqualStyle(chartLineData.Style, Color.MediumSeaGreen, 2, DashStyle.Dash);
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