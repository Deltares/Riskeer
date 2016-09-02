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

namespace Ringtoets.StabilityStoneCover.Data
{
    /// <summary>
    /// Class that holds all the static stability stone cover wave conditions input parameters.
    /// </summary>
    public class GeneralStabilityStoneCoverWaveConditionsInput
    {
        /// <summary>
        /// Creates a new instance of <see cref="GeneralStabilityStoneCoverWaveConditionsInput"/>.
        /// </summary>
        public GeneralStabilityStoneCoverWaveConditionsInput()
        {
            ABlocks = 1.0;
            BBlocks = 1.0;
            CBlocks = 1.0;

            AColumns = 1.0;
            BColumns = 0.4;
            CColumns = 0.8;
        }

        /// <summary>
        /// Gets the 'a' parameter used in wave conditions calculations for blocks.
        /// </summary>
        public double ABlocks { get; private set; }

        /// <summary>
        /// Gets the 'b' parameter used in wave conditions calculations for blocks.
        /// </summary>
        public double BBlocks { get; private set; }

        /// <summary>
        /// Gets the 'c' parameter used in wave conditions calculations for blocks.
        /// </summary>
        public double CBlocks { get; private set; }

        /// <summary>
        /// Gets the 'a' parameter used in wave conditions calculations for columns.
        /// </summary>
        public double AColumns { get; private set; }

        /// <summary>
        /// Gets the 'b' parameter used in wave conditions calculations for columns.
        /// </summary>
        public double BColumns { get; private set; }

        /// <summary>
        /// Gets the 'c' parameter used in wave conditions calculations for columns.
        /// </summary>
        public double CColumns { get; private set; }
    }
}