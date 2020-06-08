// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Piping.Service;

namespace Riskeer.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class PipingCalculationScenarioTestFactoryTest
    {
        [Test]
        public void CreatePipingCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationScenarioTestFactory.CreatePipingCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreatePipingCalculationScenario_WithSection_CreatesRelevantCalculationWithOutput()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            PipingCalculationScenario scenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenario(section);

            // Assert
            Assert.IsNotNull(scenario.Output);
            Assert.IsTrue(scenario.IsRelevant);
            Assert.IsNotNull(scenario.InputParameters.SurfaceLine);
            Assert.AreSame(section.StartPoint, scenario.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint);
        }

        [Test]
        public void CreateIrrelevantPipingCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationScenarioTestFactory.CreateIrrelevantPipingCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateIrrelevantPipingCalculationScenario_WithSection_CreatesIrrelevantCalculation()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            PipingCalculationScenario scenario = PipingCalculationScenarioTestFactory.CreateIrrelevantPipingCalculationScenario(section);

            // Assert
            Assert.IsFalse(scenario.IsRelevant);
        }

        [Test]
        public void CreateNotCalculatedPipingCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateNotCalculatedPipingCalculationScenario_WithSection_CreatesRelevantCalculationWithoutOutput()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            PipingCalculationScenario scenario = PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(section);

            // Assert
            Assert.IsNull(scenario.Output);
            Assert.IsTrue(scenario.IsRelevant);
            Assert.IsNotNull(scenario.InputParameters.SurfaceLine);
            Assert.AreSame(section.StartPoint, scenario.InputParameters.SurfaceLine.ReferenceLineIntersectionWorldPoint);
        }

        [Test]
        public void CreatePipingCalculationScenarioWithInvalidInput_CreatesPipingCalculationScenarioWithInvalidInput()
        {
            // Call
            PipingCalculationScenario scenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithInvalidInput();

            // Assert
            Assert.IsFalse(PipingCalculationService.Validate(scenario, (RoundedDouble) 1.1));
        }

        [Test]
        public void CreatePipingCalculationScenarioWithValidInput_HydraulicBoundaryLocationNull_ThrowsArgumentnullException()
        {
            // Call
            void Call() => PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void CreatePipingCalculationScenarioWithValidInput_HydraulicBoundaryLocation_CreatesPipingCalculationScenarioWithValidInput()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            PipingCalculationScenario scenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, scenario.InputParameters.HydraulicBoundaryLocation);
            Assert.IsTrue(PipingCalculationService.Validate(scenario, (RoundedDouble) 1.1));
        }
    }
}