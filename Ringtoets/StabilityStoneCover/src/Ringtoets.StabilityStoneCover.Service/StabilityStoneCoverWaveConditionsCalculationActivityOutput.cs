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

using System;
using System.Collections.Generic;
using Ringtoets.Revetment.Data;

namespace Ringtoets.StabilityStoneCover.Service
{
    /// <summary>
    /// Container for the results of a stability stone cover wave conditions calculation.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsCalculationActivityOutput
    {
        private readonly ICollection<WaveConditionsOutput> columnsOutput;
        private readonly ICollection<WaveConditionsOutput> blocksOutput;

        internal StabilityStoneCoverWaveConditionsCalculationActivityOutput()
        {
            columnsOutput = new List<WaveConditionsOutput>();
            blocksOutput = new List<WaveConditionsOutput>();
        }

        /// <summary>
        /// Gets the wave conditions output for columns.
        /// </summary>
        internal IEnumerable<WaveConditionsOutput> ColumnsOutput
        {
            get
            {
                return columnsOutput;
            }
        }

        /// <summary>
        /// Gets the wave conditions output for blocks.
        /// </summary>
        internal IEnumerable<WaveConditionsOutput> BlocksOutput
        {
            get
            {
                return blocksOutput;
            }
        }

        /// <summary>
        /// Adds an <see cref="WaveConditionsOutput"/> to <see cref="BlocksOutput"/>.
        /// </summary>
        /// <param name="element">The output to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> 
        /// is <c>null</c>.</exception>
        internal void AddBlocksOutput(WaveConditionsOutput element)
        {
            AddElementToList(blocksOutput, element);
        }

        /// <summary>
        /// Adds an <see cref="WaveConditionsOutput"/> to <see cref="ColumnsOutput"/>.
        /// </summary>
        /// <param name="element">The output to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="element"/> 
        /// is <c>null</c>.</exception>
        internal void AddColumnsOutput(WaveConditionsOutput element)
        {
            AddElementToList(columnsOutput, element);
        }

        private static void AddElementToList(ICollection<WaveConditionsOutput> list, WaveConditionsOutput element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            list.Add(element);
        }
    }
}