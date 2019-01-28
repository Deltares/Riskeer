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

using System;
using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.HydraRing.Calculation.Calculator.Factory
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
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IDesignWaterLevelCalculator"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IDesignWaterLevelCalculator CreateDesignWaterLevelCalculator(HydraRingCalculationSettings calculationSettings);

        /// <summary>
        /// Creates a calculator for performing an overtopping calculation.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IOvertoppingCalculator"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IOvertoppingCalculator CreateOvertoppingCalculator(HydraRingCalculationSettings calculationSettings);

        /// <summary>
        /// Creates a calculator for calculating a dike height.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IHydraulicLoadsCalculator"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IHydraulicLoadsCalculator CreateDikeHeightCalculator(HydraRingCalculationSettings calculationSettings);

        /// <summary>
        /// Creates a calculator for calculating an overtopping rate.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IHydraulicLoadsCalculator"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IHydraulicLoadsCalculator CreateOvertoppingRateCalculator(HydraRingCalculationSettings calculationSettings);

        /// <summary>
        /// Creates a calculator for calculating wave conditions.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IWaveConditionsCosineCalculator"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IWaveConditionsCosineCalculator CreateWaveConditionsCosineCalculator(HydraRingCalculationSettings calculationSettings);

        /// <summary>
        /// Creates a calculator for calculating a wave height.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IWaveHeightCalculator"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IWaveHeightCalculator CreateWaveHeightCalculator(HydraRingCalculationSettings calculationSettings);

        /// <summary>
        /// Creates a calculator for performing a calculation for dunes boundary conditions.
        /// </summary>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IDunesBoundaryConditionsCalculator"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IDunesBoundaryConditionsCalculator CreateDunesBoundaryConditionsCalculator(HydraRingCalculationSettings calculationSettings);

        /// <summary>
        /// Creates a calculator for performing a calculation for structures.
        /// </summary>
        /// <typeparam name="TCalculationInput">The type of the input.</typeparam>
        /// <param name="calculationSettings">The <see cref="HydraRingCalculationSettings"/> with the
        /// Hydra-Ring calculation settings.</param>
        /// <returns>A new <see cref="IStructuresCalculator{TInput}"/>.</returns>>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="calculationSettings"/>
        /// is <c>null</c>.</exception>
        /// <remarks>Preprocessing is disabled when <see cref="HydraRingCalculationSettings.PreprocessorDirectory"/>
        /// equals <see cref="string.Empty"/>.</remarks>
        IStructuresCalculator<TCalculationInput> CreateStructuresCalculator<TCalculationInput>(HydraRingCalculationSettings calculationSettings)
            where TCalculationInput : ExceedanceProbabilityCalculationInput;
    }
}