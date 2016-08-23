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
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.WaveConditions
{
    [TestFixture]
    public class WaveConditionsTrapezoidInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int norm = 111;
            const int sectionId = 2;
            const int hydraulicBoundaryLocationId = 3000;
            var forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 2.2);

            // Call
            var waveConditionsTrapezoidInput = new WaveConditionsTrapezoidInput(sectionId, hydraulicBoundaryLocationId, norm,
                forelandPoints, breakWater);

            // Assert
            const int expectedCalculationTypeId = 6;
            const int expectedVariableId = 114;
            double expectedBeta = StatisticsConverter.NormToBeta(norm);
            Assert.IsInstanceOf<WaveConditionsInput>(waveConditionsTrapezoidInput);
            Assert.AreEqual(HydraRingFailureMechanismType.QVariant, waveConditionsTrapezoidInput.FailureMechanismType);
            Assert.AreEqual(expectedCalculationTypeId, waveConditionsTrapezoidInput.CalculationTypeId);
            Assert.AreEqual(expectedVariableId, waveConditionsTrapezoidInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, waveConditionsTrapezoidInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(waveConditionsTrapezoidInput.Section);
            Assert.AreEqual(sectionId, waveConditionsTrapezoidInput.Section.SectionId);
            Assert.AreSame(forelandPoints, waveConditionsTrapezoidInput.ForelandsPoints);
            Assert.AreSame(breakWater, waveConditionsTrapezoidInput.BreakWater);
            Assert.AreEqual(expectedBeta, waveConditionsTrapezoidInput.Beta);
        }

        [Test]
        [TestCase(3, null)]
        [TestCase(4, 70)]
        [TestCase(5, 70)]
        [TestCase(6, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Call
            var waveConditionsTrapezoidInput = new WaveConditionsTrapezoidInput(1, 1000, 111,
                Enumerable.Empty<HydraRingForelandPoint>(),
                new HydraRingBreakWater(1, 2.2));

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, waveConditionsTrapezoidInput.GetSubMechanismModelId(subMechanismModelId));
        }
    }
}
