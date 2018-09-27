// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Ringtoets.GrassCoverErosionOutwards.Util
{
    /// <summary>
    /// Interface for providing meta data attribute names of hydraulic boundary locations.
    /// </summary>
    public interface IGrassCoverErosionOutwardsHydraulicBoundaryLocationMetaDataAttributeNameProvider
    {
        /// <summary>
        /// Gets the meta data attribute name of the water level calculation
        /// for mechanism specific factorized signaling norm.
        /// </summary>
        string WaterLevelCalculationForMechanismSpecificFactorizedSignalingNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the water level calculation
        /// for mechanism specific signaling norm.
        /// </summary>
        string WaterLevelCalculationForMechanismSpecificSignalingNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the water level calculation
        /// for mechanism specific lower limit norm.
        /// </summary>
        string WaterLevelCalculationForMechanismSpecificLowerLimitNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the water level calculation
        /// for lower limit norm.
        /// </summary>
        string WaterLevelCalculationForLowerLimitNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the water level calculation
        /// for factorized lower limit norm.
        /// </summary>
        string WaterLevelCalculationForFactorizedLowerLimitNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the wave height calculation
        /// for mechanism specific factorized signaling norm.
        /// </summary>
        string WaveHeightCalculationForMechanismSpecificFactorizedSignalingNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the wave height calculation
        /// for mechanism specific signaling norm.
        /// </summary>
        string WaveHeightCalculationForMechanismSpecificSignalingNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the wave height calculation
        /// for mechanism specific lower limit norm.
        /// </summary>
        string WaveHeightCalculationForMechanismSpecificLowerLimitNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the wave height calculation
        /// for lower limit norm.
        /// </summary>
        string WaveHeightCalculationForLowerLimitNormAttributeName { get; }

        /// <summary>
        /// Gets the meta data attribute name of the wave height calculation
        /// for factorized lower limit norm.
        /// </summary>
        string WaveHeightCalculationForFactorizedLowerLimitNormAttributeName { get; }
    }
}