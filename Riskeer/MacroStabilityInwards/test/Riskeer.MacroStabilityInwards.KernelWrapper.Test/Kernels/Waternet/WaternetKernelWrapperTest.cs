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

using System.Linq;
using Core.Common.Util.Reflection;
using Deltares.MacroStability.Geometry;
using Deltares.MacroStability.Standard;
using Deltares.MacroStability.WaternetCreator;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.KernelWrapper.Kernels.Waternet;
using WtiStabilityWaternet = Deltares.MacroStability.Geometry.Waternet;

namespace Riskeer.MacroStabilityInwards.KernelWrapper.Test.Kernels.Waternet
{
    [TestFixture]
    public class WaternetKernelWrapperTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var location = new Location();

            // Call
            var kernel = new TestWaternetKernelWrapper(location, "Waternet");

            // Assert
            Assert.IsInstanceOf<IWaternetKernel>(kernel);
            Assert.IsNull(location.Surfaceline);
            Assert.IsNull(location.SoilProfile2D);
        }

        [Test]
        public void Constructor_CompleteInput_InputCorrectlySetToWrappedKernel()
        {
            // Setup
            var location = new Location();
            var soilProfile2D = new SoilProfile2D();
            var surfaceLine = new SurfaceLine2();

            // Call
            var kernel = new TestWaternetKernelWrapper(location, "Waternet");
            kernel.SetSoilProfile(soilProfile2D);
            kernel.SetSurfaceLine(surfaceLine);

            // Assert
            var waternetCreator = TypeUtils.GetField<WaternetCreator>(kernel, "waternetCreator");

            Assert.AreSame(surfaceLine, location.Surfaceline);
            Assert.AreSame(soilProfile2D, location.SoilProfile2D);

            AssertIrrelevantValues(kernel.Waternet, waternetCreator);
        }

        [Test]
        public void Calculate_ExceptionInWrappedKernel_ThrowsWaternetKernelWrapperException()
        {
            // Setup
            var kernel = new TestWaternetKernelWrapper(new Location(), "Waternet");

            // Call
            void Call() => kernel.Calculate();

            // Assert
            var exception = Assert.Throws<WaternetKernelWrapperException>(Call);
            Assert.IsNotNull(exception.InnerException);
            Assert.AreEqual(exception.InnerException.Message, exception.Message);
        }

        private static void AssertIrrelevantValues(WtiStabilityWaternet waternet, WaternetCreator waternetCreator)
        {
            Assert.AreEqual("Waternet", waternet.Name);
            Assert.AreEqual(9.81, waternet.UnitWeight);
            Assert.IsFalse(waternet.IsGenerated);

            Assert.AreEqual(Enumerable.Empty<LogMessage>(), waternetCreator.LogMessages);
            Assert.AreEqual(LanguageType.Dutch, waternetCreator.Language);
        }

        private class TestWaternetKernelWrapper : WaternetKernelWrapper
        {
            public TestWaternetKernelWrapper(Location location, string waternetName)
                : base(location, waternetName) {}
        }
    }
}