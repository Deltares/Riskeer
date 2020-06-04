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
using Riskeer.MacroStabilityInwards.Service;

namespace Riskeer.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioTestFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenario_WithSection_CreatesCalculationWithOutputSet()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.IsTrue(scenario.IsRelevant);

            MacroStabilityInwardsOutput output = scenario.Output;
            Assert.IsNotNull(output);
            Assert.IsTrue(IsValidDouble(output.FactorOfStability));
            Assert.IsTrue(IsValidDouble(output.ZValue));
            Assert.IsTrue(IsValidDouble(output.ForbiddenZonesXEntryMax));
            Assert.IsTrue(IsValidDouble(output.ForbiddenZonesXEntryMin));
            Assert.IsNotNull(output.SlidingCurve);
            Assert.IsNotNull(output.SlipPlane);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithFactorOfStability_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(double.NaN, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        [TestCase(double.NaN, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(NaN)")]
        [TestCase(0.0, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(0.0)")]
        [TestCase(0.8, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(0.8)")]
        [TestCase(1.0, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(1.0)")]
        public void CreateMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(double factoryOfStability)
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenario(factoryOfStability, section);

            // Assert
            Assert.IsNotNull(scenario.Output);
            Assert.AreEqual(factoryOfStability, scenario.Output.FactorOfStability, 1e-6);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput_WithSection_CreatesRelevantCalculationWithOutputSetToNaN()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithNaNOutput(section);

            // Assert
            Assert.IsNotNull(scenario.Output);
            Assert.IsNaN(scenario.Output.FactorOfStability);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateIrrelevantMacroStabilityInwardsCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateIrrelevantMacroStabilityInwardsCalculationScenario_WithSection_CreatesIrrelevantCalculation()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.IsFalse(scenario.IsRelevant);
            Assert.IsNotNull(scenario.Output);
            Assert.AreEqual(0.2, scenario.Output.FactorOfStability);
        }

        [Test]
        public void CreateNotCalculatedMacroStabilityInwardsCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateNotCalculatedMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithoutOutput()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.IsNull(scenario.Output);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput_ReturnMacroStabilityInwardsCalculationScenario()
        {
            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();

            // Assert
            Assert.IsFalse(MacroStabilityInwardsCalculationService.Validate(scenario, (RoundedDouble) 1.1));
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithValidInput_HydraulicBoundaryLocationNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryLocation", exception.ParamName);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithValidInput_HydraulicBoundaryLocation_CreatesPipingCalculationScenarioWithValidInput()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);

            // Assert
            Assert.AreSame(hydraulicBoundaryLocation, scenario.InputParameters.HydraulicBoundaryLocation);
            Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(scenario, (RoundedDouble) 1.1));
        }

        private static bool IsValidDouble(double value)
        {
            return !double.IsNaN(value) && !double.IsInfinity(value);
        }
    }
}