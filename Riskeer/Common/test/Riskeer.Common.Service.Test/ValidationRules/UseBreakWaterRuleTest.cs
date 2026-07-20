// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Service.ValidationRules;

namespace Riskeer.Common.Service.Test.ValidationRules
{
    [TestFixture]
    public class UseBreakWaterRuleTest
    {
        [SetUp]
        public void SetUp() {}

        [Test]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Dam)]
        public void Validate_ValidBreakWaterHeight_NoErrorMessage(BreakWaterType type)
        {
            // Setup
            var breakWater = Substitute.For<IUseBreakWater>();
            breakWater.UseBreakWater = true;
            breakWater.BreakWater.Returns(new BreakWater(type, 5.0));
            var rule = new UseBreakWaterRule(breakWater);

            // Call 
            IEnumerable<string> message = rule.Validate();

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        [Combinatorial]
        public void Validate_DoesNotUseBreakWaterWithInvalidBreakWaterHeight_NoErrorMessage(
            [Values(BreakWaterType.Wall, BreakWaterType.Caisson, BreakWaterType.Dam)]
            BreakWaterType type,
            [Values(double.NaN, double.NegativeInfinity, double.PositiveInfinity)]
            double height)
        {
            // Setup
            var breakWater = Substitute.For<IUseBreakWater>();
            breakWater.UseBreakWater = false;
            breakWater.BreakWater.Returns(new BreakWater(type, height));
            var rule = new UseBreakWaterRule(breakWater);

            // Call 
            IEnumerable<string> message = rule.Validate();

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        [Combinatorial]
        public void Validate_UseBreakWaterWithInvalidBreakWaterHeight_ErrorMessage(
            [Values(BreakWaterType.Wall, BreakWaterType.Caisson, BreakWaterType.Dam)]
            BreakWaterType type,
            [Values(double.NaN, double.NegativeInfinity, double.PositiveInfinity)]
            double height)
        {
            // Setup
            var breakWater = Substitute.For<IUseBreakWater>();
            breakWater.UseBreakWater = true;
            breakWater.BreakWater.Returns(new BreakWater(type, height));
            var rule = new UseBreakWaterRule(breakWater);

            // Call 
            IEnumerable<string> messages = rule.Validate();

            string[] validationMessages = messages.ToArray();

            // Assert
            Assert.AreEqual(1, validationMessages.Length);
            const string expectedMessage = "De waarde voor 'hoogte' van de dam moet een concreet getal zijn.";
            StringAssert.StartsWith(expectedMessage, validationMessages[0]);
        }
    }
}