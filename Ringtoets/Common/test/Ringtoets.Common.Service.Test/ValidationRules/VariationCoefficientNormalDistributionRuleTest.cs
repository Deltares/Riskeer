﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
    public class VariationCoefficientNormalDistributionRuleTest
    {
        private const string paramName = "<a very nice parameter name>";

        [Test]
        public void Validate_ValidDistribution_NoErrorMessage()
        {
            // Setup
            var rule = new VariationCoefficientNormalDistributionRule(new VariationCoefficientNormalDistribution(2),
                                                                      paramName);

            // Call
            IEnumerable<string> message = rule.Validate();

            // Assert
            CollectionAssert.IsEmpty(message);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void Validate_InvalidMean_ErrorMessage(double value)
        {
            // Setup 
            var distribution = new VariationCoefficientNormalDistribution(2)
            {
                Mean = (RoundedDouble) value
            };

            var rule = new VariationCoefficientNormalDistributionRule(distribution, paramName);

            // Call
            IEnumerable<string> messages = rule.Validate();

            string[] validationMessages = messages.ToArray();

            // Assert
            Assert.AreEqual(1, validationMessages.Length);
            string expectedMessage = $"De verwachtingswaarde voor '{paramName}' moet een concreet getal zijn.";
            StringAssert.StartsWith(expectedMessage, validationMessages[0]);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(double.PositiveInfinity)]
        public void ValidateDistribution_InvalidVariationCoefficient_ErrorMessage(double value)
        {
            // Setup 
            var distribution = new VariationCoefficientNormalDistribution(2)
            {
                CoefficientOfVariation = (RoundedDouble) value
            };

            var rule = new VariationCoefficientNormalDistributionRule(distribution, paramName);

            // Call
            IEnumerable<string> messages = rule.Validate();

            string[] validationMessages = messages.ToArray();

            // Assert
            Assert.AreEqual(1, validationMessages.Length);
            string expectedMessage = $"De variatiecoëfficiënt voor '{paramName}' moet groter zijn dan of gelijk zijn aan 0.";
            StringAssert.StartsWith(expectedMessage, validationMessages[0]);
        }
    }
}