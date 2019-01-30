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

using System;
using System.Linq;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;

namespace Riskeer.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class MapViewTestHelperTest
    {
        [Test]
        public void GetCalculationFuncs_Always_ReturnsExpectedCases()
        {
            // Setup
            var assessmentSection = new AssessmentSectionStub();
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                new HydraulicBoundaryLocation(1, "test", 2, 3)
            });

            // Call
            TestCaseData[] cases = MapViewTestHelper.GetCalculationFuncs.ToArray();

            // Assert
            var expectedCases = new[]
            {
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => a.WaterLevelCalculationsForFactorizedSignalingNorm.First())),
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => assessmentSection.WaterLevelCalculationsForSignalingNorm.First())),
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => a.WaterLevelCalculationsForLowerLimitNorm.First())),
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => a.WaterLevelCalculationsForFactorizedLowerLimitNorm.First())),
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => a.WaveHeightCalculationsForFactorizedSignalingNorm.First())),
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => a.WaveHeightCalculationsForSignalingNorm.First())),
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => a.WaveHeightCalculationsForLowerLimitNorm.First())),
                new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                     a => a.WaveHeightCalculationsForFactorizedLowerLimitNorm.First()))
            };

            Assert.AreEqual(expectedCases.Length, cases.Length);
            for (var i = 0; i < expectedCases.Length; i++)
            {
                var expectedFunc = (Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>) expectedCases[i].Arguments[0];
                HydraulicBoundaryLocationCalculation expectedFuncOutput = expectedFunc(assessmentSection);

                var actualFunc = (Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>) cases[i].Arguments[0];
                HydraulicBoundaryLocationCalculation actualFuncOutput = actualFunc(assessmentSection);

                Assert.AreSame(expectedFuncOutput, actualFuncOutput);
            }
        }
    }
}