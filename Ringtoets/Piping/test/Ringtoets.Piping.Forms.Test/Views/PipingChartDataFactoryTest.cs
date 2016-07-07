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
            var surfaceLine = GetSurfaceLineWithGeometry();
            surfaceLine.Name = "Surface line name";

            // Call
            ChartData data = PipingChartDataFactory.Create(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartLineData>(data);
            ChartLineData chartLineData = (ChartLineData) data;
            Assert.AreEqual(2, chartLineData.Points.Count());
            Assert.AreEqual(surfaceLine.Name, data.Name);

            AssertEqualPointCollections(surfaceLine.ProjectGeometryToLZ(), chartLineData.Points);

            AssertEqualStyle(chartLineData.Style, Color.Sienna, 2, DashStyle.Solid);
        }

        [Test]
        public void CreateEntryPoint_EntryPointNaN_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateEntryPoint((RoundedDouble) double.NaN, null);

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
            var surfaceLine = GetSurfaceLineWithGeometry();

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
            AssertEqualPointCollections(new[]
            {
                entryPointOnLine
            }, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Gold, 8, Color.Transparent, 0, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateExitPoint_ExitPointNaN_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateExitPoint((RoundedDouble) double.NaN, null);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("exitPoint", exception.ParamName);
        }

        [Test]
        public void CreateExitPoint_SurfaceLineNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateExitPoint((RoundedDouble) 1.0, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateExitPoint_GivenSurfaceLine_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            var surfaceLine = GetSurfaceLineWithGeometry();

            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine
            };

            // Call
            ChartData data = PipingChartDataFactory.CreateExitPoint(input.ExitPointL, input.SurfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(Resources.PipingInput_ExitPointL_DisplayName, chartPointData.Name);

            Point2D exitPointOnLine = new Point2D(input.ExitPointL, surfaceLine.GetZAtL(input.ExitPointL));
            AssertEqualPointCollections(new[]
            {
                exitPointOnLine
            }, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Tomato, 8, Color.Transparent, 0, ChartPointSymbol.Triangle);
        }

        [Test]
        public void CreateDitchPolderSide_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDitchPolderSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDitchPolderSide_DitchPolderSideNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDitchPolderSide(surfaceLine);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("worldCoordinate", exception.ParamName);
        }

        [Test]
        public void CreateDitchPolderSide_GivenDitchPolderSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D ditchPolderSide = new Point3D(1.2, 2.3, 4.0);
            var surfaceLine = GetSurfaceLineWithGeometry();
            surfaceLine.SetDitchPolderSideAt(ditchPolderSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateDitchPolderSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchPolderSide, chartPointData.Name);

            AssertEqualLocalPointCollection(ditchPolderSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.IndianRed, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateBottomDitchPolderSide_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateBottomDitchPolderSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateBottomDitchPolderSide_BottomDitchPolderSideNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateBottomDitchPolderSide(surfaceLine);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("worldCoordinate", exception.ParamName);
        }

        [Test]
        public void CreateBottomDitchPolderSide_GivenBottomDitchPolderSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D bottomDitchPolderSide = new Point3D(1.2, 2.3, 4.0);
            var surfaceLine = GetSurfaceLineWithGeometry();
            surfaceLine.SetBottomDitchPolderSideAt(bottomDitchPolderSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateBottomDitchPolderSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchPolderSide, chartPointData.Name);

            AssertEqualLocalPointCollection(bottomDitchPolderSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.Teal, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateBottomDitchDikeSide_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateBottomDitchDikeSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateBottomDitchDikeSide_BottomDitchDikeSideNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateBottomDitchDikeSide(surfaceLine);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("worldCoordinate", exception.ParamName);
        }

        [Test]
        public void CreateBottomDitchDikeSide_GivenBottomDitchDikeSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D bottomDitchDikeSide = new Point3D(1.2, 2.3, 4.0);
            var surfaceLine = GetSurfaceLineWithGeometry();
            surfaceLine.SetBottomDitchDikeSideAt(bottomDitchDikeSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateBottomDitchDikeSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_BottomDitchDikeSide, chartPointData.Name);

            AssertEqualLocalPointCollection(bottomDitchDikeSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.DarkSeaGreen, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateDitchDikeSide_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDitchDikeSide(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDitchDikeSide_DitchDikeSideNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDitchDikeSide(surfaceLine);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("worldCoordinate", exception.ParamName);
        }

        [Test]
        public void CreateDitchDikeSide_GivenDitchDikeSide_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D ditchDikeSide = new Point3D(1.2, 2.3, 4.0);
            var surfaceLine = GetSurfaceLineWithGeometry();
            surfaceLine.SetDitchDikeSideAt(ditchDikeSide);

            // Call
            ChartData data = PipingChartDataFactory.CreateDitchDikeSide(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DitchDikeSide, chartPointData.Name);

            AssertEqualLocalPointCollection(ditchDikeSide, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.MediumPurple, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateDikeToeAtRiver_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDikeToeAtRiver(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDikeToeAtRiver_DikeToeAtRiverNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDikeToeAtRiver(surfaceLine);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("worldCoordinate", exception.ParamName);
        }

        [Test]
        public void CreateDikeToeAtRiver_GivenDikeToeAtRivere_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D dikeToeAtRiver = new Point3D(1.2, 2.3, 4.0);
            var surfaceLine = GetSurfaceLineWithGeometry();
            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            // Call
            ChartData data = PipingChartDataFactory.CreateDikeToeAtRiver(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtRiver, chartPointData.Name);

            AssertEqualLocalPointCollection(dikeToeAtRiver, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.DarkBlue, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreateDikeToeAtPolder_SurfaceLineNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDikeToeAtPolder(null);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void CreateDikeToeAtPolder_DikeToeAtPolderNull_ThrowsArgumentNullException()
        {
            // Setup
            var surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            TestDelegate call = () => PipingChartDataFactory.CreateDikeToeAtPolder(surfaceLine);

            // Assert 
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("worldCoordinate", exception.ParamName);
        }

        [Test]
        public void CreateDikeToeAtPolder_GivenDikeToeAtPolder_ReturnsChartDataWithDefaultStyling()
        {
            // Setup
            Point3D dikeToeAtPolder = new Point3D(1.2, 2.3, 4.0);
            var surfaceLine = GetSurfaceLineWithGeometry();
            surfaceLine.SetDikeToeAtPolderAt(dikeToeAtPolder);

            // Call
            ChartData data = PipingChartDataFactory.CreateDikeToeAtPolder(surfaceLine);

            // Assert
            Assert.IsInstanceOf<ChartPointData>(data);
            ChartPointData chartPointData = (ChartPointData) data;
            Assert.AreEqual(1, chartPointData.Points.Count());
            Assert.AreEqual(PipingDataResources.CharacteristicPoint_DikeToeAtPolder, chartPointData.Name);

            AssertEqualLocalPointCollection(dikeToeAtPolder, surfaceLine, chartPointData.Points);

            AssertEqualStyle(chartPointData.Style, Color.SlateGray, 8, Color.Transparent, 0, ChartPointSymbol.Circle);
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineNull_ThrowsArgumentNullException()
        {
            var profile = new PipingSoilProfile("name", 2.0, new[]
            {
                new PipingSoilLayer(3.2)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => PipingChartDataFactory.CreatePipingSoilLayer(0, profile, null);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("surfaceLine", paramName);
        }

        [Test]
        public void CreatePipingSoilLayer_SoilProfileNull_ThrowsArgumentException()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();

            // Call
            TestDelegate test = () => PipingChartDataFactory.CreatePipingSoilLayer(0, null, surfaceLine);

            // Assert
            var paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("soilProfile", paramName);
        }

        [Test]
        [TestCase("A", 0)]
        [TestCase("B", 3)]
        [TestCase("Random", 5)]
        public void CreatePipingSoilLayer_Always_CreatesNameFromIndexAndMaterialName(string name, int soilLayerIndex)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new []
            {
                new Point3D(0, 0, 4),
                new Point3D(0, 0, 3.2),
                new Point3D(2, 0, 4)
            });
            var layers = new List<PipingSoilLayer>();
            for (int i = 0; i < soilLayerIndex; i++)
            {
                layers.Add(new PipingSoilLayer((double)i / 10));
            }
            layers.Add(new PipingSoilLayer(-1.0) { MaterialName = name });

            var profile = new PipingSoilProfile("name", -1.0, layers, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(soilLayerIndex, profile, surfaceLine);

            // Assert
            Assert.AreEqual(string.Format("{0} {1}", soilLayerIndex + 1, name), data.Name);
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineOnTopOrAboveSoilLayer_SoilLayerAsRectangleReturned()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new []
            {
                new Point3D(0, 0, 4),
                new Point3D(0, 0, 3.2),
                new Point3D(2, 0, 4)
            });
            var profile = new PipingSoilProfile("name", 2.0, new []
            {
                new PipingSoilLayer(3.2)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(0, profile, surfaceLine);

            // Assert
            var soilLayerChartData = (ChartMultipleAreaData) data;
            Assert.AreEqual(1, soilLayerChartData.Areas.Count());
            CollectionAssert.AreEqual(new []
            {
                new Point2D(0, 3.2),
                new Point2D(2, 3.2),
                new Point2D(2, 2),
                new Point2D(0, 2)
            }, soilLayerChartData.Areas.ElementAt(0));
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineBelowSoilLayer_EmptyChartDataReturned()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(2, 0, 2.0)
            });
            var profile = new PipingSoilProfile("name", 2.0, new[]
            {
                new PipingSoilLayer(3.2)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(0, profile, surfaceLine);

            // Assert
            var soilLayerChartData = (ChartMultipleAreaData)data;
            CollectionAssert.IsEmpty(soilLayerChartData.Areas);
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineThroughMiddleLayerButNotSplittingIt_SoilLayerAsRectangleFollowingSurfaceLineReturned()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 3.0)
            });
            var bottom = 1.5;
            var top = 2.5;
            var profile = new PipingSoilProfile("name", bottom, new[]
            {
                new PipingSoilLayer(top)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(0, profile, surfaceLine);

            // Assert
            var soilLayerChartData = (ChartMultipleAreaData)data;
            Assert.AreEqual(1, soilLayerChartData.Areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top),
            }, soilLayerChartData.Areas.ElementAt(0));
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineThroughMiddleLayerButNotSplittingItIntersectionOnTopLevel_SoilLayerAsRectangleFollowingSurfaceLineReturned()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(0.5, 0, 2.5),
                new Point3D(1, 0, 2.0),
                new Point3D(1.5, 0, 2.5),
                new Point3D(2, 0, 3.0)
            });
            var bottom = 1.5;
            var top = 2.5;
            var profile = new PipingSoilProfile("name", bottom, new[]
            {
                new PipingSoilLayer(top)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(0, profile, surfaceLine);

            // Assert
            var soilLayerChartData = (ChartMultipleAreaData)data;
            Assert.AreEqual(1, soilLayerChartData.Areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top),
            }, soilLayerChartData.Areas.ElementAt(0));
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineStartsBelowLayerTopButAboveBottom_SoilLayerAsRectangleFollowingSurfaceLineReturned()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 3.0)
            });
            var bottom = 1.5;
            var top = 2.5;
            var profile = new PipingSoilProfile("name", bottom, new[]
            {
                new PipingSoilLayer(top)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(0, profile, surfaceLine);

            // Assert
            var soilLayerChartData = (ChartMultipleAreaData)data;
            Assert.AreEqual(1, soilLayerChartData.Areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
            }, soilLayerChartData.Areas.ElementAt(0));
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineEndsBelowLayerTopButAboveBottom_SoilLayerAsRectangleFollowingSurfaceLineReturned()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 2.0)
            });
            var bottom = 1.5;
            var top = 2.5;
            var profile = new PipingSoilProfile("name", bottom, new[]
            {
                new PipingSoilLayer(top)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(0, profile, surfaceLine);

            // Assert
            var soilLayerChartData = (ChartMultipleAreaData)data;
            Assert.AreEqual(1, soilLayerChartData.Areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(2, 2.0),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top),
            }, soilLayerChartData.Areas.ElementAt(0));
        }

        [Test]
        public void CreatePipingSoilLayer_SurfaceLineZigZagsThroughSoilLayer_SoilLayerSplitInMultipleSeries()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4.0),
                new Point3D(4, 0, 0.0),
                new Point3D(8, 0, 4.0)
            });
            var bottom = 1;
            var top = 3;
            var profile = new PipingSoilProfile("name", bottom, new[]
            {
                new PipingSoilLayer(top)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            ChartData data = PipingChartDataFactory.CreatePipingSoilLayer(0, profile, surfaceLine);

            // Assert
            var soilLayerChartData = (ChartMultipleAreaData)data;
            Assert.AreEqual(2, soilLayerChartData.Areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, top),
                new Point2D(3, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, soilLayerChartData.Areas.ElementAt(0));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(5, bottom),
                new Point2D(7, top),
                new Point2D(8, top),
                new Point2D(8, bottom)
            }, soilLayerChartData.Areas.ElementAt(1));
        }

        private static RingtoetsPipingSurfaceLine GetSurfaceLineWithGeometry()
        {
            var points = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            };

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(points);
            return surfaceLine;
        }

        private void AssertEqualPointCollections(IEnumerable<Point2D> points, IEnumerable<Point2D> chartPoints)
        {
            CollectionAssert.AreEqual(points, chartPoints);
        }

        private void AssertEqualLocalPointCollection(Point3D point, RingtoetsPipingSurfaceLine surfaceLine, IEnumerable<Point2D> chartPoints)
        {
            Point3D first = surfaceLine.Points.First();
            Point3D last = surfaceLine.Points.Last();
            Point2D firstPoint = new Point2D(first.X, first.Y);
            Point2D lastPoint = new Point2D(last.X, last.Y);

            var localCoordinate = point.ProjectIntoLocalCoordinates(firstPoint, lastPoint);
            AssertEqualPointCollections(new[]
            {
                new Point2D(new RoundedDouble(2, localCoordinate.X), new RoundedDouble(2, localCoordinate.Y))
            }, chartPoints);
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