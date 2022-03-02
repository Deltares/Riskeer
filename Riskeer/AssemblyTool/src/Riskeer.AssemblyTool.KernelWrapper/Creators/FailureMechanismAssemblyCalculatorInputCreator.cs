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
using Riskeer.AssemblyTool.Data;
using KernelFailureMechanismSectionAssemblyResult = Assembly.Kernel.Model.FailureMechanismSections.FailureMechanismSectionAssemblyResult;
using RiskeerFailureMechanismSectionAssemblyResult = Riskeer.AssemblyTool.Data.FailureMechanismSectionAssemblyResult;

namespace Riskeer.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input that can be used in the failure mechanism assembly calculator.
    /// </summary>
    internal static class FailureMechanismAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates an <see cref="KernelFailureMechanismSectionAssemblyResult"/> based on <paramref name="result"/>.
        /// </summary>
        /// <param name="result">The <see cref="RiskeerFailureMechanismSectionAssemblyResult"/> to create the
        /// <see cref="KernelFailureMechanismSectionAssemblyResult"/> with.</param>
        /// <returns>A <see cref="KernelFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="result"/> is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="FailureMechanismSectionAssemblyGroup"/>
        /// is a valid value, but unsupported.</exception>
        public static KernelFailureMechanismSectionAssemblyResult CreateFailureMechanismSectionAssemblyResult(RiskeerFailureMechanismSectionAssemblyResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            return new KernelFailureMechanismSectionAssemblyResult(
                AssemblyCalculatorInputCreator.CreateProbability(result.ProfileProbability),
                AssemblyCalculatorInputCreator.CreateProbability(result.SectionProbability),
                FailureMechanismSectionAssemblyGroupConverter.ConvertFrom(result.FailureMechanismSectionAssemblyGroup));
        }
    }
}