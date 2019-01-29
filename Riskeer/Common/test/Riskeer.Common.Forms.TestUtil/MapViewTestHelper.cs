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
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Forms.TestUtil
{
    /// <summary>
    /// Helper class for map views.
    /// </summary>
    public static class MapViewTestHelper
    {
        /// <summary>
        /// Gets a collection of test cases to test map view updating logic regarding
        /// <see cref="HydraulicBoundaryLocationCalculation"/>.
        /// </summary>
        public static IEnumerable<TestCaseData> GetCalculationFuncs
        {
            get
            {
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.First()));
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaterLevelCalculationsForSignalingNorm.First()));
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First()));
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.First()));
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.First()));
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaveHeightCalculationsForSignalingNorm.First()));
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaveHeightCalculationsForLowerLimitNorm.First()));
                yield return new TestCaseData(new Func<IAssessmentSection, HydraulicBoundaryLocationCalculation>(
                                                  assessmentSection => assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.First()));
            }
        }
    }
}