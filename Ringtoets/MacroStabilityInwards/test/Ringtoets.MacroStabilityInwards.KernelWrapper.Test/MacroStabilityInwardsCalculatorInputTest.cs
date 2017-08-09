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

using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorInputTest
    {
        [Test]
        public void Constructor_WithoutConstructionProperies_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new MacroStabilityInwardsCalculatorInput(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("properties", paramName);
        }

        [Test]
        public void Constructor_WithConstructionProperties_PropertiesAreSet()
        {
            // Setup
            var mocks = new MockRepository();
            var soilProfile = mocks.Stub<ISoilProfile>();
            mocks.ReplayAll();

            var random = new Random(11);

            double hRiverValue = random.NextDouble();
            var surfaceLine = new MacroStabilityInwardsSurfaceLine();

            // Call
            var input = new MacroStabilityInwardsCalculatorInput(
                new MacroStabilityInwardsCalculatorInput.ConstructionProperties
                {
                    AssessmentLevel = hRiverValue,
                    SurfaceLine = surfaceLine,
                    SoilProfile = soilProfile
                });

            // Assert
            Assert.AreEqual(hRiverValue, input.AssessmentLevel);
            Assert.AreSame(surfaceLine, input.SurfaceLine);
            Assert.AreSame(soilProfile, input.SoilProfile);

            mocks.VerifyAll();
        }
    }
}