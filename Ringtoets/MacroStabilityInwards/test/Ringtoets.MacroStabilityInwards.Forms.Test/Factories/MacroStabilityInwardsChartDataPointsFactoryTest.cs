// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.Base.TestUtil.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Forms.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsChartDataPointsFactoryTest
    {
        [Test]
        public void CreateSurfaceLinePoints_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLinePoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateSurfaceLinePoints_GivenSurfaceLine_ReturnsSurfaceLinePointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLinePoints(surfaceLine);

            // Assert
            AssertEqualPointCollection(surfaceLine.LocalGeometry, points);
        }

        [Test]
        public void CreateSurfaceLevelOutsidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelOutsidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateSurfaceLevelOutsidePoint_SurfaceLevelOutsideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelOutsidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateSurfaceLevelOutsidePoint_GivenSurfaceLineWithSurfaceLevelOutside_ReturnsSurfaceLevelOutsideArray()
        {
            // Setup
            var surfaceLevelOutside = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetSurfaceLevelOutsideAt(surfaceLevelOutside);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelOutsidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(surfaceLevelOutside, surfaceLine, points);
        }

        [Test]
        public void CreateDikeToeAtRiverPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtRiverPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtRiverPoint_SurfaceLevelOutsideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtRiverPoint_GivenSurfaceLineWithDikeToeAtRiver_ReturnsDikeToeAtRiverPointsArray()
        {
            // Setup
            var dikeToeAtRiver = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeToeAtRiver, surfaceLine, points);
        }

        [Test]
        public void CreateDikeTopAtPolderPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtPolderPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeTopAtPolderPoint_DikeTopAtPolderNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtPolderPoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeTopAtPolderPoint_GivenSurfaceLineWithDikeTopAtPolder_ReturnsDikeTopAtPolderPointsArray()
        {
            // Setup
            var dikeTopAtPolder = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeTopAtPolderAt(dikeTopAtPolder);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtPolderPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeTopAtPolder, surfaceLine, points);
        }

        [Test]
        public void CreateShoulderBaseInsidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderBaseInsidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateShoulderBaseInsidePoint_ShoulderBaseInsideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderBaseInsidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateShoulderBaseInsidePoint_GivenSurfaceLineWithShoulderBaseInside_ReturnsShoulderBaseInsidePointsArray()
        {
            // Setup
            var shoulderBaseInside = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetShoulderBaseInsideAt(shoulderBaseInside);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderBaseInsidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(shoulderBaseInside, surfaceLine, points);
        }

        [Test]
        public void CreateShoulderTopInsidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderTopInsidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateShoulderTopInsidePoint_ShoulderTopInsideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderTopInsidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateShoulderTopInsidePoint_GivenSurfaceLineWithShoulderTopInside_ReturnsShoulderTopInsidePointsArray()
        {
            // Setup
            var shoulderTopInside = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetShoulderTopInsideAt(shoulderTopInside);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateShoulderTopInsidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(shoulderTopInside, surfaceLine, points);
        }

        [Test]
        public void CreateDikeToeAtPolderPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtPolderPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtPolderPoint_DikeToeAtPolderNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtPolderPoint_GivenSurfaceLineWithDikeToeAtPolder_ReturnsDikeToeAtPolderPointsArray()
        {
            // Setup
            var dikeToeAtPolder = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeToeAtPolderAt(dikeToeAtPolder);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeToeAtPolder, surfaceLine, points);
        }

        [Test]
        public void CreateDikeTopAtRiverPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtRiverPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeTopAtRiverPoint_DikeTopAtRiverNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtRiverPoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeTopAtRiverPoint_GivenSurfaceLineWithDikeTopAtRiver_ReturnsDikeTopAtRiverPointsArray()
        {
            // Setup
            var dikeTopAtRiver = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeTopAtRiverAt(dikeTopAtRiver);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeTopAtRiverPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeTopAtRiver, surfaceLine, points);
        }

        [Test]
        public void CreateDitchDikeSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchDikeSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchDikeSidePoint_DitchDikeSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchDikeSidePoint_GivenSurfaceLineWithDitchDikeSide_ReturnsDitchDikeSidePointsArray()
        {
            // Setup
            var ditchDikeSide = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDitchDikeSideAt(ditchDikeSide);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(ditchDikeSide, surfaceLine, points);
        }

        [Test]
        public void CreateBottomDitchDikeSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchDikeSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchDikeSidePoint_BottomDitchDikeSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchDikeSidePoint_GivenSurfaceLineWithBottomDitchDikeSide_ReturnsBottomDitchDikeSidePointsArray()
        {
            // Setup
            var bottomDitchDikeSide = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetBottomDitchDikeSideAt(bottomDitchDikeSide);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(bottomDitchDikeSide, surfaceLine, points);
        }

        [Test]
        public void CreateBottomDitchPolderSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchPolderSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchPolderSidePoint_BottomDitchPolderSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchPolderSidePoint_GivenSurfaceLineWithBottomDitchPolderSide_ReturnsBottomDitchPolderSidePointsArray()
        {
            // Setup
            var bottomDitchPolderSide = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetBottomDitchPolderSideAt(bottomDitchPolderSide);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(bottomDitchPolderSide, surfaceLine, points);
        }

        [Test]
        public void CreateDitchPolderSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchPolderSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchPolderSidePoint_DitchPolderSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchPolderSidePoint_GivenSurfaceLineWithDitchPolderSide_ReturnsDitchPolderSidePointsArray()
        {
            // Setup
            var ditchPolderSide = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDitchPolderSideAt(ditchPolderSide);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(ditchPolderSide, surfaceLine, points);
        }

        [Test]
        public void CreateSurfaceLevelInsidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelInsidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateSurfaceLevelInsidePoint_SurfaceLevelInsideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelInsidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateSurfaceLevelInsidePoint_GivenSurfaceLineWithSurfaceLevelInside_ReturnsSurfaceLevelInsidePointsArray()
        {
            // Setup
            var surfaceLevelInside = new Point3D(1.2, 2.3, 4.0);
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetSurfaceLevelInsideAt(surfaceLevelInside);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelInsidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(surfaceLevelInside, surfaceLine, points);
        }

        [Test]
        public void CreateGridPoints_MacroStabilityInwardsGridNull_ReturnsEmptyPoints()
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(null, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            CollectionAssert.IsEmpty(gridPoints);
        }

        [Test]
        public void CreateGridPoints_MacroStabilityInwardsGridDeterminationTypeAutomatic_ReturnsEmptyPoints()
        {
            // Setup
            var random = new Random(21);
            var grid = new MacroStabilityInwardsGrid(random.NextDouble(),
                                                     1 + random.NextDouble(),
                                                     1 + random.NextDouble(),
                                                     random.NextDouble())
            {
                NumberOfHorizontalPoints = random.Next(1, 100),
                NumberOfVerticalPoints = random.Next(1, 100)
            };

            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Automatic);

            // Assert
            CollectionAssert.IsEmpty(gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsNoGridPoints))]
        public void CreateGridPoints_MacroStabilityInwardsGridNumberOfPointsNoGrid_ReturnsEmptyPoints(MacroStabilityInwardsGrid grid)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            CollectionAssert.IsEmpty(gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnePoint))]
        public void CreateGridPoints_MacroStabilityInwardsGridNumberOfPointsOnePoint_AlwaysReturnsBottomLeftPoint(
            MacroStabilityInwardsGrid grid)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            AssertEqualPointCollection(new[]
            {
                new Point2D(grid.XLeft, grid.ZBottom)
            }, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnlyHorizontalPoints))]
        public void CreateGridPoints_MacroStabilityInwardsGridSettingsOnlyHorizontalPoints_ReturnsGridPointsAtBottomSide(
            MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnlyVerticalPoints))]
        public void CreateGridPoints_CreateGridPoints_MacroStabilityInwardsGridSettingsOnlyVerticallPoints_ReturnsGridPointsAtLeftSide
            (MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetWellDefinedGridSettings))]
        public void CreateGridPoints_MacroStabilityInwardsWellDefinedGrid_ReturnsExpectedGridPoints(
            MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        public void CreateOuterRingArea_SoilLayerNull_ReturnsEmptyAreaCollection()
        {
            // Call
            IEnumerable<Point2D[]> outerRing = MacroStabilityInwardsChartDataPointsFactory.CreateOuterRingArea(null);

            // Assert
            CollectionAssert.IsEmpty(outerRing);
        }

        [Test]
        public void CreateOuterRingArea_WithSoilLayer_ReturnsOuterRingArea()
        {
            // Setup
            var mocks = new MockRepository();
            var outerRing = new Ring(new[]
            {
                new Point2D(0.0, 10.0),
                new Point2D(10.0, 10.0),
                new Point2D(10.0, 0.0),
                new Point2D(0.0, 0.0)
            });

            var layer = mocks.Stub<IMacroStabilityInwardsSoilLayer2D>();
            layer.Stub(l => l.OuterRing).Return(outerRing);

            mocks.ReplayAll();

            // Call
            IEnumerable<Point2D[]> outerRingChartData = MacroStabilityInwardsChartDataPointsFactory.CreateOuterRingArea(layer);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                outerRing.Points
            }, outerRingChartData);

            mocks.VerifyAll();
        }

        [Test]
        public void CreatePhreaticLinePoints_PhreaticLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreatePhreaticLinePoints(null);

            // Assert
            CollectionAssert.IsEmpty(line);
        }

        [Test]
        public void CreatePhreaticLinePoints_WithPhreaticLine_ReturnsEmptyPointsArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(0, 0),
                new Point2D(10, 10),
                new Point2D(5, 4)
            };

            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreatePhreaticLinePoints(new MacroStabilityInwardsPhreaticLine("Test",
                                                                                                                                        points));

            // Assert
            CollectionAssert.AreEqual(points, line);
        }

        [Test]
        public void CreateWaternetZonePoints_WaternetLineNull_ReturnsEmptyPointsArray()
        {
            // Setup
            MacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            IEnumerable<Point2D[]> zone = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(null, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(zone);
        }

        [Test]
        public void CreateWaternetZonePoints_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Setup
            var waternetLine = new TestMacroStabilityInwardsWaternetLine();

            // Call
            IEnumerable<Point2D[]> zone = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, null);

            // Assert
            CollectionAssert.IsEmpty(zone);
        }

        [Test]
        [TestCaseSource(nameof(GetPhreaticLineAndWaternetLineConfigurationsBelowSurfaceLine))]
        public void CreateWaternetZonePoints_DifferentWaternetLineAndPhreaticLineBelowSurfaceLineConfigurations_ReturnsPointsArray(
            MacroStabilityInwardsSurfaceLine surfaceLine,
            MacroStabilityInwardsWaternetLine waternetLine,
            Point2D[][] expectedZones)
        {
            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(expectedZones.Length, zones.Length);
            for (var i = 0; i < expectedZones.Length; i++)
            {
                CollectionAssert.AreEqual(expectedZones[i], zones[i]);
            }
        }

        [Test]
        public void CreateWaternetZonePoints_PhreaticLineAboveSurfaceLineBeforeIntersectionPointWithSurfaceLine_ReturnsPointsArray()
        {
            // Setup
            var waternetLineGeometry = new[]
            {
                new Point2D(0, -2),
                new Point2D(10, -2)
            };
            var phreaticLineGeometry = new[]
            {
                new Point2D(0, 6),
                new Point2D(2, 4),
                new Point2D(10, 0)
            };

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaterNetLine(phreaticLineGeometry, waternetLineGeometry);

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 2, 2),
                new Point3D(3, 10, 10),
                new Point3D(6, 8, 8),
                new Point3D(10, 2, 2)
            });

            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(1, zones.Length);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2),
                new Point2D(1.09, 4.91),
                new Point2D(2, 4),
                new Point2D(3, 3.5),
                new Point2D(6, 2),
                new Point2D(10, 0),
                new Point2D(10, -2),
                new Point2D(6, -2),
                new Point2D(3, -2),
                new Point2D(2, -2),
                new Point2D(1.09, -2),
                new Point2D(0, -2),
                new Point2D(0, 2)
            }, zones[0], new Point2DComparerWithTolerance(1e-2));
        }

        [Test]
        public void CreateWaternetZonePoints_PhreaticLineBelowSurfaceLineBeforeIntersectionPointWithSurfaceLine_ReturnsPointsArray()
        {
            // Setup
            var waternetLineGeometry = new[]
            {
                new Point2D(0, -2),
                new Point2D(10, -2)
            };
            var phreaticLineGeometry = new[]
            {
                new Point2D(0, 2),
                new Point2D(10, 2)
            };

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaterNetLine(phreaticLineGeometry, waternetLineGeometry);

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 2, 2),
                new Point3D(3, 10, 10),
                new Point3D(6, 8, 2),
                new Point3D(10, 2, 0)
            });

            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(1, zones.Length);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2),
                new Point2D(3, 2),
                new Point2D(6, 2),
                new Point2D(10, 0),
                new Point2D(10, -2),
                new Point2D(6, -2),
                new Point2D(3, -2),
                new Point2D(0, -2),
                new Point2D(0, 2)
            }, zones[0], new Point2DComparerWithTolerance(1e-2));
        }

        [Test]
        public void CreateWaternetZonePoints_WaterNetAndPhreaticLinesIntersectSurfaceLine_ReturnsPointsArray()
        {
            // Setup
            var waternetLineGeometry = new[]
            {
                new Point2D(0, 5),
                new Point2D(10, 5)
            };
            var phreaticLineGeometry = new[]
            {
                new Point2D(0, 7),
                new Point2D(10, 7)
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 2, 2),
                new Point3D(5, 10, 10),
                new Point3D(10, 2, 2)
            });

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaterNetLine(phreaticLineGeometry, waternetLineGeometry);

            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(1, zones.Length);

            CollectionAssert.AreEqual(new[]
            {
                new Point2D(3.125, 7),
                new Point2D(5, 7),
                new Point2D(6.875, 7),
                new Point2D(6.875, 5),
                new Point2D(5, 5),
                new Point2D(3.125, 5),
                new Point2D(3.125, 7)
            }, zones[0]);
        }

        [Test]
        public void CreateWaternetZonePoints_PhreaticLineIntersectsSurfaceLineAtMultiplePoints_ReturnsPointsArray()
        {
            // Setup
            var waternetLineGeometry = new[]
            {
                new Point2D(0, -2),
                new Point2D(15, -2)
            };
            var phreaticLineGeometry = new[]
            {
                new Point2D(0, 7),
                new Point2D(15, 7)
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 2, 2),
                new Point3D(5, 10, 10),
                new Point3D(10, 2, 2),
                new Point3D(15, 2, 10)
            });

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaterNetLine(phreaticLineGeometry, waternetLineGeometry);

            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(1, zones.Length);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2),
                new Point2D(3.125, 7),
                new Point2D(5, 7),
                new Point2D(6.875, 7),
                new Point2D(10, 2),
                new Point2D(13.125, 7),
                new Point2D(15, 7),
                new Point2D(15, -2),
                new Point2D(13.125, -2),
                new Point2D(10, -2),
                new Point2D(6.875, -2),
                new Point2D(5, -2),
                new Point2D(3.125, -2),
                new Point2D(0, -2),
                new Point2D(0, 2)
            }, zones[0]);
        }

        [Test]
        public void CreateWaternetZonePoints_PhreaticLineIntersectsSurfaceLineAtMultiplePointsAndIntersectsWaterNetLine_ReturnsPointsArray()
        {
            // Setup
            var waternetLineGeometry = new[]
            {
                new Point2D(0, -2),
                new Point2D(15, -2)
            };
            var phreaticLineGeometry = new[]
            {
                new Point2D(0, 7),
                new Point2D(10, 7),
                new Point2D(15, -8)
            };

            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 2, 2),
                new Point3D(5, 10, 10),
                new Point3D(10, 2, 2),
                new Point3D(15, 2, 10)
            });

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaterNetLine(phreaticLineGeometry, waternetLineGeometry);

            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(2, zones.Length);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2),
                new Point2D(3.125, 7),
                new Point2D(5, 7),
                new Point2D(6.875, 7),
                new Point2D(10, 2),
                new Point2D(11.09, 3.74),
                new Point2D(13, -2),
                new Point2D(13, -2),
                new Point2D(11.09, -2),
                new Point2D(10, -2),
                new Point2D(6.875, -2),
                new Point2D(5, -2),
                new Point2D(3.125, -2),
                new Point2D(0, -2),
                new Point2D(0, 2)
            }, zones[0], new Point2DComparerWithTolerance(1e-2));

            CollectionAssert.AreEqual(new[]
            {
                new Point2D(13, -2),
                new Point2D(15, -2),
                new Point2D(15, -8),
                new Point2D(13, -2),
                new Point2D(13, -2)
            }, zones[1], new Point2DComparerWithTolerance(1e-2));
        }

        [Test]
        [TestCaseSource(nameof(GetPhreaticLineAndWaternetLineWithDifferentLengths))]
        public void CreateWaternetZonePoints_PhreaticLineNotSameLength_ReturnsPointArray(IEnumerable<Point2D> waternetLineGeometry,
                                                                            IEnumerable<Point2D> phreaticLineGeometry)
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 2, 5),
                new Point3D(10, 2, 5)
            });

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaterNetLine(waternetLineGeometry, phreaticLineGeometry);

            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(1, zones.Length);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 0),
                new Point2D(5, 0),
                new Point2D(5, -2),
                new Point2D(0, -2),
                new Point2D(0, 0)
            }, zones[0], new Point2DComparerWithTolerance(1e-2));
        }

        private static MacroStabilityInwardsWaternetLine CreateWaterNetLine(IEnumerable<Point2D> waternetLineGeometry,
                                                                            IEnumerable<Point2D> phreaticLineGeometry)
        {
            return new MacroStabilityInwardsWaternetLine("Line",
                                                         waternetLineGeometry,
                                                         new MacroStabilityInwardsPhreaticLine("Line 2",
                                                                                               phreaticLineGeometry));
        }

        private static void AssertEqualPointCollection(IEnumerable<Point2D> points, IEnumerable<Point2D> chartPoints)
        {
            CollectionAssert.AreEqual(points, chartPoints);
        }

        private static void AssertEqualLocalPointCollection(Point3D point, MacroStabilityInwardsSurfaceLine surfaceLine, IEnumerable<Point2D> chartPoints)
        {
            Point3D first = surfaceLine.Points.First();
            Point3D last = surfaceLine.Points.Last();
            var firstPoint = new Point2D(first.X, first.Y);
            var lastPoint = new Point2D(last.X, last.Y);

            Point2D localCoordinate = point.ProjectIntoLocalCoordinates(firstPoint, lastPoint);
            AssertEqualPointCollection(new[]
            {
                new Point2D(new RoundedDouble(2, localCoordinate.X), new RoundedDouble(2, localCoordinate.Y))
            }, chartPoints);
        }

        private static MacroStabilityInwardsSurfaceLine GetSurfaceLineWithGeometry()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            });

            return surfaceLine;
        }

        #region TestData

        #region Grid Settings

        private static IEnumerable<TestCaseData> GetGridSettingsNoGridPoints()
        {
            var random = new Random(21);

            double xLeft = random.NextDouble();
            double xRight = 1 + random.NextDouble();
            double zTop = 1 + random.NextDouble();
            double zBottom = random.NextDouble();

            yield return new TestCaseData(new MacroStabilityInwardsGrid(double.NaN, xRight, zTop, zBottom)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            }).SetName("XLeftNaN");
            yield return new TestCaseData(new MacroStabilityInwardsGrid(xLeft, double.NaN, zTop, zBottom)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            }).SetName("XRightNaN");
            yield return new TestCaseData(new MacroStabilityInwardsGrid(xLeft, xRight, double.NaN, zBottom)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            }).SetName("ZTopNaN");
            yield return new TestCaseData(new MacroStabilityInwardsGrid(xLeft, xRight, zTop, double.NaN)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            }).SetName("ZBottomNaN");
        }

        private static IEnumerable<TestCaseData> GetGridSettingsOnlyHorizontalPoints()
        {
            var gridRightLargerThanLeft = new MacroStabilityInwardsGrid(1, 4, 3, 1)
            {
                NumberOfHorizontalPoints = 4,
                NumberOfVerticalPoints = 1
            };
            yield return new TestCaseData(gridRightLargerThanLeft, new[]
            {
                new Point2D(gridRightLargerThanLeft.XLeft, gridRightLargerThanLeft.ZBottom),
                new Point2D(2, gridRightLargerThanLeft.ZBottom),
                new Point2D(3, gridRightLargerThanLeft.ZBottom),
                new Point2D(gridRightLargerThanLeft.XRight, gridRightLargerThanLeft.ZBottom)
            }).SetName("XRight > XLeft");
        }

        private static IEnumerable<TestCaseData> GetGridSettingsOnlyVerticalPoints()
        {
            var gridTopLargerThanBottom = new MacroStabilityInwardsGrid(1, 3, 4, 1)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 4
            };
            yield return new TestCaseData(gridTopLargerThanBottom, new[]
            {
                new Point2D(gridTopLargerThanBottom.XLeft, gridTopLargerThanBottom.ZBottom),
                new Point2D(gridTopLargerThanBottom.XLeft, 2),
                new Point2D(gridTopLargerThanBottom.XLeft, 3),
                new Point2D(gridTopLargerThanBottom.XLeft, gridTopLargerThanBottom.ZTop)
            }).SetName("ZTop > ZBottom");
        }

        private static IEnumerable<TestCaseData> GetGridSettingsOnePoint()
        {
            const double zBottom = 1.0;

            var grid = new MacroStabilityInwardsGrid(1, 2, 3, zBottom)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            };
            yield return new TestCaseData(grid).SetName("XRight > XLeft, ZTop > ZBottom");
        }

        private static IEnumerable<TestCaseData> GetWellDefinedGridSettings()
        {
            var grid = new MacroStabilityInwardsGrid(1, 3, 3, 1)
            {
                NumberOfHorizontalPoints = 3,
                NumberOfVerticalPoints = 3
            };
            yield return new TestCaseData(grid, new[]
            {
                new Point2D(grid.XLeft, grid.ZBottom),
                new Point2D(2, grid.ZBottom),
                new Point2D(grid.XRight, grid.ZBottom),
                new Point2D(grid.XLeft, 2),
                new Point2D(2, 2),
                new Point2D(grid.XRight, 2),
                new Point2D(grid.XLeft, grid.ZTop),
                new Point2D(2, grid.ZTop),
                new Point2D(grid.XRight, grid.ZTop)
            }).SetName("XRight > XLeft, ZTop > ZBottom");
        }

        #endregion

        #region WaterNet Configurations

        private static IEnumerable<TestCaseData> GetPhreaticLineAndWaternetLineWithDifferentLengths()
        {
            yield return new TestCaseData(new[]
                                          {
                                              new Point2D(0, -2),
                                              new Point2D(10, -2)
                                          },
                                          new[]
                                          {
                                              new Point2D(0, 0),
                                              new Point2D(5, 0)
                                          }).SetName("PhreaticLine not same length");

            yield return new TestCaseData(new[]
                                          {
                                              new Point2D(0, -2),
                                              new Point2D(5, -2)
                                          },
                                          new[]
                                          {
                                              new Point2D(0, 0),
                                              new Point2D(10, 0)
                                          }).SetName("WaternetLine not same length");
        }

        private static IEnumerable<TestCaseData> GetPhreaticLineAndWaternetLineConfigurationsBelowSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("Test");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 2, 5),
                new Point3D(10, 2, 5)
            });

            var topPoints = new[]
            {
                new Point2D(0, 0),
                new Point2D(10, 0)
            };
            var bottomPoints = new[]
            {
                new Point2D(0, -2),
                new Point2D(10, -2)
            };

            yield return new TestCaseData(surfaceLine,
                                          CreateWaterNetLine(topPoints, bottomPoints),
                                          new[]
                                          {
                                              new[]
                                              {
                                                  new Point2D(0, -2),
                                                  new Point2D(10, -2),
                                                  new Point2D(10, 0),
                                                  new Point2D(0, 0),
                                                  new Point2D(0, -2)
                                              }
                                          })
                .SetName("Waternetline above phreatic line");

            yield return new TestCaseData(surfaceLine,
                                          CreateWaterNetLine(bottomPoints, topPoints),
                                          new[]
                                          {
                                              new[]
                                              {
                                                  new Point2D(0, 0),
                                                  new Point2D(10, 0),
                                                  new Point2D(10, -2),
                                                  new Point2D(0, -2),
                                                  new Point2D(0, 0)
                                              }
                                          })
                .SetName("Waternetline below phreatic line");

            var linearlyIncreasingPoints = new[]
            {
                new Point2D(0, -2),
                new Point2D(5, 0),
                new Point2D(10, 2)
            };
            var linearlyDecreasingPoints = new[]
            {
                new Point2D(0, 2),
                new Point2D(5, 0),
                new Point2D(10, -2)
            };
            yield return new TestCaseData(surfaceLine,
                                          CreateWaterNetLine(linearlyIncreasingPoints, linearlyDecreasingPoints),
                                          new[]
                                          {
                                              new[]
                                              {
                                                  new Point2D(0, 2),
                                                  new Point2D(5, 0),
                                                  new Point2D(5, 0),
                                                  new Point2D(0, -2),
                                                  new Point2D(0, 2)
                                              },
                                              new[]
                                              {
                                                  new Point2D(5, 0),
                                                  new Point2D(10, -2),
                                                  new Point2D(10, 2),
                                                  new Point2D(5, 0),
                                                  new Point2D(5, 0)
                                              }
                                          })
                .SetName("Waternetline above phreatic line after intersection.");
            yield return new TestCaseData(surfaceLine,
                                          CreateWaterNetLine(linearlyDecreasingPoints, linearlyIncreasingPoints),
                                          new[]
                                          {
                                              new[]
                                              {
                                                  new Point2D(0, -2),
                                                  new Point2D(5, 0),
                                                  new Point2D(5, 0),
                                                  new Point2D(0, 2),
                                                  new Point2D(0, -2)
                                              },
                                              new[]
                                              {
                                                  new Point2D(5, 0),
                                                  new Point2D(10, 2),
                                                  new Point2D(10, -2),
                                                  new Point2D(5, 0),
                                                  new Point2D(5, 0)
                                              }
                                          })
                .SetName("Waternetline below phreatic line after intersection.");
        }

        #endregion

        #endregion
    }
}