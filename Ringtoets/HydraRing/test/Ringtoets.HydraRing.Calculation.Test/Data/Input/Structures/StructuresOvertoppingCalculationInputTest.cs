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
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresOvertoppingCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            const int variableId = 60;
            HydraRingSection hydraRingSection = new HydraRingSection(variableId, "1000", double.NaN, double.NaN);
            const double gravitationalAcceleration = 9.81;
            const double modelFactorOvertoppingMean = 0.09;
            const double modelFactorOvertoppingStandardDeviation = 0.06;
            const double levelOfCrestOfStructureStandardDeviation = 0.05;
            const double modelFactorOvertoppingSupercriticalFlowMean = 1.1;
            const double modelFactorOvertoppingSupercriticalFlowStandardDeviation = 0.03;
            const double allowableIncreaseOfLevelForStorageStandardDeviation = 0.1;
            const double modelFactorForStorageVolumeMean = 1.0;
            const double modelFactorForStorageVolumeStandardDeviation = 0.2;
            const double storageStructureAreaStandardDeviation = 0.1;
            const double modelFactorForIncomingFlowVolume = 1;
            const double flowWidthAtBottomProtectionStandardDeviation = 0.05;
            const double criticalOvertoppingDischargeMeanStandardDeviation = 0.15;
            const double widthOfFlowAperturesStandardDeviation = 0.05;
            const double stormDurationMean = 7.5;
            const double stormDurationStandardDeviation = 0.25;

            const double levelOfCrestOfStructureMean = 1.1;
            const double orientationOfTheNormalOfTheStructure = 2.2;
            const double allowableIncreaseOfLevelForStorageMean = 3.3;
            const double storageStructureAreaMean = 4.4;
            const double flowWidthAtBottomProtectionMean = 5.5;
            const double criticalOvertoppingDischargeMean = 6.6;
            const double failureProbabilityOfStructureGivenErosion = 7.7;
            const double widthOfFlowAperturesMean = 8.8;
            const double deviationOfTheWaveDirection = 9.9;

            // Call
            StructuresOvertoppingCalculationInput structuresOvertoppingCalculationInput =
                new StructuresOvertoppingCalculationInput(hydraulicBoundaryLocationId, hydraRingSection,
                                                          gravitationalAcceleration,
                                                          modelFactorOvertoppingMean, modelFactorOvertoppingStandardDeviation,
                                                          levelOfCrestOfStructureMean, levelOfCrestOfStructureStandardDeviation,
                                                          orientationOfTheNormalOfTheStructure,
                                                          modelFactorOvertoppingSupercriticalFlowMean, modelFactorOvertoppingSupercriticalFlowStandardDeviation,
                                                          allowableIncreaseOfLevelForStorageMean, allowableIncreaseOfLevelForStorageStandardDeviation,
                                                          modelFactorForStorageVolumeMean, modelFactorForStorageVolumeStandardDeviation,
                                                          storageStructureAreaMean, storageStructureAreaStandardDeviation,
                                                          modelFactorForIncomingFlowVolume,
                                                          flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                          criticalOvertoppingDischargeMean, criticalOvertoppingDischargeMeanStandardDeviation,
                                                          failureProbabilityOfStructureGivenErosion,
                                                          widthOfFlowAperturesMean, widthOfFlowAperturesStandardDeviation,
                                                          deviationOfTheWaveDirection,
                                                          stormDurationMean, stormDurationStandardDeviation);

            // Assert
            const int expectedCalculationTypeId = 1;
            Assert.AreEqual(expectedCalculationTypeId, structuresOvertoppingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, structuresOvertoppingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresOvertopping, structuresOvertoppingCalculationInput.FailureMechanismType);
            Assert.AreEqual(variableId, structuresOvertoppingCalculationInput.VariableId);
            Assert.AreEqual(hydraRingSection, structuresOvertoppingCalculationInput.Section);
            CheckOvertoppingVariables(GetDefaultOvertoppingVariables().ToArray(), structuresOvertoppingCalculationInput.Variables.ToArray());
        }

        private static void CheckOvertoppingVariables(HydraRingVariable[] expected, HydraRingVariable[] actual)
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

        private static IEnumerable<HydraRingVariable> GetDefaultOvertoppingVariables()
        {
            yield return new HydraRingVariableImplementation(58, HydraRingDistributionType.Deterministic, 9.81, HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(59, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 0.09, 0.06, double.NaN);
            yield return new HydraRingVariableImplementation(60, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.1, 0.05, double.NaN);
            yield return new HydraRingVariableImplementation(61, HydraRingDistributionType.Deterministic, 2.2, HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(62, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.1, 0.03, double.NaN);
            yield return new HydraRingVariableImplementation(94, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 3.3, 0.1, double.NaN);
            yield return new HydraRingVariableImplementation(95, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 1, 0.2, double.NaN);
            yield return new HydraRingVariableImplementation(96, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 4.4, 0.1, double.NaN);
            yield return new HydraRingVariableImplementation(97, HydraRingDistributionType.Deterministic, 1, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(103, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 5.5, 0.05, double.NaN);
            yield return new HydraRingVariableImplementation(104, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 6.6, 0.15, double.NaN);
            yield return new HydraRingVariableImplementation(105, HydraRingDistributionType.Deterministic, 7.7, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(106, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 8.8, 0.05, double.NaN);
            yield return new HydraRingVariableImplementation(107, HydraRingDistributionType.Deterministic, 9.9, HydraRingDeviationType.Variation, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariableImplementation(108, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 7.5, 0.25, double.NaN);
        }

        private class HydraRingVariableImplementation : HydraRingVariable
        {
            public HydraRingVariableImplementation(int variableId, HydraRingDistributionType distributionType, double value, HydraRingDeviationType deviationType, double mean, double variability, double shift) :
                base(variableId, distributionType, value, deviationType, mean, variability, shift) {}
        }
    }
}