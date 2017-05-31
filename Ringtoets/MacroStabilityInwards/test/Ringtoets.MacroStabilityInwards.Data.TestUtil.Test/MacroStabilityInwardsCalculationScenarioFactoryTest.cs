// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.MacroStabilityInwards.Service;

namespace Ringtoets.MacroStabilityInwards.Data.TestUtil.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationScenarioFactoryTest
    {
        [Test]
        public void CreateMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(double.NaN, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        [TestCase(double.NaN, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(NaN)")]
        [TestCase(0.0, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(0.0)")]
        [TestCase(0.8, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(0.8)")]
        [TestCase(1.0, TestName = "CreateCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(1.0)")]
        public void CreateMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSet(double probability)
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenario(probability, section);

            // Assert
            Assert.NotNull(scenario.Output);
            Assert.NotNull(scenario.SemiProbabilisticOutput);
            Assert.AreEqual(probability, scenario.SemiProbabilisticOutput.MacroStabilityInwardsProbability, 1e-6);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateFailedMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateFailedMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithOutputSetToNaN()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateFailedMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.NotNull(scenario.Output);
            Assert.NotNull(scenario.SemiProbabilisticOutput);
            Assert.IsNaN(scenario.SemiProbabilisticOutput.MacroStabilityInwardsProbability);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateIrrelevantMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateIrrelevantMacroStabilityInwardsCalculationScenario_WithSection_CreatesIrrelevantCalculation()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateIrrelevantMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.IsFalse(scenario.IsRelevant);
        }

        [Test]
        public void CreateNotCalculatedMacroStabilityInwardsCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => MacroStabilityInwardsCalculationScenarioFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
        }

        [Test]
        public void CreateNotCalculatedMacroStabilityInwardsCalculationScenario_WithSection_CreatesRelevantCalculationWithoutOutput()
        {
            // Setup
            FailureMechanismSection section = CreateSection();

            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateNotCalculatedMacroStabilityInwardsCalculationScenario(section);

            // Assert
            Assert.IsNull(scenario.Output);
            Assert.IsNull(scenario.SemiProbabilisticOutput);
            Assert.IsTrue(scenario.IsRelevant);
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput_CreatesMacroStabilityInwardsCalculationScenarioWithInvalidInput()
        {
            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithInvalidInput();

            // Assert
            Assert.IsFalse(MacroStabilityInwardsCalculationService.Validate(scenario));
        }

        [Test]
        public void CreateMacroStabilityInwardsCalculationScenarioWithValidInput_CreatesMacroStabilityInwardsCalculationScenarioWithValidInput()
        {
            // Call
            MacroStabilityInwardsCalculationScenario scenario = MacroStabilityInwardsCalculationScenarioFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput();

            // Assert
            Assert.IsTrue(MacroStabilityInwardsCalculationService.Validate(scenario));
        }

        private static FailureMechanismSection CreateSection()
        {
            return new FailureMechanismSection("name", new[]
            {
                new Point2D(0, 0)
            });
        }
    }
}