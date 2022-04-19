﻿// Copyright (C) Stichting Deltares 2022. All rights reserved.
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

namespace Riskeer.Common.Plugin.FileImporters
{
    /// <summary>
    /// An update strategy that can be used to update a <see cref="NonAdoptableWithProfileProbabilityFailureMechanismSectionResult"/>
    /// instance with data from an old instance.
    /// </summary>
    public class NonAdoptableWithProfileProbabilityFailureMechanismSectionResultUpdateStrategy
        : IFailureMechanismSectionResultUpdateStrategy<NonAdoptableWithProfileProbabilityFailureMechanismSectionResult>
    {
        public void UpdateSectionResult(NonAdoptableWithProfileProbabilityFailureMechanismSectionResult origin, NonAdoptableWithProfileProbabilityFailureMechanismSectionResult target)
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
            target.InitialFailureMechanismResultType = origin.InitialFailureMechanismResultType;
            target.ManualInitialFailureMechanismResultSectionProbability = origin.ManualInitialFailureMechanismResultSectionProbability;
            target.ManualInitialFailureMechanismResultProfileProbability = origin.ManualInitialFailureMechanismResultProfileProbability;
            target.FurtherAnalysisType = origin.FurtherAnalysisType;
            target.RefinedSectionProbability = origin.RefinedSectionProbability;
            target.RefinedProfileProbability = origin.RefinedProfileProbability;
        }
    }
}