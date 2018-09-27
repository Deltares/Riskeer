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

using System;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Service;

namespace Ringtoets.Piping.Data.TestUtil.Test
{
    [TestFixture]
    public class PipingCalculationScenarioTestFactoryTest
    {
        [Test]
        public void CreateIrrelevantPipingCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationScenarioTestFactory.CreateIrrelevantPipingCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
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
        public void CreateNotCalculatedPipingCalculationScenario_WithNoSection_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingCalculationScenarioTestFactory.CreateNotCalculatedPipingCalculationScenario(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("section", paramName);
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
            TestDelegate test = () => PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocation", paramName);
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