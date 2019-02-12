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

using System;
using System.Collections.Generic;
using Riskeer.Revetment.Data;

namespace Riskeer.StabilityStoneCover.Data
{
    /// <summary>
    /// Factory for creating instances of <see cref="StabilityStoneCoverWaveConditionsOutput"/>.
    /// </summary>
    public static class StabilityStoneCoverWaveConditionsOutputFactory
    {
        /// <summary>
        /// Creates <see cref="StabilityStoneCoverWaveConditionsOutput"/> with blocks set.
        /// </summary>
        /// <param name="blocks">The blocks output to set.</param>
        /// <returns>The created <see cref="StabilityStoneCoverWaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="blocks"/>
        /// is <c>null</c>.</exception>
        public static StabilityStoneCoverWaveConditionsOutput CreateOutputWithBlocks(IEnumerable<WaveConditionsOutput> blocks)
        {
            if (blocks == null)
            {
                throw new ArgumentNullException(nameof(blocks));
            }

            return new StabilityStoneCoverWaveConditionsOutput(null, blocks);
        }

        /// <summary>
        /// Creates <see cref="StabilityStoneCoverWaveConditionsOutput"/> with columns set.
        /// </summary>
        /// <param name="columns">The columns output to set.</param>
        /// <returns>The created <see cref="StabilityStoneCoverWaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columns"/>
        /// is <c>null</c>.</exception>
        public static StabilityStoneCoverWaveConditionsOutput CreateOutputWithColumns(IEnumerable<WaveConditionsOutput> columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            return new StabilityStoneCoverWaveConditionsOutput(columns, null);
        }

        /// <summary>
        /// Creates <see cref="StabilityStoneCoverWaveConditionsOutput"/> with columns and blocks set.
        /// </summary>
        /// <param name="columns">The columns output to set.</param>
        /// <param name="blocks">The blocks output to set.</param>
        /// <returns>The created <see cref="StabilityStoneCoverWaveConditionsOutput"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public static StabilityStoneCoverWaveConditionsOutput CreateOutputWithColumnsAndBlocks(
            IEnumerable<WaveConditionsOutput> columns, IEnumerable<WaveConditionsOutput> blocks)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            if (blocks == null)
            {
                throw new ArgumentNullException(nameof(blocks));
            }

            return new StabilityStoneCoverWaveConditionsOutput(columns, blocks);
        }
    }
}