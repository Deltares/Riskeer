﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.IO.FileImporters;
using Riskeer.DuneErosion.Data;

namespace Riskeer.DuneErosion.Plugin.FileImporters
{
    /// <summary>
    /// An update strategy that can be used to update a <see cref="DuneErosionFailureMechanismSectionResultOld"/> instance with data
    /// from an old <see cref="DuneErosionFailureMechanismSectionResultOld"/> instance.
    /// </summary>
    public class DuneErosionFailureMechanismSectionResultUpdateStrategy
        : IFailureMechanismSectionResultUpdateStrategy<DuneErosionFailureMechanismSectionResultOld, AdoptableFailureMechanismSectionResult>
    {
        public void UpdateSectionResultOld(DuneErosionFailureMechanismSectionResultOld origin, DuneErosionFailureMechanismSectionResultOld target)
        {
            if (origin == null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.SimpleAssessmentResult = origin.SimpleAssessmentResult;
            target.DetailedAssessmentResultForFactorizedSignalingNorm = origin.DetailedAssessmentResultForFactorizedSignalingNorm;
            target.DetailedAssessmentResultForSignalingNorm = origin.DetailedAssessmentResultForSignalingNorm;
            target.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = origin.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm;
            target.DetailedAssessmentResultForLowerLimitNorm = origin.DetailedAssessmentResultForLowerLimitNorm;
            target.DetailedAssessmentResultForFactorizedLowerLimitNorm = origin.DetailedAssessmentResultForFactorizedLowerLimitNorm;
            target.TailorMadeAssessmentResult = origin.TailorMadeAssessmentResult;
            target.UseManualAssembly = origin.UseManualAssembly;
            target.ManualAssemblyCategoryGroup = origin.ManualAssemblyCategoryGroup;
        }

        public void UpdateSectionResult(AdoptableFailureMechanismSectionResult origin, AdoptableFailureMechanismSectionResult target)
        {
            if (origin == null)
            {
                throw new ArgumentNullException(nameof(origin));
            }

            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            target.IsRelevant = origin.IsRelevant;
            target.InitialFailureMechanismResult = origin.InitialFailureMechanismResult;
            target.ManualInitialFailureMechanismResultSectionProbability = origin.ManualInitialFailureMechanismResultSectionProbability;
            target.FurtherAnalysisNeeded = origin.FurtherAnalysisNeeded;
            target.RefinedSectionProbability = origin.RefinedSectionProbability;
        }
    }
}