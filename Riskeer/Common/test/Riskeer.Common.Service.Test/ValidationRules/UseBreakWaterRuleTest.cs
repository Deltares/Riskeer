// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Service.ValidationRules;

namespace Riskeer.Common.Service.Test.ValidationRules
{
    [TestFixture]
    public class UseBreakWaterRuleTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        [TestCase(BreakWaterType.Wall)]
        [TestCase(BreakWaterType.Caisson)]
        [TestCase(BreakWaterType.Dam)]
        public void Validate_ValidBreakWaterHeight_NoErrorMessage(BreakWaterType type)
        {
            // Setup
            var breakWater = mockRepository.Stub<IUseBreakWater>();
            breakWater.UseBreakWater = true;
            breakWater.Stub(call => breakWater.BreakWater).Return(new BreakWater(type, 5.0));
            mockRepository.ReplayAll();

            var rule = new UseBreakWaterRule(breakWater);

            // Call 
            IEnumerable<string> message = rule.Validate();

            // Assert
            CollectionAssert.IsEmpty(message);
            mockRepository.VerifyAll();
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
            var breakWater = mockRepository.Stub<IUseBreakWater>();
            breakWater.UseBreakWater = false;
            breakWater.Stub(call => breakWater.BreakWater).Return(new BreakWater(type, height));
            mockRepository.ReplayAll();

            var rule = new UseBreakWaterRule(breakWater);

            // Call 
            IEnumerable<string> message = rule.Validate();

            // Assert
            CollectionAssert.IsEmpty(message);
            mockRepository.VerifyAll();
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
            var breakWater = mockRepository.Stub<IUseBreakWater>();
            breakWater.UseBreakWater = true;
            breakWater.Stub(call => breakWater.BreakWater).Return(new BreakWater(type, height));
            mockRepository.ReplayAll();

            var rule = new UseBreakWaterRule(breakWater);

            // Call 
            IEnumerable<string> messages = rule.Validate();

            string[] validationMessages = messages.ToArray();

            // Assert
            Assert.AreEqual(1, validationMessages.Length);
            const string expectedMessage = "De waarde voor 'hoogte' van de dam moet een concreet getal zijn.";
            StringAssert.StartsWith(expectedMessage, validationMessages[0]);
            mockRepository.VerifyAll();
        }
    }
}