// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Service.ValidationRules;

namespace Ringtoets.Common.Service.Test.ValidationRules
{
    [TestFixture]
    public class VariationCoefficientLogNormalDistributionRuleTest
    {
        private const string paramName = "<a very nice parameter name>";

        [Test]
        public void Validate_ValidDistribution_NoErrorMessage()
        {
            // Setup
            ValidationRule rule = new VariationCoefficientLogNormalDistributionRule(new VariationCoefficientLogNormalDistribution(2),
                                                                                    paramName);

            // Call
            IEnumerable<string> message = rule.Validate();

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidMean_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De verwachtingswaarde voor '{0}' moet een positief getal zijn.", paramName);
            var distribution = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) value
            };

            ValidationRule rule = new VariationCoefficientLogNormalDistributionRule(distribution, paramName);

            // Call
            IEnumerable<string> messages = rule.Validate();

            string[] validationMessages = messages.ToArray();

            // Assert
            Assert.AreEqual(1, validationMessages.Length);
            StringAssert.StartsWith(expectedMessage, validationMessages[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidVariationCoefficient_ErrorMessage(double value)
        {
            // Setup 
            var expectedMessage = string.Format("De variatiecoëfficient voor '{0}' moet groter zijn dan of gelijk zijn aan 0.", paramName);
            var distribution = new VariationCoefficientLogNormalDistribution(2)
            {
                CoefficientOfVariation = (RoundedDouble) value
            };

            ValidationRule rule = new VariationCoefficientLogNormalDistributionRule(distribution, paramName);

            // Call
            IEnumerable<string> messages = rule.Validate();

            string[] validationMessages = messages.ToArray();

            // Assert
            Assert.AreEqual(1, validationMessages.Length);
            StringAssert.StartsWith(expectedMessage, validationMessages[0]);
        }
    }
}