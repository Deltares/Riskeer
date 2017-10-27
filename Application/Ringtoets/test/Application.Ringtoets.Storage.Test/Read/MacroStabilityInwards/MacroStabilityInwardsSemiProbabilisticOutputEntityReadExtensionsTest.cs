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
using Application.Ringtoets.Storage.DbContext;
using Application.Ringtoets.Storage.Read.MacroStabilityInwards;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;

namespace Application.Ringtoets.Storage.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSemiProbabilisticOutputEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsSemiProbabilisticOutputEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_EntityWithValues_ReturnsSemiProbabilisticOutput()
        {
            // Setup
            var random = new Random(21);
            var entity = new MacroStabilityInwardsSemiProbabilisticOutputEntity
            {
                FactorOfStability = random.NextDouble(),
                RequiredProbability = random.NextDouble(),
                RequiredReliability = random.NextDouble(),
                MacroStabilityInwardsProbability = random.NextDouble(),
                MacroStabilityInwardsReliability = random.NextDouble(),
                MacroStabilityInwardsFactorOfSafety = random.NextDouble()
            };

            MacroStabilityInwardsSemiProbabilisticOutput output = entity.Read();

            // Assert
            Assert.IsNotNull(output);

            AssertAreEqual(entity.FactorOfStability, output.FactorOfStability);
            AssertAreEqual(entity.RequiredProbability, output.RequiredProbability);
            AssertAreEqual(entity.RequiredReliability, output.RequiredReliability);
            AssertAreEqual(entity.MacroStabilityInwardsProbability, output.MacroStabilityInwardsProbability);
            AssertAreEqual(entity.MacroStabilityInwardsReliability, output.MacroStabilityInwardsReliability);
            AssertAreEqual(entity.MacroStabilityInwardsFactorOfSafety, output.MacroStabilityInwardsFactorOfSafety);
        }

        [Test]
        public void Read_EntityPropertiesNull_ReturnsSemiProbabilisticOutput()
        {
            // Setup
            var entity = new MacroStabilityInwardsSemiProbabilisticOutputEntity();

            // Call
            MacroStabilityInwardsSemiProbabilisticOutput output = entity.Read();

            // Assert
            Assert.IsNotNull(output);

            Assert.IsNaN(output.FactorOfStability);
            Assert.IsNaN(output.RequiredProbability);
            Assert.IsNaN(output.RequiredReliability);
            Assert.IsNaN(output.MacroStabilityInwardsProbability);
            Assert.IsNaN(output.MacroStabilityInwardsReliability);
            Assert.IsNaN(output.MacroStabilityInwardsFactorOfSafety);
        }

        private static void AssertAreEqual(double? expectedParameterValue, double actualParameterValue)
        {
            Assert.AreEqual(expectedParameterValue, actualParameterValue);
        }

        private static void AssertAreEqual(double? expectedParameterValue, RoundedDouble actualParameterValue)
        {
            Assert.IsTrue(expectedParameterValue.HasValue);
            Assert.AreEqual(expectedParameterValue.Value, actualParameterValue, actualParameterValue.GetAccuracy());
        }
    }
}