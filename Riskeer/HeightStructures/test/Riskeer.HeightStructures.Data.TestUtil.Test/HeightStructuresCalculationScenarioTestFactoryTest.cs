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
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.HeightStructures.Data.TestUtil.Test
{
    [TestFixture]
    public class HeightStructuresCalculationScenarioTestFactoryTest
    {
        [Test]
        public void CreateHeightStructuresCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HeightStructuresCalculationScenarioTestFactory.CreateHeightStructuresCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateHeightStructuresCalculationScenario_WithSection_CreatesRelevantCalculationWithOutput()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            StructuresCalculationScenario<HeightStructuresInput> scenario = HeightStructuresCalculationScenarioTestFactory.CreateHeightStructuresCalculationScenario(section);

            // Assert
            Assert.IsNotNull(scenario.Output);
            Assert.IsTrue(scenario.IsRelevant);
            Assert.IsNotNull(scenario.InputParameters.Structure);
            Assert.AreSame(section.StartPoint, scenario.InputParameters.Structure.Location);
        }

        [Test]
        public void CreateNotCalculatedHeightStructuresCalculationScenario_SectionNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => HeightStructuresCalculationScenarioTestFactory.CreateNotCalculatedHeightStructuresCalculationScenario(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("section", exception.ParamName);
        }

        [Test]
        public void CreateNotCalculatedHeightStructuresCalculationScenario_WithSection_CreatesRelevantCalculationWithoutOutput()
        {
            // Setup
            FailureMechanismSection section = FailureMechanismSectionTestFactory.CreateFailureMechanismSection();

            // Call
            StructuresCalculationScenario<HeightStructuresInput> scenario = HeightStructuresCalculationScenarioTestFactory.CreateNotCalculatedHeightStructuresCalculationScenario(section);

            // Assert
            Assert.IsNull(scenario.Output);
            Assert.IsTrue(scenario.IsRelevant);
            Assert.IsNotNull(scenario.InputParameters.Structure);
            Assert.AreSame(section.StartPoint, scenario.InputParameters.Structure.Location);
        }
    }
}