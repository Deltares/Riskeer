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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.MacroStabilityInwards.CalculatedInput.Converters;
using Ringtoets.MacroStabilityInwards.CalculatedInput.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.TestUtil;

namespace Riskeer.MacroStabilityInwards.CalculatedInput.Test.Converters
{
    [TestFixture]
    public class SoilProfileConverterTest
    {
        [Test]
        public void Convert_SoilProfileNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => SoilProfileConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("soilProfile", exception.ParamName);
        }

        [Test]
        public void Convert_WithSoilProfile_ReturnSoilProfile()
        {
            // Setup
            var random = new Random(22);

            MacroStabilityInwardsSoilLayer2D soilLayer1 = CreateRandomSoilLayer(22, new[]
            {
                CreateRandomSoilLayer(23, Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>()),
                CreateRandomSoilLayer(24, Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>())
            });

            MacroStabilityInwardsSoilLayer2D soilLayer2 = CreateRandomSoilLayer(25, new[]
            {
                CreateRandomSoilLayer(26, new[]
                {
                    CreateRandomSoilLayer(27, Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>())
                })
            });

            var preconsolidationStress = new MacroStabilityInwardsPreconsolidationStress(new Point2D(random.NextDouble(), random.NextDouble()),
                                                                                         new VariationCoefficientLogNormalDistribution
                                                                                         {
                                                                                             Mean = (RoundedDouble) 0.05,
                                                                                             CoefficientOfVariation = random.NextRoundedDouble()
                                                                                         });

            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(
                new[]
                {
                    soilLayer1,
                    soilLayer2
                },
                new[]
                {
                    preconsolidationStress
                });

            // Call
            SoilProfile soilProfile = SoilProfileConverter.Convert(profile);

            // Assert
            CalculatorInputAssert.AssertSoilProfile(profile, soilProfile);
        }

        [Test]
        public void Convert_SoilProfileWithSoilLayerWithEmptyName_ReturnSoilProfile()
        {
            // Setup
            MacroStabilityInwardsSoilLayer2D soilLayer = CreateRandomSoilLayer(22, Enumerable.Empty<MacroStabilityInwardsSoilLayer2D>());
            soilLayer.Data.MaterialName = string.Empty;

            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(
                new[]
                {
                    soilLayer
                },
                new IMacroStabilityInwardsPreconsolidationStress[0]);

            // Call
            SoilProfile soilProfile = SoilProfileConverter.Convert(profile);

            // Assert
            Assert.AreEqual(1, soilProfile.Layers.Count());
            Assert.AreEqual("Onbekend", soilProfile.Layers.First().MaterialName);
        }

        [Test]
        public void Convert_InvalidShearStrengthModel_ThrowInvalidEnumArgumentException()
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing())
                {
                    Data =
                    {
                        ShearStrengthModel = (MacroStabilityInwardsShearStrengthModel) 99
                    }
                }
            }, new MacroStabilityInwardsPreconsolidationStress[0]);

            // Call
            TestDelegate test = () => SoilProfileConverter.Convert(profile);

            // Assert
            const string message = "The value of argument 'shearStrengthModel' (99) is invalid for Enum type 'MacroStabilityInwardsShearStrengthModel'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhi, ShearStrengthModel.CPhi)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.CPhiOrSuCalculated, ShearStrengthModel.CPhiOrSuCalculated)]
        [TestCase(MacroStabilityInwardsShearStrengthModel.SuCalculated, ShearStrengthModel.SuCalculated)]
        public void Convert_ValidShearStrengthModel_ReturnExpectedShearStrengthModel(MacroStabilityInwardsShearStrengthModel originalShearStrengthModel,
                                                                                     ShearStrengthModel expectedShearStrengthModel)
        {
            // Setup
            var profile = new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing())
                {
                    Data =
                    {
                        ShearStrengthModel = originalShearStrengthModel
                    }
                }
            }, new MacroStabilityInwardsPreconsolidationStress[0]);

            // Call
            SoilProfile soilProfile = SoilProfileConverter.Convert(profile);

            // Assert
            Assert.AreEqual(expectedShearStrengthModel, soilProfile.Layers.First().ShearStrengthModel);
        }

        private static MacroStabilityInwardsSoilLayer2D CreateRandomSoilLayer(int seed, IEnumerable<MacroStabilityInwardsSoilLayer2D> nestedLayers)
        {
            return new MacroStabilityInwardsSoilLayer2D(RingTestFactory.CreateRandomRing(seed),
                                                        CreateRandomSoilLayerData(seed),
                                                        nestedLayers);
        }

        private static MacroStabilityInwardsSoilLayerData CreateRandomSoilLayerData(int seed)
        {
            var random = new Random(seed);

            return new MacroStabilityInwardsSoilLayerData
            {
                UsePop = random.NextBoolean(),
                IsAquifer = random.NextBoolean(),
                MaterialName = "Test",
                AbovePhreaticLevel =
                {
                    Mean = random.NextRoundedDouble(1, 10),
                    CoefficientOfVariation = random.NextRoundedDouble(0, 1),
                    Shift = random.NextRoundedDouble(0, 1)
                },
                BelowPhreaticLevel =
                {
                    Mean = random.NextRoundedDouble(1, 10),
                    CoefficientOfVariation = random.NextRoundedDouble(0, 1),
                    Shift = random.NextRoundedDouble(0, 1)
                },
                Cohesion =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                FrictionAngle =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                StrengthIncreaseExponent =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                ShearStrengthRatio =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                },
                Pop =
                {
                    Mean = random.NextRoundedDouble(),
                    CoefficientOfVariation = random.NextRoundedDouble()
                }
            };
        }
    }
}