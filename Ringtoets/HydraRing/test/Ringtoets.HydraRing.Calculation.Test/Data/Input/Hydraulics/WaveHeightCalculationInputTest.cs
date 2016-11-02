// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class WaveHeightCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int norm = 10000;
            const int sectionId = 1;
            const long hydraulicBoundaryLocationId = 1234;

            // Call
            var waveHeightCalculationInput = new WaveHeightCalculationInput(sectionId, hydraulicBoundaryLocationId, norm);

            // Assert
            double expectedBeta = StatisticsConverter.NormToBeta(norm);
            Assert.AreEqual(HydraRingFailureMechanismType.WaveHeight, waveHeightCalculationInput.FailureMechanismType);
            Assert.AreEqual(9, waveHeightCalculationInput.CalculationTypeId);
            Assert.AreEqual(28, waveHeightCalculationInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, waveHeightCalculationInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(waveHeightCalculationInput.Section);
            Assert.AreEqual(1, waveHeightCalculationInput.Variables.Count());
            CollectionAssert.IsEmpty(waveHeightCalculationInput.ProfilePoints);
            CollectionAssert.IsEmpty(waveHeightCalculationInput.ForelandsPoints);
            Assert.IsNull(waveHeightCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, waveHeightCalculationInput.Beta);

            HydraRingSection section = waveHeightCalculationInput.Section;
            Assert.AreEqual(sectionId, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.IsNaN(section.CrossSectionNormal);

            var waveHeightVariable = waveHeightCalculationInput.Variables.First();
            Assert.AreEqual(28, waveHeightVariable.VariableId);
            Assert.AreEqual(HydraRingDistributionType.Deterministic, waveHeightVariable.DistributionType);
            Assert.AreEqual(0.0, waveHeightVariable.Value);
            Assert.AreEqual(HydraRingDeviationType.Standard, waveHeightVariable.DeviationType);
            Assert.IsNaN(waveHeightVariable.Mean);
            Assert.IsNaN(waveHeightVariable.Variability);
            Assert.IsNaN(waveHeightVariable.Shift);
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