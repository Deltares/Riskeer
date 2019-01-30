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

using Riskeer.Common.Data.Hydraulics;
using Riskeer.Revetment.Data;
using Riskeer.Storage.Core.DbContext;
using Riskeer.WaveImpactAsphaltCover.Data;

namespace Riskeer.Storage.Core.Read.WaveImpactAsphaltCover
{
    /// <summary>
    /// This class defines extension methods for read operations for a <see cref="WaveImpactAsphaltCoverWaveConditionsOutput"/>
    /// based on the <see cref="WaveImpactAsphaltCoverWaveConditionsOutputEntity"/>.
    /// </summary>
    internal static class WaveImpactAsphaltCoverWaveConditionsOutputEntityReadExtensions
    {
        /// <summary>
        /// Reads the <see cref="WaveImpactAsphaltCoverWaveConditionsOutputEntity"/>
        /// and use the information to construct a <see cref="WaveConditionsOutput"/>.
        /// </summary>
        /// <param name="entity">The <see cref="WaveImpactAsphaltCoverWaveConditionsOutputEntity"/>
        /// to create <see cref="WaveConditionsOutput"/> for.</param>
        /// <returns>A new <see cref="WaveConditionsOutput"/>.</returns>
        internal static WaveConditionsOutput Read(this WaveImpactAsphaltCoverWaveConditionsOutputEntity entity)
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