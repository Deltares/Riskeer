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
using Application.Ringtoets.Storage.Create.MacroStabilityInwards;
using Application.Ringtoets.Storage.DbContext;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Data;

namespace Application.Ringtoets.Storage.Test.Create.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsSemiProbabilisticOutputCreateExtensionsTest
    {
        [Test]
        public void Create_SemiProbabilisticOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((MacroStabilityInwardsSemiProbabilisticOutput) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("semiProbabilisticOutput", exception.ParamName);
        }

        [Test]
        public void Create_WithValues_ReturnEntityWithExpectedPropertyValues()
        {
            // Setup
            var random = new Random(21);
            var output = new MacroStabilityInwardsSemiProbabilisticOutput(random.NextDouble(),
                                                                          random.NextDouble(),
                                                                          random.NextDouble(),
                                                                          random.NextDouble(),
                                                                          random.NextDouble(),
                                                                          random.NextDouble());

            // Call
            MacroStabilityInwardsSemiProbabilisticOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);

            AssertAreEqual(output.FactorOfStability, entity.FactorOfStability);
            Assert.AreEqual(output.MacroStabilityInwardsProbability, entity.MacroStabilityInwardsProbability);
            AssertAreEqual(output.MacroStabilityInwardsReliability, entity.MacroStabilityInwardsReliability);
            Assert.AreEqual(output.RequiredProbability, entity.RequiredProbability);
            AssertAreEqual(output.RequiredReliability, entity.RequiredReliability);
            AssertAreEqual(output.MacroStabilityInwardsFactorOfSafety, entity.MacroStabilityInwardsFactorOfSafety);
        }

        [Test]
        public void Create_WithNaNValues_ReturnEntityWithNullPropertyValues()
        {
            // Setup
            var output = new MacroStabilityInwardsSemiProbabilisticOutput(double.NaN,
                                                                          double.NaN,
                                                                          double.NaN,
                                                                          double.NaN,
                                                                          double.NaN,
                                                                          double.NaN);

            // Call
            MacroStabilityInwardsSemiProbabilisticOutputEntity entity = output.Create();

            // Assert
            Assert.IsNotNull(entity);

            Assert.IsNull(entity.FactorOfStability);
            Assert.IsNull(entity.MacroStabilityInwardsProbability);
            Assert.IsNull(entity.MacroStabilityInwardsReliability);
            Assert.IsNull(entity.RequiredProbability);
            Assert.IsNull(entity.RequiredReliability);
            Assert.IsNull(entity.MacroStabilityInwardsFactorOfSafety);
        }

        private static void AssertAreEqual(RoundedDouble expectedValue, double? actualValue)
        {
            Assert.AreEqual(expectedValue, actualValue, expectedValue.GetAccuracy());
        }
    }
}