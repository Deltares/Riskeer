// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using NUnit.Framework;
using Riskeer.Common.Forms.Providers;

namespace Riskeer.Common.Forms.Test.Providers
{
    [TestFixture]
    public class FailureMechanismSectionResultRowErrorProviderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var errorProvider = new FailureMechanismSectionResultRowErrorProvider();

            // Assert
            Assert.IsInstanceOf<IFailureMechanismSectionResultRowErrorProvider>(errorProvider);
        }

        [Test]
        public void GetManualProbabilityValidationError_ProbabilityNaN_ReturnsErrorMessage()
        {
            // Setup
            var errorProvider = new FailureMechanismSectionResultRowErrorProvider();

            // Call
            string errorMessage = errorProvider.GetManualProbabilityValidationError(double.NaN);

            // Assert
            Assert.AreEqual("Er moet een waarde worden ingevuld voor de faalkans.", errorMessage);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(-1)]
        [TestCase(double.NegativeInfinity)]
        [TestCase(double.PositiveInfinity)]
        public void GetManualProbabilityValidationError_ProbabilityNotNaN_ReturnsNoErrorMessage(double probability)
        {
            // Setup
            var errorProvider = new FailureMechanismSectionResultRowErrorProvider();

            // Call
            string errorMessage = errorProvider.GetManualProbabilityValidationError(probability);

            // Assert
            Assert.IsEmpty(errorMessage);
        }
    }
}