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

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.GrassCoverErosionOutwards.Data.TestUtil.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelperTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetHydraulicBoundaryLocationsWithAssessmentSection_Always_SetsLocationsAndCalculations(bool setCalculationOutput)
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "Test", 1, 1),
                new HydraulicBoundaryLocation(2, "Test", 2, 2)
            };

            // Call
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, assessmentSection, locations, setCalculationOutput);

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

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void SetHydraulicBoundaryLocations_Always_SetsLocationsAndCalculations(bool setCalculationOutput)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var locations = new[]
            {
                new HydraulicBoundaryLocation(1, "Test", 1, 1),
                new HydraulicBoundaryLocation(2, "Test", 2, 2)
            };

            // Call
            GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.SetHydraulicBoundaryLocations(failureMechanism, locations, setCalculationOutput);

            // Assert
            AssertHydraulicBoundaryCalculations(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm, locations, setCalculationOutput);
            AssertHydraulicBoundaryCalculations(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm, locations, setCalculationOutput);
        }

        [Test]
        public void GetAllHydraulicBoundaryLocationCalculationsWithOutput_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        [Test]
        public void GetAllHydraulicBoundaryLocationCalculationsWithOutput_FailureMechanismWithoutHydraulicBoundaryLocationCalculations_ReturnsEmpty()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            // Call
            IEnumerable<HydraulicBoundaryLocationCalculation> calculations =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(failureMechanism);

            // Assert
            CollectionAssert.IsEmpty(calculations);
        }

        [Test]
        public void GetAllHydraulicBoundaryLocationCalculationsWithOutput_FailureMechanismWithHydraulicBoundaryCalculations_ReturnsCalculationsWithOutput()
        {
            // Setup
            var hydraulicBoundaryLocations = new[]
            {
                new TestHydraulicBoundaryLocation(),
                new TestHydraulicBoundaryLocation()
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            HydraulicBoundaryLocationCalculation calculation1 = failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm.First();
            HydraulicBoundaryLocationCalculation calculation2 = failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm.First();
            HydraulicBoundaryLocationCalculation calculation3 = failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm.First();
            HydraulicBoundaryLocationCalculation calculation4 = failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm.First();
            HydraulicBoundaryLocationCalculation calculation5 = failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm.First();
            HydraulicBoundaryLocationCalculation calculation6 = failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm.First();

            calculation1.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            calculation2.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            calculation3.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            calculation4.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            calculation5.Output = new TestHydraulicBoundaryLocationCalculationOutput();
            calculation6.Output = new TestHydraulicBoundaryLocationCalculationOutput();

            // Call
            IEnumerable<HydraulicBoundaryLocationCalculation> calculations =
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.GetAllHydraulicBoundaryLocationCalculationsWithOutput(failureMechanism);

            // Assert
            HydraulicBoundaryLocationCalculation[] expectedCalculations =
            {
                calculation1,
                calculation2,
                calculation3,
                calculation4,
                calculation5,
                calculation6
            };
            CollectionAssert.AreEquivalent(expectedCalculations, calculations);
        }

        [Test]
        public void AssertHydraulicBoundaryLocationCalculationsHaveNoOutputs_FailureMechanismNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () =>
                GrassCoverErosionOutwardsHydraulicBoundaryLocationsTestHelper.AssertHydraulicBoundaryLocationCalculationsHaveNoOutputs(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("failureMechanism", exception.ParamName);
        }

        private static void AssertHydraulicBoundaryCalculations(IEnumerable<HydraulicBoundaryLocationCalculation> calculations,
                                                                HydraulicBoundaryLocation[] expectedHydraulicBoundaryLocations,
                                                                bool expectedHasOutput)
        {
            HydraulicBoundaryLocationCalculation[] calculationsArray = calculations.ToArray();

            Assert.AreEqual(expectedHydraulicBoundaryLocations.Length, calculationsArray.Length);

            for (var i = 0; i < expectedHydraulicBoundaryLocations.Length; i++)
            {
                Assert.AreSame(expectedHydraulicBoundaryLocations[i], calculationsArray[i].HydraulicBoundaryLocation);
                Assert.AreEqual(expectedHasOutput, calculationsArray[i].HasOutput);
            }
        }
    }
}