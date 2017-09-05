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
            var random = new Random();
            var shearStrengthModel = random.NextEnumValue<MacroStabilityInwardsShearStrengthModel>();
            bool usePop = random.NextBoolean();
            bool isAquifer = random.NextBoolean();
            double abovePhreaticLevelMean = random.NextDouble();
            double abovePhreaticLevelCoefficientOfVariation = random.NextDouble();
            RoundedDouble abovePhreaticLevelDesignVariable = random.NextRoundedDouble();
            const string materialName = "Clay";

            var constructionProperties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
            {
                ShearStrengthModel = shearStrengthModel,
                UsePop = usePop,
                IsAquifer = isAquifer,
                MaterialName = materialName,
                AbovePhreaticLevelMean = abovePhreaticLevelMean,
                AbovePhreaticLevelCoefficientOfVariation = abovePhreaticLevelCoefficientOfVariation
            };

            // Call
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(constructionProperties)
            {
                AbovePhreaticLevelDesignVariable = abovePhreaticLevelDesignVariable
            };

            // Assert
            Assert.AreEqual(shearStrengthModel, properties.ShearStrengthModel);
            Assert.AreEqual(usePop, properties.UsePop);
            Assert.AreEqual(isAquifer, properties.IsAquifer);
            Assert.AreEqual(materialName, properties.MaterialName);
            Assert.AreEqual(abovePhreaticLevelDesignVariable, properties.AbovePhreaticLevelDesignVariable);
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) abovePhreaticLevelMean,
                CoefficientOfVariation = (RoundedDouble) abovePhreaticLevelCoefficientOfVariation
            }, properties.AbovePhreaticLevel);
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
            Assert.AreEqual(MacroStabilityInwardsShearStrengthModel.None, properties.ShearStrengthModel);
            Assert.IsFalse(properties.UsePop);
            Assert.IsFalse(properties.IsAquifer);
            Assert.AreEqual(string.Empty, properties.MaterialName);
        }
    }
}