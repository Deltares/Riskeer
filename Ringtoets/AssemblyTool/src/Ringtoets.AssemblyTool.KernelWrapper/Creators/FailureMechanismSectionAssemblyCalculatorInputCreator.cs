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
using AssemblyTool.Kernel.Assembly;
using AssemblyTool.Kernel.Data.CalculationResults;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances that can be used in <see cref="IFailureMechanismSectionAssemblyCalculator"/>.
    /// </summary>
    public static class FailureMechanismSectionAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates <see cref="SimpleCalculationResult"/> based on the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultType"/> to create the result for.</param>
        /// <returns>The created <see cref="SimpleCalculationResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is a valid value, but unsupported.</exception>
        public static SimpleCalculationResult CreateSimpleCalculationResult(SimpleAssessmentResultType input)
        {
            if (!Enum.IsDefined(typeof(SimpleAssessmentResultType), input))
            {
                throw new InvalidEnumArgumentException(nameof(input),
                                                       (int) input,
                                                       typeof(SimpleAssessmentResultType));
            }

            switch (input)
            {
                case SimpleAssessmentResultType.NotApplicable:
                    return SimpleCalculationResult.NVT;
                case SimpleAssessmentResultType.ProbabilityNegligible:
                    return SimpleCalculationResult.FV;
                case SimpleAssessmentResultType.AssessFurther:
                    return SimpleCalculationResult.VB;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates <see cref="SimpleCalculationResultValidityOnly"/> based on the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultValidityOnlyType"/> to create the result for.</param>
        /// <returns>The created <see cref="SimpleCalculationResultValidityOnly"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SimpleAssessmentResultValidityOnlyType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SimpleAssessmentResultValidityOnlyType"/>
        /// is a valid value, but unsupported.</exception>
        public static SimpleCalculationResultValidityOnly CreateSimplecalclCalculationResultValidityOnly(SimpleAssessmentResultValidityOnlyType input)
        {
            if (!Enum.IsDefined(typeof(SimpleAssessmentResultValidityOnlyType), input))
            {
                throw new InvalidEnumArgumentException(nameof(input),
                                                       (int) input,
                                                       typeof(SimpleAssessmentResultValidityOnlyType));
            }

            switch (input)
            {
                case SimpleAssessmentResultValidityOnlyType.NotApplicable:
                    return SimpleCalculationResultValidityOnly.NVT;
                case SimpleAssessmentResultValidityOnlyType.Applicable:
                    return SimpleCalculationResultValidityOnly.WVT;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}