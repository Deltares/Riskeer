// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.ComponentModel;
using Assembly.Kernel.Model;
using Riskeer.Common.Primitives;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input that can be used in the failure mechanism section assembly calculator.
    /// </summary>
    public static class FailureMechanismSectionAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Converts a <see cref="FailureMechanismSectionResultFurtherAnalysisType"/> into an
        /// <see cref="ERefinementStatus"/>.
        /// </summary>
        /// <param name="furtherAnalysisType">The <see cref="FailureMechanismSectionResultFurtherAnalysisType"/> to convert.</param>
        /// <returns>The converted <see cref="ERefinementStatus"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="furtherAnalysisType"/> is invalid.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="furtherAnalysisType"/>
        /// is valid but not supported.</exception>
        public static ERefinementStatus ConvertFailureMechanismSectionResultFurtherAnalysisType(FailureMechanismSectionResultFurtherAnalysisType furtherAnalysisType)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionResultFurtherAnalysisType), furtherAnalysisType))
            {
                throw new InvalidEnumArgumentException(nameof(furtherAnalysisType),
                                                       (int) furtherAnalysisType,
                                                       typeof(FailureMechanismSectionResultFurtherAnalysisType));
            }

            switch (furtherAnalysisType)
            {
                case FailureMechanismSectionResultFurtherAnalysisType.NotNecessary:
                    return ERefinementStatus.NotNecessary;
                case FailureMechanismSectionResultFurtherAnalysisType.Necessary:
                    return ERefinementStatus.Necessary;
                case FailureMechanismSectionResultFurtherAnalysisType.Executed:
                    return ERefinementStatus.Performed;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}