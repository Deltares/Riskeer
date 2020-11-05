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

using Deltares.MacroStability.CSharpWrapper.Input;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Test.Kernels
{
    [TestFixture]
    public class TestMacroStabilityInwardsKernelFactoryTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var factory = new TestMacroStabilityInwardsKernelFactory();

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsKernelFactory>(factory);
            Assert.IsNotNull(factory.LastCreatedUpliftVanKernel);
            Assert.IsNotNull(factory.LastCreatedWaternetDailyKernel);
            Assert.IsNotNull(factory.LastCreatedWaternetExtremeKernel);
        }

        [Test]
        public void CreateUpliftVanKernel_Always_ReturnLastCreatedUpliftVanKernel()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsKernelFactory();
            var input = new MacroStabilityInput();

            // Call
            var upliftVanKernel = (UpliftVanKernelStub) factory.CreateUpliftVanKernel(input);

            // Assert
            Assert.AreSame(factory.LastCreatedUpliftVanKernel, upliftVanKernel);
            Assert.AreSame(input, upliftVanKernel.KernelInput);
        }

        [Test]
        public void CreateWaternetExtremeKernel_Always_ReturnLastCreatedWaternetExtremeKernel()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsKernelFactory();
            var input = new MacroStabilityInput();

            // Call
            var waternetKernel = (WaternetKernelStub) factory.CreateWaternetExtremeKernel(input);

            // Assert
            Assert.AreSame(factory.LastCreatedWaternetExtremeKernel, waternetKernel);
            Assert.AreSame(input, waternetKernel.KernelInput);
        }

        [Test]
        public void CreateWaternetDailyKernel_Always_ReturnLastCreatedWaternetDailyKernel()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsKernelFactory();
            var input = new MacroStabilityInput();

            // Call
            var waternetKernel = (WaternetKernelStub) factory.CreateWaternetDailyKernel(input);

            // Assert
            Assert.AreSame(factory.LastCreatedWaternetDailyKernel, waternetKernel);
            Assert.AreSame(input, waternetKernel.KernelInput);
        }
    }
}