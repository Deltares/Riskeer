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

using System.Collections.Generic;
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
        private static readonly Dictionary<EAssemblyErrors, string> errorTranslations = new Dictionary<EAssemblyErrors, string>
        {
            {EAssemblyErrors.SignallingLimitOutOfRange, Resources.AssemblyErrorMessageTranslator_SignallingLimitOutOfRange},
            {EAssemblyErrors.LowerLimitOutOfRange, Resources.AssemblyErrorMessageTranslator_LowerLimitOutOfRange},
            {EAssemblyErrors.FailurePropbabilityMarginOutOfRange, Resources.AssemblyErrorMessageTranslator_FailurePropbabilityMarginOutOfRange},
            {EAssemblyErrors.LengthEffectFactorOutOfRange, Resources.AssemblyErrorMessageTranslator_LengthEffectFactorOutOfRange},
            {EAssemblyErrors.SectionLengthOutOfRange, Resources.AssemblyErrorMessageTranslator_SectionLengthOutOfRange},
            {EAssemblyErrors.SignallingLimitAboveLowerLimit, Resources.AssemblyErrorMessageTranslator_SignallingLimitAboveLowerLimit},
            {EAssemblyErrors.PsigDsnAbovePsig, Resources.AssemblyErrorMessageTranslator_PsigDsnAbovePsig},
            {EAssemblyErrors.PlowDsnAbovePlow, Resources.AssemblyErrorMessageTranslator_PlowDsnAbovePlow},
            {EAssemblyErrors.LowerLimitIsAboveUpperLimit, Resources.AssemblyErrorMessageTranslator_LowerLimitIsAboveUpperLimit},
            {EAssemblyErrors.CategoryLowerLimitOutOfRange, Resources.AssemblyErrorMessageTranslator_CategoryLowerLimitOutOfRange},
            {EAssemblyErrors.CategoryUpperLimitOutOfRange, Resources.AssemblyErrorMessageTranslator_CategoryUpperLimitOutOfRange},
            {EAssemblyErrors.TranslateAssessmentInvalidInput, Resources.AssemblyErrorMessageTranslator_TranslateAssessmentInvalidInput},
            {EAssemblyErrors.ValueMayNotBeNull, Resources.AssemblyErrorMessageTranslator_ValueMayNotBeNull},
            {EAssemblyErrors.CategoryNotAllowed, Resources.AssemblyErrorMessageTranslator_CategoryNotAllowed},
            {EAssemblyErrors.DoesNotComplyAfterComply, Resources.AssemblyErrorMessageTranslator_DoesNotComplyAfterComply},
            {EAssemblyErrors.FmSectionLengthInvalid, Resources.AssemblyErrorMessageTranslator_FmSectionLengthInvalid},
            {EAssemblyErrors.FmSectionSectionStartEndInvalid, Resources.AssemblyErrorMessageTranslator_FmSectionSectionStartEndInvalid},
            {EAssemblyErrors.FailureProbabilityOutOfRange, Resources.AssemblyErrorMessageTranslator_FailureProbabilityOutOfRange},
            {EAssemblyErrors.InputNotTheSameType, Resources.AssemblyErrorMessageTranslator_InputNotTheSameType},
            {EAssemblyErrors.FailureMechanismAssemblerInputInvalid, Resources.AssemblyErrorMessageTranslator_FailureMechanismAssemblerInputInvalid},
            {EAssemblyErrors.CommonFailureMechanismSectionsInvalid, Resources.AssemblyErrorMessageTranslator_CommonFailureMechanismSectionsInvalid},
            {EAssemblyErrors.CommonFailureMechanismSectionsNotConsecutive, Resources.AssemblyErrorMessageTranslator_CommonFailureMechanismSectionsNotConsecutive},
            {EAssemblyErrors.RequestedPointOutOfRange, Resources.AssemblyErrorMessageTranslator_RequestedPointOutOfRange},
            {EAssemblyErrors.FailureMechanismDuplicateSection, Resources.AssemblyErrorMessageTranslator_FailureMechanismDuplicateSection}
        };

        /// <summary>
        /// Creates a localized error message based on the contents of <paramref name="errorMessages"/>.
        /// </summary>
        /// <param name="errorMessages">The collection of <see cref="AssemblyErrorMessage"/> to localize.</param>
        /// <returns>A localized string containing the error message(s).</returns>
        public static string CreateErrorMessage(IEnumerable<AssemblyErrorMessage> errorMessages)
        {
            return errorMessages.Count() == 1 
                       ? errorTranslations[errorMessages.Single().ErrorCode] 
                       : errorMessages.Aggregate(string.Empty, (current, message) => current + "- " + errorTranslations[message.ErrorCode] + "\n");
        }
    }
}