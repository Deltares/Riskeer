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
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class SoilProfileUnderSurfaceLineFactoryTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowArgumentNullException()
        {
            // Setup
            var surfaceLine = new RingtoetsMacroStabilityInwardsSurfaceLine();

            // Call
            TestDelegate test = () => SoilProfileUnderSurfaceLineFactory.Create(null, surfaceLine);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Create_SurfaceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var soilProfile = new MacroStabilityInwardsSoilProfile("name", 2.0, new[]
            {
                new MacroStabilityInwardsSoilLayer(2)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            TestDelegate test = () => SoilProfileUnderSurfaceLineFactory.Create(soilProfile, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
        }

        [Test]
        public void Create_SurfaceLineOnTopOrAboveSoilLayer_ReturnsSoilLayerPointsAsRectangle()
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
            SoilProfileUnderSurfaceLine areas = SoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(1, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(2, 3.2),
                new Point2D(2, 2),
                new Point2D(0, 2),
                new Point2D(0, 3.2)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterLoop);
        }

        [Test]
        public void Create_SurfaceLineBelowSoilLayer_ReturnsEmptyAreasCollection()
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
            SoilProfileUnderSurfaceLine areas = SoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas.LayersUnderSurfaceLine);
        }

        [Test]
        public void Create_SurfaceLineThroughMiddleLayerButNotSplittingIt_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
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
            SoilProfileUnderSurfaceLine areas = SoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(1, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterLoop);
        }

        [Test]
        public void Create_SurfaceLineThroughMiddleLayerButNotSplittingItIntersectionOnTopLevel_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
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
            SoilProfileUnderSurfaceLine areas = SoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(1, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterLoop);
        }

        [Test]
        public void Create_SurfaceLineStartsBelowLayerTopButAboveBottom_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
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
            SoilProfileUnderSurfaceLine areas = SoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(1, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterLoop);
        }

        [Test]
        public void Create_SurfaceLineEndsBelowLayerTopButAboveBottom_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
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
            SoilProfileUnderSurfaceLine areas = SoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(1, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(2, 2.0),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterLoop);
        }

        [Test]
        public void Create_SurfaceLineZigZagsThroughSoilLayer_ReturnsSoilLayerPointsSplitInMultipleAreas()
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
            SoilProfileUnderSurfaceLine areas = SoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(2, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, top),
                new Point2D(3, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterLoop);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(5, bottom),
                new Point2D(7, top),
                new Point2D(8, top),
                new Point2D(8, bottom)
            }, areas.LayersUnderSurfaceLine.ElementAt(1).OuterLoop);
        }
    }
}