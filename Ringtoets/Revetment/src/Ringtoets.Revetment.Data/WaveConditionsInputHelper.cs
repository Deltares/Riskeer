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

using System;
using System.ComponentModel;
using Core.Common.Base.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Revetment.Data
{
    /// <summary>
    /// Class containing helper methods related to <see cref="WaveConditionsInput"/>.
    /// </summary>
    public static class WaveConditionsInputHelper
    {
        private const double assessmentLevelSubstraction = 0.01;

        /// <summary>
        /// Gets an upper boundary based on the provided assessment level (that can be used while
        /// determining water levels for wave condition calculations).
        /// </summary>
        /// <param name="assessmentLevel">The assessment level at stake.</param>
        /// <returns>The corresponding assessment level upper boundary.</returns>
        public static RoundedDouble GetUpperBoundaryAssessmentLevel(RoundedDouble assessmentLevel)
        {
            return new RoundedDouble(2, assessmentLevel - assessmentLevelSubstraction);
        }

        /// <summary>
        /// Sets the <see cref="AssessmentSectionCategoryType"/> of the <paramref name="waveConditionsInput"/>
        /// based on the <see cref="NormType"/>.
        /// </summary>
        /// <param name="waveConditionsInput">The <see cref="AssessmentSectionCategoryWaveConditionsInput"/>
        /// to set the category type for.</param>
        /// <param name="normType">The <see cref="NormType"/> to set the <paramref name="waveConditionsInput"/> for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
        /// but unsupported.</exception>
        public static void SetCategoryType(AssessmentSectionCategoryWaveConditionsInput waveConditionsInput,
                                           NormType normType)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (!Enum.IsDefined(typeof(NormType), normType))
            {
                throw new InvalidEnumArgumentException(nameof(normType),
                                                       (int) normType,
                                                       typeof(NormType));
            }

            switch (normType)
            {
                case NormType.LowerLimit:
                    waveConditionsInput.CategoryType = AssessmentSectionCategoryType.LowerLimitNorm;
                    break;
                case NormType.Signaling:
                    waveConditionsInput.CategoryType = AssessmentSectionCategoryType.SignalingNorm;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Sets the <see cref="FailureMechanismCategoryType"/> of the <paramref name="waveConditionsInput"/>
        /// based on the <see cref="NormType"/>.
        /// </summary>
        /// <param name="waveConditionsInput">The <see cref="FailureMechanismCategoryWaveConditionsInput"/>
        /// to set the category type for.</param>
        /// <param name="normType">The <see cref="NormType"/> to set the <paramref name="waveConditionsInput"/> for.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="waveConditionsInput"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="normType"/> is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="normType"/> is a valid value,
        /// but unsupported.</exception>
        public static void SetCategoryType(FailureMechanismCategoryWaveConditionsInput waveConditionsInput,
                                           NormType normType)
        {
            if (waveConditionsInput == null)
            {
                throw new ArgumentNullException(nameof(waveConditionsInput));
            }

            if (!Enum.IsDefined(typeof(NormType), normType))
            {
                throw new InvalidEnumArgumentException(nameof(normType),
                                                       (int) normType,
                                                       typeof(NormType));
            }

            switch (normType)
            {
                case NormType.LowerLimit:
                    waveConditionsInput.CategoryType = FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm;
                    break;
                case NormType.Signaling:
                    waveConditionsInput.CategoryType = FailureMechanismCategoryType.MechanismSpecificSignalingNorm;
                    break;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}