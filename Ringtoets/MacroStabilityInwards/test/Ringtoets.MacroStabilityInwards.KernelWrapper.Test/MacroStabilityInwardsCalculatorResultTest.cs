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

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorResultTest
    {
        [Test]
        public void Constructor_ConstructionPropertiesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculatorResult(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("properties", exception.ParamName);
        }

        [Test]
        public void Constructor_EmptyConstructionProperties_ExpectedValues()
        {
            // Call
            var result = new MacroStabilityInwardsCalculatorResult(new MacroStabilityInwardsCalculatorResult.ConstructionProperties());

            // Assert
            Assert.IsNaN(result.FactorOfStability);
        }

        [Test]
        public void Constructor_ConstructionPropertiesWithData_ExcpectedValues()
        {
            // Setup
            var random = new Random(21);
            double factorOfStability = random.NextDouble();

            var constructionProperties = new MacroStabilityInwardsCalculatorResult.ConstructionProperties
            {
                FactorOfStability = factorOfStability
            };

            // Call
            var result = new MacroStabilityInwardsCalculatorResult(constructionProperties);

            // Assert
            Assert.AreEqual(factorOfStability, result.FactorOfStability);
        }
    }
}