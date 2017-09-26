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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.Data.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsSoilProfileUnderSurfaceLineFactoryTest
    {
        [Test]
        public void Create_SoilProfileNull_ThrowArgumentNullException()
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
            var soilLayer = new MacroStabilityInwardsSoilLayer1D(3.2);
            var soilProfile = new MacroStabilityInwardsSoilProfile1D("name", 2.0, new[]
            {
                soilLayer
            });

            // Call
            TestDelegate test = () => MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(soilProfile, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("surfaceLine", exception.ParamName);
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
            Assert.AreEqual(1, areas.Layers.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(2, 3.2),
                new Point2D(2, 2),
                new Point2D(0, 2),
                new Point2D(0, 3.2)
            }, areas.Layers.ElementAt(0).OuterRing);
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
            CollectionAssert.IsEmpty(areas.Layers);
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
            Assert.AreEqual(1, areas.Layers.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.Layers.ElementAt(0).OuterRing);
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
            Assert.AreEqual(1, areas.Layers.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.Layers.ElementAt(0).OuterRing);
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
            Assert.AreEqual(1, areas.Layers.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0, 2.0),
                new Point2D(1, 2.0),
                new Point2D(1.5, top),
                new Point2D(2, top),
                new Point2D(2, bottom),
                new Point2D(0, bottom)
            }, areas.Layers.ElementAt(0).OuterRing);
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
            Assert.AreEqual(1, areas.Layers.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(0.5, top),
                new Point2D(1, 2.0),
                new Point2D(2, 2.0),
                new Point2D(2, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.Layers.ElementAt(0).OuterRing);
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
            Assert.AreEqual(2, areas.Layers.Count());
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(1, top),
                new Point2D(3, bottom),
                new Point2D(0, bottom),
                new Point2D(0, top)
            }, areas.Layers.ElementAt(0).OuterRing);
            CollectionAssert.AreEqual(new[]
            {
                new Point2D(5, bottom),
                new Point2D(7, top),
                new Point2D(8, top),
                new Point2D(8, bottom)
            }, areas.Layers.ElementAt(1).OuterRing);
        }

        [Test]
        public void SoilProfile1DCreate_WithProperties_ReturnsProperties()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4.0),
                new Point3D(4, 0, 0.0),
                new Point3D(8, 0, 4.0)
            });

            var random = new Random(21);
            bool usePop = random.NextBoolean();
            bool isAquifer = random.NextBoolean();
            var shearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>();
            const double abovePhreaticLevelMean = 10;
            const double abovePhreaticLevelCoefficientOfVariation = 0.2;
            const double abovePhreaticLevelShift = 0.1;
            const double belowPhreaticLevelMean = 9;
            const double belowPhreaticLevelCoefficientOfVariation = 0.4;
            const double belowPhreaticLevelShift = 0.2;
            double cohesionMean = random.NextDouble();
            double cohesionCoefficientOfVariation = random.NextDouble();
            double frictionAngleMean = random.NextDouble();
            double frictionAngleCoefficientOfVariation = random.NextDouble();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioCoefficientOfVariation = random.NextDouble();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            double popMean = random.NextDouble();
            double popCoefficientOfVariation = random.NextDouble();
            const string material = "Clay";

            var layer = new MacroStabilityInwardsSoilLayer1D(1)
            {
                Properties =
                {
                    UsePop = usePop,
                    IsAquifer = isAquifer,
                    ShearStrengthModel = shearStrengthModel,
                    MaterialName = material,
                    AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) abovePhreaticLevelMean,
                        CoefficientOfVariation = (RoundedDouble) abovePhreaticLevelCoefficientOfVariation,
                        Shift = (RoundedDouble) abovePhreaticLevelShift
                    },
                    BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) belowPhreaticLevelMean,
                        CoefficientOfVariation = (RoundedDouble) belowPhreaticLevelCoefficientOfVariation,
                        Shift = (RoundedDouble) belowPhreaticLevelShift
                    },
                    Cohesion = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) cohesionMean,
                        CoefficientOfVariation = (RoundedDouble) cohesionCoefficientOfVariation
                    },
                    FrictionAngle = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) frictionAngleMean,
                        CoefficientOfVariation = (RoundedDouble) frictionAngleCoefficientOfVariation
                    },
                    ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) shearStrengthRatioMean,
                        CoefficientOfVariation = (RoundedDouble) shearStrengthRatioCoefficientOfVariation
                    },
                    StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) strengthIncreaseExponentMean,
                        CoefficientOfVariation = (RoundedDouble) strengthIncreaseExponentCoefficientOfVariation
                    },
                    Pop = new VariationCoefficientLogNormalDistribution
                    {
                        Mean = (RoundedDouble) popMean,
                        CoefficientOfVariation = (RoundedDouble) popCoefficientOfVariation
                    }
                }
            };

            var profile = new MacroStabilityInwardsSoilProfile1D("name", 0, new[]
            {
                layer
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine profileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(
                profile, surfaceLine);

            // Assert
            MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine layerUnderSurfaceLineProperties = profileUnderSurfaceLine.Layers.First().Properties;
            Assert.AreEqual(usePop, layerUnderSurfaceLineProperties.UsePop);
            Assert.AreEqual(isAquifer, layerUnderSurfaceLineProperties.IsAquifer);
            Assert.AreEqual(shearStrengthModel, layerUnderSurfaceLineProperties.ShearStrengthModel);
            Assert.AreEqual(material, layerUnderSurfaceLineProperties.MaterialName);

            AssertDistributionsAndDesignVariables(layerUnderSurfaceLineProperties,
                                                  abovePhreaticLevelMean, abovePhreaticLevelCoefficientOfVariation,
                                                  belowPhreaticLevelMean, belowPhreaticLevelCoefficientOfVariation,
                                                  cohesionMean, cohesionCoefficientOfVariation,
                                                  frictionAngleMean, frictionAngleCoefficientOfVariation,
                                                  shearStrengthRatioMean, shearStrengthRatioCoefficientOfVariation,
                                                  strengthIncreaseExponentMean, strengthIncreaseExponentCoefficientOfVariation,
                                                  popMean, popCoefficientOfVariation);
        }

        [Test]
        public void SoilProfile1DCreate_ValidSoilProfile1D_ReturnsEmptyPreconsolidationStresses()
        {
            // Setup
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 4.0),
                new Point3D(4, 0, 0.0),
                new Point3D(8, 0, 4.0)
            });

            var layer = new MacroStabilityInwardsSoilLayer1D(1);
            var profile = new MacroStabilityInwardsSoilProfile1D("name", 0, new[]
            {
                layer
            });

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine profileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(
                profile, surfaceLine);

            // Assert
            CollectionAssert.IsEmpty(profileUnderSurfaceLine.PreconsolidationStresses);
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
            var profile = new MacroStabilityInwardsSoilProfile2D("name", layers, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine profileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(profile, new MacroStabilityInwardsSurfaceLine(string.Empty));

            // Assert
            Assert.AreEqual(2, profileUnderSurfaceLine.Layers.Count());
            CollectionAssert.AreEqual(new[]
            {
                outerRingA.Points,
                outerRingB.Points
            }, profileUnderSurfaceLine.Layers.Select(layer => layer.OuterRing));
            CollectionAssert.AreEqual(new[]
            {
                holesA.Select(h => h.Points),
                holesB.Select(h => h.Points)
            }, profileUnderSurfaceLine.Layers.Select(layer => layer.Holes));
        }

        [Test]
        public void SoilProfile2DCreate_WithProperties_ReturnsProperties()
        {
            // Setup
            const string material = "Clay";

            var random = new Random(21);
            bool usePop = random.NextBoolean();
            bool isAquifer = random.NextBoolean();
            var shearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>();

            const double abovePhreaticLevelMean = 10;
            const double abovePhreaticLevelCoefficientOfVariation = 0.5;
            const double abovePhreaticLevelShift = 1;
            const double belowPhreaticLevelMean = 9;
            const double belowPhreaticLevelCoefficientOfVariation = 0.4;
            const double belowPhreaticLevelShift = 0.2;
            double cohesionMean = random.NextDouble();
            double cohesionCoefficientOfVariation = random.NextDouble();
            double frictionAngleMean = random.NextDouble();
            double frictionAngleCoefficientOfVariation = random.NextDouble();
            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioCoefficientOfVariation = random.NextDouble();
            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            double popMean = random.NextDouble();
            double popCoefficientOfVariation = random.NextDouble();

            MacroStabilityInwardsSoilLayer2D layer = GetSoilLayer();
            layer.Properties.UsePop = usePop;
            layer.Properties.IsAquifer = isAquifer;
            layer.Properties.ShearStrengthModel = shearStrengthModel;
            layer.Properties.MaterialName = material;

            layer.Properties.AbovePhreaticLevel = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) abovePhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) abovePhreaticLevelCoefficientOfVariation,
                Shift = (RoundedDouble) abovePhreaticLevelShift
            };
            layer.Properties.BelowPhreaticLevel = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) belowPhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) belowPhreaticLevelCoefficientOfVariation,
                Shift = (RoundedDouble) belowPhreaticLevelShift
            };
            layer.Properties.Cohesion = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) cohesionMean,
                CoefficientOfVariation = (RoundedDouble) cohesionCoefficientOfVariation
            };
            layer.Properties.FrictionAngle = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) frictionAngleMean,
                CoefficientOfVariation = (RoundedDouble) frictionAngleCoefficientOfVariation
            };
            layer.Properties.ShearStrengthRatio = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) shearStrengthRatioMean,
                CoefficientOfVariation = (RoundedDouble) shearStrengthRatioCoefficientOfVariation
            };
            layer.Properties.StrengthIncreaseExponent = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) strengthIncreaseExponentMean,
                CoefficientOfVariation = (RoundedDouble) strengthIncreaseExponentCoefficientOfVariation
            };
            layer.Properties.Pop = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) popMean,
                CoefficientOfVariation = (RoundedDouble) popCoefficientOfVariation
            };

            var profile = new MacroStabilityInwardsSoilProfile2D("name", new[]
            {
                layer
            }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>());

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine profileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(
                profile, new MacroStabilityInwardsSurfaceLine(string.Empty));

            // Assert
            MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine layerUnderSurfaceLineProperties = profileUnderSurfaceLine.Layers.First().Properties;
            Assert.AreEqual(usePop, layerUnderSurfaceLineProperties.UsePop);
            Assert.AreEqual(isAquifer, layerUnderSurfaceLineProperties.IsAquifer);
            Assert.AreEqual(shearStrengthModel, layerUnderSurfaceLineProperties.ShearStrengthModel);
            Assert.AreEqual(material, layerUnderSurfaceLineProperties.MaterialName);

            AssertDistributionsAndDesignVariables(layerUnderSurfaceLineProperties,
                                                  abovePhreaticLevelMean, abovePhreaticLevelCoefficientOfVariation,
                                                  belowPhreaticLevelMean, belowPhreaticLevelCoefficientOfVariation,
                                                  cohesionMean, cohesionCoefficientOfVariation,
                                                  frictionAngleMean, frictionAngleCoefficientOfVariation,
                                                  shearStrengthRatioMean, shearStrengthRatioCoefficientOfVariation,
                                                  strengthIncreaseExponentMean, strengthIncreaseExponentCoefficientOfVariation,
                                                  popMean, popCoefficientOfVariation);
        }

        [Test]
        [TestCaseSource(nameof(GetPreconsolidationStresses))]
        public void SoilProfile2DCreate_WithPreconsolidationStresses_ReturnsExpectedPreconsolidationStresses(
            IEnumerable<MacroStabilityInwardsPreconsolidationStress> preconsolidationStresses)
        {
            // Setup
            MacroStabilityInwardsPreconsolidationStress[] expectedStresses = preconsolidationStresses.ToArray();
            var profile = new MacroStabilityInwardsSoilProfile2D("name", new[]
            {
                GetSoilLayer()
            }, expectedStresses);

            // Call
            MacroStabilityInwardsSoilProfileUnderSurfaceLine profileUnderSurfaceLine = MacroStabilityInwardsSoilProfileUnderSurfaceLineFactory.Create(
                profile, new MacroStabilityInwardsSurfaceLine(string.Empty));

            // Assert
            MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine[] preconsolidationStressesUnderSurfaceLine = profileUnderSurfaceLine.PreconsolidationStresses.ToArray();
            int expectedNrOfStresses = expectedStresses.Length;
            Assert.AreEqual(expectedNrOfStresses, preconsolidationStressesUnderSurfaceLine.Length);
            for (var i = 0; i < expectedNrOfStresses; i++)
            {
                AssertPreconsolidationStress(expectedStresses[i], preconsolidationStressesUnderSurfaceLine[i]);
            }
        }

        private static void AssertPreconsolidationStress(MacroStabilityInwardsPreconsolidationStress preconsolidationStress,
                                                         MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine preconsolidationStressUnderSurfaceLine)
        {
            Assert.AreEqual(preconsolidationStress.Location.X, preconsolidationStressUnderSurfaceLine.XCoordinate);
            Assert.AreEqual(preconsolidationStress.Location.Y, preconsolidationStressUnderSurfaceLine.ZCoordinate);

            Assert.AreEqual(preconsolidationStress.Stress.Mean,
                            preconsolidationStressUnderSurfaceLine.PreconsolidationStress.Mean,
                            preconsolidationStressUnderSurfaceLine.PreconsolidationStress.GetAccuracy());
            Assert.AreEqual(preconsolidationStress.Stress.CoefficientOfVariation,
                            preconsolidationStressUnderSurfaceLine.PreconsolidationStress.CoefficientOfVariation,
                            preconsolidationStressUnderSurfaceLine.PreconsolidationStress.GetAccuracy());

            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(preconsolidationStressUnderSurfaceLine).GetDesignValue(),
                            preconsolidationStressUnderSurfaceLine.PreconsolidationStressDesignVariable);
        }

        private static void AssertDistributionsAndDesignVariables(MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine properties,
                                                                  double abovePhreaticLevelMean, double abovePhreaticLevelCoefficientOfVariation,
                                                                  double belowPhreaticLevelMean, double belowPhreaticLevelCoefficientOfVariation,
                                                                  double cohesionMean, double cohesionCoefficientOfVariation,
                                                                  double frictionAngleMean, double frictionAngleCoefficientOfVariation,
                                                                  double shearStrengthRatioMean, double shearStrengthRatioCoefficientOfVariation,
                                                                  double strengthIncreaseExponentMean, double strengthIncreaseExponentCoefficientOfVariation,
                                                                  double popMean, double popCoefficientOfVariation)
        {
            Assert.AreEqual(abovePhreaticLevelMean, properties.AbovePhreaticLevel.Mean,
                            properties.AbovePhreaticLevel.Mean.GetAccuracy());
            Assert.AreEqual(abovePhreaticLevelCoefficientOfVariation, properties.AbovePhreaticLevel.CoefficientOfVariation,
                            properties.AbovePhreaticLevel.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(properties).GetDesignValue(),
                            properties.AbovePhreaticLevelDesignVariable);

            Assert.AreEqual(belowPhreaticLevelMean, properties.BelowPhreaticLevel.Mean,
                            properties.BelowPhreaticLevel.Mean.GetAccuracy());
            Assert.AreEqual(belowPhreaticLevelCoefficientOfVariation, properties.BelowPhreaticLevel.CoefficientOfVariation,
                            properties.BelowPhreaticLevel.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(properties).GetDesignValue(),
                            properties.BelowPhreaticLevelDesignVariable);

            Assert.AreEqual(cohesionMean, properties.Cohesion.Mean,
                            properties.Cohesion.Mean.GetAccuracy());
            Assert.AreEqual(cohesionCoefficientOfVariation, properties.Cohesion.CoefficientOfVariation,
                            properties.Cohesion.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(properties).GetDesignValue(),
                            properties.CohesionDesignVariable);

            Assert.AreEqual(frictionAngleMean, properties.FrictionAngle.Mean,
                            properties.FrictionAngle.Mean.GetAccuracy());
            Assert.AreEqual(frictionAngleCoefficientOfVariation, properties.FrictionAngle.CoefficientOfVariation,
                            properties.FrictionAngle.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(properties).GetDesignValue(),
                            properties.FrictionAngleDesignVariable);

            Assert.AreEqual(shearStrengthRatioMean, properties.ShearStrengthRatio.Mean,
                            properties.ShearStrengthRatio.Mean.GetAccuracy());
            Assert.AreEqual(shearStrengthRatioCoefficientOfVariation, properties.ShearStrengthRatio.CoefficientOfVariation,
                            properties.ShearStrengthRatio.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(properties).GetDesignValue(),
                            properties.ShearStrengthRatioDesignVariable);

            Assert.AreEqual(strengthIncreaseExponentMean, properties.StrengthIncreaseExponent.Mean,
                            properties.StrengthIncreaseExponent.Mean.GetAccuracy());
            Assert.AreEqual(strengthIncreaseExponentCoefficientOfVariation, properties.StrengthIncreaseExponent.CoefficientOfVariation,
                            properties.StrengthIncreaseExponent.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(properties).GetDesignValue(),
                            properties.StrengthIncreaseExponentDesignVariable);

            Assert.AreEqual(popMean, properties.Pop.Mean,
                            properties.Pop.Mean.GetAccuracy());
            Assert.AreEqual(popCoefficientOfVariation, properties.Pop.CoefficientOfVariation,
                            properties.Pop.CoefficientOfVariation.GetAccuracy());
            Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(properties).GetDesignValue(),
                            properties.PopDesignVariable);
        }

        private static MacroStabilityInwardsSoilLayer2D GetSoilLayer()
        {
            return new MacroStabilityInwardsSoilLayer2D(CreateRing(21), Enumerable.Empty<Ring>());
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

        private static IEnumerable<TestCaseData> GetPreconsolidationStresses()
        {
            yield return new TestCaseData(Enumerable.Empty<MacroStabilityInwardsPreconsolidationStress>())
                .SetName("No preconsolidation stresses");

            var random = new Random(21);
            var preconsolidationStresses = new List<MacroStabilityInwardsPreconsolidationStress>
            {
                new MacroStabilityInwardsPreconsolidationStress(random.NextDouble(),
                                                                random.NextDouble(),
                                                                random.NextDouble(),
                                                                random.NextDouble()),
                new MacroStabilityInwardsPreconsolidationStress(random.NextDouble(),
                                                                random.NextDouble(),
                                                                random.NextDouble(),
                                                                random.NextDouble())
            };
            yield return new TestCaseData(preconsolidationStresses)
                .SetName("Multiple preconsolidation stresses");
        }
    }
}