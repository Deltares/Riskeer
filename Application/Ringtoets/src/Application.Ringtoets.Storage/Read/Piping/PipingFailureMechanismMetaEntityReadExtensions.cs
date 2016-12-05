// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Piping.Data;

namespace Application.Ringtoets.Storage.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingProbabilityAssessmentInput"/> based on the
    /// <see cref="PipingFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class PipingFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="PipingFailureMechanismMetaEntity"/> and use the information to construct a <see cref="PipingProbabilityAssessmentInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismMetaEntity"/> to create <see cref="PipingProbabilityAssessmentInput"/> for.</param>
        /// <returns>A new <see cref="PipingProbabilityAssessmentInput"/>.</returns>
        internal static PipingProbabilityAssessmentInput ReadPipingProbabilityAssessmentInput(this PipingFailureMechanismMetaEntity entity)
        {
            return new PipingProbabilityAssessmentInput
            {
                A = entity.A,
            };
        }

        /// <summary>
        /// Read the <see cref="PipingFailureMechanismMetaEntity"/> and use the information to construct a <see cref="GeneralPipingInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismMetaEntity"/> to create <see cref="GeneralPipingInput"/> for.</param>
        /// <returns>A new <see cref="GeneralPipingInput"/>.</returns>
        internal static GeneralPipingInput ReadGeneralPipingInput(this PipingFailureMechanismMetaEntity entity)
        {
            return new GeneralPipingInput
            {
                WaterVolumetricWeight = entity.WaterVolumetricWeight
            };
        }
    }
}