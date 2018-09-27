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
using Core.Common.Base.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Storage.Core.DbContext;

namespace Ringtoets.Storage.Core.Read.Piping
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="PipingProbabilityAssessmentInput"/>
    /// and a <see cref="GeneralPipingInput"/> based on the <see cref="PipingFailureMechanismMetaEntity"/>.
    /// </summary>
    internal static class PipingFailureMechanismMetaEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="PipingFailureMechanismMetaEntity"/> and use the information to update the
        /// <paramref name="probabilityAssessmentInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismMetaEntity"/> to use to update the
        /// <paramref name="probabilityAssessmentInput"/>.</param>
        /// <param name="probabilityAssessmentInput">The <see cref="PipingProbabilityAssessmentInput"/> to be
        /// updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void ReadProbabilityAssessmentInput(this PipingFailureMechanismMetaEntity entity,
                                                            PipingProbabilityAssessmentInput probabilityAssessmentInput)
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

        /// <summary>
        /// Read the <see cref="PipingFailureMechanismMetaEntity"/> and use the information to update the
        /// <paramref name="generalPipingInput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="PipingFailureMechanismMetaEntity"/> to use to update the
        /// <paramref name="generalPipingInput"/>.</param>
        /// <param name="generalPipingInput">The <see cref="GeneralPipingInput"/> to be updated.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        internal static void ReadGeneralPipingInput(this PipingFailureMechanismMetaEntity entity,
                                                    GeneralPipingInput generalPipingInput)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (generalPipingInput == null)
            {
                throw new ArgumentNullException(nameof(generalPipingInput));
            }

            generalPipingInput.WaterVolumetricWeight = (RoundedDouble) entity.WaterVolumetricWeight;
        }
    }
}