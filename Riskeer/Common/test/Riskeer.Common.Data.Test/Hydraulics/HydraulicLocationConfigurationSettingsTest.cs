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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.Test.Hydraulics
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
            Assert.AreEqual(0, settings.Year);
            Assert.IsNull(settings.Scope);
            Assert.IsNull(settings.SeaLevel);
            Assert.IsNull(settings.RiverDischarge);
            Assert.IsNull(settings.LakeLevel);
            Assert.IsNull(settings.WindDirection);
            Assert.IsNull(settings.WindSpeed);
            Assert.IsNull(settings.Comment);
            Assert.IsFalse(settings.UsePreprocessorClosure);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("    ")]
        public void SetValues_InvalidFilePathNull_ThrowsArgumentException(string invalidFilePath)
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
            TestDelegate call = () => settings.SetValues(null, scenarioName, year, scope, false,
                                                         seaLevel, riverDischarge, lakeLevel,
                                                         windDirection, windSpeed, comment);

            // Assert
            const string expectedMessage = "'filePath' is null, empty or consists of whitespace.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
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
            TestDelegate call = () => settings.SetValues(filePath, null, year, scope, false,
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
            TestDelegate call = () => settings.SetValues(filePath, scenarioName, year, null, false,
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
            bool usePreprocessorClosure = random.NextBoolean();
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            var settings = new HydraulicLocationConfigurationSettings();

            // Call
            settings.SetValues(filePath, scenarioName, year, scope,
                               usePreprocessorClosure, seaLevel, riverDischarge,
                               lakeLevel, windDirection, windSpeed, comment);

            // Assert
            Assert.AreEqual(filePath, settings.FilePath);
            Assert.AreEqual(scenarioName, settings.ScenarioName);
            Assert.AreEqual(year, settings.Year);
            Assert.AreEqual(scope, settings.Scope);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
            Assert.AreEqual(seaLevel, settings.SeaLevel);
            Assert.AreEqual(riverDischarge, settings.RiverDischarge);
            Assert.AreEqual(lakeLevel, settings.LakeLevel);
            Assert.AreEqual(windDirection, settings.WindDirection);
            Assert.AreEqual(windSpeed, settings.WindSpeed);
            Assert.AreEqual(comment, settings.Comment);
        }

        [Test]
        public void SetValues_OptionalArgumentsNull_SetsExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string filePath = "FilePath";
            const string scenarioName = "ScenarioName";
            int year = random.Next();
            const string scope = "Scope";
            bool usePreprocessorClosure = random.NextBoolean();

            var settings = new HydraulicLocationConfigurationSettings();

            // Call
            settings.SetValues(filePath, scenarioName, year, scope,
                               usePreprocessorClosure,
                               null, null, null, null, null, null);

            // Assert
            Assert.AreEqual(filePath, settings.FilePath);
            Assert.AreEqual(scenarioName, settings.ScenarioName);
            Assert.AreEqual(year, settings.Year);
            Assert.AreEqual(scope, settings.Scope);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);

            Assert.IsNull(settings.SeaLevel);
            Assert.IsNull(settings.RiverDischarge);
            Assert.IsNull(settings.LakeLevel);
            Assert.IsNull(settings.WindDirection);
            Assert.IsNull(settings.WindSpeed);
            Assert.IsNull(settings.Comment);
        }
    }
}