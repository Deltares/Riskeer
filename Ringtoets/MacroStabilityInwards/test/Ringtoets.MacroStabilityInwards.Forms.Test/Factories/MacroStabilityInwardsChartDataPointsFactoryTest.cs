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

using System.Collections.Generic;
using System.Linq;
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
            AssertEqualPointCollections(surfaceLine.ProjectGeometryToLZ(), points);
        }

        [Test]
        public void CreateSoilLayerAreas_SoilLayerNull_ReturnsEmptyAreasCollection()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", 2.0, new[]
            {
                new MacroStabilityInwardsSoilLayer(3.2)
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(3.2);
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", 2.0, new[]
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", 2.0, new[]
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", 2.0, new[]
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", bottom, new[]
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", bottom, new[]
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", bottom, new[]
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", bottom, new[]
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
            var soilLayer = new MacroStabilityInwardsSoilLayer(top);
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", bottom, new[]
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