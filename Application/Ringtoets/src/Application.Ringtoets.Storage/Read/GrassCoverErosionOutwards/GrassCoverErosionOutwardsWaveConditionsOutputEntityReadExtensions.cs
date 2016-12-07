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

using Application.Ringtoets.Storage.DbContext;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Application.Ringtoets.Storage.Read.GrassCoverErosionOutwards
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="GrassCoverErosionOutwardsWaveConditionsOutput"/>
    /// based on the <see cref="GrassCoverErosionOutwardsWaveConditionsOutputEntity"/>.
    /// </summary>
    internal static class GrassCoverErosionOutwardsWaveConditionsOutputEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="GrassCoverErosionOutwardsWaveConditionsOutputEntity"/>
        /// and use the information to construct a <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="GrassCoverErosionOutwardsWaveConditionsOutputEntity"/>
        /// to create <see cref="WaveConditionsOutput"/> for.</param>
        /// <returns>A new <see cref="WaveConditionsOutput"/>.</returns>
        internal static WaveConditionsOutput Read(this GrassCoverErosionOutwardsWaveConditionsOutputEntity entity)
        {
            return new WaveConditionsOutput(entity.WaterLevel.ToNullAsNaN(),
                                            entity.WaveHeight.ToNullAsNaN(),
                                            entity.WavePeakPeriod.ToNullAsNaN(),
                                            entity.WaveAngle.ToNullAsNaN(),
                                            entity.WaveDirection.ToNullAsNaN(),
                                            entity.TargetProbability.ToNullAsNaN(),
                                            entity.TargetReliability.ToNullAsNaN(),
                                            entity.CalculatedProbability.ToNullAsNaN(),
                                            entity.CalculatedReliability.ToNullAsNaN(),
                                            (CalculationConvergence) entity.CalculationConvergence);
        }
    }
}