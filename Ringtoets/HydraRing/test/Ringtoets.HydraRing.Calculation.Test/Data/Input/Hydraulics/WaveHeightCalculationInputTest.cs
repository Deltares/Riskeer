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
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Hydraulics;
using Riskeer.HydraRing.Calculation.Data.Variables;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class WaveHeightCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const double norm = 1.0 / 10000;
            const int sectionId = 1;
            const long hydraulicBoundaryLocationId = 1234;

            // Call
            var waveHeightCalculationInput = new WaveHeightCalculationInput(sectionId, hydraulicBoundaryLocationId, norm);

            // Assert
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.IsInstanceOf<ReliabilityIndexCalculationInput>(waveHeightCalculationInput);
            Assert.AreEqual(HydraRingFailureMechanismType.WaveHeight, waveHeightCalculationInput.FailureMechanismType);
            Assert.AreEqual(9, waveHeightCalculationInput.CalculationTypeId);
            Assert.AreEqual(28, waveHeightCalculationInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, waveHeightCalculationInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(waveHeightCalculationInput.Section);
            CollectionAssert.IsEmpty(waveHeightCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(waveHeightCalculationInput.ForelandsPoints);
            Assert.IsNull(waveHeightCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, waveHeightCalculationInput.Beta);

            HydraRingSection section = waveHeightCalculationInput.Section;
            Assert.AreEqual(sectionId, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.IsNaN(section.CrossSectionNormal);

            HydraRingVariable[] variables = waveHeightCalculationInput.Variables.ToArray();
            Assert.AreEqual(1, variables.Length);
            HydraRingVariable waveHeightVariable = variables.First();
            Assert.IsInstanceOf<DeterministicHydraRingVariable>(waveHeightVariable);
            Assert.AreEqual(28, waveHeightVariable.VariableId);
            Assert.AreEqual(0.0, waveHeightVariable.Value);
        }

        [Test]
        public void GetSubMechanismModelId_ReturnsExpectedValues()
        {
            // Call
            var waveHeightCalculationInput = new WaveHeightCalculationInput(1, 1, 2.2);

            // Assert
            Assert.IsNull(waveHeightCalculationInput.GetSubMechanismModelId(1));
        }
    }
}