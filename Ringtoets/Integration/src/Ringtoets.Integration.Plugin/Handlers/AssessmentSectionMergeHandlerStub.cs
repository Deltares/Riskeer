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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Integration.Data;

namespace Ringtoets.Integration.Plugin.Handlers
{
    /// <summary>
    /// Stub for handling the merge of the <see cref="AssessmentSection"/>.
    /// </summary>
    public class AssessmentSectionMergeHandlerStub : IAssessmentSectionMergeHandler
    {
        public void PerformMerge(AssessmentSection originalAssessmentSection, AssessmentSection assessmentSectionToMerge,
                                 IEnumerable<IFailureMechanism> failureMechanismToMerge)
        {
            for (var i = 0; i < originalAssessmentSection.HydraulicBoundaryDatabase.Locations.Count; i++)
            {
                ReplaceOutput(originalAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i), assessmentSectionToMerge.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i));
                ReplaceOutput(originalAssessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(i), assessmentSectionToMerge.WaterLevelCalculationsForSignalingNorm.ElementAt(i));
                ReplaceOutput(originalAssessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(i), assessmentSectionToMerge.WaterLevelCalculationsForLowerLimitNorm.ElementAt(i));
                ReplaceOutput(originalAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(i), assessmentSectionToMerge.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(i));

                ReplaceOutput(originalAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ElementAt(i), assessmentSectionToMerge.WaveHeightCalculationsForFactorizedSignalingNorm.ElementAt(i));
                ReplaceOutput(originalAssessmentSection.WaveHeightCalculationsForSignalingNorm.ElementAt(i), assessmentSectionToMerge.WaveHeightCalculationsForSignalingNorm.ElementAt(i));
                ReplaceOutput(originalAssessmentSection.WaveHeightCalculationsForLowerLimitNorm.ElementAt(i), assessmentSectionToMerge.WaveHeightCalculationsForLowerLimitNorm.ElementAt(i));
                ReplaceOutput(originalAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(i), assessmentSectionToMerge.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(i));
            }
        }

        private static void ReplaceOutput(HydraulicBoundaryLocationCalculation originalCalculation, HydraulicBoundaryLocationCalculation newCalculation)
        {
            originalCalculation.Output = newCalculation.Output;
            originalCalculation.NotifyObservers();
        }
    }
}