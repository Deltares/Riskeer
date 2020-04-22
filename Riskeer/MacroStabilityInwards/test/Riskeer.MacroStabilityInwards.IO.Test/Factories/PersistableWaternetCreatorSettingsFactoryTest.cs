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

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Components.Persistence.Stability.Data;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.TestUtil;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class PersistableWaternetCreatorSettingsFactoryTest
    {
        [Test]
        public void Create_InputNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetCreatorSettingsFactory.Create(null, RoundedDouble.NaN, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("input", exception.ParamName);
        }

        [Test]
        public void Create_IdFactoryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetCreatorSettingsFactory.Create(new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties()), RoundedDouble.NaN,
                                                                            null, new MacroStabilityInwardsExportRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("idFactory", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => PersistableWaternetCreatorSettingsFactory.Create(new MacroStabilityInwardsInput(new MacroStabilityInwardsInput.ConstructionProperties()),
                                                                            RoundedDouble.NaN, new IdFactory(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_WithValidData_ReturnsPersistableWaternetCreatorSettingsCollection()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(
                new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsInput input = calculation.InputParameters;

            RoundedDouble normativeAssessmentLevel = RoundedDouble.NaN;

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            PersistableGeometryFactory.Create(input.SoilProfileUnderSurfaceLine, idFactory, registry);

            // Call
            IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettingsCollection = PersistableWaternetCreatorSettingsFactory.Create(input, normativeAssessmentLevel,
                                                                                                                                                 idFactory, registry);

            // Assert
            Assert.AreEqual(2, waternetCreatorSettingsCollection.Count());

            IEnumerable<MacroStabilityInwardsSoilLayer2D> aquiferLayers = MacroStabilityInwardsSoilProfile2DLayersHelper.GetLayersRecursively(input.SoilProfileUnderSurfaceLine.Layers)
                                                                                                                        .Where(l => l.Data.IsAquifer);

            var stages = new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            };

            for (var i = 0; i < waternetCreatorSettingsCollection.Count(); i++)
            {
                PersistableWaternetCreatorSettings waternetCreatorSettings = waternetCreatorSettingsCollection.ElementAt(i);

                Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopPolder, waternetCreatorSettings.InitialLevelEmbankmentTopWaterSide);
                Assert.AreEqual(input.MinimumLevelPhreaticLineAtDikeTopRiver, waternetCreatorSettings.InitialLevelEmbankmentTopLandSide);
                Assert.AreEqual(input.AdjustPhreaticLine3And4ForUplift, waternetCreatorSettings.AdjustForUplift);
                Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine3, waternetCreatorSettings.PleistoceneLeakageLengthInwards);
                Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine3, waternetCreatorSettings.PleistoceneLeakageLengthInwards);
                Assert.AreEqual(input.LeakageLengthInwardsPhreaticLine4, waternetCreatorSettings.AquiferLayerInsideAquitardLeakageLengthInwards);
                Assert.AreEqual(input.LeakageLengthOutwardsPhreaticLine4, waternetCreatorSettings.AquiferLayerInsideAquitardLeakageLengthOutwards);
                Assert.AreEqual(input.PiezometricHeadPhreaticLine2Inwards, waternetCreatorSettings.AquitardHeadLandSide);
                Assert.AreEqual(input.PiezometricHeadPhreaticLine2Outwards, waternetCreatorSettings.AquitardHeadWaterSide);
                Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorSettings.MeanWaterLevel);
                Assert.AreEqual(input.DrainageConstructionPresent, waternetCreatorSettings.IsDrainageConstructionPresent);
                Assert.AreEqual(input.XCoordinateDrainageConstruction, waternetCreatorSettings.DrainageConstruction.X);
                Assert.AreEqual(input.ZCoordinateDrainageConstruction, waternetCreatorSettings.DrainageConstruction.Z);
                Assert.AreEqual(IsDitchPresent(input.SurfaceLine), waternetCreatorSettings.IsDitchPresent);
                AssertDitchCharacteristics(input.SurfaceLine, waternetCreatorSettings.DitchCharacteristics, IsDitchPresent(input.SurfaceLine));
                AssertEmbankmentCharacteristics(input.SurfaceLine, waternetCreatorSettings.EmbankmentCharacteristics);
                Assert.AreEqual(GetEmbankmentSoilScenario(input.DikeSoilScenario), waternetCreatorSettings.EmbankmentSoilScenario);
                Assert.IsFalse(waternetCreatorSettings.IsAquiferLayerInsideAquitard);
                Assert.AreEqual(registry.GeometryLayers[stages[i]][aquiferLayers.Single()], waternetCreatorSettings.AquiferLayerId);

                if (stages[i] == MacroStabilityInwardsExportStageType.Daily)
                {
                    Assert.AreEqual(input.WaterLevelRiverAverage, waternetCreatorSettings.NormativeWaterLevel);
                    Assert.AreEqual(input.LocationInputDaily.WaterLevelPolder, waternetCreatorSettings.WaterLevelHinterland);
                    Assert.AreEqual(input.LocationInputDaily.PenetrationLength, waternetCreatorSettings.IntrusionLength);
                    AssertOffsets(input.LocationInputDaily, waternetCreatorSettings);
                }
                else if (stages[i] == MacroStabilityInwardsExportStageType.Extreme)
                {
                    Assert.AreEqual(normativeAssessmentLevel, waternetCreatorSettings.NormativeWaterLevel);
                    Assert.AreEqual(input.LocationInputExtreme.WaterLevelPolder, waternetCreatorSettings.WaterLevelHinterland);
                    Assert.AreEqual(input.LocationInputExtreme.PenetrationLength, waternetCreatorSettings.IntrusionLength);
                    AssertOffsets(input.LocationInputExtreme, waternetCreatorSettings);
                }
            }

            AssertRegistry(registry, stages, waternetCreatorSettingsCollection);
        }

        [Test]
        public void Create_WithDifferentCharacteristics_ReturnsPersistableWaternetCreatorSettingsCollection([Values(true, false)] bool isDitchPresent,
                                                                                                            [Values(true, false)] bool isShouldBaseInsidePresent)
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(
                new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsInput input = calculation.InputParameters;

            if (isDitchPresent)
            {
                input.SurfaceLine.SetDitchPolderSideAt(new Point3D(0.1, 0, 2));
                input.SurfaceLine.SetBottomDitchPolderSideAt(new Point3D(0.2, 0, 2));
                input.SurfaceLine.SetBottomDitchDikeSideAt(new Point3D(0.3, 0, 3));
                input.SurfaceLine.SetDitchDikeSideAt(new Point3D(0.4, 0, 3));
            }

            if (isShouldBaseInsidePresent)
            {
                input.SurfaceLine.SetShoulderBaseInsideAt(new Point3D(0.5, 0, 1));
            }

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            PersistableGeometryFactory.Create(input.SoilProfileUnderSurfaceLine, idFactory, registry);

            // Call
            IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettingsCollection = PersistableWaternetCreatorSettingsFactory.Create(
                input, RoundedDouble.NaN, idFactory, registry);

            // Assert
            foreach (PersistableWaternetCreatorSettings waternetCreatorSettings in waternetCreatorSettingsCollection)
            {
                Assert.AreEqual(isDitchPresent, waternetCreatorSettings.IsDitchPresent);
                AssertDitchCharacteristics(input.SurfaceLine, waternetCreatorSettings.DitchCharacteristics, isDitchPresent);
                AssertEmbankmentCharacteristics(input.SurfaceLine, waternetCreatorSettings.EmbankmentCharacteristics);
            }
        }

        [Test]
        public void Create_WithMultipleAquiferLayers_ReturnsPersistableWaternetCreatorSettingsCollection()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(
                new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsInput input = calculation.InputParameters;

            foreach (IMacroStabilityInwardsSoilLayer layer in input.StochasticSoilProfile.SoilProfile.Layers)
            {
                layer.Data.IsAquifer = true;
            }

            // Call
            IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettingsCollection = PersistableWaternetCreatorSettingsFactory.Create(
                input, RoundedDouble.NaN, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            Assert.IsTrue(waternetCreatorSettingsCollection.All(wcsc => wcsc.AquiferLayerId == null));
        }

        [Test]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay)]
        [TestCase(MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand)]
        public void Create_WithDifferentDikeSoilScenarios_ReturnsPersistableWaternetCreatorSettingsCollection(MacroStabilityInwardsDikeSoilScenario soilScenario)
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(
                new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsInput input = calculation.InputParameters;
            input.DikeSoilScenario = soilScenario;

            var idFactory = new IdFactory();
            var registry = new MacroStabilityInwardsExportRegistry();

            PersistableGeometryFactory.Create(input.SoilProfileUnderSurfaceLine, idFactory, registry);

            // Call
            IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettingsCollection = PersistableWaternetCreatorSettingsFactory.Create(
                input, RoundedDouble.NaN, idFactory, registry);

            // Assert
            foreach (PersistableWaternetCreatorSettings waternetCreatorSettings in waternetCreatorSettingsCollection)
            {
                Assert.AreEqual(GetEmbankmentSoilScenario(input.DikeSoilScenario), waternetCreatorSettings.EmbankmentSoilScenario);
            }
        }

        [Test]
        public void Create_InvalidDikeSoilScenarios_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            MacroStabilityInwardsCalculationScenario calculation = MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(
                new TestHydraulicBoundaryLocation());
            MacroStabilityInwardsInput input = calculation.InputParameters;
            input.DikeSoilScenario = (MacroStabilityInwardsDikeSoilScenario) 99;

            // Call
            void Call() => PersistableWaternetCreatorSettingsFactory.Create(input, RoundedDouble.NaN, new IdFactory(), new MacroStabilityInwardsExportRegistry());

            // Assert
            string expectedMessage = $"The value of argument 'dikeSoilScenario' ({input.DikeSoilScenario}) is invalid for Enum type '{nameof(MacroStabilityInwardsDikeSoilScenario)}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(Call, expectedMessage);
        }

        private PersistableEmbankmentSoilScenario GetEmbankmentSoilScenario(MacroStabilityInwardsDikeSoilScenario dikeSoilScenario)
        {
            switch (dikeSoilScenario)
            {
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnClay:
                    return PersistableEmbankmentSoilScenario.ClayEmbankmentOnClay;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnClay:
                    return PersistableEmbankmentSoilScenario.SandEmbankmentOnClay;
                case MacroStabilityInwardsDikeSoilScenario.ClayDikeOnSand:
                    return PersistableEmbankmentSoilScenario.ClayEmbankmentOnSand;
                case MacroStabilityInwardsDikeSoilScenario.SandDikeOnSand:
                    return PersistableEmbankmentSoilScenario.SandEmbankmentOnSand;
                default:
                    throw new NotSupportedException();
            }
        }

        private static void AssertOffsets(IMacroStabilityInwardsLocationInput locationInput, PersistableWaternetCreatorSettings waternetCreatorSettings)
        {
            Assert.AreEqual(locationInput.UseDefaultOffsets, waternetCreatorSettings.UseDefaultOffsets);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowDikeTopAtPolder, waternetCreatorSettings.OffsetEmbankmentTopLandSide);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowDikeTopAtRiver, waternetCreatorSettings.OffsetEmbankmentTopWaterSide);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowDikeToeAtPolder, waternetCreatorSettings.OffsetEmbankmentToeLandSide);
            Assert.AreEqual(locationInput.PhreaticLineOffsetBelowShoulderBaseInside, waternetCreatorSettings.OffsetShoulderBaseLandSide);
        }

        private static void AssertRegistry(MacroStabilityInwardsExportRegistry registry, MacroStabilityInwardsExportStageType[] stages, IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettingsCollection)
        {
            Assert.AreEqual(stages.Length, registry.WaternetCreatorSettings.Count);

            for (var i = 0; i < stages.Length; i++)
            {
                PersistableWaternetCreatorSettings waternetCreatorSettings = waternetCreatorSettingsCollection.ElementAt(i);
                Assert.AreEqual(registry.WaternetCreatorSettings[stages[i]], waternetCreatorSettings.Id);
            }
        }

        private static bool IsDitchPresent(MacroStabilityInwardsSurfaceLine surfaceLine)
        {
            return surfaceLine.DitchDikeSide != null
                   && surfaceLine.DitchPolderSide != null
                   && surfaceLine.BottomDitchDikeSide != null
                   && surfaceLine.BottomDitchPolderSide != null;
        }

        private static void AssertDitchCharacteristics(MacroStabilityInwardsSurfaceLine surfaceLine, PersistableDitchCharacteristics ditchCharacteristics, bool isDitchPresent)
        {
            if (isDitchPresent)
            {
                Assert.AreEqual(surfaceLine.BottomDitchDikeSide.X, ditchCharacteristics.DitchBottomEmbankmentSide);
                Assert.AreEqual(surfaceLine.BottomDitchPolderSide.X, ditchCharacteristics.DitchBottomLandSide);
                Assert.AreEqual(surfaceLine.DitchDikeSide.X, ditchCharacteristics.DitchEmbankmentSide);
                Assert.AreEqual(surfaceLine.DitchPolderSide.X, ditchCharacteristics.DitchLandSide);
            }
            else
            {
                Assert.IsNaN(ditchCharacteristics.DitchBottomEmbankmentSide);
                Assert.IsNaN(ditchCharacteristics.DitchBottomLandSide);
                Assert.IsNaN(ditchCharacteristics.DitchEmbankmentSide);
                Assert.IsNaN(ditchCharacteristics.DitchLandSide);
            }
        }

        private static void AssertEmbankmentCharacteristics(MacroStabilityInwardsSurfaceLine surfaceLine, PersistableEmbankmentCharacteristics embankmentCharacteristics)
        {
            Assert.AreEqual(surfaceLine.DikeToeAtPolder.X, embankmentCharacteristics.EmbankmentToeLandSide);
            Assert.AreEqual(surfaceLine.DikeTopAtPolder.X, embankmentCharacteristics.EmbankmentTopLandSide);
            Assert.AreEqual(surfaceLine.DikeTopAtRiver.X, embankmentCharacteristics.EmbankmentTopWaterSide);
            Assert.AreEqual(surfaceLine.DikeToeAtRiver.X, embankmentCharacteristics.EmbankmentToeWaterSide);
            Assert.AreEqual(surfaceLine.ShoulderBaseInside?.X ?? double.NaN, embankmentCharacteristics.ShoulderBaseLandSide);
        }
    }
}