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
using Riskeer.MacroStabilityInwards.Data.TestUtil;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.IO.TestUtil;
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
            var stages = new[]
            {
                MacroStabilityInwardsExportStageType.Daily,
                MacroStabilityInwardsExportStageType.Extreme
            };

            PersistableDataModelTestHelper.AssertWaternetCreatorSettings(input, waternetCreatorSettingsCollection, normativeAssessmentLevel, stages);
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
                PersistableDataModelTestHelper.AssertDitchCharacteristics(input.SurfaceLine, waternetCreatorSettings.DitchCharacteristics, isDitchPresent);
                PersistableDataModelTestHelper.AssertEmbankmentCharacteristics(input.SurfaceLine, waternetCreatorSettings.EmbankmentCharacteristics);
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
                Assert.AreEqual(PersistableDataModelTestHelper.GetEmbankmentSoilScenario(input.DikeSoilScenario), waternetCreatorSettings.EmbankmentSoilScenario);
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

        private static void AssertRegistry(MacroStabilityInwardsExportRegistry registry, MacroStabilityInwardsExportStageType[] stages, IEnumerable<PersistableWaternetCreatorSettings> waternetCreatorSettingsCollection)
        {
            Assert.AreEqual(stages.Length, registry.WaternetCreatorSettings.Count);

            for (var i = 0; i < stages.Length; i++)
            {
                PersistableWaternetCreatorSettings waternetCreatorSettings = waternetCreatorSettingsCollection.ElementAt(i);
                Assert.AreEqual(registry.WaternetCreatorSettings[stages[i]], waternetCreatorSettings.Id);
            }
        }
    }
}