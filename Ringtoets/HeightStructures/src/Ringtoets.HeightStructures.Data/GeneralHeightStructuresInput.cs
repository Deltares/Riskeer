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
using Ringtoets.Common.Data.Probabilistics;

namespace Ringtoets.HeightStructures.Data
{
    /// <summary>
    /// Class that holds all static height structures calculation specific input parameters.
    /// </summary>
    public class GeneralHeightStructuresInput
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GeneralHeightStructuresInput"/> class.
        /// </summary>
        public GeneralHeightStructuresInput()
        {
            GravitationalAcceleration = new RoundedDouble(2, 9.87);

            ModelfactorOvertopping = new LognormalDistribution(3)
            {
                Mean = new RoundedDouble(3, 0.09),
                StandardDeviation = new RoundedDouble(3, 0.06)
            };

            ModelFactorForStorageVolume = new LognormalDistribution(2)
            {
                Mean = new RoundedDouble(2, 1),
                StandardDeviation = new RoundedDouble(2, 0.2)
            };

            ModelFactorForIncomingFlowVolume = new RoundedDouble(2, 1);
        }

        /// <summary>
        /// Gets the gravitational acceleration.
        /// </summary>
        public RoundedDouble GravitationalAcceleration { get; private set; }

        /// <summary>
        /// Gets the model factor overtopping.
        /// </summary>
        public LognormalDistribution ModelfactorOvertopping { get; private set; }

        /// <summary>
        /// Gets the model factor for storage volume.
        /// </summary>
        public LognormalDistribution ModelFactorForStorageVolume { get; private set; }

        /// <summary>
        /// Gets the model factor for incoming flow volume.
        /// </summary>
        public RoundedDouble ModelFactorForIncomingFlowVolume { get; private set; }
    }
}