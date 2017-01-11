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

using Application.Ringtoets.Storage.DbContext;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.DuneErosion.Data;

namespace Application.Ringtoets.Storage.Read.DuneErosion
{
    /// <summary>
    /// Extension methods for <see cref="DuneLocationOutputEntity"/> related to creating 
    /// a <see cref="DuneLocationOutput"/>.
    /// </summary>
    internal static class DuneLocationOutputEntityReadExtensions
    {
        /// <summary>
        /// Read the <see cref="DuneLocationOutputEntity"/> and use the information to construct a <see cref="DuneLocationOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="DuneLocationOutputEntity"/> to create <see cref="DuneLocationOutput"/> for.</param>
        /// <returns>A new <see cref="DuneLocationOutput"/>.</returns>
        internal static DuneLocationOutput Read(this DuneLocationOutputEntity entity)
        {
            return new DuneLocationOutput((CalculationConvergence) entity.CalculationConvergence, new DuneLocationOutput.ConstructionProperties
            {
                WaterLevel = entity.WaterLevel.ToNullAsNaN(),
                WaveHeight = entity.WaveHeight.ToNullAsNaN(),
                WavePeriod = entity.WavePeriod.ToNullAsNaN(),
                TargetProbability = entity.TargetProbability.ToNullAsNaN(),
                TargetReliability = entity.TargetReliability.ToNullAsNaN(),
                CalculatedProbability = entity.CalculatedProbability.ToNullAsNaN(),
                CalculatedReliability = entity.CalculatedReliability.ToNullAsNaN()
            });
        }
    }
}