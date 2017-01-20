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
using Core.Common.Base.Data;
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
        /// Read the <see cref="PipingFailureMechanismMetaEntity"/> and use the information to set 
        /// <see cref="PipingProbabilityAssessmentInput.A"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismMetaEntity"/> to obtain value for A from.</param>
        /// <param name="input">The <see cref="PipingProbabilityAssessmentInput"/> to set the 
        /// <see cref="PipingProbabilityAssessmentInput.A"/> for.</param>
        internal static void ReadProbabilityAssessmentInput(this PipingFailureMechanismMetaEntity entity, PipingProbabilityAssessmentInput input)
        {
            input.A = entity.A;
        }

        /// <summary>
        /// Read the <see cref="PipingFailureMechanismMetaEntity"/> and use the information to set
        /// <see cref="GeneralPipingInput.WaterVolumetricWeight"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismMetaEntity"/> to obtain value for
        /// WaterVolumetricWeight from.</param>
        /// <param name="input">The <see cref="GeneralPipingInput"/> to set the 
        /// <see cref="GeneralPipingInput.WaterVolumetricWeight"/> for.</param>
        internal static void ReadGeneralPipingInput(this PipingFailureMechanismMetaEntity entity, GeneralPipingInput input)
        {
            input.WaterVolumetricWeight = (RoundedDouble) entity.WaterVolumetricWeight;
        }
    }
}