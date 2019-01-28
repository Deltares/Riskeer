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

using System.Linq;
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.WTIStability.Data.Geo.Waternet;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.Waternet
{
    [TestFixture]
    public class WaternetExtremeCalculatorTest
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
            var calculator = new WaternetExtremeCalculator(input, factory);

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
                new WaternetExtremeCalculator(input, factory).Calculate();

                // Assert
                LayerWithSoil[] layersWithSoil = LayerWithSoilCreator.Create(input.SoilProfile);

                KernelInputAssert.AssertSoilModels(SoilModelCreator.Create(layersWithSoil.Select(lws => lws.Soil).ToArray()), waternetKernel.SoilModel);
                KernelInputAssert.AssertSoilProfiles(SoilProfileCreator.Create(input.SoilProfile.PreconsolidationStresses, layersWithSoil), waternetKernel.SoilProfile);
                KernelInputAssert.AssertStabilityLocations(WaternetStabilityLocationCreator.Create(input), waternetKernel.Location);
                KernelInputAssert.AssertSurfaceLines(SurfaceLineCreator.Create(input.SurfaceLine, input.LandwardDirection), waternetKernel.SurfaceLine);
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