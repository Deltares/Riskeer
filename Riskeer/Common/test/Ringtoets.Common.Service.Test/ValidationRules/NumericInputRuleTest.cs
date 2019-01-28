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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Service.ValidationRules;

namespace Ringtoets.Common.Service.Test.ValidationRules
{
    [TestFixture]
    public class NumericInputRuleTest
    {
        private const string paramName = "<a very nice parameter name>";

        [Test]
        public void Validate_ValidOrientation_NoErrorMessage()
        {
            // Setup
            var rule = new NumericInputRule(new RoundedDouble(2), paramName);

            // Call
            IEnumerable<string> message = rule.Validate();

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        public void Validate_InvalidOrientation_ErrorMessage()
        {
            // Setup 
            var orientation = new RoundedDouble(2, double.NaN);

            var rule = new NumericInputRule(orientation, paramName);

            // Call
            IEnumerable<string> messages = rule.Validate();

            string[] validationMessages = messages.ToArray();

            // Assert
            Assert.AreEqual(1, validationMessages.Length);
            string expectedMessage = $"De waarde voor '{paramName}' moet een concreet getal zijn.";
            StringAssert.StartsWith(expectedMessage, validationMessages[0]);
        }
    }
}