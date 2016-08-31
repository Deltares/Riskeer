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

using System;
using System.Collections.Generic;
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Revetment.Data;

namespace Ringtoets.StabilityStoneCover.Data
{
    /// <summary>
    /// Container for the results of a stability stone cover wave conditions calculation.
    /// </summary>
    public class StabilityStoneCoverWaveConditionsOutput : Observable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="StabilityStoneCoverWaveConditionsOutput"/>.
        /// </summary>
        /// <param name="columnsOutput">The wave conditions output for columns.</param>
        /// <param name="blocksOutput">The wave conditions output for blocks.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="columnsOutput"/> 
        /// or <paramref name="blocksOutput"/> is <c>null</c>.</exception>
        public StabilityStoneCoverWaveConditionsOutput(IEnumerable<WaveConditionsOutput> columnsOutput, IEnumerable<WaveConditionsOutput> blocksOutput)
        {
            if (columnsOutput == null)
            {
                throw new ArgumentNullException("columnsOutput");
            }

            if (blocksOutput == null)
            {
                throw new ArgumentNullException("blocksOutput");
            }

            ColumnsOutput = columnsOutput;
            BlocksOutput = blocksOutput;
        }

        /// <summary>
        /// Gets the wave conditions output for columns.
        /// </summary>
        public IEnumerable<WaveConditionsOutput> ColumnsOutput { get; private set; }

        /// <summary>
        /// Gets the wave conditions output for blocks.
        /// </summary>
        public IEnumerable<WaveConditionsOutput> BlocksOutput { get; private set; }
    }
}