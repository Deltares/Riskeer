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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Util.TestUtil;

namespace Ringtoest.GrassCoverErosionOutwards.Util.TestUtil.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelperTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddHydraulicBoundaryLocations_Always_AddsLocationsAndCalculations(bool setCalculationOutput)
        {
            // Setup
            var assessmentSection = new ObservableTestAssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "Test", 1, 1),
                new HydraulicBoundaryLocation(2, "Test", 2, 2)
            };

            // Call
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AddHydraulicBoundaryLocations(failureMechanism, assessmentSection, locations, setCalculationOutput);

            // Assert
            CollectionAssert.AreEqual(locations, assessmentSection.HydraulicBoundaryDatabase.Locations);
            
            AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForLowerLimitNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForLowerLimitNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm, locations, setCalculationOutput);

            AssertHydraulicBoundaryCalculations(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, locations, setCalculationOutput);
        }

        private static void AssertHydraulicBoundaryCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                HydraulicBoundaryLocation[] expectedHydraulicBoundaryLocations,
                                                                bool expectedHasOutput)
        {
            HydraulicBoundaryLocationCalculation[] calculationsArray = calculations.ToArray();

            Assert.AreEqual(expectedHydraulicBoundaryLocations.Length, calculationsArray.Length);

            for (int i = 0; i < expectedHydraulicBoundaryLocations.Length; i++)
            {
                Assert.AreSame(expectedHydraulicBoundaryLocations[i], calculationsArray[i].HydraulicBoundaryLocation);
                Assert.AreEqual(expectedHasOutput, calculationsArray[i].HasOutput);
            }
        }
    }
}