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
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsSemiProbabilisticTestFactoryTest
    {
        [Test]
        public void CreateOutput_WithoutProbability_ReturnsSemiProbabilisticOutput()
        {
            // Call
            MacroStabilityInwardsSemiProbabilisticOutput output = MacroStabilityInwardsSemiProbabilisticOutputTestFactory.CreateOutput();

            // Assert
            Assert.IsNotNull(output);

            Assert.AreEqual(typeof(MacroStabilityInwardsSemiProbabilisticOutput), output.GetType());

            Assert.AreEqual(0, output.FactorOfStability, output.FactorOfStability.GetAccuracy());
            Assert.AreEqual(0, output.RequiredProbability);
            Assert.AreEqual(0, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
            Assert.AreEqual(0, output.MacroStabilityInwardsProbability);
            Assert.AreEqual(0, output.MacroStabilityInwardsReliability, output.MacroStabilityInwardsReliability.GetAccuracy());
            Assert.AreEqual(0, output.MacroStabilityInwardsFactorOfSafety, output.MacroStabilityInwardsFactorOfSafety.GetAccuracy());
        }

        [Test]
        public void CreateOutput_WithProbability_ReturnsSemiProbabilistyOutputWithProbability()
        {
            // Setup
            var random = new Random(21);
            double probability = random.NextDouble();

            // Call
            MacroStabilityInwardsSemiProbabilisticOutput output = MacroStabilityInwardsSemiProbabilisticOutputTestFactory.CreateOutput(probability);

            // Assert
            Assert.IsNotNull(output);

            Assert.AreEqual(typeof(MacroStabilityInwardsSemiProbabilisticOutput), output.GetType());

            Assert.AreEqual(0, output.FactorOfStability, output.FactorOfStability.GetAccuracy());
            Assert.AreEqual(0, output.RequiredProbability);
            Assert.AreEqual(0, output.RequiredReliability, output.RequiredReliability.GetAccuracy());
            Assert.AreEqual(probability, output.MacroStabilityInwardsProbability);
            Assert.AreEqual(0, output.MacroStabilityInwardsReliability, output.MacroStabilityInwardsReliability.GetAccuracy());
            Assert.AreEqual(0, output.MacroStabilityInwardsFactorOfSafety, output.MacroStabilityInwardsFactorOfSafety.GetAccuracy());
        }
    }
}