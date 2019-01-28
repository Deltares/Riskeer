// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Factories;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Factories
{
    [TestFixture]
    public class PipingChartDataPointsFactoryTest
    {
        [Test]
        public void CreateSurfaceLinePoints_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateSurfaceLinePoints(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateSurfaceLinePoints_GivenSurfaceLine_ReturnsSurfaceLinePointsArray()
        {
            // Setup
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateSurfaceLinePoints(surfaceLine);

            // Assert
            AssertEqualPointCollections(surfaceLine.LocalGeometry, points);
        }

        [Test]
        public void CreateEntryPointPoint_PipingInputNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateEntryPointPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateEntryPointPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = null,
                EntryPointL = (RoundedDouble) 10.0
            };

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateEntryPointPoint(pipingInput);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateEntryPointPoint_EntryPointNaN_ReturnsEmptyPointsArray()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = GetSurfaceLineWithGeometry(),
                EntryPointL = RoundedDouble.NaN
            };

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateEntryPointPoint(pipingInput);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateEntryPointPoint_GivenPipingInput_ReturnsEntryPointPointsArray()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = GetSurfaceLineWithGeometry()
            };

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateEntryPointPoint(pipingInput);

            // Assert
            var entryPointOnLine = new Point2D(pipingInput.EntryPointL, pipingInput.SurfaceLine.GetZAtL(pipingInput.EntryPointL));
            AssertEqualPointCollections(new[]
            {
                entryPointOnLine
            }, points);
        }

        [Test]
        public void CreateExitPointPoint_PipingInputNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateExitPointPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateExitPointPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = null,
                ExitPointL = (RoundedDouble) 10.0
            };

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateExitPointPoint(pipingInput);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateExitPointPoint_ExitPointNaN_ReturnsEmptyPointsArray()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = GetSurfaceLineWithGeometry(),
                ExitPointL = RoundedDouble.NaN
            };

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateExitPointPoint(pipingInput);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateExitPointPoint_GivenPipingInput_ReturnsExitPointPointsArray()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = GetSurfaceLineWithGeometry()
            };

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateExitPointPoint(pipingInput);

            // Assert
            var exitPointOnLine = new Point2D(pipingInput.ExitPointL, pipingInput.SurfaceLine.GetZAtL(pipingInput.ExitPointL));
            AssertEqualPointCollections(new[]
            {
                exitPointOnLine
            }, points);
        }

        [Test]
        public void CreateDitchPolderSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDitchPolderSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchPolderSidePoint_DitchPolderSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchPolderSidePoint_GivenSurfaceLineWithDitchPolderSide_ReturnsDitchPolderSidePointsArray()
        {
            // Setup
            var ditchPolderSide = new Point3D(1.2, 2.3, 4.0);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDitchPolderSideAt(ditchPolderSide);

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDitchPolderSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(ditchPolderSide, surfaceLine, points);
        }

        [Test]
        public void CreateBottomDitchPolderSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateBottomDitchPolderSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchPolderSidePoint_BottomDitchPolderSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchPolderSidePoint_GivenSurfaceLineWithBottomDitchPolderSide_ReturnsBottomDitchPolderSidePointsArray()
        {
            // Setup
            var bottomDitchPolderSide = new Point3D(1.2, 2.3, 4.0);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetBottomDitchPolderSideAt(bottomDitchPolderSide);

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateBottomDitchPolderSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(bottomDitchPolderSide, surfaceLine, points);
        }

        [Test]
        public void CreateBottomDitchDikeSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateBottomDitchDikeSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchDikeSidePoint_BottomDitchDikeSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateBottomDitchDikeSidePoint_GivenSurfaceLineWithBottomDitchDikeSide_ReturnsBottomDitchDikeSidePointsArray()
        {
            // Setup
            var bottomDitchDikeSide = new Point3D(1.2, 2.3, 4.0);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetBottomDitchDikeSideAt(bottomDitchDikeSide);

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateBottomDitchDikeSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(bottomDitchDikeSide, surfaceLine, points);
        }

        [Test]
        public void CreateDitchDikeSidePoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDitchDikeSidePoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchDikeSidePoint_DitchDikeSideNull_ReturnsEmptyPointsArray()
        {
            // Setup
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDitchDikeSidePoint_GivenSurfaceLineWithDitchDikeSide_ReturnsDitchDikeSidePointsArray()
        {
            // Setup
            var ditchDikeSide = new Point3D(1.2, 2.3, 4.0);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDitchDikeSideAt(ditchDikeSide);

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDitchDikeSidePoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(ditchDikeSide, surfaceLine, points);
        }

        [Test]
        public void CreateDikeToeAtRiverPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDikeToeAtRiverPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtRiverPoint_DikeToeAtRiverNull_ReturnsEmptyPointsArray()
        {
            // Setup
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtRiverPoint_GivenSurfaceLineWithDikeToeAtRiver_ReturnsDikeToeAtRiverPointsArray()
        {
            // Setup
            var dikeToeAtRiver = new Point3D(1.2, 2.3, 4.0);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDikeToeAtRiverPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeToeAtRiver, surfaceLine, points);
        }

        [Test]
        public void CreateDikeToeAtPolderPoint_SurfaceLineNull_ReturnsEmptyPointsArray()
        {
            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDikeToeAtPolderPoint(null);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtPolderPoint_DikeToeAtPolderNull_ReturnsEmptyPointsArray()
        {
            // Setup
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(points);
        }

        [Test]
        public void CreateDikeToeAtPolderPoint_GivenSurfaceLineWithDikeToeAtPolder_ReturnsDikeToeAtPolderPointsArray()
        {
            // Setup
            var dikeToeAtPolder = new Point3D(1.2, 2.3, 4.0);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            surfaceLine.SetDikeToeAtPolderAt(dikeToeAtPolder);

            // Call
            Point2D[] points = PipingChartDataPointsFactory.CreateDikeToeAtPolderPoint(surfaceLine);

            // Assert
            AssertEqualLocalPointCollection(dikeToeAtPolder, surfaceLine, points);
        }

        [Test]
        public void CreateSoilLayerAreas_SoilLayerNull_ReturnsEmptyAreasCollection()
        {
            // Setup
            var soilProfile = new PipingSoilProfile("name", 2.0, new[]
            {
                new PipingSoilLayer(3.2)
            }, SoilProfileType.SoilProfile1D);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(null, soilProfile, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SoilProfileNull_ReturnsEmptyAreasCollection()
        {
            // Setup
            var soilLayer = new PipingSoilLayer(3.2);
            PipingSurfaceLine surfaceLine = GetSurfaceLineWithGeometry();

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, null, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineNull_ReturnsEmptyAreasCollection()
        {
            // Setup
            var soilLayer = new PipingSoilLayer(3.2);
            var soilProfile = new PipingSoilProfile("name", 2.0, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, null);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineOnTopOrAboveSoilLayer_ReturnsSoilLayerPointsAsRectangle()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4),
                new Point3D(0, 0, 3.2),
                new Point3D(2, 0, 4)
            });
            var soilLayer = new PipingSoilLayer(3.2);
            var soilProfile = new PipingSoilProfile("name", 2.0, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(2, 0, 2.0)
            });
            var soilLayer = new PipingSoilLayer(3.2);
            var soilProfile = new PipingSoilProfile("name", 2.0, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas);
        }

        [Test]
        public void CreateSoilLayerAreas_SurfaceLineThroughMiddleLayerButNotSplittingIt_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 3.0)
            });
            const double bottom = 1.5;
            const double top = 2.5;
            var soilLayer = new PipingSoilLayer(top);
            var soilProfile = new PipingSoilProfile("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
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
            var soilLayer = new PipingSoilLayer(top);
            var soilProfile = new PipingSoilProfile("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 3.0)
            });
            const double bottom = 1.5;
            const double top = 2.5;
            var soilLayer = new PipingSoilLayer(top);
            var soilProfile = new PipingSoilProfile("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.0),
                new Point3D(1, 0, 2.0),
                new Point3D(2, 0, 2.0)
            });
            const double bottom = 1.5;
            const double top = 2.5;
            var soilLayer = new PipingSoilLayer(top);
            var soilProfile = new PipingSoilProfile("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

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
            var surfaceLine = new PipingSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4.0),
                new Point3D(4, 0, 0.0),
                new Point3D(8, 0, 4.0)
            });
            const int bottom = 1;
            const int top = 3;
            var soilLayer = new PipingSoilLayer(top);
            var soilProfile = new PipingSoilProfile("name", bottom, new[]
            {
                soilLayer
            }, SoilProfileType.SoilProfile1D);

            // Call
            IEnumerable<Point2D[]> areas = PipingChartDataPointsFactory.CreateSoilLayerAreas(soilLayer, soilProfile, surfaceLine).ToList();

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

        private static void AssertEqualLocalPointCollection(Point3D point, PipingSurfaceLine surfaceLine, IEnumerable<Point2D> chartPoints)
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

        private static PipingSurfaceLine GetSurfaceLineWithGeometry()
        {
            var surfaceLine = new PipingSurfaceLine("surface line");

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.8, 6.0)
            });

            return surfaceLine;
        }
    }
}