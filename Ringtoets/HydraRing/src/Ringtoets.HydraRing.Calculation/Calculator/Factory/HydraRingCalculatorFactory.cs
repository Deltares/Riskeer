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

using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.Calculator.Factory
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

        public IDesignWaterLevelCalculator CreateDesignWaterLevelCalculator(string hlcdDirectory, string preprocessorDirectory)
        {
            return new DesignWaterLevelCalculator(hlcdDirectory, preprocessorDirectory);
        }

        public IOvertoppingCalculator CreateOvertoppingCalculator(string hlcdDirectory, string preprocessorDirectory)
        {
            return new OvertoppingCalculator(hlcdDirectory, preprocessorDirectory);
        }

        public IHydraulicLoadsCalculator CreateDikeHeightCalculator(string hlcdDirectory, string preprocessorDirectory)
        {
            return new HydraulicLoadsCalculator(hlcdDirectory, preprocessorDirectory);
        }

        public IHydraulicLoadsCalculator CreateOvertoppingRateCalculator(string hlcdDirectory, string preprocessorDirectory)
        {
            return new HydraulicLoadsCalculator(hlcdDirectory, preprocessorDirectory);
        }

        public IWaveConditionsCosineCalculator CreateWaveConditionsCosineCalculator(string hlcdDirectory, string preprocessorDirectory)
        {
            return new WaveConditionsCosineCalculator(hlcdDirectory, preprocessorDirectory);
        }

        public IWaveHeightCalculator CreateWaveHeightCalculator(string hlcdDirectory, string preprocessorDirectory)
        {
            return new WaveHeightCalculator(hlcdDirectory, preprocessorDirectory);
        }

        public IDunesBoundaryConditionsCalculator CreateDunesBoundaryConditionsCalculator(HydraRingCalculationSettings calculationSettings)
        {
            return new DunesBoundaryConditionsCalculator(calculationSettings);
        }

        public IStructuresCalculator<TCalculationInput> CreateStructuresCalculator<TCalculationInput>(string hlcdDirectory, string preprocessorDirectory)
            where TCalculationInput : ExceedanceProbabilityCalculationInput
        {
            return new StructuresCalculator<TCalculationInput>(hlcdDirectory, preprocessorDirectory);
        }
    }
}