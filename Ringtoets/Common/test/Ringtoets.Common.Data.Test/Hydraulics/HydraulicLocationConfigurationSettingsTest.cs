using System;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicLocationConfigurationSettingsTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var settings = new HydraulicLocationConfigurationSettings();

            // Assert
            Assert.IsNull(settings.FilePath);
            Assert.IsNull(settings.ScenarioName);
            Assert.Zero(settings.Year);
            Assert.IsNull(settings.Scope);
            Assert.IsNull(settings.SeaLevel);
            Assert.IsNull(settings.RiverDischarge);
            Assert.IsNull(settings.LakeLevel);
            Assert.IsNull(settings.WindDirection);
            Assert.IsNull(settings.WindSpeed);
            Assert.IsNull(settings.Comment);
        }

        [Test]
        public void SetValues_FilePathNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            const string scenarioName = "FilePath";
            int year = random.Next();
            const string scope = "Scope";
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            var settings = new HydraulicLocationConfigurationSettings();

            // Call
            TestDelegate call = () => settings.SetValues(null, scenarioName, year, scope,
                                                         seaLevel, riverDischarge, lakeLevel,
                                                         windDirection, windSpeed, comment);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("filePath", exception.ParamName);
        }

        [Test]
        public void SetValues_ScenarioNameNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            const string filePath = "FilePath";
            int year = random.Next();
            const string scope = "Scope";
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            var settings = new HydraulicLocationConfigurationSettings();

            // Call
            TestDelegate call = () => settings.SetValues(filePath, null, year, scope,
                                                         seaLevel, riverDischarge, lakeLevel,
                                                         windDirection, windSpeed, comment);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("scenarioName", exception.ParamName);
        }

        [Test]
        public void SetValues_ScopeNull_ThrowsArgumentNullException()
        {
            // Setup
            var random = new Random(21);
            const string filePath = "FilePath";
            const string scenarioName = "ScenarioName";
            int year = random.Next();
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            var settings = new HydraulicLocationConfigurationSettings();

            // Call
            TestDelegate call = () => settings.SetValues(filePath, scenarioName, year, null,
                                                         seaLevel, riverDischarge, lakeLevel,
                                                         windDirection, windSpeed, comment);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("scope", exception.ParamName);
        }

        [Test]
        public void SetValues_WithArguments_SetsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string filePath = "FilePath";
            const string scenarioName = "ScenarioName";
            int year = random.Next();
            const string scope = "Scope";
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            var settings = new HydraulicLocationConfigurationSettings();

            // Call
            settings.SetValues(filePath, scenarioName, year, scope,
                               seaLevel, riverDischarge, lakeLevel,
                               windDirection, windSpeed, comment);

            // Assert
            Assert.AreEqual(scenarioName, settings.ScenarioName);
            Assert.AreEqual(year, settings.Year);
            Assert.AreEqual(scope, settings.Scope);
            Assert.AreEqual(seaLevel, settings.SeaLevel);
            Assert.AreEqual(riverDischarge, settings.RiverDischarge);
            Assert.AreEqual(lakeLevel, settings.LakeLevel);
            Assert.AreEqual(windDirection, settings.WindDirection);
            Assert.AreEqual(windSpeed, settings.WindSpeed);
            Assert.AreEqual(comment, settings.Comment);
        }
    }
}