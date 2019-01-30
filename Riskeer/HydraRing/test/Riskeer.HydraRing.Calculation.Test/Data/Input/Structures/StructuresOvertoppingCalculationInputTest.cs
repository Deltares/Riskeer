// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.Data.Input.Structures;
using Riskeer.HydraRing.Calculation.Data.Variables;
using Riskeer.HydraRing.Calculation.TestUtil;

namespace Riskeer.HydraRing.Calculation.Test.Data.Input.Structures
{
    [TestFixture]
    public class StructuresOvertoppingCalculationInputTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const int hydraulicBoundaryLocationId = 1000;

            IEnumerable<HydraRingForelandPoint> forelandPoints = Enumerable.Empty<HydraRingForelandPoint>();
            var breakWater = new HydraRingBreakWater(1, 1.1);

            const double sectionNormal = 26.6;
            const double gravitationalAcceleration = 1.1;
            const double modelFactorOvertoppingFlowMean = 2.2;
            const double modelFactorOvertoppingFlowStandardDeviation = 3.3;
            const double levelCrestStructureMean = 4.4;
            const double levelCrestStructureStandardDeviation = 5.5;
            const double structureNormalOrientation = 6.6;
            const double modelFactorSuperCriticalFlowMean = 7.7;
            const double modelFactorSuperCriticalFlowStandardDeviation = 8.8;
            const double allowedLevelIncreaseStorageMean = 9.9;
            const double allowedLevelIncreaseStorageStandardDeviation = 10.0;
            const double modelFactorStorageVolumeMean = 11.1;
            const double modelFactorStorageVolumeStandardDeviation = 12.2;
            const double storageStructureAreaMean = 13.3;
            const double storageStructureAreaVariation = 14.4;
            const double modelFactorInflowVolume = 15.5;
            const double flowWidthAtBottomProtectionMean = 16.6;
            const double flowWidthAtBottomProtectionStandardDeviation = 17.7;
            const double criticalOvertoppingDischargeMean = 18.8;
            const double criticalOvertoppingDischargeVariation = 19.9;
            const double failureProbabilityStructureWithErosion = 20.0;
            const double widthFlowAperturesMean = 21.1;
            const double widthFlowAperturesStandardDeviation = 22.2;
            const double deviationWaveDirection = 23.3;
            const double stormDurationMean = 24.4;
            const double stormDurationVariation = 25.5;

            // Call
            var input = new StructuresOvertoppingCalculationInput(hydraulicBoundaryLocationId,
                                                                  sectionNormal,
                                                                  forelandPoints, breakWater,
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
                                                                  widthFlowAperturesMean, widthFlowAperturesStandardDeviation,
                                                                  deviationWaveDirection,
                                                                  stormDurationMean, stormDurationVariation);

            // Assert
            const int expectedCalculationTypeId = 1;
            const int variableId = 60;
            Assert.IsInstanceOf<ExceedanceProbabilityCalculationInput>(input);
            Assert.AreEqual(expectedCalculationTypeId, input.CalculationTypeId);
            Assert.AreEqual(hydraulicBoundaryLocationId, input.HydraulicBoundaryLocationId);
            Assert.AreEqual(HydraRingFailureMechanismType.StructuresOvertopping, input.FailureMechanismType);
            Assert.AreEqual(variableId, input.VariableId);
            Assert.AreEqual(6, input.IterationMethodId);

            HydraRingSection section = input.Section;
            Assert.AreEqual(1, section.SectionId);
            Assert.IsNaN(section.SectionLength);
            Assert.AreEqual(sectionNormal, section.CrossSectionNormal);
            Assert.AreSame(forelandPoints, input.ForelandsPoints);
            Assert.AreSame(breakWater, input.BreakWater);
            HydraRingDataEqualityHelper.AreEqual(GetDefaultOvertoppingVariables().ToArray(), input.Variables.ToArray());
        }

        private static IEnumerable<HydraRingVariable> GetDefaultOvertoppingVariables()
        {
            yield return new DeterministicHydraRingVariable(58, 1.1);
            yield return new LogNormalHydraRingVariable(59, HydraRingDeviationType.Standard, 2.2, 3.3);
            yield return new NormalHydraRingVariable(60, HydraRingDeviationType.Standard, 4.4, 5.5);
            yield return new DeterministicHydraRingVariable(61, 6.6);
            yield return new NormalHydraRingVariable(62, HydraRingDeviationType.Standard, 7.7, 8.8);
            yield return new LogNormalHydraRingVariable(94, HydraRingDeviationType.Standard, 9.9, 10.0);
            yield return new LogNormalHydraRingVariable(95, HydraRingDeviationType.Standard, 11.1, 12.2);
            yield return new LogNormalHydraRingVariable(96, HydraRingDeviationType.Variation, 13.3, 14.4);
            yield return new DeterministicHydraRingVariable(97, 15.5);
            yield return new LogNormalHydraRingVariable(103, HydraRingDeviationType.Standard, 16.6, 17.7);
            yield return new LogNormalHydraRingVariable(104, HydraRingDeviationType.Variation, 18.8, 19.9);
            yield return new DeterministicHydraRingVariable(105, 20.0);
            yield return new NormalHydraRingVariable(106, HydraRingDeviationType.Standard, 21.1, 22.2);
            yield return new DeterministicHydraRingVariable(107, 23.3);
            yield return new LogNormalHydraRingVariable(108, HydraRingDeviationType.Variation, 24.4, 25.5);
        }
    }
}