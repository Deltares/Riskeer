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
using Deltares.WTIStability.Data.Geo;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Waternet.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Creators.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.UpliftVan.Input;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels.Waternet;
using Ringtoets.MacroStabilityInwards.Primitives;
using Point2D = Core.Common.Base.Geometry.Point2D;
using SoilLayer = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilLayer;
using SoilProfile = Ringtoets.MacroStabilityInwards.KernelWrapper.Calculators.Input.SoilProfile;
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

            WaternetCalculatorInput input = CreateValidCalculatorInput();

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
            WaternetCalculatorInput input = CreateCompleteCalculatorInput();
            var testMacroStabilityInwardsKernelFactory = new TestMacroStabilityInwardsKernelFactory();

            WaternetKernelStub waternetKernel = testMacroStabilityInwardsKernelFactory.LastCreatedWaternetKernel;
            SetKernelOutput(waternetKernel);

            // Call
            new WaternetExtremeCalculator(input, testMacroStabilityInwardsKernelFactory).Calculate();

            // Assert
            Soil[] soils = SoilCreator.Create(input.SoilProfile);
            Dictionary<SoilLayer, Soil> layersWithSoils =
                input.SoilProfile.Layers
                     .Zip(soils, (layer, soil) => new
                     {
                         layer,
                         soil
                     })
                     .ToDictionary(x => x.layer, x => x.soil);

            KernelInputAssert.AssertSoilModels(SoilModelCreator.Create(soils), waternetKernel.SoilModel);
            KernelInputAssert.AssertSoilProfiles(SoilProfileCreator.Create(input.SoilProfile, layersWithSoils), waternetKernel.SoilProfile);
            KernelInputAssert.AssertStabilityLocations(WaternetStabilityLocationCreator.CreateExtreme(input), waternetKernel.Location);
            KernelInputAssert.AssertSurfaceLines(SurfaceLineCreator.Create(input.SurfaceLine, input.LandwardDirection), waternetKernel.SurfaceLine);
        }

        private static void SetKernelOutput(WaternetKernelStub waternetKernel)
        {
            waternetKernel.Waternet = new WtiStabilityWaternet
            {
                PhreaticLine = new PhreaticLine()
            };
        }

        private static WaternetCalculatorInput CreateCompleteCalculatorInput()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();

            return new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = random.NextDouble(),
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(surfaceLine),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                PhreaticLineOffsetsDaily = new PhreaticLineOffsets(),
                WaterLevelRiverAverage = random.Next(),
                WaterLevelPolderExtreme = random.Next(),
                WaterLevelPolderDaily = random.Next(),
                MinimumLevelPhreaticLineAtDikeTopRiver = random.Next(),
                MinimumLevelPhreaticLineAtDikeTopPolder = random.Next(),
                LeakageLengthOutwardsPhreaticLine3 = random.Next(),
                LeakageLengthInwardsPhreaticLine3 = random.Next(),
                LeakageLengthOutwardsPhreaticLine4 = random.Next(),
                LeakageLengthInwardsPhreaticLine4 = random.Next(),
                PiezometricHeadPhreaticLine2Outwards = random.Next(),
                PiezometricHeadPhreaticLine2Inwards = random.Next(),
                PenetrationLength = random.Next(),
                AdjustPhreaticLine3And4ForUplift = random.NextBoolean(),
                DikeSoilScenario = random.NextEnumValue<MacroStabilityInwardsDikeSoilScenario>()
            });
        }

        private static WaternetCalculatorInput CreateValidCalculatorInput()
        {
            var random = new Random(21);

            MacroStabilityInwardsSurfaceLine surfaceLine = CreateValidSurfaceLine();
            return new WaternetCalculatorInput(new WaternetCalculatorInput.ConstructionProperties
            {
                AssessmentLevel = random.NextDouble(),
                SurfaceLine = surfaceLine,
                SoilProfile = CreateValidSoilProfile(surfaceLine),
                DrainageConstruction = new DrainageConstruction(),
                PhreaticLineOffsetsExtreme = new PhreaticLineOffsets(),
                PhreaticLineOffsetsDaily = new PhreaticLineOffsets()
            });
        }

        private static SoilProfile CreateValidSoilProfile(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return new SoilProfile(new[]
            {
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties()),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties
                    {
                        IsAquifer = true
                    }),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties()),
                new SoilLayer(
                    new[]
                    {
                        surfaceLine.LocalGeometry.First(),
                        surfaceLine.LocalGeometry.Last()
                    },
                    Enumerable.Empty<Point2D[]>(),
                    new SoilLayer.ConstructionProperties())
            }, new[]
            {
                new PreconsolidationStress(new Point2D(0, 0), 1.1)
            });
        }

        private static MacroStabilityInwardsSurfaceLine CreateValidSurfaceLine()
        {
            var surfaceLine = new MacroStabilityInwardsSurfaceLine(string.Empty);
            var dikeToeAtRiver = new Point3D(1, 0, 8);

            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2),
                dikeToeAtRiver,
                new Point3D(2, 0, -1)
            });

            surfaceLine.SetDikeToeAtRiverAt(dikeToeAtRiver);

            return surfaceLine;
        }
    }
}