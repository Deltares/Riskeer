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

using Core.Common.Util.Reflection;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.WaternetCreator;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.Waternet
{
    [TestFixture]
    public class WaternetKernelWrapperTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var kernel = new TestWaternetKernelWrapper();

            // Assert
            Assert.IsInstanceOf<IWaternetKernel>(kernel);
        }

        [Test]
        public void Constructor_CompleteInput_InputCorrectlySetToWrappedKernel()
        {
            // Setup
            var stabilityLocation = new Location();
            var soilProfile2D = new SoilProfile2D();
            var surfaceLine = new SurfaceLine2();

            // Call
            var kernel = new TestWaternetKernelWrapper();
            kernel.SetLocation(stabilityLocation);
            kernel.SetSoilProfile(soilProfile2D);
            kernel.SetSurfaceLine(surfaceLine);

            // Assert
            var location = TypeUtils.GetField<Location>(kernel, "location");

            Assert.AreSame(stabilityLocation, location);
            Assert.AreSame(surfaceLine, location.Surfaceline);
            Assert.AreSame(soilProfile2D, location.SoilProfile2D);
        }

        [Test]
        public void Calculate_WaternetCannotBeGenerated_ThrowsWaternetKernelWrapperExceptionAndWaternetNotSet()
        {
            // Setup
            var kernel = new TestWaternetKernelWrapper();
            var location = new Location
            {
                WaternetCreationMode = WaternetCreationMode.CreateWaternet,
                Surfaceline = null
            };
            kernel.SetLocation(location);

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            Assert.Throws<WaternetKernelWrapperException>(test);
            Assert.IsNull(kernel.Waternet);
        }

        [Test]
        public void Calculate_ExceptionInWrappedKernel_ThrowsWaternetKernelWrapperExceptionAndWaternetNotSet()
        {
            // Setup
            var kernel = new TestWaternetKernelWrapper();

            // Call
            TestDelegate test = () => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(test);
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
            Assert.IsNull(kernel.Waternet);
        }

        private class TestWaternetKernelWrapper : WaternetKernelWrapper {}
    }
}