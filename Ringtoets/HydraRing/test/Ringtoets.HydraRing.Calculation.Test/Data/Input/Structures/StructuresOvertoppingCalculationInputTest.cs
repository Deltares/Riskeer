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
using Ringtoets.HydraRing.Calculation.TestUtil;

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

            HydraRingSection section = new HydraRingSection(1, double.NaN, double.NaN);
            const double gravitationalAcceleration = 9.81;
            const double modelFactorOvertoppingFlowMean = 0.09;
            const double modelFactorOvertoppingFlowStandardDeviation = 0.06;
            const double levelCrestStructureMean = 1.1;
            const double levelCrestStructureStandardDeviation = 0.05;
            const double structureNormalOrientation = 2.2;
            const double modelFactorSuperCriticalFlowMean = 1.1;
            const double modelFactorSuperCriticalFlowStandardDeviation = 0.03;
            const double allowedLevelIncreaseStorageMean = 3.3;
            const double allowedLevelIncreaseStorageStandardDeviation = 0.1;
            const double modelFactorStorageVolumeMean = 1.0;
            const double modelFactorStorageVolumeStandardDeviation = 0.2;
            const double storageStructureAreaMean = 4.4;
            const double storageStructureAreaVariation = 0.1;
            const double modelFactorInflowVolume = 1.0;
            const double flowWidthAtBottomProtectionMean = 5.5;
            const double flowWidthAtBottomProtectionStandardDeviation = 0.05;
            const double criticalOvertoppingDischargeMean = 6.6;
            const double criticalOvertoppingDischargeVariation = 0.15;
            const double failureProbabilityStructureWithErosion = 7.7;
            const double widthFlowAperturesMean = 8.8;
            const double widthFlowAperturesVariation = 0.05;
            const double deviationWaveDirection = 9.9;
            const double stormDurationMean = 7.5;
            const double stormDurationVariation = 0.25;

            // Call
            var structuresOvertoppingCalculationInput = new StructuresOvertoppingCalculationInput(hydraulicBoundaryLocationId, section,
                                                                                                  gravitationalAcceleration,
                                                                                                  modelFactorOvertoppingFlowMean, modelFactorOvertoppingFlowStandardDeviation,
                                                                                                  levelCrestStructureMean, levelCrestStructureStandardDeviation,
                                                                                                  structureNormalOrientation,
                                                                                                  modelFactorSuperCriticalFlowMean, modelFactorSuperCriticalFlowStandardDeviation,
                                                                                                  allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                                                                                                  modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                                                                                                  storageStructureAreaMean, storageStructureAreaVariation,
                                                                                                  modelFactorInflowVolume,
                                                                                                  flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                                                                  criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                                                                                                  failureProbabilityStructureWithErosion,
                                                                                                  widthFlowAperturesMean, widthFlowAperturesVariation,
                                                                                                  deviationWaveDirection,
                                                                                                  stormDurationMean, stormDurationVariation);

            // Assert
            const int expectedCalculationTypeId = 1;
            const int variableId = 60;
            Assert.AreEqual(expectedCalculationTypeId, structuresOvertoppingCalculationInput.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, structuresOvertoppingCalculationInput.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresOvertopping, structuresOvertoppingCalculationInput.FailureMechanismType);
            Assert.AreEqual(variableId, structuresOvertoppingCalculationInput.VariableId);
            Assert.AreEqual(section, structuresOvertoppingCalculationInput.Section);
            HydraRingVariableAssert.AreEqual(GetDefaultOvertoppingVariables().ToArray(), structuresOvertoppingCalculationInput.Variables.ToArray());
        }

        private static IEnumerable<HydraRingVariable> GetDefaultOvertoppingVariables()
        {
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, 9.81, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(59, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 0.09, 0.06, double.NaN);
            yield return new HydraRingVariable(60, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.1, 0.05, double.NaN);
            yield return new HydraRingVariable(61, HydraRingDistributionType.Deterministic, 2.2, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(62, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 1.1, 0.03, double.NaN);
            yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 3.3, 0.1, double.NaN);
            yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 1.0, 0.2, double.NaN);
            yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 4.4, 0.1, double.NaN);
            yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, 1.0, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(103, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 5.5, 0.05, double.NaN);
            yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 6.6, 0.15, double.NaN);
            yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 7.7, 0.0, double.NaN);
            yield return new HydraRingVariable(106, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Variation, 8.8, 0.05, double.NaN);
            yield return new HydraRingVariable(107, HydraRingDistributionType.Deterministic, 9.9, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 7.5, 0.25, double.NaN);
        }
    }
}