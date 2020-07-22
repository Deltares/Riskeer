// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil.SoilProfile;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test.SoilProfile
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsPreconsolidationStress_ReturnsExpectedValues()
        {
            // Call
            MacroStabilityInwardsPreconsolidationStress stress =
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress();

            // Assert
            Assert.AreEqual(new Point2D(13, 34), stress.Location);

            VariationCoefficientLogNormalDistribution stressDistribution = stress.Stress;
            Assert.AreEqual(10.09, stressDistribution.Mean, stressDistribution.GetAccuracy());
            Assert.AreEqual(20.05, stressDistribution.CoefficientOfVariation, stressDistribution.GetAccuracy());
        }

        [Test]
        public void CreateMacroStabilityInwardsPreconsolidationStress_WithLocation_ReturnsExpectedValues()
        {
            // Setup
            var location = new Point2D(0, 0);

            // Call
            MacroStabilityInwardsPreconsolidationStress stress =
                MacroStabilityInwardsPreconsolidationStressTestFactory.CreateMacroStabilityInwardsPreconsolidationStress(location);

            // Assert
            Assert.AreSame(location, stress.Location);

            VariationCoefficientLogNormalDistribution stressDistribution = stress.Stress;
            Assert.AreEqual(10.09, stressDistribution.Mean, stressDistribution.GetAccuracy());
            Assert.AreEqual(20.05, stressDistribution.CoefficientOfVariation, stressDistribution.GetAccuracy());
        }
    }
}