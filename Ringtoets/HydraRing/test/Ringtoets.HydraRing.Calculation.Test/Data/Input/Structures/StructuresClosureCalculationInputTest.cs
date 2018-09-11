// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.HydraRing.Calculation.Data.Variables;
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
            IEnumerable<HydraRingForelandPoint> forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double sectionNormal = 21.1;
            const double gravitationalAcceleration = 1.1;
            const double factorStormDurationOpenStructure = 2.2;
            const double failureProbabilityOpenStructure = 3.3;
            const double failureProbabilityReparation = 4.4;
            const int identicalApertures = 5;
            const double allowedLevelIncreaseStorageMean = 6.6;
            const double allowedLevelIncreaseStorageStandardDeviation = 7.7;
            const double modelFactorStorageVolumeMean = 8.8;
            const double modelFactorStorageVolumeStandardDeviation = 9.9;
            const double storageStructureAreaMean = 10.0;
            const double storageStructureAreaVariation = 11.1;
            const double modelFactorInflowVolume = 12.2;
            const double flowWidthAtBottomProtectionMean = 13.3;
            const double flowWidthAtBottomProtectionStandardDeviation = 14.4;
            const double criticalOvertoppingDischargeMean = 15.5;
            const double criticalOvertoppingDischargeVariation = 16.6;
            const double failureProbabilityStructureWithErosion = 17.7;
            const double stormDurationMean = 18.8;
            const double stormDurationVariation = 19.9;
            const double probabilityOpenStructureBeforeFlooding = 20.0;

            // Call
            var input = new TestStructuresClosureCalculationInput(hydraulicBoundaryLocationId,
                                                                  sectionNormal,
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
            Assert.AreEqual(6, input.IterationMethodId);

            HydraRingSection section = input.Section;
            Assert.AreEqual(1, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.AreEqual(sectionNormal, section.CrossSectionNormal);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            Assert.AreSame(breakWater, input.BreakWater);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultVariables().ToArray(), input.Variables.ToArray());
        }

        private static IEnumerable<HydraRingVariable> GetDefaultVariables()
        {
            yield return new DeterministicHydraRingVariable(58, 1.1);
            yield return new DeterministicHydraRingVariable(63, 2.2);
            yield return new DeterministicHydraRingVariable(68, 3.3);
            yield return new DeterministicHydraRingVariable(69, 4.4);
            yield return new DeterministicHydraRingVariable(71, 5);
            yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, 6.6, 7.7);
            yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, 8.8, 9.9);
            yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, 10.0, 11.1);
            yield return new DeterministicHydraRingVariable(97, 12.2);
            yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, 13.3, 14.4);
            yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, 15.5, 16.6);
            yield return new DeterministicHydraRingVariable(105, 17.7);
            yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, 18.8, 19.9);
            yield return new DeterministicHydraRingVariable(129, 20.0);
        }

        private class TestStructuresClosureCalculationInput : StructuresClosureCalculationInput
        {
            public TestStructuresClosureCalculationInput(long hydraulicBoundaryLocationId,
                                                         double sectionNormal,
                                                         IEnumerable<HydraRingForelandPoint> forelandPoints,
                                                         HydraRingBreakWater breakWater,
                                                         double gravitationalAcceleration,
                                                         double factorStormDurationOpenStructure,
                                                         double failureProbabilityOpenStructure,
                                                         double failureProbabilityReparation,
                                                         int identicalApertures,
                                                         double allowedLevelIncreaseStorageMean, double allowedLevelIncreaseStorageStandardDeviation,
                                                         double modelFactorStorageVolumeMean, double modelFactorStorageVolumeStandardDeviation,
                                                         double storageStructureAreaMean, double storageStructureAreaVariation,
                                                         double modelFactorInflowVolume,
                                                         double flowWidthAtBottomProtectionMean, double flowWidthAtBottomProtectionStandardDeviation,
                                                         double criticalOvertoppingDischargeMean, double criticalOvertoppingDischargeVariation,
                                                         double failureProbabilityStructureWithErosion,
                                                         double stormDurationMean, double stormDurationVariation,
                                                         double probabilityOpenStructureBeforeFlooding)
                : base(hydraulicBoundaryLocationId,
                       sectionNormal,
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