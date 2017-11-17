﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
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
        public void CreateGridPointsWithGridDeterminationType_MacroStabilityInwardsGridNull_ReturnsEmptyPoints()
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(null, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            CollectionAssert.IsEmpty(gridPoints);
        }

        [Test]
        public void CreateGridPointsWithGridDeterminationType_MacroStabilityInwardsGridDeterminationTypeAutomatic_ReturnsEmptyPoints()
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
        [TestCaseSource(nameof(GetGridSettingsNoGridPoints), new object[]
        {
            "CreateGridPointsWithGridDeterminationType"
        })]
        public void CreateGridPointsWithGridDeterminationType_MacroStabilityInwardsGridNumberOfPointsNoGrid_ReturnsEmptyPoints(MacroStabilityInwardsGrid grid)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            CollectionAssert.IsEmpty(gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnePoint), new object[]
        {
            "CreateGridPointsWithGridDeterminationType"
        })]
        public void CreateGridPointsWithGridDeterminationType_MacroStabilityInwardsGridNumberOfPointsOnePoint_AlwaysReturnsBottomLeftPoint(
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
        [TestCaseSource(nameof(GetGridSettingsOnlyHorizontalPoints), new object[]
        {
            "CreateGridPointsWithGridDeterminationType"
        })]
        public void CreateGridPointsWithGridDeterminationType_MacroStabilityInwardsGridSettingsOnlyHorizontalPoints_ReturnsGridPointsAtBottomSide(
            MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnlyVerticalPoints), new object[]
        {
            "CreateGridPointsWithGridDeterminationType"
        })]
        public void CreateGridPointsWithGridDeterminationType_CreateGridPoints_MacroStabilityInwardsGridSettingsOnlyVerticallPoints_ReturnsGridPointsAtLeftSide
            (MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetWellDefinedGridSettings), new object[]
        {
            "CreateGridPointsWithGridDeterminationType"
        })]
        public void CreateGridPointsWithGridDeterminationType_MacroStabilityInwardsWellDefinedGrid_ReturnsExpectedGridPoints(
            MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid, MacroStabilityInwardsGridDeterminationType.Manual);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        public void CreateGridPoints_MacroStabilityInwardsGridNull_ReturnsEmptyPoints()
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(null);

            // Assert
            CollectionAssert.IsEmpty(gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsNoGridPoints), new object[]
        {
            "CreateGridPoints"
        })]
        public void CreateGridPoints_MacroStabilityInwardsGridNumberOfPointsNoGrid_ReturnsEmptyPoints(MacroStabilityInwardsGrid grid)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid);

            // Assert
            CollectionAssert.IsEmpty(gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnePoint), new object[]
        {
            "CreateGridPoints"
        })]
        public void CreateGridPoints_MacroStabilityInwardsGridNumberOfPointsOnePoint_AlwaysReturnsBottomLeftPoint(
            MacroStabilityInwardsGrid grid)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid);

            // Assert
            AssertEqualPointCollection(new[]
            {
                new Point2D(grid.XLeft, grid.ZBottom)
            }, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnlyHorizontalPoints), new object[]
        {
            "CreateGridPoints"
        })]
        public void CreateGridPoints_MacroStabilityInwardsGridSettingsOnlyHorizontalPoints_ReturnsGridPointsAtBottomSide(
            MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetGridSettingsOnlyVerticalPoints), new object[]
        {
            "CreateGridPoints"
        })]
        public void CreateGridPoints_CreateGridPoints_MacroStabilityInwardsGridSettingsOnlyVerticallPoints_ReturnsGridPointsAtLeftSide
            (MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid);

            // Assert
            AssertEqualPointCollection(expectedPoints, gridPoints);
        }

        [Test]
        [TestCaseSource(nameof(GetWellDefinedGridSettings), new object[]
        {
            "CreateGridPoints"
        })]
        public void CreateGridPoints_MacroStabilityInwardsWellDefinedGrid_ReturnsExpectedGridPoints(
            MacroStabilityInwardsGrid grid, Point2D[] expectedPoints)
        {
            // Call
            Point2D[] gridPoints = MacroStabilityInwardsChartDataPointsFactory.CreateGridPoints(grid);

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
            var outerRing = new Ring(new[]
            {
                new Point2D(0.0, 10.0),
                new Point2D(10.0, 10.0),
                new Point2D(10.0, 0.0),
                new Point2D(0.0, 0.0)
            });
            var layer = new MacroStabilityInwardsSoilLayer2D(outerRing);

            // Call
            IEnumerable<Point2D[]> outerRingChartData = MacroStabilityInwardsChartDataPointsFactory.CreateOuterRingArea(layer);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                outerRing.Points
            }, outerRingChartData);
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
        public void CreatePhreaticLinePoints_WithPhreaticLine_ReturnsPointsArray()
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
        public void CreateWaternetZonePoints_WaternetLineGeometryEmpty_ReturnsEmptyPointsArray()
        {
            // Setup
            var phreaticLineGeometry = new[]
            {
                new Point2D(0, 6),
                new Point2D(2, 4),
                new Point2D(10, 0)
            };

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(new Point2D[0], phreaticLineGeometry);

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
            CollectionAssert.IsEmpty(zones);
        }

        [Test]
        public void CreateWaternetZonePoints_PhreaticLineGeometryEmpty_ReturnsEmptyPointsArray()
        {
            // Setup
            var waternetLineGeometry = new[]
            {
                new Point2D(0, 6),
                new Point2D(2, 4),
                new Point2D(10, 0)
            };

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(waternetLineGeometry, new Point2D[0]);

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
            CollectionAssert.IsEmpty(zones);
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
            var phreaticLineGeometry = new[]
            {
                new Point2D(0, -2),
                new Point2D(10, -2)
            };
            var waternetLineGeometry = new[]
            {
                new Point2D(0, 6),
                new Point2D(2, 4),
                new Point2D(10, 0)
            };

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(waternetLineGeometry, phreaticLineGeometry);

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

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(phreaticLineGeometry, waternetLineGeometry);

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
        public void CreateWaternetZonePoints_WaternetAndPhreaticLinesIntersectSurfaceLine_ReturnsPointsArray()
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

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(phreaticLineGeometry, waternetLineGeometry);

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

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(phreaticLineGeometry, waternetLineGeometry);

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
        public void CreateWaternetZonePoints_PhreaticLineIntersectsSurfaceLineAtMultiplePointsAndIntersectsWaternetLine_ReturnsPointsArray()
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

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(phreaticLineGeometry, waternetLineGeometry);

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

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(waternetLineGeometry, phreaticLineGeometry);

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

        [Test]
        public void CreateWaternetZonePoints_WaternetLineOnSurfaceLineAndIntersectsPhreaticLine()
        {
            // Setup
            var waternetLineGeometry = new[]
            {
                new Point2D(0, 2),
                new Point2D(5, 4),
                new Point2D(10, 7),
                new Point2D(15, 10)
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
                new Point3D(5, 4, 4),
                new Point3D(10, 7, 7),
                new Point3D(15, 10, 10)
            });

            MacroStabilityInwardsWaternetLine waternetLine = CreateWaternetLine(phreaticLineGeometry, waternetLineGeometry);

            // Call
            Point2D[][] zones = MacroStabilityInwardsChartDataPointsFactory.CreateWaternetZonePoints(waternetLine, surfaceLine).ToArray();

            // Assert
            Assert.AreEqual(1, zones.Length);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(11.18, 7),
                new Point2D(15, 8.97),
                new Point2D(15, -8),
                new Point2D(11.18, 3.46),
                new Point2D(11.18, 7)
            }, zones[0], new Point2DComparerWithTolerance(1e-2));
        }

        [Test]
        public void CreateSlipPlanePoints_SlidingCurveNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateSlipPlanePoints(null);

            // Assert
            CollectionAssert.IsEmpty(line);
        }

        [Test]
        public void CreateSlipPlanePoints_SlidingCurveEmptySlices_ReturnsEmptyPointsArray()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                Enumerable.Empty<MacroStabilityInwardsSlice>(),
                0.0,
                0.0);

            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateSlipPlanePoints(slidingCurve);

            // Assert
            CollectionAssert.IsEmpty(line);
        }

        [Test]
        public void CreateSlipPlanePoints_WithSlidingCurve_ReturnsPointsArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(0, 0),
                new Point2D(2, 2),
                new Point2D(4, 4),
                new Point2D(5, 4)
            };

            var slidingCurve = new MacroStabilityInwardsSlidingCurve(
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                new[]
                {
                    new MacroStabilityInwardsSlice(new Point2D(0, 1),
                                                   new Point2D(1, 1),
                                                   new Point2D(0, 0),
                                                   new Point2D(1, 0),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties()),
                    new MacroStabilityInwardsSlice(new Point2D(2, 3),
                                                   new Point2D(3, 3),
                                                   new Point2D(2, 2),
                                                   new Point2D(3, 2),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties()),
                    new MacroStabilityInwardsSlice(new Point2D(4, 5),
                                                   new Point2D(5, 5),
                                                   new Point2D(4, 4),
                                                   new Point2D(5, 4),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties())
                },
                0.0,
                0.0);

            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateSlipPlanePoints(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(points, line);
        }

        [Test]
        public void CreateLeftCircleRadiusPoints_SlidingCurveNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateLeftCircleRadiusPoints(null);

            // Assert
            CollectionAssert.IsEmpty(line);
        }

        [Test]
        public void CreateLeftCircleRadiusPoints_SlidingCurveEmptySlices_ReturnsEmptyPointsArray()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                Enumerable.Empty<MacroStabilityInwardsSlice>(),
                0.0,
                0.0);

            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateLeftCircleRadiusPoints(slidingCurve);

            // Assert
            CollectionAssert.IsEmpty(line);
        }

        [Test]
        public void CreateLeftCircleRadiusPoints_WithSlidingCurve_ReturnsPointsArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(10, 10),
                new Point2D(0, 1)
            };

            var slidingCurve = new MacroStabilityInwardsSlidingCurve(
                new MacroStabilityInwardsSlidingCircle(new Point2D(10, 10), 0.0, true, 0.0, 0.0, 0.0, 0.0),
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                new[]
                {
                    new MacroStabilityInwardsSlice(new Point2D(0, 1),
                                                   new Point2D(1, 1),
                                                   new Point2D(0, 0),
                                                   new Point2D(1, 0),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties())
                },
                0.0,
                0.0);

            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateLeftCircleRadiusPoints(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(points, line);
        }

        [Test]
        public void CreateRightCircleRadiusPoints_SlidingCurveNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateRightCircleRadiusPoints(null);

            // Assert
            CollectionAssert.IsEmpty(line);
        }

        [Test]
        public void CreateRightCircleRadiusPoints_SlidingCurveEmptySlices_ReturnsEmptyPointsArray()
        {
            // Setup
            var slidingCurve = new MacroStabilityInwardsSlidingCurve(
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                Enumerable.Empty<MacroStabilityInwardsSlice>(),
                0.0,
                0.0);

            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateRightCircleRadiusPoints(slidingCurve);

            // Assert
            CollectionAssert.IsEmpty(line);
        }

        [Test]
        public void CreateRightCircleRadiusPoints_WithSlidingCurve_ReturnsPointsArray()
        {
            // Setup
            var points = new[]
            {
                new Point2D(20, 10),
                new Point2D(1, 1)
            };

            var slidingCurve = new MacroStabilityInwardsSlidingCurve(
                MacroStabilityInwardsSlidingCircleTestFactory.Create(),
                new MacroStabilityInwardsSlidingCircle(new Point2D(20, 10), 0.0, false, 0.0, 0.0, 0.0, 0.0),
                new[]
                {
                    new MacroStabilityInwardsSlice(new Point2D(0, 1),
                                                   new Point2D(1, 1),
                                                   new Point2D(0, 0),
                                                   new Point2D(1, 0),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties())
                },
                0.0,
                0.0);

            // Call
            Point2D[] line = MacroStabilityInwardsChartDataPointsFactory.CreateRightCircleRadiusPoints(slidingCurve);

            // Assert
            CollectionAssert.AreEqual(points, line);
        }

        [Test]
        public void CreateSliceAreas_SlicesNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSliceAreas(null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSliceAreas_WithSlices_ReturnsAreas()
        {
            // Setup
            var slices =
                new[]
                {
                    new MacroStabilityInwardsSlice(new Point2D(0, 1),
                                                   new Point2D(1, 1),
                                                   new Point2D(0, 0),
                                                   new Point2D(1, 0),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties()),
                    new MacroStabilityInwardsSlice(new Point2D(3, 4),
                                                   new Point2D(4, 4),
                                                   new Point2D(3, 3),
                                                   new Point2D(4, 3),
                                                   new MacroStabilityInwardsSlice.ConstructionProperties())
                };

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSliceAreas(slices);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 1),
                    new Point2D(1, 1),
                    new Point2D(1, 0),
                    new Point2D(0, 0)
                },
                new[]
                {
                    new Point2D(3, 4),
                    new Point2D(4, 4),
                    new Point2D(4, 3),
                    new Point2D(3, 3)
                }
            }, areas);
        }

        [Test]
        public void CreateTangentLines_TangentLinesNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(null, new MacroStabilityInwardsSurfaceLine("line"));

            // Assert
            CollectionAssert.IsEmpty(lines);
        }

        [Test]
        public void CreateTangentLines_SurfaceLineNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(Enumerable.Empty<RoundedDouble>(), null);

            // Assert
            CollectionAssert.IsEmpty(lines);
        }

        [Test]
        public void CreateTangentLines_WithSurfaceLineAndTangentLines_ReturnsLines()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(-5, 2, 2),
                new Point3D(10, 2, 2)
            });

            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(new[]
            {
                (RoundedDouble) 2.5,
                (RoundedDouble) 5.8
            }, surfaceLine);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0.0, 2.5),
                    new Point2D(15.0, 2.5)
                },
                new[]
                {
                    new Point2D(0, 5.8),
                    new Point2D(15.0, 5.8)
                }
            }, lines);
        }

        [Test]
        public void CreateTangentLines_WithBoundaryParametersSurfaceLineNull_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(
                MacroStabilityInwardsGridDeterminationType.Manual,
                MacroStabilityInwardsTangentLineDeterminationType.Specified,
                (RoundedDouble) 0.0,
                (RoundedDouble) 10.0,
                3,
                null);

            // Assert
            CollectionAssert.IsEmpty(lines);
        }

        [Test]
        [TestCase(double.NaN, 10.0)]
        [TestCase(10.0, double.NaN)]
        [TestCase(double.NegativeInfinity, 10.0)]
        [TestCase(double.PositiveInfinity, 10.0)]
        [TestCase(0.0, double.NegativeInfinity)]
        [TestCase(0.0, double.PositiveInfinity)]
        public void CreateTangentLines_WithInvalidBoundaryParameters_ReturnsEmptyCollection(double bottom, double top)
        {
            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(
                MacroStabilityInwardsGridDeterminationType.Manual,
                MacroStabilityInwardsTangentLineDeterminationType.Specified,
                (RoundedDouble) bottom,
                (RoundedDouble) top,
                3,
                GetSurfaceLineWithGeometry());

            // Assert
            CollectionAssert.IsEmpty(lines);
        }

        [Test]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Automatic, MacroStabilityInwardsTangentLineDeterminationType.Specified)]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Manual, MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated)]
        [TestCase(MacroStabilityInwardsGridDeterminationType.Automatic, MacroStabilityInwardsTangentLineDeterminationType.LayerSeparated)]
        public void CreateTangentLines_WithBoundaryParametersAndDeterminationTypeAutomatic_ReturnsEmptyCollection(
            MacroStabilityInwardsGridDeterminationType gridDeterminationType,
            MacroStabilityInwardsTangentLineDeterminationType tangentLineDeterminationType)
        {
            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(
                gridDeterminationType,
                tangentLineDeterminationType,
                (RoundedDouble) 10.0,
                (RoundedDouble) 30.0,
                3,
                GetSurfaceLineWithGeometry());

            // Assert
            CollectionAssert.IsEmpty(lines);
        }

        [Test]
        public void CreateTangentLines_WithBoundaryParametersAndSingleTangentLine_ReturnsExpectedLine()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(-5, 2, 2),
                new Point3D(10, 2, 2)
            });

            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(
                MacroStabilityInwardsGridDeterminationType.Manual,
                MacroStabilityInwardsTangentLineDeterminationType.Specified,
                (RoundedDouble) 10.0,
                (RoundedDouble) 20.0,
                1,
                surfaceLine);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0.0, 20.0),
                    new Point2D(15.0, 20.0)
                }
            }, lines);
        }

        [Test]
        public void CreateTangentLines_WithBoundaryParametersAndMultipleTangentLines_ReturnsExpectedLines()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(-5, 2, 2),
                new Point3D(10, 2, 2)
            });

            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(
                MacroStabilityInwardsGridDeterminationType.Manual,
                MacroStabilityInwardsTangentLineDeterminationType.Specified,
                (RoundedDouble) 10.0,
                (RoundedDouble) 20.0,
                3,
                surfaceLine);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0.0, 20.0),
                    new Point2D(15.0, 20.0)
                },
                new[]
                {
                    new Point2D(0, 15.0),
                    new Point2D(15.0, 15.0)
                },
                new[]
                {
                    new Point2D(0, 10.0),
                    new Point2D(15.0, 10.0)
                }
            }, lines);
        }

        [Test]
        public void CreateTangentLines_WithBoundaryParametersAndMultipleTangentLinesWithTopAndBottomSame_ReturnsExpectedLines()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(-5, 2, 2),
                new Point3D(10, 2, 2)
            });

            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(
                MacroStabilityInwardsGridDeterminationType.Manual,
                MacroStabilityInwardsTangentLineDeterminationType.Specified,
                (RoundedDouble) 10.0,
                (RoundedDouble) 10.0,
                3,
                surfaceLine);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 10.0),
                    new Point2D(15.0, 10.0)
                },
                new[]
                {
                    new Point2D(0, 10.0),
                    new Point2D(15.0, 10.0)
                },
                new[]
                {
                    new Point2D(0, 10.0),
                    new Point2D(15.0, 10.0)
                }
            }, lines);
        }

        [Test]
        public void CreateTangentLines_WithBoundaryParametersAndSingleTangentLineWithTopAndBottomSame_ReturnsExpectedLine()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine("line");
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(-5, 2, 2),
                new Point3D(10, 2, 2)
            });

            // Call
            IEnumerable<Point2D[]> lines = MacroStabilityInwardsChartDataPointsFactory.CreateTangentLines(
                MacroStabilityInwardsGridDeterminationType.Manual,
                MacroStabilityInwardsTangentLineDeterminationType.Specified,
                (RoundedDouble) 10.0,
                (RoundedDouble) 10.0,
                1,
                surfaceLine);

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                new[]
                {
                    new Point2D(0, 10.0),
                    new Point2D(15.0, 10.0)
                }
            }, lines);
        }

        private static MacroStabilityInwardsWaternetLine CreateWaternetLine(IEnumerable<Point2D> waternetLineGeometry,
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

        private static IEnumerable<TestCaseData> GetGridSettingsNoGridPoints(string prefix)
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
            }).SetName($"{prefix} - XLeftNaN");
            yield return new TestCaseData(new MacroStabilityInwardsGrid(xLeft, double.NaN, zTop, zBottom)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            }).SetName($"{prefix} - XRightNaN");
            yield return new TestCaseData(new MacroStabilityInwardsGrid(xLeft, xRight, double.NaN, zBottom)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            }).SetName($"{prefix} - ZTopNaN");
            yield return new TestCaseData(new MacroStabilityInwardsGrid(xLeft, xRight, zTop, double.NaN)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            }).SetName($"{prefix} - ZBottomNaN");
        }

        private static IEnumerable<TestCaseData> GetGridSettingsOnlyHorizontalPoints(string prefix)
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
            }).SetName($"{prefix} - XRight > XLeft");
        }

        private static IEnumerable<TestCaseData> GetGridSettingsOnlyVerticalPoints(string prefix)
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
            }).SetName($"{prefix} - ZTop > ZBottom");
        }

        private static IEnumerable<TestCaseData> GetGridSettingsOnePoint(string prefix)
        {
            const double zBottom = 1.0;

            var grid = new MacroStabilityInwardsGrid(1, 2, 3, zBottom)
            {
                NumberOfHorizontalPoints = 1,
                NumberOfVerticalPoints = 1
            };
            yield return new TestCaseData(grid).SetName($"{prefix} - XRight > XLeft, ZTop > ZBottom");
        }

        private static IEnumerable<TestCaseData> GetWellDefinedGridSettings(string prefix)
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
            }).SetName($"{prefix} - XRight > XLeft, ZTop > ZBottom");
        }

        #endregion

        #region Waternet Configurations

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
                                          CreateWaternetLine(topPoints, bottomPoints),
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
                                          CreateWaternetLine(bottomPoints, topPoints),
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
                                          CreateWaternetLine(linearlyIncreasingPoints, linearlyDecreasingPoints),
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
                                          CreateWaternetLine(linearlyDecreasingPoints, linearlyIncreasingPoints),
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