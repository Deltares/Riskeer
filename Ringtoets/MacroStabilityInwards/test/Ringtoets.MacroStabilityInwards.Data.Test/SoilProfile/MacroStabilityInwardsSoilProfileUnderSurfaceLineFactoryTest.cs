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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileUnderSurfaceLineFactoryTest
    {
        [Test]
        public void Create_SoilProfile1DNull_ThrowArgumentNullException()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(null, surfaceLine);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Create_SurfaceLineNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Create_SoilProfileNot1DOr2D_ThrowNotSupportedException()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<IMacroStabilityInwardsSoilProfile>();
            mocks.ReplayAll();

            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.Throws<NotSupportedException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void SoilProfile1DCreate_SurfaceLineOnTopOrAboveSoilLayer_ReturnsSoilLayerPointsAsRectangle()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
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
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine areas = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(1, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(2, 3.2),
                new Point2D(2, 2),
                new Point2D(0, 2),
                new Point2D(0, 3.2)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterRing);
        }

        [Test]
        public void SoilProfile1DCreate_SurfaceLineBelowSoilLayer_ReturnsEmptyAreasCollection()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(2, 0, 2.0)
            });
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", 2.0, new[]
            {
                soilLayer
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine areas = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(areas.LayersUnderSurfaceLine);
        }

        [Test]
        public void SoilProfile1DCreate_SurfaceLineThroughMiddleLayerButNotSplittingIt_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
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
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine areas = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

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
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterRing);
        }

        [Test]
        public void SoilProfile1DCreate_SurfaceLineThroughMiddleLayerButNotSplittingItIntersectionOnTopLevel_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
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
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine areas = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

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
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterRing);
        }

        [Test]
        public void SoilProfile1DCreate_SurfaceLineStartsBelowLayerTopButAboveBottom_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
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
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine areas = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

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
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterRing);
        }

        [Test]
        public void SoilProfile1DCreate_SurfaceLineEndsBelowLayerTopButAboveBottom_ReturnsSoilLayerPointsAsRectangleFollowingSurfaceLine()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
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
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine areas = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

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
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterRing);
        }

        [Test]
        public void SoilProfile1DCreate_SurfaceLineZigZagsThroughSoilLayer_ReturnsSoilLayerPointsSplitInMultipleAreas()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
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
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine areas = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, surfaceLine);

            // Assert
            Assert.AreEqual(2, areas.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, top),
                new Point2D(3, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.LayersUnderSurfaceLine.ElementAt(0).OuterRing);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(5, bottom),
                new Point2D(7, top),
                new Point2D(8, top),
                new Point2D(8, bottom)
            }, areas.LayersUnderSurfaceLine.ElementAt(1).OuterRing);
        }

        [Test]
        public void SoilProfile2DCreate_ProfileWithOuterRingAndHoles_ReturnsEqualGeometries()
        {
            // Setup
            Ring outerRingA = CreateRing(21);
            Ring outerRingB = CreateRing(12);
            var holesA = new[]
            {
                CreateRing(4),
                CreateRing(7)
            };
            var holesB = new[]
            {
                CreateRing(4),
                CreateRing(7),
                CreateRing(2)
            };
            IEnumerable<MacroStabilityInwardsSoilLayer2D> layers = new[]
            {
                new MacroStabilityInwardsSoilLayer2D(outerRingA, holesA),
                new MacroStabilityInwardsSoilLayer2D(outerRingB, holesB)
            };
            var profile = new MacroStabilityInwardsSoilProfile2D("name", layers);

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine profileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(profile, new MacroStabilityInwardsSurfaceLine(string.Empty));

            // Assert
            Assert.AreEqual(2, profileUnderSurfaceLine.LayersUnderSurfaceLine.Count());
            CollectionAssert.AreEqual(new[]
            {
                outerRingA.Points,
                outerRingB.Points
            }, profileUnderSurfaceLine.LayersUnderSurfaceLine.Select(layer => layer.OuterRing));
            CollectionAssert.AreEqual(new[]
            {
                holesA.Select(h => h.Points),
                holesB.Select(h => h.Points)
            }, profileUnderSurfaceLine.LayersUnderSurfaceLine.Select(layer => layer.Holes));
        }

        private static Ring CreateRing(int seed)
        {
            var random = new Random(seed);
            int x1 = random.Next();
            int y1 = random.Next();
            int x2 = x1;
            int y2 = y1 + random.Next();
            int x3 = x2 + random.Next();
            int y3 = y2;
            double x4 = x1 + (x3 - x1) * random.NextDouble();
            int y4 = y1;

            return new Ring(new[]
            {
                new Point2D(x1, y1),
                new Point2D(x2, y2),
                new Point2D(x3, y3),
                new Point2D(x4, y4)
            });
        }
    }
}