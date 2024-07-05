﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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

namespace Riskeer.HydraRing.Calculation.Data.Defaults
{
    /// <summary>
    /// Container for variable defaults.
    /// </summary>
    internal class VariableDefaults
    {
        /// <summary>
        /// Creates a new instance of the <see cref="VariableDefaults"/> class.
        /// </summary>
        /// <param name="correlationLength">The correlation length.</param>
        public VariableDefaults(double correlationLength)
        {
            CorrelationLength = correlationLength;
        }

        /// <summary>
        /// Gets the correlation length.
        /// </summary>
        /// <remarks>When this property equals <see cref="double.NaN"/>, the length of the section should be used.</remarks>
        public double CorrelationLength { get; }
    }
}