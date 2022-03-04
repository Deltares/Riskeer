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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Riskeer.AssemblyTool.KernelWrapper.Properties;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Class that creates to localized error messages for the assembly tool.
    /// </summary>
    public static class AssemblyErrorMessageCreator
    {
        /// <summary>
        /// Creates a localized string based on the contents of <paramref name="errorMessages"/>.
        /// </summary>
        /// <param name="errorMessages">The collection of <see cref="AssemblyErrorMessage"/> to localize.</param>
        /// <returns>A localized string containing the error messages.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="errorMessages"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="errorMessages"/> contains an
        /// invalid value of <see cref="EAssemblyErrors"/>.</exception>
        public static string CreateErrorMessage(IEnumerable<AssemblyErrorMessage> errorMessages)
        {
            if (errorMessages == null)
            {
                throw new ArgumentNullException(nameof(errorMessages));
            }

            return errorMessages.Count() == 1
                       ? GetErrorMessage(errorMessages.Single().ErrorCode)
                       : errorMessages.Aggregate(string.Empty, (current, message) => current + "- " + GetErrorMessage(message.ErrorCode) + "\n");
        }

        /// <summary>
        /// Creates a generic error message.
        /// </summary>
        /// <returns>A generic error message.</returns>
        public static string CreateGenericErrorMessage()
        {
            return Resources.AssemblyErrorMessageCreator_GenericErrorMessage;
        }

        /// <summary>
        /// Gets the localized error message that belongs to the given <see cref="EAssemblyErrors"/>.
        /// </summary>
        /// <param name="assemblyError">The <see cref="EAssemblyErrors"/> to localize.</param>
        /// <returns>A localized string.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyError"/> 
        /// is an invalid value of <see cref="EAssemblyErrors"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyError"/> is a valid value,
        /// but unsupported.</exception>
        private static string GetErrorMessage(EAssemblyErrors assemblyError)
        {
            if (!Enum.IsDefined(typeof(EAssemblyErrors), assemblyError))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyError), (int) assemblyError, typeof(EAssemblyErrors));
            }

            switch (assemblyError)
            {
                case EAssemblyErrors.LengthEffectFactorOutOfRange:
                    return Resources.AssemblyErrorMessageCreator_LengthEffectFactorOutOfRange;
                case EAssemblyErrors.SectionLengthOutOfRange:
                    return Resources.AssemblyErrorMessageCreator_SectionLengthOutOfRange;
                case EAssemblyErrors.SignalingLimitAboveLowerLimit:
                    return Resources.AssemblyErrorMessageCreator_SignalingLimitAboveLowerLimit;
                case EAssemblyErrors.LowerLimitIsAboveUpperLimit:
                    return Resources.AssemblyErrorMessageCreator_LowerLimitIsAboveUpperLimit;
                case EAssemblyErrors.ValueMayNotBeNull:
                case EAssemblyErrors.ValueMayNotBeNaN:
                case EAssemblyErrors.NonMatchingProbabilityValues:
                    return Resources.AssemblyErrorMessageCreator_InvalidInput;
                case EAssemblyErrors.FailureMechanismSectionLengthInvalid:
                    return Resources.AssemblyErrorMessageCreator_FailureMechanismSectionLengthInvalid;
                case EAssemblyErrors.FailureMechanismSectionSectionStartEndInvalid:
                    return Resources.AssemblyErrorMessageCreator_FailureMechanismSectionSectionStartEndInvalid;
                case EAssemblyErrors.FailureProbabilityOutOfRange:
                    return Resources.AssemblyErrorMessageCreator_FailureProbabilityOutOfRange;
                case EAssemblyErrors.InputNotTheSameType:
                    return Resources.AssemblyErrorMessageCreator_InputNotTheSameType;
                case EAssemblyErrors.CommonFailureMechanismSectionsInvalid:
                    return Resources.AssemblyErrorMessageCreator_CommonFailureMechanismSectionsInvalid;
                case EAssemblyErrors.CommonFailureMechanismSectionsNotConsecutive:
                    return Resources.AssemblyErrorMessageCreator_CommonFailureMechanismSectionsNotConsecutive;
                case EAssemblyErrors.RequestedPointOutOfRange:
                    return Resources.AssemblyErrorMessageCreator_RequestedPointOutOfRange;
                case EAssemblyErrors.SectionsWithoutCategory:
                    return Resources.AssemblyErrorMessageCreator_SectionsWithoutCategory;
                case EAssemblyErrors.InvalidCategoryLimits:
                    return Resources.AssemblyErrorMessageCreator_GetErrorMessage_InvalidCategoryLimits;
                case EAssemblyErrors.EmptyResultsList:
                    return Resources.AssemblyErrorMessageCreator_EmptyResultsList;
                case EAssemblyErrors.ProfileProbabilityGreaterThanSectionProbability:
                    return Resources.AssemblyErrorMessageCreator_ProfileProbabilityGreaterThanSectionProbability;
                case EAssemblyErrors.ErrorConstructingErrorMessage:
                    return Resources.AssemblyErrorMessageCreator_ErrorConstructingErrorMessage;
                case EAssemblyErrors.DominantSectionCannotBeAssembled:
                    return Resources.AssemblyErrorMessageCreator_Cannot_assemble_with_dominant_category;
                case EAssemblyErrors.EncounteredOneOrMoreSectionsWithoutResult:
                    return Resources.AssemblyErrorMessageCreator_All_sections_must_have_a_result;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}