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
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Primitives.Test
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
            var data = new MacroStabilityInwardsSoilLayerData
            {
                AbovePhreaticLevel =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation,
                    Shift = (RoundedDouble) (mean - 0.1)
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> abovePhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetAbovePhreaticLevel(data);

            // Assert
            DistributionAssert.AreEqual(data.AbovePhreaticLevel, abovePhreaticLevel.Distribution);
            AssertPercentile(0.5, abovePhreaticLevel);
        }

        [Test]
        public void GetBelowPhreaticLevel_ValidSoilLayerData_CreateDesignVariableForBelowPhreaticLevel()
        {
            // Setup
            var data = new MacroStabilityInwardsSoilLayerData
            {
                BelowPhreaticLevel =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation,
                    Shift = (RoundedDouble) (mean - 0.1)
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> belowPhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetBelowPhreaticLevel(data);

            // Assert
            DistributionAssert.AreEqual(data.BelowPhreaticLevel, belowPhreaticLevel.Distribution);
            AssertPercentile(0.5, belowPhreaticLevel);
        }

        [Test]
        public void GetCohesion_ValidSoilLayerData_CreateDesignVariableForCohesion()
        {
            // Setup
            var data = new MacroStabilityInwardsSoilLayerData
            {
                Cohesion =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> cohesion = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetCohesion(data);

            // Assert
            DistributionAssert.AreEqual(data.Cohesion, cohesion.Distribution);
            AssertPercentile(0.05, cohesion);
        }

        [Test]
        public void GetFrictionAngle_ValidSoilLayerData_CreateDesignVariableForFrictionAngle()
        {
            // Setup
            var data = new MacroStabilityInwardsSoilLayerData
            {
                FrictionAngle =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> frictionAngle = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetFrictionAngle(data);

            // Assert
            DistributionAssert.AreEqual(data.FrictionAngle, frictionAngle.Distribution);
            AssertPercentile(0.05, frictionAngle);
        }

        [Test]
        public void GetShearStrengthRatio_ValidSoilLayerData_CreateDesignVariableForShearStrengthRatio()
        {
            // Setup
            var data = new MacroStabilityInwardsSoilLayerData
            {
                ShearStrengthRatio =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> shearStrengthRatio = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetShearStrengthRatio(data);

            // Assert
            DistributionAssert.AreEqual(data.ShearStrengthRatio, shearStrengthRatio.Distribution);
            AssertPercentile(0.05, shearStrengthRatio);
        }

        [Test]
        public void GetStrengthIncreaseExponent_ValidSoilLayerData_CreateDesignVariableForStrengthIncreaseExponent()
        {
            // Setup
            var data = new MacroStabilityInwardsSoilLayerData
            {
                StrengthIncreaseExponent =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> strengthIncreaseExponent = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetStrengthIncreaseExponent(data);

            // Assert
            DistributionAssert.AreEqual(data.StrengthIncreaseExponent, strengthIncreaseExponent.Distribution);
            AssertPercentile(0.05, strengthIncreaseExponent);
        }

        [Test]
        public void GetPop_ValidSoilLayerData_CreateDesignVariableForPop()
        {
            // Setup
            var data = new MacroStabilityInwardsSoilLayerData
            {
                Pop =
                {
                    Mean = (RoundedDouble) mean,
                    CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                }
            };

            // Call
            VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> pop = MacroStabilityInwardsSemiProbabilisticDesignVariableFactory.GetPop(data);

            // Assert
            DistributionAssert.AreEqual(data.Pop, pop.Distribution);
            AssertPercentile(0.05, pop);
        }

        [Test]
        public void GetPreconsolidationStress_ValidPreconsolidationStress_CreateDesignVariableForPreconsolidationStress()
        {
            // Setup
            var location = new Point2D(random.NextDouble(), random.NextDouble());
            var stressDistribution = new VariationCoefficientLogNormalDistribution
            {
                Mean = (RoundedDouble) 0.005,
                CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
            };

            var preconsolidationStress = new MacroStabilityInwardsPreconsolidationStress(location, stressDistribution);

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