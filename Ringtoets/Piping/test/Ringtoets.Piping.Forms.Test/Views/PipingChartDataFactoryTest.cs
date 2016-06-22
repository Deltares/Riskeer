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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

using PipingDataResources = Ringtoets.Piping.Data.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingChartDataFactoryTest
    {
        [Test]
        public void CreateEmptyLineData_Always_ReturnEmptyChartLineDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            ChartLineData chartData = PipingChartDataFactory.CreateEmptyLineData(name);

            // Assert
            Assert.AreEqual(name, chartData.Name);
            Assert.IsEmpty(chartData.Points);
        }

        [Test]
        public void CreateEmptyPointData_Always_ReturnEmptyChartPointDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            ChartPointData chartData = PipingChartDataFactory.CreateEmptyPointData(name);

            // Assert
            Assert.AreEqual(name, chartData.Name);
            Assert.IsEmpty(chartData.Points);
        }

        [Test]
        public void Create_NoSurfaceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void Create_GivenSurfaceLine_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = "Surface line name"
            };
            surfaceLine.SetGeometry(points);

            // Call
            ChartData data = PipingChartDataFactory.Create(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartLineData>(data);
            ChartLineData chartLineData = (ChartLineData) data;
            Assert.AreEqual(2, chartLineData.Points.Count());
            Assert.AreEqual(surfaceLine.Name, data.Name);

            AssertEqualPointCollections(surfaceLine.ProjectGeometryToLZ(), chartLineData.Points);

            AssertEqualStyle(chartLineData.Style, Color.SaddleBrown, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateEntryPoint_EntryPointNaN_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateEntryPoint((RoundedDouble)double.NaN, null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("entryPoint", exception.ParamName);
        }

        [Test]
        public void CreateEntryPoint_SurfaceLineNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateEntryPoint((RoundedDouble) 1.0, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateEntryPoint_GivenSurfaceLine_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);

            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            // Call
            ChartData data = PipingChartDataFactory.CreateEntryPoint(input.EntryPointL, input.SurfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(Resources.PipingInput_EntryPointL_DisplayName, chartPointData.Name);

            Point2D entryPointOnLine = new Point2D(input.EntryPointL, surfaceLine.GetZAtL(input.EntryPointL));
            AssertEqualPointCollections(new[] {entryPointOnLine}, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Blue, 8, Color.Gray, 2, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateExitPoint_ExitPointNaN_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateExitPoint((RoundedDouble)double.NaN, null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("exitPoint", exception.ParamName);
        }

        [Test]
        public void CreateExitPoint_SurfaceLineNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateExitPoint((RoundedDouble)1.0, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateExitPoint_GivenSurfaceLine_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);

            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            // Call
            ChartData data = PipingChartDataFactory.CreateExitPoint(input.ExitPointL, input.SurfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData)data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(Resources.PipingInput_ExitPointL_DisplayName, chartPointData.Name);

            Point2D exitPointOnLine = new Point2D(input.ExitPointL, surfaceLine.GetZAtL(input.ExitPointL));
            AssertEqualPointCollections(new[] { exitPointOnLine }, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Brown, 8, Color.Gray, 2, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateDitchPolderSide_DitchPolderSideNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDitchPolderSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDitchPolderSide_GivenDitchPolderSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D ditchPolderSide = new Point3D(1.2, 2.3, 4.0);
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);
            surfaceLine.SetDitchPolderSideAt(ditchPolderSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateDitchPolderSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData)data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchPolderSide, chartPointData.Name);

            AssertEqualLocalPointCollection(ditchPolderSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Red, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateBottomDitchPolderSide_BottomDitchPolderSideNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateBottomDitchPolderSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateBottomDitchPolderSide_GivenBottomDitchPolderSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D bottomDitchPolderSide = new Point3D(1.2, 2.3, 4.0);
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);
            surfaceLine.SetBottomDitchPolderSideAt(bottomDitchPolderSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateBottomDitchPolderSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData)data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchPolderSide, chartPointData.Name);

            AssertEqualLocalPointCollection(bottomDitchPolderSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Blue, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateBottomDitchDikeSide_BottomDitchDikeSideNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateBottomDitchDikeSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateBottomDitchDikeSide_GivenBottomDitchDikeSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D bottomDitchDikeSide = new Point3D(1.0, 4.3, 6.4);
            var points = new[]
            {
                new Point3D(1.0, 4.3, 6.4),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);
            surfaceLine.SetBottomDitchDikeSideAt(bottomDitchDikeSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateBottomDitchDikeSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData)data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchDikeSide, chartPointData.Name);

            AssertEqualLocalPointCollection(bottomDitchDikeSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Green, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateDitchDikeSide_DitchDikeSideNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDitchDikeSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDitchDikeSide_GivenDitchDikeSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D ditchDikeSide = new Point3D(1.0, 4.3, 6.4);
            var points = new[]
            {
                new Point3D(1.0, 4.3, 6.4),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);
            surfaceLine.SetDitchDikeSideAt(ditchDikeSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateDitchDikeSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData)data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchDikeSide, chartPointData.Name);

            AssertEqualLocalPointCollection(ditchDikeSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Purple, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateDikeToeAtRiver_DikeToeAtRiverNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDikeToeAtRiver(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDikeToeAtRiver_GivenDikeToeAtRivere_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D dikeToeAtRiver = new Point3D(1.2, 2.3, 4.0);
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);
            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            // Call
            ChartData data = PipingChartDataFactory.CreateDikeToeAtRiver(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData)data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtRiver, chartPointData.Name);

            AssertEqualLocalPointCollection(dikeToeAtRiver, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Orange, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateDikeToeAtPolder_DikeToeAtPolderNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDikeToeAtPolder(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDikeToeAtPolder_GivenDikeToeAtPolder_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D dikeToeAtPolder = new Point3D(1.0, 4.3, 6.4);
            var points = new[]
            {
                new Point3D(1.0, 4.3, 6.4),
                new Point3D(2.7, 2.0, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);
            surfaceLine.SetDikeToeAtPolderAt(dikeToeAtPolder);

            // Call
            ChartData data = PipingChartDataFactory.CreateDikeToeAtPolder(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData)data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtPolder, chartPointData.Name);

            AssertEqualLocalPointCollection(dikeToeAtPolder, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Silver, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        private void AssertEqualPointCollections(IEnumerable<Point2D> points, IEnumerable<Point2D> chartPoints)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p.X, p.Y)), chartPoints);
        }

        private void AssertEqualLocalPointCollection(Point3D point, RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<Point2D> chartPoints)
        {
            Point3D first = surfaceLine.Points.First();
            Point3D last = surfaceLine.Points.Last();
            Point2D firstPoint = new Point2D(first.X, first.Y);
            Point2D lastPoint = new Point2D(last.X, last.Y);

            AssertEqualPointCollections(new[] { point.ProjectIntoLocalCoordinates(firstPoint, lastPoint) }, chartPoints);
        }

        private void AssertEqualStyle(ChartLineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        }

        private void AssertEqualStyle(ChartPointStyle pointStyle, Color color, int size, Color strokeColor, int strokeThickness, ChartPointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(size, pointStyle.Size);
            Assert.AreEqual(strokeColor, pointStyle.StrokeColor);
            Assert.AreEqual(strokeThickness, pointStyle.StrokeThickness);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }
    }
}
