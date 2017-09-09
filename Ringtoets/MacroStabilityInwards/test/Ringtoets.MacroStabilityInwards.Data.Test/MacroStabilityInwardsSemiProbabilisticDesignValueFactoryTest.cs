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
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSemiProbabilisticDesignValueFactoryTest
    {
        private Random random;
        private double mean;
        private double coefficientOfVariation;

        [SetUp]
        public void Setup()
        {
            random = new Random();
            mean = random.NextDouble();
            coefficientOfVariation = random.NextDouble();
        }

        [Test]
        public void GetAbovePhreaticLevel_ValidSoilLayerProperties_CreateDesignVariableForAbovePhreaticLevel()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    AbovePhreaticLevelMean = mean,
                    AbovePhreaticLevelCoefficientOfVariation = coefficientOfVariation
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable abovePhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetAbovePhreaticLevel(properties);

            // Assert
            DistributionAssert.AreEqual(properties.AbovePhreaticLevel, abovePhreaticLevel.Distribution);
            AssertPercentile(0.5, abovePhreaticLevel);
        }

        [Test]
        public void GetBelowPhreaticLevel_ValidSoilLayerProperties_CreateDesignVariableForBelowPhreaticLevel()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    BelowPhreaticLevelMean = mean,
                    BelowPhreaticLevelCoefficientOfVariation = coefficientOfVariation
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable belowPhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetBelowPhreaticLevel(properties);

            // Assert
            DistributionAssert.AreEqual(properties.BelowPhreaticLevel, belowPhreaticLevel.Distribution);
            AssertPercentile(0.5, belowPhreaticLevel);
        }

        [Test]
        public void GetCohesion_ValidSoilLayerProperties_CreateDesignVariableForCohesion()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    CohesionMean = mean,
                    CohesionCoefficientOfVariation = coefficientOfVariation
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable cohesion = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetCohesion(properties);

            // Assert
            DistributionAssert.AreEqual(properties.Cohesion, cohesion.Distribution);
            AssertPercentile(0.05, cohesion);
        }

        [Test]
        public void GetFrictionAngle_ValidSoilLayerProperties_CreateDesignVariableForFrictionAngle()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    FrictionAngleMean = mean,
                    FrictionAngleCoefficientOfVariation = coefficientOfVariation
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable frictionAngle = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetFrictionAngle(properties);

            // Assert
            DistributionAssert.AreEqual(properties.FrictionAngle, frictionAngle.Distribution);
            AssertPercentile(0.05, frictionAngle);
        }

        [Test]
        public void GetShearStrengthRatio_ValidSoilLayerProperties_CreateDesignVariableForShearStrengthRatio()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    ShearStrengthRatioMean = mean,
                    ShearStrengthRatioCoefficientOfVariation = coefficientOfVariation
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable shearStrengthRatio = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetShearStrengthRatio(properties);

            // Assert
            DistributionAssert.AreEqual(properties.ShearStrengthRatio, shearStrengthRatio.Distribution);
            AssertPercentile(0.05, shearStrengthRatio);
        }

        [Test]
        public void GetStrengthIncreaseExponent_ValidSoilLayerProperties_CreateDesignVariableForStrengthIncreaseExponent()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    StrengthIncreaseExponentMean = mean,
                    StrengthIncreaseExponentCoefficientOfVariation = coefficientOfVariation
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable strengthIncreaseExponent = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetStrengthIncreaseExponent(properties);

            // Assert
            DistributionAssert.AreEqual(properties.StrengthIncreaseExponent, strengthIncreaseExponent.Distribution);
            AssertPercentile(0.05, strengthIncreaseExponent);
        }

        [Test]
        public void GetPop_ValidSoilLayerProperties_CreateDesignVariableForPop()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    PopMean = mean,
                    PopCoefficientOfVariation = coefficientOfVariation
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable pop = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetPop(properties);

            // Assert
            DistributionAssert.AreEqual(properties.Pop, pop.Distribution);
            AssertPercentile(0.05, pop);
        }

        private static void AssertPercentile(double percentile, VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
        {
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariable>(designVariable);
            var percentileBasedDesignVariable = (VariationCoefficientLogNormalDistributionDesignVariable)designVariable;
            Assert.AreEqual(percentile, percentileBasedDesignVariable.Percentile);
        }
    }
}