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
using Ringtoets.DuneErosion.Data;

namespace Application.Ringtoets.Storage.Create.DuneErosion
{
    /// <summary>
    /// Extension methods for <see cref="DuneLocationOutput"/> related to creating a <see cref="DuneLocationOutputEntity"/>.
    /// </summary>
    internal static class DuneLocationOutputCreateExtensions
    {
        /// <summary>
        /// Creates a <see cref="DuneLocationOutputEntity"/> based on the information of the <see cref="DuneLocationOutput"/>.
        /// </summary>
        /// <param name="output">The output to create a database entity for.</param>
        /// <returns>A new <see cref="DuneLocationOutputEntity"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="output"/> is <c>null</c>.</exception>
        internal static DuneLocationOutputEntity Create(this DuneLocationOutput output)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            return new DuneLocationOutputEntity
            {
                WaterLevel = output.WaterLevel.ToNaNAsNull(),
                WaveHeight = output.WaveHeight.ToNaNAsNull(),
                WavePeriod = output.WavePeriod.ToNaNAsNull(),
                TargetProbability = output.TargetProbability.ToNaNAsNull(),
                TargetReliability = output.TargetReliability.ToNaNAsNull(),
                CalculatedProbability = output.CalculatedProbability.ToNaNAsNull(),
                CalculatedReliability = output.CalculatedReliability.ToNaNAsNull(),
                CalculationConvergence = (byte) output.CalculationConvergence
            };
        }
    }
}