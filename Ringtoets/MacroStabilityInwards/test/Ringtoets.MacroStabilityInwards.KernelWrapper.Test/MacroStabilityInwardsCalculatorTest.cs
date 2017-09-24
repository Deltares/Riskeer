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

using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators;
using Ringtoets.MacroStabilityInwards.KernelWrapper.SubCalculator;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculatorTest
    {
        [Test]
        public void Constructor_WithoutInput_ArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new MacroStabilityInwardsCalculator(null, factory);

            // Assert
            const string expectedMessage = "MacroStabilityInwardsCalculatorInput required for creating a MacroStabilityInwardsCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
            mocks.VerifyAll();
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
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            var input = new MacroStabilityInwardsCalculatorInput(CreateSimpleConstructionProperties());

            // Call
            var calculator = new MacroStabilityInwardsCalculator(input, factory);

            // Assert
            Assert.IsInstanceOf<IMacroStabilityInwardsCalculator>(calculator);
        }

        [Test]
        public void Calculate_CompleteValidInput_ReturnsResult()
        {
            // Setup
            var random = new Random(11);

            var input = new MacroStabilityInwardsCalculatorInput(CreateSimpleConstructionProperties());
            var testMacroStabilityInwardsSubCalculatorFactory = new TestMacroStabilityInwardsSubCalculatorFactory();
            UpliftVanCalculatorStub calculator = testMacroStabilityInwardsSubCalculatorFactory.LastCreatedUpliftVanCalculator;
            calculator.FactoryOfStability = random.NextDouble();
            calculator.ZValue = random.NextDouble();
            calculator.ForbiddenZonesXEntryMax = random.NextDouble();
            calculator.ForbiddenZonesXEntryMin = random.NextDouble();
            calculator.ForbiddenZonesAutomaticallyCalculated = random.NextBoolean();
            calculator.GridAutomaticallyCalculated = random.NextBoolean();
            calculator.SlidingCurveResult = SlidingDualCircleTestFactory.Create();
            calculator.SlipPlaneResult = SlipPlaneUpliftVanTestFactory.Create();

            // Call
            MacroStabilityInwardsCalculatorResult actual = new MacroStabilityInwardsCalculator(input, testMacroStabilityInwardsSubCalculatorFactory).Calculate();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(calculator.FactoryOfStability, actual.FactorOfStability);
            Assert.AreEqual(calculator.ZValue, actual.ZValue);
            Assert.AreEqual(calculator.ForbiddenZonesXEntryMax, actual.ForbiddenZonesXEntryMax);
            Assert.AreEqual(calculator.ForbiddenZonesXEntryMin, actual.ForbiddenZonesXEntryMin);
            Assert.AreEqual(calculator.ForbiddenZonesAutomaticallyCalculated, actual.ForbiddenZonesAutomaticallyCalculated);
            Assert.AreEqual(calculator.GridAutomaticallyCalculated, actual.GridAutomaticallyCalculated);
            MacroStabilityInwardsCalculatorResultHelper.AssertSlidingCurve(MacroStabilityInwardsSlidingCurveResultCreator.Create(calculator.SlidingCurveResult),
                                                                             actual.SlidingCurve);
            MacroStabilityInwardsCalculatorResultHelper.AssertSlipPlaneGrid(MacroStabilityInwardsUpliftVanCalculationGridResultCreator.Create(calculator.SlipPlaneResult),
                                                                            actual.UpliftVanCalculationGrid);

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
                SoilProfile = CreateValidSoilProfile(surfaceLine),
                LeftGrid = new MacroStabilityInwardsGrid(),
                RightGrid = new MacroStabilityInwardsGrid()
            };
        }

        private static MacroStabilityInwardsSoilProfileUnderSurfaceLine CreateValidSoilProfile(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return new MacroStabilityInwardsSoilProfileUnderSurfaceLine(new[]
            {
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First(),
                    surfaceLine.LocalGeometry.Last()

                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties())),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First(),
                    surfaceLine.LocalGeometry.Last()

                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties
                {
                    IsAquifer = true
                })),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First(),
                    surfaceLine.LocalGeometry.Last()

                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties())),
                new MacroStabilityInwardsSoilLayerUnderSurfaceLine(new[]
                {
                    surfaceLine.LocalGeometry.First(),
                    surfaceLine.LocalGeometry.Last()

                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties())),
            }, Enumerable.Empty<MacroStabilityInwardsPreconsolidationStressUnderSurfaceLine>());
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