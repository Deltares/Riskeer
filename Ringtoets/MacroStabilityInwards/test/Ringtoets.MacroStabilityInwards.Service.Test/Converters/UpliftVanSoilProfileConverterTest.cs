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
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;
using Ringtoets.MacroStabilityInwards.Service.Converters;

namespace Ringtoets.MacroStabilityInwards.Service.Test.Converters
{
    [TestFixture]
    public class UpliftVanSoilProfileConverterTest
    {
        [Test]
        public void Convert_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => UpliftVanSoilProfileConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Convert_WithSoilProfile_ReturnUpliftVanSoilProfile()
        {
            // Setup
            var random = new Random(22);

            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(CreateRing(21), new List<Point2D[]>
                {
                    CreateRing(11),
                    CreateRing(22)
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    UsePop = random.NextBoolean(),
                    IsAquifer = random.NextBoolean(),
                    MaterialName = "Test",
                    AbovePhreaticLevelMean = random.NextDouble(),
                    AbovePhreaticLevelCoefficientOfVariation = random.NextDouble(),
                    BelowPhreaticLevelMean = random.NextDouble(),
                    BelowPhreaticLevelCoefficientOfVariation = random.NextDouble(),
                    CohesionMean = random.NextDouble(),
                    CohesionCoefficientOfVariation = random.NextDouble(),
                    FrictionAngleMean = random.NextDouble(),
                    FrictionAngleCoefficientOfVariation = random.NextDouble(),
                    StrengthIncreaseExponentMean = random.NextDouble(),
                    StrengthIncreaseExponentCoefficientOfVariation = random.NextDouble(),
                    ShearStrengthRatioMean = random.NextDouble(),
                    ShearStrengthRatioCoefficientOfVariation = random.NextDouble(),
                    PopMean = random.NextDouble(),
                    PopCoefficientOfVariation = random.NextDouble()
                }))
            }, new[]
            {
                new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine(new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine.ConstructionProperties
                {
                    PreconsolidationStressMean = random.NextDouble(),
                    PreconsolidationStressCoefficientOfVariation = random.NextDouble(),
                    XCoordinate = random.NextDouble(),
                    ZCoordinate = random.NextDouble()
                })
            });

            // Call
            UpliftVanSoilProfile upliftVanSoilProfile = UpliftVanSoilProfileConverter.Convert(profile);

            // Assert
            MacroStabilityInwardsSoilLayerUnderSurfaceLine[] expectedLayers = profile.Layers.ToArray();
            UpliftVanSoilLayer[] actualLayers = upliftVanSoilProfile.Layers.ToArray();

            MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine[] expectedPreconsolidationStresses = profile.PreconsolidationStresses.ToArray();
            UpliftVanPreconsolidationStress[] actualPreconsolidationStresses = upliftVanSoilProfile.PreconsolidationStresses.ToArray();

            AssertLayers(expectedLayers, actualLayers);
            AssertPreconsolidationStresses(expectedPreconsolidationStresses, actualPreconsolidationStresses);
        }

        [Test]
        public void Convert_InvalidShearStrengthModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(CreateRing(21), new Point2D[0][],
                                                                   new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                                                                   {
                                                                       ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) 99
                                                                   }))
            }, new[]
            {
                new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine(new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine.ConstructionProperties())
            });

            // Call
            TestDelegate test = () => UpliftVanSoilProfileConverter.Convert(profile);

            // Assert
            const string message = "The value of argument 'shearStrengthModel' (99) is invalid for Enum type 'MacroStabilityInwardsShearStrengthModel'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhi, UpliftVanShearStrengthModel.CPhi)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated, UpliftVanShearStrengthModel.CPhiOrSuCalculated)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.SuCalculated, UpliftVanShearStrengthModel.SuCalculated)]
        public void Convert_ValidShearStrengthModel_ReturnExpectedUpliftVanShearStrengthModel(MacroStabilityInwardsShearStrengthModel originalShearStrengthModel,
                                                                                              UpliftVanShearStrengthModel expectedShearStrengthModel)
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(CreateRing(21), new Point2D[0][],
                                                                   new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                                                                   {
                                                                       ShearStrengthModel = originalShearStrengthModel
                                                                   }))
            }, new[]
            {
                new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine(new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine.ConstructionProperties())
            });

            // Call
            UpliftVanSoilProfile upliftVanSoilProfile = UpliftVanSoilProfileConverter.Convert(profile);

            // Assert
            Assert.AreEqual(expectedShearStrengthModel, upliftVanSoilProfile.Layers.First().ShearStrengthModel);
        }

        private static void AssertPreconsolidationStresses(MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine[] expectedPreconsolidationStresses,
                                                           UpliftVanPreconsolidationStress[] actualPreconsolidationStresses)
        {
            Assert.AreEqual(expectedPreconsolidationStresses.Length, actualPreconsolidationStresses.Length);
            for (var i = 0; i < expectedPreconsolidationStresses.Length; i++)
            {
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(expectedPreconsolidationStresses[i]).GetDesignValue(), actualPreconsolidationStresses[i].Stress);
                Assert.AreEqual(expectedPreconsolidationStresses[i].XCoordinate, actualPreconsolidationStresses[i].Coordinate.X);
                Assert.AreEqual(expectedPreconsolidationStresses[i].ZCoordinate, actualPreconsolidationStresses[i].Coordinate.Y);
            }
        }

        private static void AssertLayers(MacroStabilityInwardsSoilLayerUnderSurfaceLine[] expectedLayers, UpliftVanSoilLayer[] actualLayers)
        {
            Assert.AreEqual(expectedLayers.Length, actualLayers.Length);

            for (var i = 0; i < expectedLayers.Length; i++)
            {
                Assert.AreEqual(expectedLayers[i].OuterRing, actualLayers[i].OuterRing);
                CollectionAssert.AreEqual(expectedLayers[i].Holes, actualLayers[i].Holes);

                MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine expectedProperties = expectedLayers[i].Properties;
                Assert.AreEqual(expectedProperties.MaterialName, actualLayers[i].MaterialName);
                Assert.AreEqual(expectedProperties.UsePop, actualLayers[i].UsePop);
                Assert.AreEqual(expectedProperties.IsAquifer, actualLayers[i].IsAquifer);

                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(expectedProperties).GetDesignValue(), actualLayers[i].AbovePhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(expectedProperties).GetDesignValue(), actualLayers[i].BelowPhreaticLevel);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(expectedProperties).GetDesignValue(), actualLayers[i].Cohesion);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(expectedProperties).GetDesignValue(), actualLayers[i].FrictionAngle);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(expectedProperties).GetDesignValue(), actualLayers[i].StrengthIncreaseExponent);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(expectedProperties).GetDesignValue(), actualLayers[i].ShearStrengthRatio);
                Assert.AreEqual(MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(expectedProperties).GetDesignValue(), actualLayers[i].Pop);
            }
        }

        private static Point2D[] CreateRing(int seed)
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

            return new[]
            {
                new Point2D(x1, y1),
                new Point2D(x2, y2),
                new Point2D(x3, y3),
                new Point2D(x4, y4)
            };
        }
    }
}