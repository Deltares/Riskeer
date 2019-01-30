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

using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;

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
            Assert.IsNotNull(factory.LastCreatedWaternetKernel);
        }

        [Test]
        public void CreateUpliftVanKernel_Always_ReturnLastCreatedUpliftVanKernel()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsKernelFactory();

            // Call
            IUpliftVanKernel upliftVanKernel = factory.CreateUpliftVanKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedUpliftVanKernel, upliftVanKernel);
        }

        [Test]
        public void CreateWaternetExtremeKernel_Always_ReturnLastCreatedWaternetKernel()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsKernelFactory();

            // Call
            IWaternetKernel waternetKernel = factory.CreateWaternetExtremeKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedWaternetKernel, waternetKernel);
        }

        [Test]
        public void CreateWaternetDailyKernel_Always_ReturnLastCreatedWaternetKernel()
        {
            // Setup
            var factory = new TestMacroStabilityInwardsKernelFactory();

            // Call
            IWaternetKernel waternetKernel = factory.CreateWaternetDailyKernel();

            // Assert
            Assert.AreSame(factory.LastCreatedWaternetKernel, waternetKernel);
        }
    }
}