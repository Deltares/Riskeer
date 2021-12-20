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
using Riskeer.Integration.Data.StandAlone.Input;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Read.MacroStabilityOutwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="MacroStabilityOutwardsProbabilityAssessmentInput"/> 
    /// based on the <see cref="MacroStabilityOutwardsFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class MacroStabilityOutwardsFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="MacroStabilityOutwardsFailureMechanismMetaEntity"/> and use the information to
        /// update the <paramref name="probabilityAssessmentInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="MacroStabilityOutwardsFailureMechanismMetaEntity"/> to use to
        /// update the <paramref name="probabilityAssessmentInput"/>.</param>
        /// <param name="probabilityAssessmentInput">The <see cref="MacroStabilityOutwardsProbabilityAssessmentInput"/> to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void ReadProbabilityAssessmentInput(this MacroStabilityOutwardsFailureMechanismMetaEntity entity,
                                                            MacroStabilityOutwardsProbabilityAssessmentInput probabilityAssessmentInput)
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
            probabilityAssessmentInput.ApplyLengthEffectInSection = Convert.ToBoolean(entity.ApplyLengthEffectInSection);
        }
    }
}