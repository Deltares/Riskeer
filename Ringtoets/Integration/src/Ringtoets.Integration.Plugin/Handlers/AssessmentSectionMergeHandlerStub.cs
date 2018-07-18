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
    /// Stub for handling the merge of <see cref="AssessmentSection"/> data.
    /// </summary>
    public class AssessmentSectionMergeHandlerStub : IAssessmentSectionMergeHandler
    {
        public void PerformMerge(AssessmentSection targetAssessmentSection, AssessmentSection sourceAssessmentSection,
                                 IEnumerable<IFailureMechanism> failureMechanismsToMerge)
        {
            for (var i = 0; i < targetAssessmentSection.HydraulicBoundaryDatabase.Locations.Count; i++)
            {
                ReplaceOutput(targetAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i), sourceAssessmentSection.WaterLevelCalculationsForFactorizedSignalingNorm.ElementAt(i));
                ReplaceOutput(targetAssessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(i), sourceAssessmentSection.WaterLevelCalculationsForSignalingNorm.ElementAt(i));
                ReplaceOutput(targetAssessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(i), sourceAssessmentSection.WaterLevelCalculationsForLowerLimitNorm.ElementAt(i));
                ReplaceOutput(targetAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(i), sourceAssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm.ElementAt(i));

                ReplaceOutput(targetAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ElementAt(i), sourceAssessmentSection.WaveHeightCalculationsForFactorizedSignalingNorm.ElementAt(i));
                ReplaceOutput(targetAssessmentSection.WaveHeightCalculationsForSignalingNorm.ElementAt(i), sourceAssessmentSection.WaveHeightCalculationsForSignalingNorm.ElementAt(i));
                ReplaceOutput(targetAssessmentSection.WaveHeightCalculationsForLowerLimitNorm.ElementAt(i), sourceAssessmentSection.WaveHeightCalculationsForLowerLimitNorm.ElementAt(i));
                ReplaceOutput(targetAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(i), sourceAssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm.ElementAt(i));
            }
        }

        private static void ReplaceOutput(HydraulicBoundaryLocationCalculation originalCalculation, HydraulicBoundaryLocationCalculation newCalculation)
        {
            originalCalculation.Output = newCalculation.Output;
            originalCalculation.NotifyObservers();
        }
    }
}