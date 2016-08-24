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

using System.Collections.Generic;
using System.Linq;
using Core.Common.Utils;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.WaveConditions
{
    [TestFixture]
    public class WaveConditionsCosineInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int sectionId = 111;
            const int hydraulicBoundaryLocationId = 222;
            const int norm = 333;
            var forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 4.4);

            // Call
            const double waterLevel = 5.5;
            const double a = 6.6;
            const double b = 7.7;
            const double c = 8.8;

            var waveConditionsCosineInput = new WaveConditionsCosineInput(sectionId,
                                                                          hydraulicBoundaryLocationId,
                                                                          norm,
                                                                          forelandPoints,
                                                                          breakWater,
                                                                          waterLevel,
                                                                          a,
                                                                          b,
                                                                          c);

            // Assert
            const int expectedCalculationTypeId = 6;
            const int expectedVariableId = 114;
            double expectedBeta = StatisticsConverter.NormToBeta(norm);
            Assert.IsInstanceOf<WaveConditionsCalculationInput>(waveConditionsCosineInput);
            Assert.AreEqual(HydraRingFailureMechanismType.QVariant, waveConditionsCosineInput.FailureMechanismType);
            Assert.AreEqual(expectedCalculationTypeId, waveConditionsCosineInput.CalculationTypeId);
            Assert.AreEqual(expectedVariableId, waveConditionsCosineInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, waveConditionsCosineInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(waveConditionsCosineInput.Section);
            Assert.AreEqual(sectionId, waveConditionsCosineInput.Section.SectionId);
            HydraRingVariableAssert.AreEqual(GetExpectedVariables(waterLevel, a, b, c).ToArray(), waveConditionsCosineInput.Variables.ToArray());
            Assert.AreSame(forelandPoints, waveConditionsCosineInput.ForelandsPoints);
            Assert.AreSame(breakWater, waveConditionsCosineInput.BreakWater);
            Assert.AreEqual(expectedBeta, waveConditionsCosineInput.Beta);
        }

        [Test]
        [TestCase(3, null)]
        [TestCase(4, 71)]
        [TestCase(5, 71)]
        [TestCase(6, null)]
        public void GetSubMechanismModelId_Always_ReturnsExpectedValues(int subMechanismModelId, int? expectedSubMechanismModelId)
        {
            // Call
            var waveConditionsCosineInput = new WaveConditionsCosineInput(111,
                                                                          222,
                                                                          333,
                                                                          Enumerable.Empty<HydraRingForelandPoint>(),
                                                                          new HydraRingBreakWater(1, 4.4),
                                                                          5.5,
                                                                          6.6,
                                                                          7.7,
                                                                          8.8);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, waveConditionsCosineInput.GetSubMechanismModelId(subMechanismModelId));
        }

        private static IEnumerable<HydraRingVariable> GetExpectedVariables(double waterLevel, double a, double b, double c)
        {
            yield return new HydraRingVariable(113, HydraRingDistributionType.Deterministic, waterLevel, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(114, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(115, HydraRingDistributionType.Deterministic, a, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(116, HydraRingDistributionType.Deterministic, b, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(119, HydraRingDistributionType.Deterministic, c, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }
    }
}