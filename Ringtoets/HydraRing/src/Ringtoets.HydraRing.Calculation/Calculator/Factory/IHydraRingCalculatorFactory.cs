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

namespace Ringtoets.HydraRing.Calculation.Calculator.Factory
{
    /// <summary>
    /// Interface for a factory which creates calculators that are used to perform a calculation using
    /// Hydra-Ring.
    /// </summary>
    public interface IHydraRingCalculatorFactory
    {
        /// <summary>
        /// Creates a calculator for calculating a design water level.
        /// </summary>
        /// <param name="hlcdDirectory">The directory where the hydraulic database can be found.</param>
        /// <param name="ringId">The id of the traject which is used in the calculation.</param>
        /// <returns>A new <see cref="IDesignWaterLevelCalculator"/>.</returns>
        IDesignWaterLevelCalculator CreateDesignWaterLevelCalculator(string hlcdDirectory, string ringId);

        /// <summary>
        /// Creates a calculator for calculating a dike height.
        /// </summary>
        /// <param name="hlcdDirectory">The directory where the hydraulic database can be found.</param>
        /// <param name="ringId">The id of the traject which is used in the calculation.</param>
        /// <returns>A new <see cref="IDikeHeightCalculator"/>.</returns>
        IDikeHeightCalculator CreateDikeHeightCalculator(string hlcdDirectory, string ringId);

        /// <summary>
        /// Creates a calculator for performing an overtopping calculation.
        /// </summary>
        /// <param name="hlcdDirectory">The directory where the hydraulic database can be found.</param>
        /// <param name="ringId">The id of the traject which is used in the calculation.</param>
        /// <returns>A new <see cref="IOvertoppingCalculator"/>.</returns>
        IOvertoppingCalculator CreateOvertoppingCalculator(string hlcdDirectory, string ringId);

        /// <summary>
        /// Creates a calculator for calculating wave conditions.
        /// </summary>
        /// <param name="hlcdDirectory">The directory where the hydraulic database can be found.</param>
        /// <param name="ringId">The id of the traject which is used in the calculation.</param>
        /// <returns>A new <see cref="IWaveConditionsCosineCalculator"/>.</returns>
        IWaveConditionsCosineCalculator CreateWaveConditionsCosineCalculator(string hlcdDirectory, string ringId);

        /// <summary>
        /// Creates a calculator for calculating a wave height.
        /// </summary>
        /// <param name="hlcdDirectory">The directory where the hydraulic database can be found.</param>
        /// <param name="ringId">The id of the traject which is used in the calculation.</param>
        /// <returns>A new <see cref="IWaveHeightCalculator"/>.</returns>
        IWaveHeightCalculator CreateWaveHeightCalculator(string hlcdDirectory, string ringId);
    }
}