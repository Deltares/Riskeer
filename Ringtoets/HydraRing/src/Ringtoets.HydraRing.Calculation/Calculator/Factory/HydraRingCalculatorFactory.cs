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

        public IDesignWaterLevelCalculator CreateDesignWaterLevelCalculator(string hlcdDirectory)
        {
            return new DesignWaterLevelCalculator(hlcdDirectory);
        }

        public IOvertoppingCalculator CreateOvertoppingCalculator(string hlcdDirectory)
        {
            return new OvertoppingCalculator(hlcdDirectory);
        }

        public IDikeHeightCalculator CreateDikeHeightCalculator(string hlcdDirectory)
        {
            return new DikeHeightCalculator(hlcdDirectory);
        }

        public IHydraulicLoadsCalculator CreateOvertoppingRateCalculator(string hlcdDirectory)
        {
            return new OvertoppingRateCalculator(hlcdDirectory);
        }

        public IWaveConditionsCosineCalculator CreateWaveConditionsCosineCalculator(string hlcdDirectory)
        {
            return new WaveConditionsCosineCalculator(hlcdDirectory);
        }

        public IWaveHeightCalculator CreateWaveHeightCalculator(string hlcdDirectory)
        {
            return new WaveHeightCalculator(hlcdDirectory);
        }

        public IStructuresOvertoppingCalculator CreateStructuresOvertoppingCalculator(string hlcdDirectory)
        {
            return new StructuresOvertoppingCalculator(hlcdDirectory);
        }

        public IStructuresClosureCalculator CreateStructuresClosureCalculator(string hlcdDirectory)
        {
            return new StructuresClosureCalculator(hlcdDirectory);
        }

        public IStructuresStabilityPointCalculator CreateStructuresStabilityPointCalculator(string hlcdDirectory)
        {
            return new StructuresStabilityPointCalculator(hlcdDirectory);
        }

        public IDunesBoundaryConditionsCalculator CreateDunesBoundaryConditionsCalculator(string hlcdDirectory)
        {
            return new DunesBoundaryConditionsCalculator(hlcdDirectory);
        }
    }
}