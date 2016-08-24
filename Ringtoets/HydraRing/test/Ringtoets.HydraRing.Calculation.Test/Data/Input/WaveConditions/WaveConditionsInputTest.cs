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
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.WaveConditions;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.WaveConditions
{
    [TestFixture]
    public class WaveConditionsInputTest
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
            const double waterLevel = 5.5;
            const double a = 6.6;
            const double b = 7.7;

            // Call
            var waveConditionsInput = new WaveConditionsInputImplementation(sectionId,
                                                                            hydraulicBoundaryLocationId,
                                                                            norm,
                                                                            forelandPoints,
                                                                            breakWater,
                                                                            waterLevel,
                                                                            a,
                                                                            b);

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
            HydraRingVariableAssert.AreEqual(GetExpectedVariables(waterLevel, a, b).ToArray(), waveConditionsInput.Variables.ToArray());
            Assert.AreSame(forelandPoints, waveConditionsInput.ForelandsPoints);
            Assert.AreSame(breakWater, waveConditionsInput.BreakWater);
            Assert.AreEqual(expectedBeta, waveConditionsInput.Beta);
        }

        private class WaveConditionsInputImplementation : WaveConditionsInput
        {
            public WaveConditionsInputImplementation(int sectionId,
                                                     long hydraulicBoundaryLocationId,
                                                     double norm,
                                                     IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                     HydraRingBreakWater breakWater,
                                                     double waterLevel,
                                                     double a,
                                                     double b)
                : base(sectionId,
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
            yield return new HydraRingVariable(113, HydraRingDistributionType.Deterministic, waterLevel, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(114, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(115, HydraRingDistributionType.Deterministic, a, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(116, HydraRingDistributionType.Deterministic, b, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }
    }
}