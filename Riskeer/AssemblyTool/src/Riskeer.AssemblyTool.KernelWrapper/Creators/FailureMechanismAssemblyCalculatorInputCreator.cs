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
using System.ComponentModel;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.Categories;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Primitives;
using AssemblyFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances that can be used in the failure mechanism assembly calculator.
    /// </summary>
    internal static class FailureMechanismAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates an <see cref="AssemblyFailureMechanismSectionAssemblyResult"/> based on <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="RiskeerFailureMechanismSectionAssemblyResult"/> to create the
        /// <see cref="AssemblyFailureMechanismSectionAssemblyResult"/> with.</param>
        /// <returns>An <see cref="AssemblyFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static AssemblyFailureMechanismSectionAssemblyResult CreateFailureMechanismSectionAssemblyResult(RiskeerFailureMechanismSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new AssemblyFailureMechanismSectionAssemblyResult(new Probability(result.ProfileProbability),
                                                                     new Probability(result.SectionProbability),
                                                                     CreateInterpretationCategory(result.AssemblyGroup));
        }

        /// <summary>
        /// Converts a <see cref="EInterpretationCategory"/> into a <see cref="FailureMechanismSectionAssemblyGroup"/>.
        /// </summary>
        /// <param name="assemblyGroup">The <see cref="FailureMechanismSectionAssemblyGroup"/> to convert.</param>
        /// <returns>A <see cref="EInterpretationCategory"/> based on <paramref name="assemblyGroup"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="assemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="assemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        private static EInterpretationCategory CreateInterpretationCategory(FailureMechanismSectionAssemblyGroup assemblyGroup)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyGroup), assemblyGroup))
            {
                throw new InvalidEnumArgumentException(nameof(assemblyGroup),
                                                       (int) assemblyGroup,
                                                       typeof(FailureMechanismSectionAssemblyGroup));
            }

            switch (assemblyGroup)
            {
                case FailureMechanismSectionAssemblyGroup.NotDominant:
                    return EInterpretationCategory.NotDominant;
                case FailureMechanismSectionAssemblyGroup.III:
                    return EInterpretationCategory.III;
                case FailureMechanismSectionAssemblyGroup.II:
                    return EInterpretationCategory.II;
                case FailureMechanismSectionAssemblyGroup.I:
                    return EInterpretationCategory.I;
                case FailureMechanismSectionAssemblyGroup.Zero:
                    return EInterpretationCategory.Zero;
                case FailureMechanismSectionAssemblyGroup.IMin:
                    return EInterpretationCategory.IMin;
                case FailureMechanismSectionAssemblyGroup.IIMin:
                    return EInterpretationCategory.IIMin;
                case FailureMechanismSectionAssemblyGroup.IIIMin:
                    return EInterpretationCategory.IIIMin;
                case FailureMechanismSectionAssemblyGroup.Dominant:
                    return EInterpretationCategory.Dominant;
                case FailureMechanismSectionAssemblyGroup.Gr:
                    return EInterpretationCategory.Gr;
                default:
                    throw new NotSupportedException();
            }
        }
        
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