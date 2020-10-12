﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Riskeer.Common.Data.Calculation;

namespace Riskeer.Piping.Data
{
    /// <summary>
    /// Interface that represents information about a calculation for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    /// <typeparam name="TPipingInput">The type of calculation input.</typeparam>
    /// <typeparam name="TPipingOutput">The type of calculation output.</typeparam>
    public interface IPipingCalculation<out TPipingInput, out TPipingOutput> : ICalculation<TPipingInput>
        where TPipingInput : PipingInput
        where TPipingOutput : PipingOutput
    {
        /// <summary>
        /// Gets the results of the piping calculation.
        /// </summary>
        TPipingOutput Output { get; }
    }
}