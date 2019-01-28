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
using Ringtoets.Common.Service.ValidationRules;

namespace Ringtoets.Common.Service.Test.ValidationRules
{
    [TestFixture]
    public class ValidationRuleTest
    {
        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void IsNotConcreteNumber_ValidatesInvalidNumber_ReturnTrue(double value)
        {
            // Call
            bool isNumberNotConcrete = TestRule.PublicIsNumberValid(value);

            // Assert
            Assert.IsTrue(isNumberNotConcrete);
        }
    }

    public class TestRule : ValidationRule
    {
        public static bool PublicIsNumberValid(double value)
        {
            return IsNotConcreteNumber(value);
        }

        public override IEnumerable<string> Validate()
        {
            return Enumerable.Empty<string>();
        }
    }
}