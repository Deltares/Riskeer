// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using Ringtoets.Revetment.Data;

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
            GeneralBlocksWaveConditionsInput = new GeneralWaveConditionsInput(1.0, 1.0, 1.0);
            GeneralColumnsWaveConditionsInput = new GeneralWaveConditionsInput(1.0, 0.4, 0.8);
        }

        /// <summary>
        /// Gets the general input parameter used in wave conditions calculations for blocks.
        /// </summary>
        public GeneralWaveConditionsInput GeneralBlocksWaveConditionsInput { get; private set; }

        /// <summary>
        /// Gets the general input parameter used in wave conditions calculations for columns.
        /// </summary>
        public GeneralWaveConditionsInput GeneralColumnsWaveConditionsInput { get; private set; }
    }
}