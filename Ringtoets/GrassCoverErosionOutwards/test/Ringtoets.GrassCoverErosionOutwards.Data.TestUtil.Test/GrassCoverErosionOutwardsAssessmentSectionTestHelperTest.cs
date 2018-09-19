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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.GrassCoverErosionOutwards.Data.TestUtil.Test
{
    [TestFixture]
    public class GrassCoverErosionOutwardsAssessmentSectionTestHelperTest
    {
        [Test]
        public void GetHydraulicBoundaryLocationCalculationConfigurationPerFailureMechanismCategoryType_Always_ReturnsExpectedTestCaseDataCollection()
        {
            // Call
            TestCaseData[] testCaseDataCollection = GrassCoverErosionOutwardsAssessmentSectionTestHelper.GetHydraulicBoundaryLocationCalculationConfigurationPerFailureMechanismCategoryType().ToArray();

            // Assert
            AssertTestCaseData(testCaseDataCollection,
                               "MechanismSpecificFactorizedSignalingNorm",
                               FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm,
                               (a, fm) => fm.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm);
            AssertTestCaseData(testCaseDataCollection,
                               "MechanismSpecificSignalingNorm",
                               FailureMechanismCategoryType.MechanismSpecificSignalingNorm,
                               (a, fm) => fm.WaterLevelCalculationsForMechanismSpecificSignalingNorm);
            AssertTestCaseData(testCaseDataCollection,
                               "MechanismSpecificLowerLimitNorm",
                               FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm,
                               (a, fm) => fm.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm);
            AssertTestCaseData(testCaseDataCollection,
                               "LowerLimitNorm",
                               FailureMechanismCategoryType.LowerLimitNorm,
                               (a, fm) => a.WaterLevelCalculationsForLowerLimitNorm);
            AssertTestCaseData(testCaseDataCollection,
                               "FactorizedLowerLimitNorm",
                               FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                               (a, fm) => a.WaterLevelCalculationsForFactorizedLowerLimitNorm);
        }

        private static void AssertTestCaseData(IEnumerable<TestCaseData> testCaseDataCollection,
                                               string expectedTestName,
                                               FailureMechanismCategoryType categoryType,
                                               Func<IAssessmentSection, GrassCoverErosionOutwardsFailureMechanism, IEnumerable<HydraulicBoundaryLocationCalculation>> getCalculationsFunc)
        {
            TestCaseData testCaseData = testCaseDataCollection.Single(td => categoryType.Equals(td.Arguments.ElementAt(3)));

            Assert.AreEqual(expectedTestName, testCaseData.TestName);

            var assessmentSection = (IAssessmentSection) testCaseData.Arguments.ElementAt(0);
            var failureMechanism = (GrassCoverErosionOutwardsFailureMechanism) testCaseData.Arguments.ElementAt(1);
            var hydraulicBoundaryLocation = (HydraulicBoundaryLocation) testCaseData.Arguments.ElementAt(2);
            var hydraulicBoundaryLocationCalculation = (HydraulicBoundaryLocationCalculation) testCaseData.Arguments.ElementAt(4);

            HydraulicBoundaryLocationCalculation expectedHydraulicBoundaryLocationCalculation = getCalculationsFunc(assessmentSection, failureMechanism)
                .First(c => ReferenceEquals(c.HydraulicBoundaryLocation, hydraulicBoundaryLocation));
            Assert.AreSame(expectedHydraulicBoundaryLocationCalculation, hydraulicBoundaryLocationCalculation);
        }
    }
}