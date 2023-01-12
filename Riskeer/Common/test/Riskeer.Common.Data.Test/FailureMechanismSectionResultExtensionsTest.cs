// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Data.Test
{
    [TestFixture]
    public class FailureMechanismSectionResultExtensionsTest
    {
        [Test]
        public void GetRelevantCalculationScenarios_SectionResultNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => FailureMechanismSectionResultExtensions.GetRelevantCalculationScenarios<ICalculationScenario>(
                null, Enumerable.Empty<ICalculationScenario>(), (scenario, segments) => false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("sectionResult", exception.ParamName);
        }

        [Test]
        public void GetRelevantCalculationScenarios_CalculationScenariosNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            void Call() => sectionResult.GetRelevantCalculationScenarios<ICalculationScenario>(null, (scenario, segments) => false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationScenarios", exception.ParamName);
        }

        [Test]
        public void GetRelevantCalculationScenarios_IntersectionFuncNull_ThrowsArgumentNullException()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            void Call() => sectionResult.GetRelevantCalculationScenarios<ICalculationScenario>(Enumerable.Empty<ICalculationScenario>(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("intersectionFunc", exception.ParamName);
        }

        [Test]
        public void GetRelevantCalculationScenarios_WithRelevantAndIrrelevantScenariosOfDifferentTypes_ReturnsRelevantCalculationScenarios()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioStub1 = mocks.Stub<ICalculationScenario>();
            var calculationScenarioStub2 = mocks.Stub<ICalculationScenario>();
            mocks.ReplayAll();

            calculationScenarioStub1.IsRelevant = true;
            calculationScenarioStub2.IsRelevant = false;

            var calculationScenario1 = new TestCalculationScenario();
            var calculationScenario2 = new TestCalculationScenario
            {
                IsRelevant = false
            };

            ICalculationScenario[] calculationScenarios =
            {
                calculationScenarioStub1,
                calculationScenarioStub2,
                calculationScenario1,
                calculationScenario2
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            IEnumerable<ICalculationScenario> relevantScenarios = sectionResult.GetRelevantCalculationScenarios<TestCalculationScenario>(
                calculationScenarios, (scenario, segments) => true);

            // Assert
            Assert.AreEqual(calculationScenario1, relevantScenarios.Single());
            mocks.VerifyAll();
        }

        [Test]
        public void GetRelevantCalculationScenarios_WithoutScenarioIntersectingSection_ReturnsNoCalculationScenarios()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationScenarioStub1 = mocks.Stub<ICalculationScenario>();
            var calculationScenarioStub2 = mocks.Stub<ICalculationScenario>();
            mocks.ReplayAll();

            calculationScenarioStub1.IsRelevant = true;
            calculationScenarioStub2.IsRelevant = false;

            var calculationScenario1 = new TestCalculationScenario();
            var calculationScenario2 = new TestCalculationScenario
            {
                IsRelevant = false
            };

            ICalculationScenario[] calculationScenarios =
            {
                calculationScenarioStub1,
                calculationScenarioStub2,
                calculationScenario1,
                calculationScenario2
            };

            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();
            var sectionResult = new TestFailureMechanismSectionResult(section);

            // Call
            IEnumerable<ICalculationScenario> relevantScenarios = sectionResult.GetRelevantCalculationScenarios<TestCalculationScenario>(
                calculationScenarios, (scenario, segments) => false);

            // Assert
            CollectionAssert.IsEmpty(relevantScenarios);
            mocks.VerifyAll();
        }
    }
}