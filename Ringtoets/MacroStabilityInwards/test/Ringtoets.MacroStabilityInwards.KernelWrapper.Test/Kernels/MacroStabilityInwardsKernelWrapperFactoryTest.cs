﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Kernels
{
    [TestFixture]
    public class MacroStabilityInwardsKernelWrapperFactoryTest
    {
        [Test]
        public void Instance_Always_ReturnsAnInstance()
        {
            // Call
            IMacroStabilityInwardsKernelFactory factory = MacroStabilityInwardsKernelWrapperFactory.Instance;

            // Assert
            Assert.IsInstanceOf<MacroStabilityInwardsKernelWrapperFactory>(factory);
        }

        [Test]
        public void Instance_WhenSetToNull_ReturnsNewInstance()
        {
            // Setup
            IMacroStabilityInwardsKernelFactory firstFactory = MacroStabilityInwardsKernelWrapperFactory.Instance;
            MacroStabilityInwardsKernelWrapperFactory.Instance = null;

            // Call
            IMacroStabilityInwardsKernelFactory secondFactory = MacroStabilityInwardsKernelWrapperFactory.Instance;

            // Assert
            Assert.AreNotSame(firstFactory, secondFactory);
        }

        [Test]
        public void Instance_WhenSetToInstance_ReturnsThatInstance()
        {
            // Setup
            var firstFactory = new TestMacroStabilityInwardsKernelFactory();
            MacroStabilityInwardsKernelWrapperFactory.Instance = firstFactory;

            // Call
            IMacroStabilityInwardsKernelFactory secondFactory = MacroStabilityInwardsKernelWrapperFactory.Instance;

            // Assert
            Assert.AreSame(firstFactory, secondFactory);
        }

        [Test]
        public void CreateUpliftVanKernel_Always_ReturnsUpliftVanKernelWrapper()
        {
            // Setup
            IMacroStabilityInwardsKernelFactory factory = MacroStabilityInwardsKernelWrapperFactory.Instance;

            // Call
            IUpliftVanKernel upliftVanKernel = factory.CreateUpliftVanKernel();

            // Assert
            Assert.IsInstanceOf<UpliftVanKernelWrapper>(upliftVanKernel);
        }

        [Test]
        public void CreateWaternetKernel_Always_ReturnsWaternetKernelWrapper()
        {
            // Setup
            IMacroStabilityInwardsKernelFactory factory = MacroStabilityInwardsKernelWrapperFactory.Instance;

            // Call
            IWaternetKernel waternetKernel = factory.CreateWaternetKernel();

            // Assert
            Assert.IsInstanceOf<WaternetKernelWrapper>(waternetKernel);
        }
    }
}