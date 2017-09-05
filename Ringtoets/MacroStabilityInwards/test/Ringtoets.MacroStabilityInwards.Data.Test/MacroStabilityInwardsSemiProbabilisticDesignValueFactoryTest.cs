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

using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSemiProbabilisticDesignValueFactoryTest
    {
        [Test]
        public void GetAbovePhreaticLevel_ValidSoilLayerProperties_CreateDesignValueForAbovePhreaticLevel()
        {
            // Setup
            var properties = new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(
                new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    AbovePhreaticLevelMean = 1,
                    AbovePhreaticLevelCoefficientOfVariation = 0.4
                });

            // Call
            VariationCoefficientLogNormalDistributionDesignVariable abovePhreaticLevel = MacroStabilityInwardsSemiProbabilisticDesignValueFactory.GetAbovePhreaticLevel(properties);

            // Assert
            DistributionAssert.AreEqual(properties.AbovePhreaticLevel, abovePhreaticLevel.Distribution);
            AssertPercentile(0.5, abovePhreaticLevel);
        }

        private static void AssertPercentile(double percentile, VariationCoefficientDesignVariable<VariationCoefficientLogNormalDistribution> designVariable)
        {
            Assert.IsInstanceOf<VariationCoefficientLogNormalDistributionDesignVariable>(designVariable);
            var percentileBasedDesignVariable = (VariationCoefficientLogNormalDistributionDesignVariable)designVariable;
            Assert.AreEqual(percentile, percentileBasedDesignVariable.Percentile);
        }
    }
}