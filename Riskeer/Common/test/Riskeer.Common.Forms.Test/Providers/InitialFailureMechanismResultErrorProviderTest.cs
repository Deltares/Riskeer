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

using System;
using System.Linq;
using Core.Common.Base.Data;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.Providers;

namespace Riskeer.Common.Forms.Test.Providers
{
    [TestFixture]
    public class InitialFailureMechanismResultErrorProviderTest
    {
        [Test]
        public void Constructor_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                null, Enumerable.Empty<ICalculationScenario>(), (scenario, segments) => false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void Constructor_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            void Call() => new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, null, (scenario, segments) => false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void Constructor_IntersectionFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            void Call() => new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, Enumerable.Empty<ICalculationScenario>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("intersectionFunc", exception.ParamName);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            var errorProvider = new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, Enumerable.Empty<ICalculationScenario>(), (scenario, segments) => false);

            // Assert
            Assert.IsInstanceOf<IInitialFailureMechanismResultErrorProvider>(errorProvider);
        }

        [Test]
        public void GetProbabilityValidationError_GetProbabilityFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            var errorProvider = new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, Enumerable.Empty<ICalculationScenario>(), (scenario, segments) => false);

            // Call
            void Call() => errorProvider.GetProbabilityValidationError(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getProbabilityFunc", exception.ParamName);
        }

        [Test]
        public void GetProbabilityValidationError_NoRelevantScenarios_ReturnsErrorMessage()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            var calculationScenarios = new[]
            {
                new TestCalculationScenario(),
                new TestCalculationScenario()
            };

            var errorProvider = new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, calculationScenarios, (scenario, segments) => false);

            // Call
            string errorMessage = errorProvider.GetProbabilityValidationError(() => double.NaN);

            // Assert
            Assert.AreEqual("Er moet minimaal één maatgevende berekening voor dit vak worden gedefinieerd.", errorMessage);
        }

        [Test]
        [TestCase(0.043)]
        [TestCase(0.689)]
        public void GetProbabilityValidationError_TotalContributionNotOne_ReturnsErrorMessage(double contribution)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            var calculationScenarios = new[]
            {
                new TestCalculationScenario
                {
                    Contribution = (RoundedDouble) 0.5
                },
                new TestCalculationScenario
                {
                    Contribution = (RoundedDouble) contribution
                }
            };

            var errorProvider = new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, calculationScenarios, (scenario, segments) => true);

            // Call
            string errorMessage = errorProvider.GetProbabilityValidationError(() => double.NaN);

            // Assert
            Assert.AreEqual("De bijdragen van de maatgevende scenario's voor dit vak moeten opgeteld gelijk zijn aan 100%.", errorMessage);
        }

        [Test]
        public void GetProbabilityValidationError_CalculationScenarioWithoutOutput_ReturnsErrorMessage()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            var calculationScenarios = new[]
            {
                new TestCalculationScenario()
            };

            var errorProvider = new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, calculationScenarios, (scenario, segments) => true);

            // Call
            string errorMessage = errorProvider.GetProbabilityValidationError(() => double.NaN);

            // Assert
            Assert.AreEqual("Alle maatgevende berekeningen voor dit vak moeten uitgevoerd zijn.", errorMessage);
        }

        [Test]
        public void GetProbabilityValidationError_CalculationScenarioWithInvalidOutput_ReturnsErrorMessage()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            var calculationScenarios = new[]
            {
                new TestCalculationScenario
                {
                    Output = new object()
                }
            };

            var errorProvider = new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, calculationScenarios, (scenario, segments) => true);

            // Call
            string errorMessage = errorProvider.GetProbabilityValidationError(() => double.NaN);

            // Assert
            Assert.AreEqual("Alle maatgevende berekeningen voor dit vak moeten een geldige uitkomst hebben.", errorMessage);
        }

        [Test]
        public void GetProbabilityValidationError_ValidCalculationScenarios_ReturnsEmptyMessage()
        {
            // Setup
            var random = new Random(39);
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            var calculationScenarios = new[]
            {
                new TestCalculationScenario
                {
                    Contribution = (RoundedDouble) 0.5,
                    Output = new object()
                },
                new TestCalculationScenario
                {
                    Contribution = (RoundedDouble) 0.5,
                    Output = new object()
                }
            };

            var errorProvider = new InitialFailureMechanismResultErrorProvider<ICalculationScenario>(
                sectionResult, calculationScenarios, (scenario, segments) => true);

            // Call
            string errorMessage = errorProvider.GetProbabilityValidationError(() => random.NextDouble());

            // Assert
            Assert.AreEqual(string.Empty, errorMessage);
        }
    }
}