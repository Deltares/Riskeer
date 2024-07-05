// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Helpers;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test.Helpers
{
    [TestFixture]
    public class CalculationScenarioHelperTest
    {
        [Test]
        public void ContributionNumberOfDecimalPlaces_Always_ReturnsExpectedValue()
        {
            // Call
            int numberOfDecimalPlaces = CalculationScenarioHelper.ContributionNumberOfDecimalPlaces;

            // Assert
            Assert.AreEqual(4, numberOfDecimalPlaces);
        }

        [Test]
        [TestCaseSource(typeof(CalculationScenarioTestHelper), nameof(CalculationScenarioTestHelper.GetInvalidScenarioContributionValues))]
        public void ValidateScenarioContribution_InvalidValue_ThrowsArgumentException(double newValue)
        {
            // Call
            void Call() => CalculationScenarioHelper.ValidateScenarioContribution((RoundedDouble) newValue);

            // Assert
            const string expectedMessage = "De waarde voor de bijdrage moet binnen het bereik [0, 100] liggen.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(Call, expectedMessage);
        }

        [Test]
        [TestCase(-0.0)]
        [TestCase(0.0)]
        [TestCase(1.0)]
        public void ValidateScenarioContribution_ValidValue_DoesNotThrow(double newValue)
        {
            // Call
            void Call() => CalculationScenarioHelper.ValidateScenarioContribution((RoundedDouble) newValue);

            // Assert
            Assert.DoesNotThrow(Call);
        }

        [Test]
        public void GetTotalContribution_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => CalculationScenarioHelper.GetTotalContribution<ICalculationScenario>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetTotalContribution_WithCalculationScenarios_ReturnsTotalContribution()
        {
            // Setup
            var calculationScenario1 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) 0.4323
            };
            var calculationScenario2 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) 0.1226
            };

            // Call
            RoundedDouble totalContribution = CalculationScenarioHelper.GetTotalContribution(new[]
            {
                calculationScenario1,
                calculationScenario2
            });

            // Assert
            RoundedDouble expectedTotalContribution = calculationScenario1.Contribution + calculationScenario2.Contribution;
            Assert.AreEqual(expectedTotalContribution, totalContribution);
        }

        [Test]
        public void ScenariosAreValid_RelevantScenariosNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => CalculationScenarioHelper.ScenariosAreValid<ICalculationScenario>(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("relevantScenarios", exception.ParamName);
        }

        [Test]
        public void ScenariosAreValid_ScenariosEmpty_ReturnsFalse()
        {
            // Call
            bool valid = CalculationScenarioHelper.ScenariosAreValid(Array.Empty<ICalculationScenario>());

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        public void ScenariosAreValid_ScenariosWithoutOutput_ReturnsFalse()
        {
            // Setup
            var calculationScenario1 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) 0.4323,
                Output = new object()
            };
            var calculationScenario2 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) 0.1226
            };

            // Call
            bool valid = CalculationScenarioHelper.ScenariosAreValid(new[]
            {
                calculationScenario1,
                calculationScenario2
            });

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        [TestCase(0.4323, 0.1226)]
        [TestCase(0.9000, 0.9000)]
        [TestCase(1, 0.0001)]
        [TestCase(0.0001, 0.9998)]
        public void ScenariosAreValid_ScenariosContributionNotOne_ReturnsFalse(double scenarioContribution1,
                                                                               double scenarioContribution2)
        {
            // Setup
            var calculationScenario1 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) scenarioContribution1,
                Output = new object()
            };
            var calculationScenario2 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) scenarioContribution2,
                Output = new object()
            };

            // Call
            bool valid = CalculationScenarioHelper.ScenariosAreValid(new[]
            {
                calculationScenario1,
                calculationScenario2
            });

            // Assert
            Assert.IsFalse(valid);
        }

        [Test]
        [TestCase(1, 0)]
        [TestCase(0.0001, 0.9999)]
        public void ScenariosAreValid_ScenariosWithOutputAndContributionOne_ReturnsTrue(double scenarioContribution1,
                                                                                        double scenarioContribution2)
        {
            // Setup
            var calculationScenario1 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) scenarioContribution1,
                Output = new object()
            };
            var calculationScenario2 = new TestCalculationScenario
            {
                Contribution = (RoundedDouble) scenarioContribution2,
                Output = new object()
            };

            // Call
            bool valid = CalculationScenarioHelper.ScenariosAreValid(new[]
            {
                calculationScenario1,
                calculationScenario2
            });

            // Assert
            Assert.IsTrue(valid);
        }
    }
}