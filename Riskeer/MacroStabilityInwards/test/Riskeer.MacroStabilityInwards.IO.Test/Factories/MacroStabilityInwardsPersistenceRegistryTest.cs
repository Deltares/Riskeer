using System;
using System.Collections.Generic;
using System.Linq;
using Components.Persistence.Stability.Data;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Factories;

namespace Riskeer.MacroStabilityInwards.IO.Test.Factories
{
    [TestFixture]
    public class MacroStabilityInwardsPersistenceRegistryTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var registry = new MacroStabilityInwardsPersistenceRegistry();

            // Assert
            Assert.IsEmpty(registry.Settings);
        }

        [Test]
        public void AddSettings_SettingsNull_ThrowsArgumentNullException()
        {
            // Setup
            var registry = new MacroStabilityInwardsPersistenceRegistry();

            // Call
            void Call() => registry.Add(null, "1");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("createdSettings", exception.ParamName);
        }

        [Test]
        public void AddSettings_WithSettings_AddsSettings()
        {
            // Setup
            var registry = new MacroStabilityInwardsPersistenceRegistry();
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
    }
}