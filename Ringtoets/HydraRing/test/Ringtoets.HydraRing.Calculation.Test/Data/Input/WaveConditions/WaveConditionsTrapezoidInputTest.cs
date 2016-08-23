﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.WaveConditions
{
    [TestFixture]
    public class WaveConditionsTrapezoidInputTest
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
            const double beta1 = 8.8;
            const double beta2 = 9.9;

            // Call
            var waveConditionsTrapezoidInput = new WaveConditionsTrapezoidInput(sectionId,
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
            CheckVariables(GetExpectedVariables(waterLevel, a, b, beta1, beta2).ToArray(), waveConditionsTrapezoidInput.Variables.ToArray());
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
            var waveConditionsTrapezoidInput = new WaveConditionsTrapezoidInput(111,
                                                                                222,
                                                                                333,
                                                                                Enumerable.Empty<HydraRingForelandPoint>(),
                                                                                new HydraRingBreakWater(1, 4.4),
                                                                                5.5,
                                                                                6.6,
                                                                                7.7,
                                                                                8.8,
                                                                                9.9);

            // Assert
            Assert.AreEqual(expectedSubMechanismModelId, waveConditionsTrapezoidInput.GetSubMechanismModelId(subMechanismModelId));
        }

        private static void CheckVariables(HydraRingVariable[] expected, HydraRingVariable[] actual)
        {
            Assert.AreEqual(expected.Length, actual.Length);

            for (var i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i].Value, actual[i].Value, 1e-6);
                Assert.AreEqual(expected[i].DeviationType, actual[i].DeviationType);
                Assert.AreEqual(expected[i].DistributionType, actual[i].DistributionType);
                Assert.AreEqual(expected[i].Mean, actual[i].Mean, 1e-6);
                Assert.AreEqual(expected[i].Shift, actual[i].Shift, 1e-6);
                Assert.AreEqual(expected[i].Variability, actual[i].Variability, 1e-6);
                Assert.AreEqual(expected[i].VariableId, actual[i].VariableId, 1e-6);
            }
        }

        private static IEnumerable<HydraRingVariable> GetExpectedVariables(double waterLevel, double a, double b, double beta1, double beta2)
        {
            yield return new HydraRingVariable(113, HydraRingDistributionType.Deterministic, waterLevel, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(114, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(115, HydraRingDistributionType.Deterministic, a, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(116, HydraRingDistributionType.Deterministic, b, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(117, HydraRingDistributionType.Deterministic, beta1, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(118, HydraRingDistributionType.Deterministic, beta2, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }
    }
}