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

using System.Linq;
using Core.Common.Util;
using NUnit.Framework;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class DunesBoundaryConditionsCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double norm = 1.0 / 10000;
            const int sectionId = 1;
            const long hydraulicBoundaryLocationId = 1234;

            // Call
            var dunesBoundaryConditionsCalculationInput = new DunesBoundaryConditionsCalculationInput(sectionId, hydraulicBoundaryLocationId, norm);

            // Assert
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.IsInstanceOf<AssessmentLevelCalculationInput>(dunesBoundaryConditionsCalculationInput);
            Assert.AreEqual(HydraRingFailureMechanismType.DunesBoundaryConditions, dunesBoundaryConditionsCalculationInput.FailureMechanismType);
            Assert.AreEqual(2, dunesBoundaryConditionsCalculationInput.CalculationTypeId);
            Assert.AreEqual(26, dunesBoundaryConditionsCalculationInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, dunesBoundaryConditionsCalculationInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(dunesBoundaryConditionsCalculationInput.Section);
            CollectionAssert.IsEmpty(dunesBoundaryConditionsCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(dunesBoundaryConditionsCalculationInput.ForelandsPoints);
            Assert.IsNull(dunesBoundaryConditionsCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, dunesBoundaryConditionsCalculationInput.Beta);

            HydraRingSection section = dunesBoundaryConditionsCalculationInput.Section;
            Assert.AreEqual(sectionId, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.IsNaN(section.CrossSectionNormal);

            HydraRingVariable[] hydraRingVariables = dunesBoundaryConditionsCalculationInput.Variables.ToArray();
            Assert.AreEqual(1, hydraRingVariables.Length);
            HydraRingVariable waterLevelVariable = hydraRingVariables.First();
            Assert.IsInstanceOf<DeterministicHydraRingVariable>(waterLevelVariable);
            Assert.AreEqual(26, waterLevelVariable.VariableId);
            Assert.AreEqual(0.0, waterLevelVariable.Value);
        }

        [Test]
        public void GetSubMechanismModelId_ReturnsExpectedValues()
        {
            // Call
            var dunesBoundaryConditionsCalculationInput = new DunesBoundaryConditionsCalculationInput(1, 1, 2.2);

            // Assert
            Assert.IsNull(dunesBoundaryConditionsCalculationInput.GetSubMechanismModelId(1));
        }
    }
}