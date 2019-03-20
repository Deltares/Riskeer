// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Linq;
using Riskeer.Revetment.Data;

namespace Riskeer.StabilityStoneCover.Data.TestUtil
{
    /// <summary>
    /// Factory that creates simple instances of <see cref="StabilityStoneCoverWaveConditionsOutput"/>
    /// that can be used in tests.
    /// </summary>
    public static class StabilityStoneCoverWaveConditionsOutputTestFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsOutput"/>
        /// with default output.
        /// </summary>
        /// <returns>The created <see cref="StabilityStoneCoverWaveConditionsOutput"/>.</returns>
        public static StabilityStoneCoverWaveConditionsOutput Create()
        {
            return new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(), Enumerable.Empty<WaveConditionsOutput>());
        }

        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsOutput"/>
        /// with the given output.
        /// </summary>
        /// <param name="columnsOutput">The columns output.</param>
        /// <param name="blocksOutput">The blocks output.</param>
        /// <returns>The created <see cref="StabilityStoneCoverWaveConditionsOutput"/>.</returns>
        public static StabilityStoneCoverWaveConditionsOutput Create(IEnumerable<WaveConditionsOutput> columnsOutput, IEnumerable<WaveConditionsOutput> blocksOutput)
        {
            return new StabilityStoneCoverWaveConditionsOutput(columnsOutput, blocksOutput);
        }
    }
}