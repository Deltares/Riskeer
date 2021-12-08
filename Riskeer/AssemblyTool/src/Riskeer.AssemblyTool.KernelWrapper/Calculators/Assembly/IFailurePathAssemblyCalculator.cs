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
using Riskeer.AssemblyTool.Data;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing a failure path assembly calculator.
    /// </summary>
    public interface IFailurePathAssemblyCalculator
    {
        /// <summary>
        /// Assembles a failure path based on the input.
        /// </summary>
        /// <param name="failurePathN">The length effect factor 'N' for an entire failure path.</param>
        /// <param name="sectionAssemblyResults">A collection of <see cref="FailureMechanismSectionAssemblyResult"/>.</param>
        /// <returns>A probability representing the assembly result of the failure path.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionAssemblyResults"/> is <c>null</c>.</exception>
        /// <exception cref="FailurePathAssemblyCalculatorException">Thrown when an error occurs while assembling.</exception>
        double Assemble(double failurePathN, IEnumerable<FailureMechanismSectionAssemblyResult> sectionAssemblyResults);
    }
}