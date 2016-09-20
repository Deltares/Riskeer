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
using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Revetment.Data;

namespace Application.Ringtoets.Storage.Create
{
    /// <summary>
    /// Extension methods for <see cref="WaveConditionsOutput"/> related to 
    /// creating entities for various failure mechanisms.
    /// </summary>
    internal static class WaveConditionsOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="GrassCoverErosionOutwardsWaveConditionsOutputEntity"/> based on the information
        /// of the <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for grass cover erosion outwards failure mechanism to 
        /// create a database entity for.</param>
        /// <param name="order">The position of <paramref name="output"/> in the list of all outputs.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="GrassCoverErosionOutwardsWaveConditionsOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static GrassCoverErosionOutwardsWaveConditionsOutputEntity CreateGrassCoverErosionOutwardsWaveConditionsOutputEntity(
            this WaveConditionsOutput output, int order, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var entity = new GrassCoverErosionOutwardsWaveConditionsOutputEntity
            {
                Order = order,
                WaterLevel = output.WaterLevel.Value.ToNaNAsNull(),
                WaveHeight = output.WaveHeight.Value.ToNaNAsNull(),
                WavePeakPeriod = output.WavePeakPeriod.Value.ToNaNAsNull(),
                WaveAngle = output.WaveAngle.Value.ToNaNAsNull()
            };
            return entity;
        }

        /// <summary>
        /// Creates a <see cref="StabilityStoneCoverWaveConditionsOutputEntity"/> based on the information
        /// of the <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for stability stone cover failure mechanism to 
        /// create a database entity for.</param>
        /// <param name="type">The type of the <see cref="WaveConditionsOutput"/>.</param>
        /// <param name="order">The position of <paramref name="output"/> in the list of all outputs.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="StabilityStoneCoverWaveConditionsOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static StabilityStoneCoverWaveConditionsOutputEntity CreateStabilityStoneCoverWaveConditionsOutputEntity(
            this WaveConditionsOutput output, WaveConditionsOutputType type, int order, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var entity = new StabilityStoneCoverWaveConditionsOutputEntity
            {
                Order = order,
                WaterLevel = output.WaterLevel.Value.ToNaNAsNull(),
                WaveHeight = output.WaveHeight.Value.ToNaNAsNull(),
                WavePeakPeriod = output.WavePeakPeriod.Value.ToNaNAsNull(),
                WaveAngle = output.WaveAngle.Value.ToNaNAsNull(),
                OutputType = (byte) type
            };
            return entity;
        }

        /// <summary>
        /// Creates a <see cref="WaveImpactAsphaltCoverWaveConditionsOutputEntity"/> based on the information
        /// of the <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="output">The calculation output for wave impact asphalt cover mechanism to 
        /// create a database entity for.</param>
        /// <param name="order">The position of <paramref name="output"/> in the list of all outputs.</param>
        /// <param name="registry">The object keeping track of create operations.</param>
        /// <returns>A new <see cref="WaveImpactAsphaltCoverWaveConditionsOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="registry"/> is <c>null</c>.</exception>
        internal static WaveImpactAsphaltCoverWaveConditionsOutputEntity CreateWaveImpactAsphaltCoverWaveConditionsOutputEntity(
            this WaveConditionsOutput output, int order, PersistenceRegistry registry)
        {
            if (registry == null)
            {
                throw new ArgumentNullException("registry");
            }
            var entity = new WaveImpactAsphaltCoverWaveConditionsOutputEntity
            {
                Order = order,
                WaterLevel = output.WaterLevel.Value.ToNaNAsNull(),
                WaveHeight = output.WaveHeight.Value.ToNaNAsNull(),
                WavePeakPeriod = output.WavePeakPeriod.Value.ToNaNAsNull(),
                WaveAngle = output.WaveAngle.Value.ToNaNAsNull()
            };
            return entity;
        }
    }
}