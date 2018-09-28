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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Util;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.Data.Variables;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.WaveConditions
{
    [TestFixture]
    public class WaveConditionsTrapezoidCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int sectionId = 111;
            const double sectionNormal = 90;
            const int hydraulicBoundaryLocationId = 222;
            const double norm = 1.0 / 333;
            IEnumerable<HydraRingForelandPoint> forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 4.4);
            const double waterLevel = 5.5;
            const double a = 6.6;
            const double b = 7.7;
            const double beta1 = 8.8;
            const double beta2 = 9.9;

            // Call
            var waveConditionsTrapezoidCalculationInput = new WaveConditionsTrapezoidCalculationInput(sectionId,
                                                                                                      sectionNormal,
                                                                                                      hydraulicBoundaryLocationId,
                                                                                                      norm,
                                                                                                      forelandPoints,
                                                                                                      breakWater,
                                                                                                      waterLevel,
                                                                                                      a,
                                                                                                      b,
                                                                                                      beta1,
                                                                                                      beta2);

            // Assert
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.IsInstanceOf<WaveConditionsCalculationInput>(waveConditionsTrapezoidCalculationInput);
            Assert.AreEqual(HydraRingFailureMechanismType.QVariant, waveConditionsTrapezoidCalculationInput.FailureMechanismType);
            Assert.AreEqual(8, waveConditionsTrapezoidCalculationInput.CalculationTypeId);
            Assert.AreEqual(114, waveConditionsTrapezoidCalculationInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, waveConditionsTrapezoidCalculationInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(waveConditionsTrapezoidCalculationInput.Section);
            Assert.AreEqual(sectionId, waveConditionsTrapezoidCalculationInput.Section.SectionId);
            Assert.AreEqual(sectionNormal, waveConditionsTrapezoidCalculationInput.Section.CrossSectionNormal);
            HydraRingDataEqualityHelper.AreEqual(GetExpectedVariables(waterLevel, a, b, beta1, beta2).ToArray(), waveConditionsTrapezoidCalculationInput.Variables.ToArray());
            Assert.AreSame(forelandPoints, waveConditionsTrapezoidCalculationInput.ForelandsPoints);
            Assert.AreSame(breakWater, waveConditionsTrapezoidCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, waveConditionsTrapezoidCalculationInput.Beta);
        }

        [Test]
        [TestCase(3, null)]
        [TestCase(4, null)]
        [TestCase(5, 70)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Call
            var waveConditionsTrapezoidCalculationInput = new WaveConditionsTrapezoidCalculationInput(111,
                                                                                                      1.1,
                                                                                                      222,
                                                                                                      0.3333,
                                                                                                      Enumerable.Empty<HydraRingForelandPoint>(),
                                                                                                      new HydraRingBreakWater(1, 4.4),
                                                                                                      5.5,
                                                                                                      6.6,
                                                                                                      7.7,
                                                                                                      8.8,
                                                                                                      9.9);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, waveConditionsTrapezoidCalculationInput.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetExpectedVariables(double waterLevel, double a, double b, double beta1, double beta2)
        {
            yield return new DeterministicHydraRingVariable(113, waterLevel);
            yield return new DeterministicHydraRingVariable(114, 1.0);
            yield return new DeterministicHydraRingVariable(115, a);
            yield return new DeterministicHydraRingVariable(116, b);
            yield return new DeterministicHydraRingVariable(117, beta1);
            yield return new DeterministicHydraRingVariable(118, beta2);
        }
    }
}