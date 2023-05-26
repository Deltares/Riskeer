﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Probabilistics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read.MacroStabilityInwards;

namespace Riskeer.Storage.Core.Test.Read.MacroStabilityInwards
{
    [TestFixture]
    public class MacroStabilityInwardsPreconsolidationStressEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => ((MacroStabilityInwardsPreconsolidationStressEntity) null).Read();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_WithValues_ReturnsExpectedMacroStabilityInwardsPreconsolidationStress()
        {
            // Setup
            var random = new Random(31);
            var entity = new MacroStabilityInwardsPreconsolidationStressEntity
            {
                CoordinateX = random.NextDouble(),
                CoordinateZ = random.NextDouble(),
                PreconsolidationStressMean = random.NextDouble(),
                PreconsolidationStressCoefficientOfVariation = random.NextDouble()
            };

            // Call
            MacroStabilityInwardsPreconsolidationStress stress = entity.Read();

            // Assert
            Assert.IsNotNull(stress);
            Assert.AreEqual(entity.CoordinateX, stress.Location.X);
            Assert.AreEqual(entity.CoordinateZ, stress.Location.Y);

            VariationCoefficientLogNormalDistribution preconsolidationStressDistribution = stress.Stress;
            AssertAreEqual(entity.PreconsolidationStressMean, preconsolidationStressDistribution.Mean);
            AssertAreEqual(entity.PreconsolidationStressCoefficientOfVariation, preconsolidationStressDistribution.CoefficientOfVariation);
        }

        [Test]
        public void Read_WithNullValues_ReturnsExpectedMacroStabilityInwardsPreconsolidationStress()
        {
            // Setup
            var random = new Random(31);
            var entity = new MacroStabilityInwardsPreconsolidationStressEntity
            {
                CoordinateX = random.NextDouble(),
                CoordinateZ = random.NextDouble()
            };

            // Call
            MacroStabilityInwardsPreconsolidationStress stress = entity.Read();

            // Assert
            Assert.IsNotNull(stress);

            VariationCoefficientLogNormalDistribution preconsolidationStressDistribution = stress.Stress;
            Assert.IsNaN(preconsolidationStressDistribution.Mean);
            Assert.IsNaN(preconsolidationStressDistribution.CoefficientOfVariation);
        }

        private static void AssertAreEqual(double? expectedValue, RoundedDouble actualValue)
        {
            Assert.IsTrue(expectedValue.HasValue);
            Assert.AreEqual(expectedValue.Value, actualValue, actualValue.GetAccuracy());
        }
    }
}