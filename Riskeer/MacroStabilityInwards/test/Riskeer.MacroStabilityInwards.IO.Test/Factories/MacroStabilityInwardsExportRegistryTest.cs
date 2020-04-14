using System;
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.MacroStabilityInwards.IO.Factories;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsExportRegistryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var registry = new MacroStabilityInwardsExportRegistry();

            // Assert
            CollectionAssert.IsEmpty(registry.Settings);
            CollectionAssert.IsEmpty(registry.Soils);
        }

        [Test]
        public void AddSettings_SettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            void Call() => registry.Add((PersistableCalculationSettings) null, "1");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("createdSettings", exception.ParamName);
        }

        [Test]
        public void AddSettings_WithSettings_AddsSettings()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();
            var settings = new PersistableCalculationSettings();
            const string id = "1";

            // Call
            registry.Add(settings, id);

            // Assert
            Assert.AreEqual(1, registry.Settings.Count);
            KeyValuePair<PersistableCalculationSettings, string> storedSettings = registry.Settings.Single();
            Assert.AreSame(settings, storedSettings.Key);
            Assert.AreEqual(id, storedSettings.Value);
        }

        [Test]
        public void AddSoil_SoilLayerNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new MacroStabilityInwardsExportRegistry();

            // Call
            void Call() => registry.Add((IMacroStabilityInwardsSoilLayer) null, "1");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("soilLayer", exception.ParamName);
        }

        [Test]
        public void AddSoil_WithSoilLayer_AddsSoil()
        {
            // Setup
            var mocks = new MockRepository();
            var soilLayer = mocks.Stub<IMacroStabilityInwardsSoilLayer>();
            mocks.ReplayAll();

            var registry = new MacroStabilityInwardsExportRegistry();
            const string id = "1";

            // Call
            registry.Add(soilLayer, id);

            // Assert
            Assert.AreEqual(1, registry.Soils.Count);
            KeyValuePair<IMacroStabilityInwardsSoilLayer, string> storedSoil = registry.Soils.Single();
            Assert.AreSame(soilLayer, storedSoil.Key);
            Assert.AreEqual(id, storedSoil.Value);
        }
    }
}