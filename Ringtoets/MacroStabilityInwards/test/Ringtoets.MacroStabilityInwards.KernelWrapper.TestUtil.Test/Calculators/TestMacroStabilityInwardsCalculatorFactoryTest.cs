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

using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Calculators
{
    [TestFixture]
    public class TestMacroStabilityInwardsCalculatorFactoryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var factory = new TestMacroStabilityInwardsCalculatorFactory();

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsCalculatorFactory>(factory);
            Assert.IsNotNull(factory.LastCreatedUpliftVanCalculator);
            Assert.IsNull(factory.LastCreatedUpliftVanCalculator.Input);
            Assert.IsNotNull(factory.LastCreatedWaternetCalculator);
            Assert.IsNull(factory.LastCreatedWaternetCalculator.Input);
        }

        [Test]
        public void CreateUpliftVanCalculator_Always_ReturnStubWithInputSet()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsCalculatorFactory();
            var input = new UpliftVanCalculatorInput(new UpliftVanCalculatorInput.ConstructionProperties
            {
                SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                SoilProfile = new TestSoilProfile(),
                PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                DrainageConstruction = new DrainageConstruction(),
                SlipPlane = new UpliftVanSlipPlane()
            });

            // Call
            var calculator = (UpliftVanCalculatorStub) factory.CreateUpliftVanCalculator(input, null);

            // Assert
            Assert.AreSame(input, calculator.Input);
        }

        [Test]
        public void CreateWaternetExtremeCalculator_Always_ReturnStubWithInputSet()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsCalculatorFactory();
            var input = new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
            {
                SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                SoilProfile = new TestSoilProfile(),
                PhreaticLineOffsets = new PhreaticLineOffsets(),
                DrainageConstruction = new DrainageConstruction()
            });

            // Call
            var calculator = (WaternetCalculatorStub) factory.CreateWaternetExtremeCalculator(input, null);

            // Assert
            Assert.IsInstanceOf<WaternetCalculatorStub>(calculator);
            Assert.AreSame(input, calculator.Input);
        }

        [Test]
        public void CreateWaternetDailyCalculator_Always_ReturnStubWithInputSet()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsCalculatorFactory();
            var input = new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
            {
                SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                SoilProfile = new TestSoilProfile(),
                PhreaticLineOffsets = new PhreaticLineOffsets(),
                DrainageConstruction = new DrainageConstruction()
            });

            // Call
            var calculator = (WaternetCalculatorStub) factory.CreateWaternetDailyCalculator(input, null);

            // Assert
            Assert.IsInstanceOf<WaternetCalculatorStub>(calculator);
            Assert.AreSame(input, calculator.Input);
        }
    }
}