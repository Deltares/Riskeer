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
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.MacroStabilityInwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityInwardsProbabilityAssessmentInput"/> 
    /// based on the <see cref="MacroStabilityInwardsFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class MacroStabilityInwardsFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="MacroStabilityInwardsFailureMechanismMetaEntity"/> and use the information to
        /// update the <paramref name="probabilityAssessmentInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityInwardsFailureMechanismMetaEntity"/> to use to
        /// update the <paramref name="probabilityAssessmentInput"/>.</param>
        /// <param name="probabilityAssessmentInput">The <see cref="MacroStabilityInwardsProbabilityAssessmentInput"/> to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void ReadProbabilityAssessmentInput(this MacroStabilityInwardsFailureMechanismMetaEntity entity,
                                                            MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (probabilityAssessmentInput == null)
            {
                throw new ArgumentNullException(nameof(probabilityAssessmentInput));
            }

            probabilityAssessmentInput.A = entity.A;
        }
    }
}