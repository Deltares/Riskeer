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

using Core.Common.Base.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.Service
{
    /// <summary>
    /// Service responsible for performing a wave condition calculation.
    /// </summary>
    public interface IWaveConditionsCalculationService
    {
        /// <summary>
        /// Performs a wave conditions cosine calculation based on the supplied <see cref="WaveConditionsInput"/>
        /// and returns the <see cref="WaveConditionsOutput"/> if the calculation was succesful.
        /// Error and status information is logged during the execution of the operation.
        /// </summary>
        /// <param name="waterLevel">The water level to calculate the wave conditions for.</param>
        /// <param name="a">The a-value to use during the calculation.</param>
        /// <param name="b">The b-value to use during the calculation.</param>
        /// <param name="c">The c-value to use during the calculation.</param>
        /// <param name="norm">The norm to use during the calculation.</param>
        /// <param name="input">The <see cref="WaveConditionsInput"/> that holds the information required to perform a calculation.</param>
        /// <param name="hlcdDirectory">The directory of the HLCD file that should be used for performing the calculation.</param>
        /// <param name="ringId">The id of the ring to perform the calculation for.</param>
        /// <param name="name">The name of the calculation to perform.</param>
        /// <returns>A <see cref="WaveConditionsOutput"/> on a succesful calculation. <c>null</c> otherwise.</returns>
        WaveConditionsOutput Calculate(RoundedDouble waterLevel,
                                       double a,
                                       double b,
                                       double c,
                                       double norm,
                                       WaveConditionsInput input,
                                       string hlcdDirectory,
                                       string ringId,
                                       string name);
    }
}