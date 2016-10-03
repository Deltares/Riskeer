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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresClosureCalculationInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;
            var section = new HydraRingSection(1, double.NaN, double.NaN);
            var forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double gravitationalAcceleration = 9.81;
            const double factorStormDurationOpenStructure = 0.1;
            const double failureProbabilityOpenStructure = 0.04;
            const double failureProbabilityReparation = 0.08;
            const double identicalApertures = 0.4;
            const double allowedLevelIncreaseStorageMean = 3.3;
            const double allowedLevelIncreaseStorageStandardDeviation = 0.1;
            const double modelFactorStorageVolumeMean = 1.0;
            const double modelFactorStorageVolumeStandardDeviation = 0.2;
            const double storageStructureAreaMean = 4.4;
            const double storageStructureAreaVariation = 0.1;
            const double modelFactorInflowVolume = 1;
            const double flowWidthAtBottomProtectionMean = 5.5;
            const double flowWidthAtBottomProtectionStandardDeviation = 0.05;
            const double criticalOvertoppingDischargeMean = 6.6;
            const double criticalOvertoppingDischargeVariation = 0.15;
            const double failureProbabilityStructureWithErosion = 7.7;
            const double stormDurationMean = 7.5;
            const double stormDurationVariation = 0.25;
            const double probabilityOpenStructureBeforeFlooding = 0.04;

            // Call
            var input = new TestStructuresClosureCalculationInput(hydraulicBoundaryLocationId, section,
                                                                  forelandPoints, breakWater,
                                                                  gravitationalAcceleration,
                                                                  factorStormDurationOpenStructure,
                                                                  failureProbabilityOpenStructure,
                                                                  failureProbabilityReparation,
                                                                  identicalApertures,
                                                                  allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                                                                  modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                                                                  storageStructureAreaMean, storageStructureAreaVariation,
                                                                  modelFactorInflowVolume,
                                                                  flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                                                                  criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                                                                  failureProbabilityStructureWithErosion,
                                                                  stormDurationMean, stormDurationVariation,
                                                                  probabilityOpenStructureBeforeFlooding);

            // Assert
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationInput>(input);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(1, input.CalculationTypeId);
            Assert.AreEqual(58, input.VariableId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresClosure, input.FailureMechanismType);
            Assert.AreSame(section, input.Section);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            Assert.AreSame(breakWater, input.BreakWater);
            HydraRingVariableAssert.AreEqual(GetDefaultVariables().ToArray(), input.Variables.ToArray());
        }

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new HydraRingVariable(58, HydraRingDistributionType.Deterministic, 9.81, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(63, HydraRingDistributionType.Deterministic, 0.1, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(68, HydraRingDistributionType.Deterministic, 0.04, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(69, HydraRingDistributionType.Deterministic, 0.08, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(71, HydraRingDistributionType.Deterministic, 0.4, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(94, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 3.3, 0.1, double.NaN);
            yield return new HydraRingVariable(95, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 1.0, 0.2, double.NaN);
            yield return new HydraRingVariable(96, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 4.4, 0.1, double.NaN);
            yield return new HydraRingVariable(97, HydraRingDistributionType.Deterministic, 1, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
            yield return new HydraRingVariable(103, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Standard, 5.5, 0.05, double.NaN);
            yield return new HydraRingVariable(104, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 6.6, 0.15, double.NaN);
            yield return new HydraRingVariable(105, HydraRingDistributionType.Normal, double.NaN, HydraRingDeviationType.Standard, 7.7, 0.0, double.NaN);
            yield return new HydraRingVariable(108, HydraRingDistributionType.LogNormal, double.NaN, HydraRingDeviationType.Variation, 7.5, 0.25, double.NaN);
            yield return new HydraRingVariable(129, HydraRingDistributionType.Deterministic, 0.04, HydraRingDeviationType.Standard, double.NaN, double.NaN, double.NaN);
        }

        private class TestStructuresClosureCalculationInput : StructuresClosureCalculationInput
        {
            public TestStructuresClosureCalculationInput(long hydraulicBoundaryLocationId, HydraRingSection section,
                                                         IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                         HydraRingBreakWater breakWater,
                                                         double gravitationalAcceleration,
                                                         double factorStormDurationOpenStructure,
                                                         double failureProbabilityOpenStructure,
                                                         double failureProbabilityReparation,
                                                         double identicalApertures,
                                                         double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                                         double modelFactorStorageVolumeMean, double modelFactorStorageVolumeStandardDeviation,
                                                         double storageStructureAreaMean, double storageStructureAreaVariation,
                                                         double modelFactorInflowVolume,
                                                         double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                                         double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeVariation,
                                                         double failureProbabilityStructureWithErosion,
                                                         double stormDurationMean, double stormDurationVariation,
                                                         double probabilityOpenStructureBeforeFlooding)
                : base(hydraulicBoundaryLocationId, section,
                       forelandPoints, breakWater,
                       gravitationalAcceleration,
                       factorStormDurationOpenStructure,
                       failureProbabilityOpenStructure,
                       failureProbabilityReparation,
                       identicalApertures,
                       allowedLevelIncreaseStorageMean, allowedLevelIncreaseStorageStandardDeviation,
                       modelFactorStorageVolumeMean, modelFactorStorageVolumeStandardDeviation,
                       storageStructureAreaMean, storageStructureAreaVariation,
                       modelFactorInflowVolume,
                       flowWidthAtBottomProtectionMean, flowWidthAtBottomProtectionStandardDeviation,
                       criticalOvertoppingDischargeMean, criticalOvertoppingDischargeVariation,
                       failureProbabilityStructureWithErosion,
                       stormDurationMean, stormDurationVariation,
                       probabilityOpenStructureBeforeFlooding) {}

            public override int? GetSubMechanismModelId(int subMechanismId)
            {
                throw new NotImplementedException();
            }
        }
    }
}