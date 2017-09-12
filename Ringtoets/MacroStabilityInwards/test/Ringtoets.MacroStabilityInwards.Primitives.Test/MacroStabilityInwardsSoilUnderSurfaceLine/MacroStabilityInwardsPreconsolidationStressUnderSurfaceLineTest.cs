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
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.Primitives.Test.MacroStabilityInwardsSoilUnderSurfaceLine
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressUnderSurfaceLineTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_DefaultConstructionProperties_ReturnsExpectedValues()
        {
            // Setup
            var constructionProperties = new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine.ConstructionProperties();

            // Call
            var stressProperties = new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine(constructionProperties);

            // Assert
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = RoundedDouble.NaN,
                CoefficientOfVariation = RoundedDouble.NaN
            }, stressProperties.PreconsolidationStress);

            Assert.IsNaN(stressProperties.XCoordinate);
            Assert.IsNaN(stressProperties.ZCoordinate);
            Assert.IsNaN(stressProperties.PreconsolidationStressDesignVariable);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithValues_ReturnsExpectedValues()
        {
            // Setup
            var random = new Random(30);
            var constructionProperties = new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine.ConstructionProperties
            {
                XCoordinate = random.NextDouble(),
                ZCoordinate = random.NextDouble(),
                PreconsolidationStressMean = random.NextDouble(),
                PreconsolidationStressCoefficientOfVariation = random.NextDouble()
            };

            RoundedDouble preconsolidationStressDesignVariable = random.NextRoundedDouble();

            // Call
            var stressProperties = new MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine(constructionProperties)
            {
                PreconsolidationStressDesignVariable = preconsolidationStressDesignVariable
            };

            // Assert
            DistributionAssert.AreEqual(new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) constructionProperties.PreconsolidationStressMean,
                CoefficientOfVariation = (RoundedDouble) constructionProperties.PreconsolidationStressCoefficientOfVariation
            }, stressProperties.PreconsolidationStress);

            Assert.AreEqual(constructionProperties.XCoordinate, stressProperties.XCoordinate);
            Assert.AreEqual(constructionProperties.ZCoordinate, stressProperties.ZCoordinate);
            Assert.AreEqual(preconsolidationStressDesignVariable, stressProperties.PreconsolidationStressDesignVariable);
        }
    }
}