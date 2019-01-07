// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Ringtoets.HydraRing.IO.Test.HydraulicLocationConfigurationDatabase
{
    [TestFixture]
    public class ReadHydraulicLocationConfigurationDatabaseSettingsTest
    {
        [Test]
        public void Constructor_WithArgumentsNull_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            int year = random.Next();

            // Call
            var settings = new ReadHydraulicLocationConfigurationDatabaseSettings(null, year, null, null, null,
                                                                                  null, null, null, null);

            // Assert
            Assert.IsNull(settings.ScenarioName);
            Assert.AreEqual(year, settings.Year);
            Assert.IsNull(settings.Scope);
            Assert.IsNull(settings.SeaLevel);
            Assert.IsNull(settings.RiverDischarge);
            Assert.IsNull(settings.LakeLevel);
            Assert.IsNull(settings.WindDirection);
            Assert.IsNull(settings.WindSpeed);
            Assert.IsNull(settings.Comment);
        }

        [Test]
        public void Constructor_WithArgumentValues_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            const string scenarioName = "ScenarioName";
            int year = random.Next();
            const string scope = "Scope";
            const string seaLevel = "SeaLevel";
            const string riverDischarge = "RiverDischarge";
            const string lakeLevel = "LakeLevel";
            const string windDirection = "WindDirection";
            const string windSpeed = "WindSpeed";
            const string comment = "Comment";

            // Call
            var settings = new ReadHydraulicLocationConfigurationDatabaseSettings(scenarioName, year, scope,
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