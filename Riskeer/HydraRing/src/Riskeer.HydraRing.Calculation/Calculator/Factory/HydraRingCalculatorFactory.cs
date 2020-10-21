﻿// Copyright (C) Stichting Deltares 2019. All rights reserved.
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

using Riskeer.HydraRing.Calculation.Data.Input;

namespace Riskeer.HydraRing.Calculation.Calculator.Factory
{
    /// <summary>
    /// The factory for creating <see cref="HydraRingCalculatorBase"/> instances.
    /// </summary>
    public class HydraRingCalculatorFactory : IHydraRingCalculatorFactory
    {
        private static IHydraRingCalculatorFactory instance;

        /// <summary>
        /// Sets the current <see cref="IHydraRingCalculatorFactory"/>, which is used to create 
        /// <see cref="HydraRingCalculatorBase"/> instances.
        /// </summary>
        public static IHydraRingCalculatorFactory Instance
        {
            get
            {
                return instance ?? (instance = new HydraRingCalculatorFactory());
            }
            internal set
            {
                instance = value;
            }
        }

        public IDesignWaterLevelCalculator CreateDesignWaterLevelCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new DesignWaterLevelCalculator(calculationSettings);
        }

        public IOvertoppingCalculator CreateOvertoppingCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new OvertoppingCalculator(calculationSettings);
        }

        public IHydraulicLoadsCalculator CreateDikeHeightCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new HydraulicLoadsCalculator(calculationSettings);
        }

        public IHydraulicLoadsCalculator CreateOvertoppingRateCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new HydraulicLoadsCalculator(calculationSettings);
        }

        public IWaveConditionsCosineCalculator CreateWaveConditionsCosineCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new WaveConditionsCosineCalculator(calculationSettings);
        }

        public IWaveHeightCalculator CreateWaveHeightCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new WaveHeightCalculator(calculationSettings);
        }

        public IDunesBoundaryConditionsCalculator CreateDunesBoundaryConditionsCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new DunesBoundaryConditionsCalculator(calculationSettings);
        }

        public IStructuresCalculator<TCalculationInput> CreateStructuresCalculator<TCalculationInput>(HydraRingCalculationSettings calculationSettings)
            where TCalculationInput : ExceedanceProbabilityCalculationInput
        {
            return new StructuresCalculator<TCalculationInput>(calculationSettings);
        }
        
        public IPipingCalculator CreatePipingCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new PipingCalculator(calculationSettings);
        }
    }
}