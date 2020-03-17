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

using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators
{
    /// <summary>
    /// Factory which creates macro stability inwards calculator stubs for testing purposes.
    /// </summary>
    public class TestMacroStabilityInwardsCalculatorFactory : IMacroStabilityInwardsCalculatorFactory
    {
        /// <summary>
        /// Creates a new instance of <see cref="TestMacroStabilityInwardsCalculatorFactory"/>.
        /// </summary>
        public TestMacroStabilityInwardsCalculatorFactory()
        {
            LastCreatedUpliftVanCalculator = new UpliftVanCalculatorStub();
            LastCreatedWaternetCalculator = new WaternetCalculatorStub();
        }

        /// <summary>
        /// Gets the last created <see cref="UpliftVanCalculatorStub"/>.
        /// </summary>
        public UpliftVanCalculatorStub LastCreatedUpliftVanCalculator { get; }

        /// <summary>
        /// Gets the last created <see cref="WaternetCalculatorStub"/>.
        /// </summary>
        public WaternetCalculatorStub LastCreatedWaternetCalculator { get; }

        public IUpliftVanCalculator CreateUpliftVanCalculator(UpliftVanCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
        {
            LastCreatedUpliftVanCalculator.Input = input;

            return LastCreatedUpliftVanCalculator;
        }

        public IWaternetCalculator CreateWaternetExtremeCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
        {
            return CreateWaternetCalculator(input);
        }

        public IWaternetCalculator CreateWaternetDailyCalculator(WaternetCalculatorInput input, IMacroStabilityInwardsKernelFactory factory)
        {
            return CreateWaternetCalculator(input);
        }

        private IWaternetCalculator CreateWaternetCalculator(WaternetCalculatorInput input)
        {
            LastCreatedWaternetCalculator.Input = input;

            return LastCreatedWaternetCalculator;
        }
    }
}