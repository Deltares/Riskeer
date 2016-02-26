﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.HydraRing.Calculation.Settings
{
    /// <summary>
    /// Container for variable defaults.
    /// </summary>
    public class VariableDefaults
    {
        private readonly double correlationLength;

        /// <summary>
        /// Creates a new instance of the <see cref="VariableDefaults"/> class.
        /// </summary>
        /// <param name="correlationLength">The correlation length.</param>
        public VariableDefaults(double correlationLength)
        {
            this.correlationLength = correlationLength;
        }

        /// <summary>
        /// Gets the correlation length.
        /// </summary>
        public double CorrelationLength
        {
            get
            {
                return correlationLength;
            }
        }
    }
}
