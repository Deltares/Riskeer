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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="StabilityStoneCoverWaveConditionsOutput"/> related to 
    /// creating a <see cref="StabilityStoneCoverWaveConditionsOutputEntity"/>.
    /// </summary>
    internal static class WaveConditionsOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="StabilityStoneCoverWaveConditionsOutputEntity"/> based on the information
        /// of the <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for stability stone cover failure mechanism to 
        /// create a database entity for.</param>
        /// <param name="type">The type of the <see cref="WaveConditionsOutput"/>.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="StabilityStoneCoverWaveConditionsOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StabilityStoneCoverWaveConditionsOutputEntity CreateStabilityStoneCoverWaveConditionsOutput(
            this WaveConditionsOutput output, WaveConditionsOutputType type, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var entity = new StabilityStoneCoverWaveConditionsOutputEntity
            {
                WaterLevel = output.WaterLevel.Value.ToNaNAsNull(),
                WaveHeight = output.WaveHeight.Value.ToNaNAsNull(),
                WavePeakPeriod = output.WavePeakPeriod.Value.ToNaNAsNull(),
                WaveAngle = output.WaveAngle.Value.ToNaNAsNull(),
                OutputType = (byte) type
            };
            return entity;
        }

        /// <summary>
        /// Creates a <see cref="StabilityStoneCoverWaveConditionsOutputEntity"/> based on the information
        /// of the <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for stability stone cover failure mechanism to 
        /// create a database entity for.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="StabilityStoneCoverWaveConditionsOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static WaveImpactAsphaltCoverWaveConditionsOutputEntity CreateWaveImpactAsphaltCoverWaveConditionsOutput(
            this WaveConditionsOutput output, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var entity = new WaveImpactAsphaltCoverWaveConditionsOutputEntity
            {
                WaterLevel = output.WaterLevel.Value.ToNaNAsNull(),
                WaveHeight = output.WaveHeight.Value.ToNaNAsNull(),
                WavePeakPeriod = output.WavePeakPeriod.Value.ToNaNAsNull(),
                WaveAngle = output.WaveAngle.Value.ToNaNAsNull(),
            };
            return entity;
        }
    }
}