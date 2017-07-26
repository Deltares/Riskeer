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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Forms.Factories;
using Ringtoets.MacroStabilityInwards.Primitives;

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLinePoints(surfaceLine);

            // Assert
            AssertEqualPointCollections(surfaceLine.LocalGeometry, points);
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeToeAtRiver, surfaceLine, points);
        }

        [Test]
        public void CreateTrafficLoadOutsidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadOutsidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateTrafficLoadOutsidePoint_SurfaceLevelOutsideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadOutsidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateTrafficLoadOutsidePoint_GivenSurfaceLineWithTrafficLoadOutside_ReturnsTrafficLoadOutsidePointsArray()
        {
            // Setup
            var trafficLoadOutside = new Point3D(1.2, 2.3, 4.0);
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetTrafficLoadOutsideAt(trafficLoadOutside);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadOutsidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(trafficLoadOutside, surfaceLine, points);
        }

        [Test]
        public void CreateTrafficLoadInsidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadInsidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateTrafficLoadInsidePoint_TrafficLoadInsideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadInsidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateTrafficLoadInsidePoint_GivenSurfaceLineWithTrafficLoadInside_ReturnsTrafficLoadInsidePointsArray()
        {
            // Setup
            var trafficLoadInside = new Point3D(1.2, 2.3, 4.0);
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetTrafficLoadInsideAt(trafficLoadInside);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateTrafficLoadInsidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(trafficLoadInside, surfaceLine, points);
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeToeAtPolderAt(dikeToeAtPolder);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeToeAtPolder, surfaceLine, points);
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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

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
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetSurfaceLevelInsideAt(surfaceLevelInside);

            // Call
            Point2D[] points = MacroStabilityInwardsChartDataPointsFactory.CreateSurfaceLevelInsidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(surfaceLevelInside, surfaceLine, points);
        }

        [Test]
        public void CreateSoilLayerAreas_SoilLayerNull_ReturnsEmptyAreasCollection()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", 2.0, new[]
            {
                new MacroStabilityInwardsSoilLayer1D(3.2)
            }, SoilProfileType.SoilProfile1D, 0);
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(null, soilProfile, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SoilProfileNull_ReturnsEmptyAreasCollection()
        {
            // Setup
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(3.2);
            RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, null, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineNull_ReturnsEmptyAreasCollection()
        {
            // Setup
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", 2.0, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineOnTopOrAboveSoilLayer_ReturnsSoilLayerPointsAsRectangle()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4),
                new Point3D(0, 0, 3.2),
                new Point3D(2, 0, 4)
            });
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", 2.0, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

            // Assert
            Assert.AreEqual(1, areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 3.2),
                new Point2D(2, 3.2),
                new Point2D(2, 2),
                new Point2D(0, 2)
            }, areas.ElementAt(0));
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineBelowSoilLayer_ReturnsEmptyAreasCollection()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(2, 0, 2.0)
            });
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", 2.0, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineThroughMiddleLayerButNotSplittingIt_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 3.0)
            });
            const double bottom = 1.5;
            const double top = 2.5;
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

            // Assert
            Assert.AreEqual(1, areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.ElementAt(0));
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineThroughMiddleLayerButNotSplittingItIntersectionOnTopLevel_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(0.5, 0, 2.5),
                new Point3D(1, 0, 2.0),
                new Point3D(1.5, 0, 2.5),
                new Point3D(2, 0, 3.0)
            });
            const double bottom = 1.5;
            const double top = 2.5;
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

            // Assert
            Assert.AreEqual(1, areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.ElementAt(0));
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineStartsBelowLayerTopButAboveBottom_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 3.0)
            });
            const double bottom = 1.5;
            const double top = 2.5;
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

            // Assert
            Assert.AreEqual(1, areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom)
            }, areas.ElementAt(0));
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineEndsBelowLayerTopButAboveBottom_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 2.0)
            });
            const double bottom = 1.5;
            const double top = 2.5;
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

            // Assert
            Assert.AreEqual(1, areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(2, 2.0),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.ElementAt(0));
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineZigZagsThroughSoilLayer_ReturnsSoilLayerPointsSplitInMultipleAreas()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4.0),
                new Point3D(4, 0, 0.0),
                new Point3D(8, 0, 4.0)
            });
            const int bottom = 1;
            const int top = 3;
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            IEnumerable<Point2D[]> areas = MacroStabilityInwardsChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

            // Assert
            Assert.AreEqual(2, areas.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, top),
                new Point2D(3, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.ElementAt(0));
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(5, bottom),
                new Point2D(7, top),
                new Point2D(8, top),
                new Point2D(8, bottom)
            }, areas.ElementAt(1));
        }

        private static void AssertEqualPointCollections(IEnumerable<Point2D> points, IEnumerable<Point2D> chartPoints)
        {
            CollectionAssert.AreEqual(points, chartPoints);
        }

        private static void AssertEqualLocalPointCollection(Point3D point, RingtoetsMacroStabilityInwardsSurfaceLine surfaceLine, IEnumerable<Point2D> chartPoints)
        {
            Point3D first = surfaceLine.Points.First();
            Point3D last = surfaceLine.Points.Last();
            var firstPoint = new Point2D(first.X, first.Y);
            var lastPoint = new Point2D(last.X, last.Y);

            Point2D localCoordinate = point.ProjectIntoLocalCoordinates(firstPoint, lastPoint);
            AssertEqualPointCollections(new[]
            {
                new Point2D(new RoundedDouble(2, localCoordinate.X), new RoundedDouble(2, localCoordinate.Y))
            }, chartPoints);
        }

        private static RingtoetsMacroStabilityInwardsSurfaceLine GetSurfaceLineWithGeometry()
        {
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            });

            return surfaceLine;
        }
    }
}