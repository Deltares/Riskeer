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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Assembly.Kernel.Exceptions;
using Ringtoets.AssemblyTool.KernelWrapper.Properties;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators
{
    /// <summary>
    /// Class that can translate collections of <see cref="AssemblyErrorMessage"/> to
    /// localized error messages.
    /// </summary>
    public static class AssemblyErrorMessageTranslator
    {
        /// <summary>
        /// Creates a localized error message based on the contents of <paramref name="errorMessages"/>.
        /// </summary>
        /// <param name="errorMessages">The collection of <see cref="AssemblyErrorMessage"/> to localize.</param>
        /// <returns>A localized string containing the error message(s).</returns>
        public static string CreateErrorMessage(IEnumerable<AssemblyErrorMessage> errorMessages)
        {
            return errorMessages.Count() == 1
                       ? GetErrorMessage(errorMessages.Single().ErrorCode)
                       : errorMessages.Aggregate(string.Empty, (current, message) => current + "- " + GetErrorMessage(message.ErrorCode) + "\n");
        }

        /// <summary>
        /// Creates a generic error message for when an unexpected exception has been thrown.
        /// </summary>
        /// <returns>A generic error message.</returns>
        public static string CreateGenericErrorMessage()
        {
            return Resources.AssemblyErrorMessageTranslator_GenericErrorMessage;
        }

        private static string GetErrorMessage(EAssemblyErrors assemblyError)
        {
            if (!Enum.IsDefined(typeof(EAssemblyErrors), assemblyError))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyError), (int) assemblyError, typeof(EAssemblyErrors));
            }

            switch (assemblyError)
            {
                case EAssemblyErrors.SignallingLimitOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_SignallingLimitOutOfRange;
                case EAssemblyErrors.LowerLimitOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_LowerLimitOutOfRange;
                case EAssemblyErrors.FailurePropbabilityMarginOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_FailurePropbabilityMarginOutOfRange;
                case EAssemblyErrors.LengthEffectFactorOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_LengthEffectFactorOutOfRange;
                case EAssemblyErrors.SectionLengthOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_SectionLengthOutOfRange;
                case EAssemblyErrors.SignallingLimitAboveLowerLimit:
                    return Resources.AssemblyErrorMessageTranslator_SignallingLimitAboveLowerLimit;
                case EAssemblyErrors.PsigDsnAbovePsig:
                    return Resources.AssemblyErrorMessageTranslator_PsigDsnAbovePsig;
                case EAssemblyErrors.PlowDsnAbovePlow:
                    return Resources.AssemblyErrorMessageTranslator_PlowDsnAbovePlow;
                case EAssemblyErrors.LowerLimitIsAboveUpperLimit:
                    return Resources.AssemblyErrorMessageTranslator_LowerLimitIsAboveUpperLimit;
                case EAssemblyErrors.CategoryLowerLimitOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_CategoryLowerLimitOutOfRange;
                case EAssemblyErrors.CategoryUpperLimitOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_CategoryUpperLimitOutOfRange;
                case EAssemblyErrors.TranslateAssessmentInvalidInput:
                    return Resources.AssemblyErrorMessageTranslator_TranslateAssessmentInvalidInput;
                case EAssemblyErrors.ValueMayNotBeNull:
                    return Resources.AssemblyErrorMessageTranslator_ValueMayNotBeNull;
                case EAssemblyErrors.CategoryNotAllowed:
                    return Resources.AssemblyErrorMessageTranslator_CategoryNotAllowed;
                case EAssemblyErrors.DoesNotComplyAfterComply:
                    return Resources.AssemblyErrorMessageTranslator_DoesNotComplyAfterComply;
                case EAssemblyErrors.FmSectionLengthInvalid:
                    return Resources.AssemblyErrorMessageTranslator_FmSectionLengthInvalid;
                case EAssemblyErrors.FmSectionSectionStartEndInvalid:
                    return Resources.AssemblyErrorMessageTranslator_FmSectionSectionStartEndInvalid;
                case EAssemblyErrors.FailureProbabilityOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_FailureProbabilityOutOfRange;
                case EAssemblyErrors.InputNotTheSameType:
                    return Resources.AssemblyErrorMessageTranslator_InputNotTheSameType;
                case EAssemblyErrors.FailureMechanismAssemblerInputInvalid:
                    return Resources.AssemblyErrorMessageTranslator_NoSectionsImported;
                case EAssemblyErrors.CommonFailureMechanismSectionsInvalid:
                    return Resources.AssemblyErrorMessageTranslator_NoSectionsImported;
                case EAssemblyErrors.CommonFailureMechanismSectionsNotConsecutive:
                    return Resources.AssemblyErrorMessageTranslator_CommonFailureMechanismSectionsNotConsecutive;
                case EAssemblyErrors.RequestedPointOutOfRange:
                    return Resources.AssemblyErrorMessageTranslator_RequestedPointOutOfRange;
                case EAssemblyErrors.FailureMechanismDuplicateSection:
                    return Resources.AssemblyErrorMessageTranslator_FailureMechanismDuplicateSection;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}