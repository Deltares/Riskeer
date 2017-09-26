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
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.UpliftVan.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Output;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernel;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Result;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Primitives.MacroStabilityInwardsSoilUnderSurfaceLine;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper.Test.Calculators.UpliftVan
{
    [TestFixture]
    public class UpliftVanCalculatorTest
    {
        [Test]
        public void Constructor_WithoutInput_ArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new UpliftVanCalculator(null, factory);

            // Assert
            const string expectedMessage = "UpliftVanCalculatorInput required for creating a UpliftVanCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Call
            var input = new UpliftVanCalculatorInput(CreateSimpleConstructionProperties());
            TestDelegate call = () => new UpliftVanCalculator(input, null);

            // Assert
            const string expectedMessage = "IMacroStabilityInwardsKernelFactory required for creating a UpliftVanCalculator.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var factory = mocks.Stub<IMacroStabilityInwardsKernelFactory>();
            mocks.ReplayAll();

            var input = new UpliftVanCalculatorInput(CreateSimpleConstructionProperties());

            // Call
            var calculator = new UpliftVanCalculator(input, factory);

            // Assert
            Assert.IsInstanceOf<IUpliftVanCalculator>(calculator);
        }

        [Test]
        public void Calculate_CompleteValidInput_ReturnsResult()
        {
            // Setup
            var random = new Random(11);

            var input = new UpliftVanCalculatorInput(CreateSimpleConstructionProperties());
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();
            UpliftVanKernelStub upliftVanKernel = testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel;
            upliftVanKernel.FactorOfStability = random.NextDouble();
            upliftVanKernel.ZValue = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMax = random.NextDouble();
            upliftVanKernel.ForbiddenZonesXEntryMin = random.NextDouble();
            upliftVanKernel.ForbiddenZonesAutomaticallyCalculated = random.NextBoolean();
            upliftVanKernel.GridAutomaticallyCalculated = random.NextBoolean();
            upliftVanKernel.SlidingCurveResult = SlidingDualCircleTestFactory.Create();
            upliftVanKernel.SlipPlaneResult = SlipPlaneUpliftVanTestFactory.Create();

            // Call
            UpliftVanCalculatorResult actual = new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(upliftVanKernel.FactorOfStability, actual.FactorOfStability);
            Assert.AreEqual(upliftVanKernel.ZValue, actual.ZValue);
            Assert.AreEqual(upliftVanKernel.ForbiddenZonesXEntryMax, actual.ForbiddenZonesXEntryMax);
            Assert.AreEqual(upliftVanKernel.ForbiddenZonesXEntryMin, actual.ForbiddenZonesXEntryMin);
            Assert.AreEqual(upliftVanKernel.ForbiddenZonesAutomaticallyCalculated, actual.ForbiddenZonesAutomaticallyCalculated);
            Assert.AreEqual(upliftVanKernel.GridAutomaticallyCalculated, actual.GridAutomaticallyCalculated);
            UpliftVanCalculatorResultHelper.AssertSlidingCurve(UpliftVanSlidingCurveResultCreator.Create(upliftVanKernel.SlidingCurveResult),
                                                               actual.SlidingCurveResult);
            UpliftVanCalculatorResultHelper.AssertSlipPlaneGrid(UpliftVanCalculationGridResultCreator.Create(upliftVanKernel.SlipPlaneResult),
                                                                actual.CalculationGridResult);

            Assert.IsTrue(testMacroStabilityInwardsKernelFactory.LastCreatedUpliftVanKernel.Calculated);
        }

        [Test]
        public void Validate_Always_ReturnEmptyList()
        {
            // Setup
            var input = new UpliftVanCalculatorInput(CreateSimpleConstructionProperties());
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            // Call
            List<string> validationResult = new UpliftVanCalculator(input, testMacroStabilityInwardsKernelFactory).Validate();

            // Assert
            CollectionAssert.IsEmpty(validationResult);
        }

        private static UpliftVanCalculatorInput.ConstructionProperties CreateSimpleConstructionProperties()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();
            return new UpliftVanCalculatorInput.ConstructionProperties
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
                }, new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine(new MacroStabilityInwardsSoilLayerPropertiesUnderSurfaceLine.ConstructionProperties()))
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