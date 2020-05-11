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

using System.Collections.Generic;
using Deltares.MacroStability.Geometry;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Riskeer.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;
using SoilLayer = Riskeer.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Calculators.Waternet
{
    [TestFixture]
    public class WaternetDailyCalculatorTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateValidCalculatorInput();

            // Call
            var calculator = new WaternetDailyCalculator(input, factory);

            // Assert
            Assert.IsInstanceOf<WaternetCalculator>(calculator);
            mocks.VerifyAll();
        }

        [Test]
        public void Calculate_CalculatorWithCompleteInput_InputCorrectlySetToKernel()
        {
            // Setup
            WaternetCalculatorInput input = WaternetCalculatorInputTestFactory.CreateCompleteCalculatorInput();

            using (new MacroStabilityInwardsKernelFactoryConfig())
            {
                var factory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;
                WaternetKernelStub waternetKernel = factory.LastCreatedWaternetKernel;
                SetKernelOutput(waternetKernel);

                // Call
                new WaternetDailyCalculator(input, factory).Calculate();

                // Assert
                LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile, out IDictionary<SoilLayer, LayerWithSoil> layerLookup);

                KernelInputAssert.AssertSoilProfiles(SoilProfileCreator.Create(layersWithSoil), waternetKernel.SoilProfile);
                KernelInputAssert.AssertStabilityLocations(WaternetLocationCreator.Create(input), waternetKernel.Location);
                KernelInputAssert.AssertSurfaceLines(SurfaceLineCreator.Create(input.SurfaceLine), waternetKernel.SurfaceLine);
            }
        }

        private static void SetKernelOutput(WaternetKernelStub waternetKernel)
        {
            waternetKernel.Waternet = new WtiStabilityWaternet
            {
                PhreaticLine = new PhreaticLine()
            };
        }
    }
}