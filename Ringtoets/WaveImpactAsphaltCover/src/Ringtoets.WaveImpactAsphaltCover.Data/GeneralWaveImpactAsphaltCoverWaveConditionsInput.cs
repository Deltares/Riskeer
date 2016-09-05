// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.WaveImpactAsphaltCover.Data
{
    /// <summary>
    /// Class that holds all the static wave impact asphalt cover wave conditions input parameters.
    /// </summary>
    public class GeneralWaveImpactAsphaltCoverWaveConditionsInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralWaveImpactAsphaltCoverWaveConditionsInput"/>.
        /// </summary>
        public GeneralWaveImpactAsphaltCoverWaveConditionsInput()
        {
            A = 1.0;
            B = 0.0;
            C = 0.0;
        }

        /// <summary>
        /// Gets the 'a' parameter used in wave conditions calculations for blocks.
        /// </summary>
        public double A { get; private set; }

        /// <summary>
        /// Gets the 'b' parameter used in wave conditions calculations for blocks.
        /// </summary>
        public double B { get; private set; }

        /// <summary>
        /// Gets the 'c' parameter used in wave conditions calculations for blocks.
        /// </summary>
        public double C { get; private set; }
    }
}