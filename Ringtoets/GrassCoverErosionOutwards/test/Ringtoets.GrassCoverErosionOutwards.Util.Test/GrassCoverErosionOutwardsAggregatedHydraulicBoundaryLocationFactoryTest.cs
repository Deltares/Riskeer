﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;

namespace Ringtoets.GrassCoverErosionOutwards.Util.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationFactoryTest
    {
        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_AssessmentSectionNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                null, new GrassCoverErosionOutwardsFailureMechanism());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("assessmentSection", exception.ParamName);
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                assessmentSection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_LocationsWithOutput_ReturnGrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocations()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            assessmentSection.AddHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "location1", 1, 1), true);
            assessmentSection.AddHydraulicBoundaryLocation(new HydraulicBoundaryLocation(2, "location2", 2, 2), true);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.SetHydraulicBoundaryLocationCalculations(assessmentSection.HydraulicBoundaryDatabase.Locations);

            SetOutputOnHydraulicBoundaryCalculations(failureMechanism);

            // Call
            GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation[] aggregatedLocations =
                GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                    assessmentSection, failureMechanism).ToArray();

            // Assert
            HydraulicBoundaryLocation[] expectedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(expectedLocations.Length, aggregatedLocations.Length);

            for (var i = 0; i < expectedLocations.Length; i++)
            {
                Assert.AreEqual(expectedLocations[i].Id, aggregatedLocations[i].Id);
                Assert.AreEqual(expectedLocations[i].Name, aggregatedLocations[i].Name);
                Assert.AreEqual(expectedLocations[i].Location, aggregatedLocations[i].Location);

                Assert.AreEqual(GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm);
                Assert.AreEqual(GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForMechanismSpecificSignalingNorm);
                Assert.AreEqual(GetExpectedResult(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForMechanismSpecificLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaterLevelCalculationForFactorizedLowerLimitNorm);

                Assert.AreEqual(GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm);
                Assert.AreEqual(GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForMechanismSpecificSignalingNorm);
                Assert.AreEqual(GetExpectedResult(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForMechanismSpecificLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForLowerLimitNorm);
                Assert.AreEqual(GetExpectedResult(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, expectedLocations[i]),
                                aggregatedLocations[i].WaveHeightCalculationForFactorizedLowerLimitNorm);
            }
        }

        [Test]
        public void CreateAggregatedHydraulicBoundaryLocations_LocationsWithoutOutput_ReturnGrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocations()
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            assessmentSection.AddHydraulicBoundaryLocation(new HydraulicBoundaryLocation(1, "location1", 1, 1));
            assessmentSection.AddHydraulicBoundaryLocation(new HydraulicBoundaryLocation(2, "location2", 2, 2));

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.SetHydraulicBoundaryLocationCalculations(assessmentSection.HydraulicBoundaryDatabase.Locations);

            // Call
            GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocation[] aggregatedLocations =
                GrassCoverErosionOutwardsAggregatedHydraulicBoundaryLocationFactory.CreateAggregatedHydraulicBoundaryLocations(
                    assessmentSection, failureMechanism).ToArray();

            // Assert
            HydraulicBoundaryLocation[] expectedLocations = assessmentSection.HydraulicBoundaryDatabase.Locations.ToArray();
            Assert.AreEqual(expectedLocations.Length, aggregatedLocations.Length);

            for (var i = 0; i < expectedLocations.Length; i++)
            {
                Assert.AreEqual(expectedLocations[i].Id, aggregatedLocations[i].Id);
                Assert.AreEqual(expectedLocations[i].Name, aggregatedLocations[i].Name);
                Assert.AreEqual(expectedLocations[i].Location, aggregatedLocations[i].Location);

                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForMechanismSpecificFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForMechanismSpecificSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForMechanismSpecificLowerLimitNorm);
                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForLowerLimitNorm);
                Assert.IsNaN(aggregatedLocations[i].WaterLevelCalculationForFactorizedLowerLimitNorm);

                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForMechanismSpecificFactorizedSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForMechanismSpecificSignalingNorm);
                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForMechanismSpecificLowerLimitNorm);
                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForLowerLimitNorm);
                Assert.IsNaN(aggregatedLocations[i].WaveHeightCalculationForFactorizedLowerLimitNorm);
            }
        }

        private static void SetOutputOnHydraulicBoundaryCalculations(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            var random = new Random(39);

            SetOutputOnCalculationsInlist(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, random);
            SetOutputOnCalculationsInlist(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, random);
            SetOutputOnCalculationsInlist(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, random);
            SetOutputOnCalculationsInlist(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, random);
            SetOutputOnCalculationsInlist(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, random);
            SetOutputOnCalculationsInlist(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, random);
        }

        private static void SetOutputOnCalculationsInlist(IEnumerable<HydraulicBoundaryLocationCalculation> calculationsList, Random random)
        {
            foreach (HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation in calculationsList)
            {
                hydraulicBoundaryLocationCalculation.Output = new TestHydraulicBoundaryLocationOutput(random.NextDouble());
            }
        }

        private static RoundedDouble GetExpectedResult(IEnumerable<HydraulicBoundaryLocationCalculation> calculationList,
                                                       HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return calculationList
                   .Where(calculation => calculation.HydraulicBoundaryLocation.Equals(hydraulicBoundaryLocation))
                   .Select(calculation => calculation.Output.Result)
                   .Single();
        }
    }
}