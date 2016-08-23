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

using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Hydraulics
{
    [TestFixture]
    public class WaveConditionsInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int norm = 111;
            const int sectionId = 2;
            const int hydraulicBoundaryLocationId = 3000;

            // Call
            var waveConditionsInput = new WaveConditionsInput(sectionId, hydraulicBoundaryLocationId, norm);

            // Assert
            const int expectedCalculationTypeId = 6;
            const int expectedVariableId = 114;
            double expectedBeta = StatisticsConverter.NormToBeta(norm);
            Assert.IsInstanceOf<HydraRingCalculationInput>(waveConditionsInput);
            Assert.AreEqual(HydraRingFailureMechanismType.QVariant, waveConditionsInput.FailureMechanismType);
            Assert.AreEqual(expectedCalculationTypeId, waveConditionsInput.CalculationTypeId);
            Assert.AreEqual(expectedVariableId, waveConditionsInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, waveConditionsInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(waveConditionsInput.Section);
            Assert.AreEqual(sectionId, waveConditionsInput.Section.SectionId);
            Assert.AreEqual(expectedBeta, waveConditionsInput.Beta);
        }
    }
}