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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data.SoilProfile;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSemiProbabilisticDesignVariableFactoryTest
    {
        private Random random;
        private double mean;
        private double coefficientOfVariation;

        [SetUp]
        public void Setup()
        {
            random = new Random(21);
            mean = random.NextDouble();
            coefficientOfVariation = random.NextDouble();
        }

        [Test]
        public void GetAbovePhreaticLevel_ValidSoilLayerData_CreateDesignVariableForAbovePhreaticLevel()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerData
            {
                AbovePhreaticLevel =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation,
                    Shift = (RoundedDouble) (mean - 0.1)
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> abovePhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(properties);

            // Assert
            DistributionAssert.AreEqual(properties.AbovePhreaticLevel, abovePhreaticLevel.Distribution);
            AssertPercentile(0.5, abovePhreaticLevel);
        }

        [Test]
        public void GetBelowPhreaticLevel_ValidSoilLayerData_CreateDesignVariableForBelowPhreaticLevel()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerData
            {
                BelowPhreaticLevel = 
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation,
                    Shift = (RoundedDouble) (mean - 0.1)
                }
            };


            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> belowPhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(properties);

            // Assert
            DistributionAssert.AreEqual(properties.BelowPhreaticLevel, belowPhreaticLevel.Distribution);
            AssertPercentile(0.5, belowPhreaticLevel);
        }

        [Test]
        public void GetCohesion_ValidSoilLayerData_CreateDesignVariableForCohesion()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerData
            {
                Cohesion =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };


            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> cohesion = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(properties);

            // Assert
            DistributionAssert.AreEqual(properties.Cohesion, cohesion.Distribution);
            AssertPercentile(0.05, cohesion);
        }

        [Test]
        public void GetFrictionAngle_ValidSoilLayerData_CreateDesignVariableForFrictionAngle()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerData
            {
                FrictionAngle =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };


            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> frictionAngle = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(properties);

            // Assert
            DistributionAssert.AreEqual(properties.FrictionAngle, frictionAngle.Distribution);
            AssertPercentile(0.05, frictionAngle);
        }

        [Test]
        public void GetShearStrengthRatio_ValidSoilLayerData_CreateDesignVariableForShearStrengthRatio()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerData
            {
                ShearStrengthRatio = 
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };


            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> shearStrengthRatio = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(properties);

            // Assert
            DistributionAssert.AreEqual(properties.ShearStrengthRatio, shearStrengthRatio.Distribution);
            AssertPercentile(0.05, shearStrengthRatio);
        }

        [Test]
        public void GetStrengthIncreaseExponent_ValidSoilLayerData_CreateDesignVariableForStrengthIncreaseExponent()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerData
            {
                StrengthIncreaseExponent = 
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };


            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> strengthIncreaseExponent = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(properties);

            // Assert
            DistributionAssert.AreEqual(properties.StrengthIncreaseExponent, strengthIncreaseExponent.Distribution);
            AssertPercentile(0.05, strengthIncreaseExponent);
        }

        [Test]
        public void GetPop_ValidSoilLayerData_CreateDesignVariableForPop()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerData
            {
                Pop =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };


            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> pop = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(properties);

            // Assert
            DistributionAssert.AreEqual(properties.Pop, pop.Distribution);
            AssertPercentile(0.05, pop);
        }

        [Test]
        public void GetPreconsolidationStress_ValidPreconsolidationStress_CreateDesignVariableForPreconsolidationStress()
        {
            // Setup
            var preconsolidationStress = new MacroStabilityInwardsPreconsolidationStress(0, 0, mean, coefficientOfVariation);

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> preconsoldationStress =
                MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPreconsolidationStress(preconsolidationStress);

            // Assert
            DistributionAssert.AreEqual(preconsolidationStress.Stress, preconsoldationStress.Distribution);
            AssertPercentile(0.05, preconsoldationStress);
        }

        private static void AssertPercentile(double percentile, VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
        {
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariable>(designVariable);
            var percentileBasedDesignVariable = (VariationCoefficientLogNormalDistributionDesignVariable) designVariable;
            Assert.AreEqual(percentile, percentileBasedDesignVariable.Percentile);
        }
    }
}