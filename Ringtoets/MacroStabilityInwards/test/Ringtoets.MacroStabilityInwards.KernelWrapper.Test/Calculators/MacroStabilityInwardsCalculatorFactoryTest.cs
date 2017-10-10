// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IMacroStabilityInwardsCalculatorFactory factory = MacroStabilityInwardsCalculatorFactory.Instance;

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsCalculatorFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IMacroStabilityInwardsCalculatorFactory firstFactory = MacroStabilityInwardsCalculatorFactory.Instance;
            MacroStabilityInwardsCalculatorFactory.Instance = null;

            // Call
            IMacroStabilityInwardsCalculatorFactory secondFactory = MacroStabilityInwardsCalculatorFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestMacroStabilityInwardsCalculatorFactory();
            MacroStabilityInwardsCalculatorFactory.Instance = firstFactory;

            // Call
            IMacroStabilityInwardsCalculatorFactory secondFactory = MacroStabilityInwardsCalculatorFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateUpliftVanCalculator_Always_ReturnsUpliftVanCalculator()
        {
            // Setup
            IMacroStabilityInwardsCalculatorFactory factory = MacroStabilityInwardsCalculatorFactory.Instance;

            // Call
            IUpliftVanCalculator upliftVanCalculator = factory.CreateUpliftVanCalculator(
                new UpliftVanCalculatorInput(new UpliftVanCalculatorInput.ConstructionProperties
                {
                    SurfaceLine = new MacroStabilityInwardsSurfaceLine("test"),
                    SoilProfile = new TestUpliftVanSoilProfile(),
                    PhreaticLineOffsetsExtreme = new UpliftVanPhreaticLineOffsets(),
                    PhreaticLineOffsetsDaily = new UpliftVanPhreaticLineOffsets(),
                    DrainageConstruction = new UpliftVanDrainageConstruction(),
                    SlipPlane = new UpliftVanSlipPlane()
                }),
                new TestMacroStabilityInwardsKernelFactory());

            // Assert
            Assert.IsInstanceOf<UpliftVanCalculator>(upliftVanCalculator);
        }
    }
}