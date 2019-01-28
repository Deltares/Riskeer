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
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.WaveConditions;
using Riskeer.HydraRing.Calculation.Data.Variables;
using Riskeer.HydraRing.Calculation.TestUtil;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input.WaveConditions
{
    [TestFixture]
    public class WaveConditionsCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int sectionId = 111;
            const double sectionNormal = 90;
            const int hydraulicBoundaryLocationId = 222;
            const double norm = 0.333;
            IEnumerable<HydraRingForelandPoint> forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 4.4);
            const double waterLevel = 5.5;
            const double a = 6.6;
            const double b = 7.7;

            // Call
            var waveConditionsCalculationInput = new WaveConditionsCalculationInputImplementation(sectionId,
                                                                                                  sectionNormal,
                                                                                                  hydraulicBoundaryLocationId,
                                                                                                  norm,
                                                                                                  forelandPoints,
                                                                                                  breakWater,
                                                                                                  waterLevel,
                                                                                                  a,
                                                                                                  b);

            // Assert
            double expectedBeta = StatisticsConverter.ProbabilityToReliability(norm);
            Assert.IsInstanceOf<HydraRingCalculationInput>(waveConditionsCalculationInput);
            Assert.AreEqual(HydraRingFailureMechanismType.QVariant, waveConditionsCalculationInput.FailureMechanismType);
            Assert.AreEqual(8, waveConditionsCalculationInput.CalculationTypeId);
            Assert.AreEqual(114, waveConditionsCalculationInput.VariableId);
            Assert.AreEqual(hydraulicBoundaryLocationId, waveConditionsCalculationInput.HydraulicBoundaryLocationId);
            Assert.IsNotNull(waveConditionsCalculationInput.Section);
            Assert.AreEqual(sectionId, waveConditionsCalculationInput.Section.SectionId);
            Assert.AreEqual(sectionNormal, waveConditionsCalculationInput.Section.CrossSectionNormal);
            HydraRingDataEqualityHelper.AreEqual(GetExpectedVariables(waterLevel, a, b).ToArray(), waveConditionsCalculationInput.Variables.ToArray());
            Assert.AreSame(forelandPoints, waveConditionsCalculationInput.ForelandsPoints);
            Assert.AreSame(breakWater, waveConditionsCalculationInput.BreakWater);
            Assert.AreEqual(expectedBeta, waveConditionsCalculationInput.Beta);
        }

        private class WaveConditionsCalculationInputImplementation : WaveConditionsCalculationInput
        {
            public WaveConditionsCalculationInputImplementation(int sectionId,
                                                                double sectionNormal,
                                                                long hydraulicBoundaryLocationId,
                                                                double norm,
                                                                IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                                HydraRingBreakWater breakWater,
                                                                double waterLevel,
                                                                double a,
                                                                double b)
                : base(sectionId,
                       sectionNormal,
                       hydraulicBoundaryLocationId,
                       norm,
                       forelandPoints,
                       breakWater,
                       waterLevel,
                       a,
                       b) {}
        }

        private static IEnumerable<HydraRingVariable> GetExpectedVariables(double waterLevel, double a, double b)
        {
            yield return new DeterministicHydraRingVariable(113, waterLevel);
            yield return new DeterministicHydraRingVariable(114, 1.0);
            yield return new DeterministicHydraRingVariable(115, a);
            yield return new DeterministicHydraRingVariable(116, b);
        }
    }
}