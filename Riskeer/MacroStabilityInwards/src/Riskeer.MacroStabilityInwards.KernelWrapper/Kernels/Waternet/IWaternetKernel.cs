﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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

using System.Collections.Generic;
using Deltares.MacroStability.CSharpWrapper.Output;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using CSharpWrapperWaternet = Deltares.MacroStability.CSharpWrapper.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet
{
    /// <summary>
    /// Interface representing Waternet kernel input, methods and output.
    /// </summary>
    /// <remarks>
    /// This interface is introduced for being able to test the conversion of:
    /// <list type="bullet">
    /// <item><see cref="WaternetCalculator"/> input into kernel input;</item>
    /// <item>kernel output into <see cref="WaternetCalculator"/> output.</item>
    /// </list>
    /// </remarks>
    public interface IWaternetKernel
    {
        /// <summary>
        /// Gets the Waternet result.
        /// </summary>
        CSharpWrapperWaternet Waternet { get; }

        /// <summary>
        /// Performs the Waternet calculation.
        /// </summary>
        /// <exception cref="WaternetKernelWrapperException">Thrown when
        /// an error occurs when performing the calculation.</exception>
        void Calculate();

        /// <summary>
        /// Validates the input for the Waternet calculation.
        /// </summary>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="Message"/> objects.</returns>
        /// <exception cref="WaternetKernelWrapperException">Thrown when 
        /// an error occurs when performing the validation.</exception>
        IEnumerable<Message> Validate();
    }
}