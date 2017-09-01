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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorTest
    {
        [Test]
        public void Constructor_WithoutInput_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculator(null, null);

            // Assert
            const string expectedMessage = "MacroStabilityInwardsCalculatorInput required for creating a MacroStabilityInwardsCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Call
            var input = new MacroStabilityInwardsCalculatorInput(CreateSimpleConstructionProperties());
            TestDelegate call = () => new MacroStabilityInwardsCalculator(input, null);

            // Assert
            const string expectedMessage = "IMacroStabilityInwardsSubCalculatorFactory required for creating a MacroStabilityInwardsCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Calculate_Always_ReturnResult()
        {
            // Setup
            var input = new MacroStabilityInwardsCalculatorInput(CreateSimpleConstructionProperties());
            var testMacroStabilityInwardsSubCalculatorFactory = new TestMacroStabilityInwardsSubCalculatorFactory();

            // Call
            MacroStabilityInwardsCalculatorResult actual = new MacroStabilityInwardsCalculator(input, testMacroStabilityInwardsSubCalculatorFactory).Calculate();

            // Assert
            Assert.IsNotNull(actual);

            Assert.IsTrue(testMacroStabilityInwardsSubCalculatorFactory.LastCreatedUpliftVanCalculator.Calculated);
        }

        [Test]
        public void Validate_Always_ReturnEmptyList()
        {
            // Setup
            var input = new MacroStabilityInwardsCalculatorInput(CreateSimpleConstructionProperties());
            var testMacroStabilityInwardsSubCalculatorFactory = new TestMacroStabilityInwardsSubCalculatorFactory();

            // Call
            List<string> validationResult = new MacroStabilityInwardsCalculator(input, testMacroStabilityInwardsSubCalculatorFactory).Validate();

            // Assert
            CollectionAssert.IsEmpty(validationResult);
        }

        private static MacroStabilityInwardsCalculatorInput.ConstructionProperties CreateSimpleConstructionProperties()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();
            return new MacroStabilityInwardsCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = random.NextDouble(),
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(surfaceLine)
            };
        }

        private static MacroStabilityInwardsSoilProfileUnderSurfaceLine CreateValidSoilProfile(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First()

                }, new MacroStabilityInwardsSoilLayerProperties()),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First()

                }, new MacroStabilityInwardsSoilLayerProperties
                {
                    IsAquifer = true
                }),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First()

                }, new MacroStabilityInwardsSoilLayerProperties()),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First()

                }, new MacroStabilityInwardsSoilLayerProperties()),
            });
        }

        private static MacroStabilityInwardsSurfaceLine CreateValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                new Point3D(1, 0, 8),
                new Point3D(2, 0, -1)
            });
            return surfaceLine;
        }
    }
}