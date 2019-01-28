// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.StabilityStoneCover.Data;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Create.StabilityStoneCover
{
    /// <summary>
    /// Extension methods for <see cref="StabilityStoneCoverFailureMechanismSectionResult"/> related to creating a 
    /// <see cref="StabilityStoneCoverSectionResultEntity"/>.
    /// </summary>
    internal static class StabilityStoneCoverFailureMechanismSectionResultCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StabilityStoneCoverSectionResultEntity"/> based on the information of the <see cref="StabilityStoneCoverFailureMechanismSectionResult"/>.
        /// </summary>
        /// <param name="result">The result to create a database entity for.</param>
        /// <returns>A new <see cref="StabilityStoneCoverSectionResultEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        internal static StabilityStoneCoverSectionResultEntity Create(this StabilityStoneCoverFailureMechanismSectionResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new StabilityStoneCoverSectionResultEntity
            {
                SimpleAssessmentResult = Convert.ToByte(result.SimpleAssessmentResult),
                DetailedAssessmentResultForFactorizedSignalingNorm = Convert.ToByte(result.DetailedAssessmentResultForFactorizedSignalingNorm),
                DetailedAssessmentResultForSignalingNorm = Convert.ToByte(result.DetailedAssessmentResultForSignalingNorm),
                DetailedAssessmentResultForMechanismSpecificLowerLimitNorm = Convert.ToByte(result.DetailedAssessmentResultForMechanismSpecificLowerLimitNorm),
                DetailedAssessmentResultForLowerLimitNorm = Convert.ToByte(result.DetailedAssessmentResultForLowerLimitNorm),
                DetailedAssessmentResultForFactorizedLowerLimitNorm = Convert.ToByte(result.DetailedAssessmentResultForFactorizedLowerLimitNorm),
                TailorMadeAssessmentResult = Convert.ToByte(result.TailorMadeAssessmentResult),
                UseManualAssembly = Convert.ToByte(result.UseManualAssembly),
                ManualAssemblyCategoryGroup = Convert.ToByte(result.ManualAssemblyCategoryGroup)
            };
        }
    }
}