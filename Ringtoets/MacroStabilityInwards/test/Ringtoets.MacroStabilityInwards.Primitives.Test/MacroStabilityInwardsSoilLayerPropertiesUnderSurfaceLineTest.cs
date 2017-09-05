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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLineTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_WithConstructionProperties_ExpectedValues()
        {
            // Setup
            const string materialName = "Clay";

            var random = new Random();
            var shearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>();
            bool usePop = random.NextBoolean();
            bool isAquifer = random.NextBoolean();

            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelCoefficientOfVariation = random.NextDouble();
            RoundedDouble abovePhreaticLevelDesignVariable = random.NextRoundedDouble();

            double belowPhreaticLevelMean = random.NextDouble();
            double belowPhreaticLevelCoefficientOfVariation = random.NextDouble();
            RoundedDouble belowPhreaticLevelDesignVariable = random.NextRoundedDouble();

            double cohesionMean = random.NextDouble();
            double cohesionCoefficientOfVariation = random.NextDouble();
            RoundedDouble cohesionDesignVariable = random.NextRoundedDouble();

            double frictionAngleMean = random.NextDouble();
            double frictionAngleCoefficientOfVariation = random.NextDouble();
            RoundedDouble frictionAngleDesignVariable = random.NextRoundedDouble();

            double strengthIncreaseExponentMean = random.NextDouble();
            double strengthIncreaseExponentCoefficientOfVariation = random.NextDouble();
            RoundedDouble strengthIncreaseExponentDesignVariable = random.NextRoundedDouble();

            double shearStrengthRatioMean = random.NextDouble();
            double shearStrengthRatioCoefficientOfVariation = random.NextDouble();
            RoundedDouble shearStrengthRatioDesignVariable = random.NextRoundedDouble();

            double popMean = random.NextDouble();
            double popCoefficientOfVariation = random.NextDouble();
            RoundedDouble popDesignVariable = random.NextRoundedDouble();

            var constructionProperties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
            {
                ShearStrengthModel = shearStrengthModel,
                UsePop = usePop,
                IsAquifer = isAquifer,
                MaterialName = materialName,
                AbovePhreaticLevelMean = abovePhreaticLevelMean,
                AbovePhreaticLevelCoefficientOfVariation = abovePhreaticLevelCoefficientOfVariation,
                BelowPhreaticLevelMean = belowPhreaticLevelMean,
                BelowPhreaticLevelCoefficientOfVariation = belowPhreaticLevelCoefficientOfVariation,
                CohesionMean = cohesionMean,
                CohesionCoefficientOfVariation = cohesionCoefficientOfVariation,
                FrictionAngleMean = frictionAngleMean,
                FrictionAngleCoefficientOfVariation = frictionAngleCoefficientOfVariation,
                StrengthIncreaseExponentMean = strengthIncreaseExponentMean,
                StrengthIncreaseExponentCoefficientOfVariation = strengthIncreaseExponentCoefficientOfVariation,
                ShearStrengthRatioMean = shearStrengthRatioMean,
                ShearStrengthRatioCoefficientOfVariation = shearStrengthRatioCoefficientOfVariation,
                PopMean = popMean,
                PopCoefficientOfVariation = popCoefficientOfVariation
            };

            // Call
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(constructionProperties)
            {
                AbovePhreaticLevelDesignVariable = abovePhreaticLevelDesignVariable,
                BelowPhreaticLevelDesignVariable = belowPhreaticLevelDesignVariable,
                CohesionDesignVariable = cohesionDesignVariable,
                FrictionAngleDesignVariable = frictionAngleDesignVariable,
                StrengthIncreaseExponentDesignVariable = strengthIncreaseExponentDesignVariable,
                ShearStrengthRatioDesignVariable = shearStrengthRatioDesignVariable,
                PopDesignVariable = popDesignVariable
            };

            // Assert
            Assert.AreEqual(shearStrengthModel, properties.ShearStrengthModel);
            Assert.AreEqual(usePop, properties.UsePop);
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(materialName, properties.MaterialName);
            Assert.AreEqual(abovePhreaticLevelDesignVariable, properties.AbovePhreaticLevelDesignVariable);
            Assert.AreEqual(belowPhreaticLevelDesignVariable, properties.BelowPhreaticLevelDesignVariable);
            Assert.AreEqual(cohesionDesignVariable, properties.CohesionDesignVariable);
            Assert.AreEqual(frictionAngleDesignVariable, properties.FrictionAngleDesignVariable);
            Assert.AreEqual(strengthIncreaseExponentDesignVariable, properties.StrengthIncreaseExponentDesignVariable);
            Assert.AreEqual(shearStrengthRatioDesignVariable, properties.ShearStrengthRatioDesignVariable);
            Assert.AreEqual(popDesignVariable, properties.PopDesignVariable);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) abovePhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) abovePhreaticLevelCoefficientOfVariation
            }, properties.AbovePhreaticLevel);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) belowPhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) belowPhreaticLevelCoefficientOfVariation
            }, properties.BelowPhreaticLevel);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) cohesionMean,
                CoefficientOfVariation = (RoundedDouble) cohesionCoefficientOfVariation
            }, properties.Cohesion);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) frictionAngleMean,
                CoefficientOfVariation = (RoundedDouble) frictionAngleCoefficientOfVariation
            }, properties.FrictionAngle);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) strengthIncreaseExponentMean,
                CoefficientOfVariation = (RoundedDouble) strengthIncreaseExponentCoefficientOfVariation
            }, properties.StrengthIncreaseExponent);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) shearStrengthRatioMean,
                CoefficientOfVariation = (RoundedDouble) shearStrengthRatioCoefficientOfVariation
            }, properties.ShearStrengthRatio);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) popMean,
                CoefficientOfVariation = (RoundedDouble) popCoefficientOfVariation
            }, properties.Pop);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Call
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties());

            // Assert
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.AbovePhreaticLevel);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.BelowPhreaticLevel);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.Cohesion);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.FrictionAngle);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.StrengthIncreaseExponent);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.ShearStrengthRatio);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, properties.Pop);

            Assert.AreEqual(MacroStabilityInwardsShearStrengthModel.None, properties.ShearStrengthModel);
            Assert.IsFalse(properties.UsePop);
            Assert.IsFalse(properties.IsAquifer);
            Assert.AreEqual(string.Empty, properties.MaterialName);
        }
    }
}